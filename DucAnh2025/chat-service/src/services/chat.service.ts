import { Types } from "mongoose";
import fs from "fs/promises";
import path from "path";
import type { AuthUser } from "../auth/jwt.ts";
import { env } from "../config/env.ts";
import { findExistingUsersByIds } from "../db/sql.ts";
import { AiService } from "./ai.service.ts";
import {
  Conversation,
  ConversationMember,
  type ConversationMemberDocument,
  type ConversationDocument,
} from "../models/conversation.model.ts";
import {
  Message,
  MessageReaction,
  MessageRead,
} from "../models/message.model.ts";
import { logger, serializeError } from "../utils/logger.ts";

const MUTED_FOREVER = new Date("9999-12-31T23:59:59.999Z");
const uploadRoot = path.resolve(env.MEDIA_ROOT);
const AI_USER_ID = "ai-assistant";

type MessageAttachment = {
  type?: string;
  url?: string;
  fileName?: string;
  mimeType?: string;
  size?: number;
  durationMs?: number | null;
  recalledAt?: Date | string | null;
  deletedForUserIds?: string[];
};

export class ChatService {
  private ai = new AiService();

  private isSystemAdmin(actor: AuthUser) {
    return (actor.roles || []).some((role) => /admin/i.test(role));
  }

  private displayName(
    user: Pick<AuthUser, "firstName" | "lastName" | "userName" | "email">,
  ) {
    const fullName = `${user.firstName || ""} ${user.lastName || ""}`.trim();
    return fullName || user.userName || user.email || "";
  }

  private visibleAttachments(attachments: MessageAttachment[] = [], userId: string) {
    return attachments.filter((attachment) => {
      if (attachment.recalledAt) return false;
      return !(attachment.deletedForUserIds ?? []).includes(userId);
    });
  }

  private filterMessageForUser<T extends Record<string, any>>(
    message: T,
    userId: string,
  ) {
    const filtered: T = {
      ...message,
      attachments: this.visibleAttachments(message.attachments, userId),
    };

    if ((filtered as any).replyTo) {
      (filtered as any).replyTo = {
        ...(filtered as any).replyTo,
        attachments: this.visibleAttachments((filtered as any).replyTo.attachments, userId),
      };
    }

    return filtered;
  }

  private normalizeAttachmentUrl(value: unknown) {
    const raw = String(value ?? "").trim();
    if (!raw) return "";

    try {
      const parsed = new URL(raw);
      return parsed.pathname;
    } catch {
      return raw.split("?")[0];
    }
  }

  private attachmentMatches(attachment: MessageAttachment, attachmentUrl?: string, attachmentIndex?: number) {
    if (typeof attachmentIndex === "number") return false;
    if (!attachmentUrl) return false;
    return this.normalizeAttachmentUrl(attachment.url) === this.normalizeAttachmentUrl(attachmentUrl);
  }

  private resolveLocalUploadPath(url: unknown) {
    const normalized = this.normalizeAttachmentUrl(url);
    const prefix = "/uploads/chat/";
    if (!normalized.startsWith(prefix)) return null;

    const fileName = path.basename(decodeURIComponent(normalized.slice(prefix.length)));
    if (!fileName) return null;

    const filePath = path.resolve(uploadRoot, fileName);
    if (!filePath.startsWith(`${uploadRoot}${path.sep}`)) return null;
    return filePath;
  }

  private async deleteLocalUploadIfUnused(url: unknown) {
    const filePath = this.resolveLocalUploadPath(url);
    if (!filePath) return;

    const normalized = this.normalizeAttachmentUrl(url);
    const stillUsed = await Message.exists({
      isActive: 1,
      attachments: {
        $elemMatch: {
          url: normalized,
          recalledAt: { $exists: false },
        },
      },
    });
    if (stillUsed) return;

    try {
      await fs.unlink(filePath);
    } catch (error) {
      if ((error as NodeJS.ErrnoException).code !== "ENOENT") {
        logger.warn("Unable to delete recalled attachment file", {
          url: normalized,
          filePath,
          error: serializeError(error),
        });
      }
    }
  }

  async assertMember(conversationId: string, userId: string) {
    const member = await ConversationMember.findOne({
      conversationId,
      userId,
      isActive: 1,
    }).lean();

    if (!member) throw new Error("You are not a member of this conversation.");
    return member;
  }

  async assertManager(conversationId: string, userId: string) {
    const member = await this.assertMember(conversationId, userId);
    if (!["owner", "admin"].includes(String(member.role)))
      throw new Error("You do not have permission to manage this group.");
    return member;
  }

  async ensureAiConversation(actor: AuthUser) {
    let conversation = await Conversation.findOne({
      type: "ai",
      createdBy: actor.id,
      companyId: actor.companyId,
      groupId: actor.groupId,
      isActive: 1,
    });

    if (!conversation) {
      conversation = await Conversation.create({
        type: "ai",
        title: env.AI_ASSISTANT_NAME,
        companyId: actor.companyId,
        groupId: actor.groupId,
        createdBy: actor.id,
        lastMessageAt: new Date(),
      });
    }

    await ConversationMember.updateOne(
      { conversationId: conversation._id, userId: actor.id },
      {
        $set: {
          userName: this.displayName(actor),
          role: "owner",
          isActive: 1,
          removedAt: null,
          hiddenAt: null,
        },
        $setOnInsert: { joinedAt: new Date() },
      },
      { upsert: true },
    );

    return conversation.toObject();
  }

  async isAiConversation(conversationId: string, actorId?: string) {
    const query: Record<string, unknown> = {
      _id: conversationId,
      type: "ai",
      isActive: 1,
    };
    if (actorId) query.createdBy = actorId;
    return Boolean(await Conversation.exists(query));
  }

  async createPrivateConversation(
    actor: AuthUser,
    targetUserId: string,
    targetUserName: string,
  ) {
    if (actor.id === targetUserId)
      throw new Error("Cannot create a private conversation with yourself.");

    const existing = await ConversationMember.aggregate([
      { $match: { userId: { $in: [actor.id, targetUserId] }, isActive: 1 } },
      {
        $group: {
          _id: "$conversationId",
          users: { $addToSet: "$userId" },
          count: { $sum: 1 },
        },
      },
      { $match: { users: { $all: [actor.id, targetUserId] }, count: 2 } },
      {
        $lookup: {
          from: "conversations",
          localField: "_id",
          foreignField: "_id",
          as: "conversation",
        },
      },
      { $unwind: "$conversation" },
      {
        $match: { "conversation.type": "private", "conversation.isActive": 1 },
      },
      { $limit: 1 },
    ]);

    if (existing[0]?._id) return existing[0]._id;

    const conversation = await Conversation.create({
      type: "private",
      companyId: actor.companyId,
      groupId: actor.groupId,
      createdBy: actor.id,
      lastMessageAt: new Date(),
    });

    await ConversationMember.insertMany([
      {
        conversationId: conversation._id,
        userId: actor.id,
        userName: this.displayName(actor),
        role: "owner",
      },
      {
        conversationId: conversation._id,
        userId: targetUserId,
        userName: targetUserName,
        role: "member",
      },
    ]);

    return conversation._id;
  }

  async createGroupConversation(
    actor: AuthUser,
    title: string,
    memberIds: Array<{ userId: string; userName: string }>,
  ) {
    const conversation = await Conversation.create({
      type: "group",
      title,
      companyId: actor.companyId,
      groupId: actor.groupId,
      createdBy: actor.id,
      lastMessageAt: new Date(),
    });

    const uniqueMembers = new Map(memberIds.map((m) => [m.userId, m]));
    uniqueMembers.set(actor.id, {
      userId: actor.id,
      userName: this.displayName(actor),
    });

    await ConversationMember.insertMany(
      Array.from(uniqueMembers.values()).map((member) => ({
        conversationId: conversation._id,
        userId: member.userId,
        userName: member.userName,
        role: member.userId === actor.id ? "owner" : "member",
      })),
    );

    return conversation;
  }

  async listConversations(actor: AuthUser, limit = 30, before?: string) {
    await this.ensureAiConversation(actor);

    const membership = await ConversationMember.find({ userId: actor.id, isActive: 1 })
      .select('conversationId hiddenAt mutedUntil')
      .lean();
    const ids = membership.map((m) => m.conversationId);
    const hiddenByConversation = new Map(membership.map((m) => [String(m.conversationId), m.hiddenAt ? new Date(m.hiddenAt) : null]));
    const mutedUntilByConversation = new Map(membership.map((m) => [String(m.conversationId), m.mutedUntil ? new Date(m.mutedUntil) : null]));

    const query: Record<string, unknown> = { _id: { $in: ids }, isActive: 1 };
    if (before) query.lastMessageAt = { $lt: new Date(before) };

    const conversations = await Conversation.find(query)
      .sort({ lastMessageAt: -1 })
      .limit(Math.min(limit, 50))
      .lean();

    // Enhance conversations: for private ones, set title to the other user's display name
    // Also add last message content for display
    if (conversations.length > 0) {
      type ConversationMemberLean = Pick<ConversationMemberDocument, "conversationId" | "userId" | "userName">;
      // Fetch all active members for these conversation ids
      const conversationMembers = await ConversationMember.find({
        conversationId: { $in: ids },
        isActive: 1,
      }).select('conversationId userId userName').lean() as ConversationMemberLean[];

      // Group by conversationId
      const membersByConversation = new Map<string, ConversationMemberLean[]>();
      for (const member of conversationMembers) {
        const convId = String(member.conversationId);
        let members = membersByConversation.get(convId);
        if (!members) {
          members = [];
          membersByConversation.set(convId, members);
        }
        members.push(member);
      }

      // Collect all userIds from these members
      const userIds = Array.from(new Set(
        conversationMembers.map(m => m.userId)
      ));

      // Fetch user profiles for these userIds
      const profiles = await findExistingUsersByIds(userIds);
      const profileById = new Map(profiles.map(p => [p.id, p]));

      // Fetch last message content for conversations that have one
      const lastMessageIds = conversations
        .map(c => c.lastMessageId)
        .filter((id): id is Types.ObjectId => id != null);
      const lastMessages = lastMessageIds.length > 0
        ? await Message.find({ _id: { $in: lastMessageIds } })
          .select('_id body type attachments createdAt')
          .lean()
        : [];
      const lastMessageById = new Map(lastMessages.map(m => [m._id.toString(), m]));

      // Now, enhance each conversation
      for (const conversation of conversations) {
        if (conversation.type === "ai") {
          conversation.title = conversation.title || env.AI_ASSISTANT_NAME;
          (conversation as any).isAi = true;
        }

        // Add last message content if available
        if (conversation.lastMessageId) {
          const lastMessage = lastMessageById.get(conversation.lastMessageId.toString());
          if (lastMessage) {
            // Using type assertion to add lastMessage property
            (conversation as any).lastMessage = this.filterMessageForUser(lastMessage, actor.id);
          }
        }

        // For private conversations, set title to the other user's display name
        if (conversation.type === "private") {
          const membersForConvo = membersByConversation.get(String(conversation._id)) || [];
          // Find the member that is not the actor
          const otherMember = membersForConvo.find(m => m.userId !== actor.id);
          if (otherMember) {
            const profile = profileById.get(otherMember.userId);
            conversation.title = profile
              ? this.displayName(profile)
            : otherMember.userName;
          }
        }

        const mutedUntil = mutedUntilByConversation.get(String(conversation._id));
        (conversation as any).mutedUntil = mutedUntil;
        (conversation as any).notificationsEnabled = !mutedUntil || mutedUntil <= new Date();
      }
    }

    return conversations.filter((conversation) => {
      const hiddenAt = hiddenByConversation.get(String(conversation._id));
      if (!hiddenAt) return true;
      const lastMessageAt = conversation.lastMessageAt ? new Date(conversation.lastMessageAt) : null;
      return Boolean(lastMessageAt && lastMessageAt > hiddenAt);
    });
  }
  


  async listMessages(
    actor: AuthUser,
    conversationId: string,
    limit = 30,
    before?: string,
  ) {
    await this.assertMember(conversationId, actor.id);

    const query: Record<string, unknown> = {
      conversationId: new Types.ObjectId(conversationId),
      isActive: 1,
      deletedForUserIds: { $ne: actor.id },
    };
    if (before) query.createdAt = { $lt: new Date(before) };

    const messages = await Message.find(query)
      .sort({ _id: -1 })
      .limit(Math.min(limit, 50))
      .lean();

    const ids = messages.map((message) => message._id);
    const replyIds = messages
      .map((message) => message.replyToMessageId)
      .filter(Boolean);
    const [reactions, replies] = await Promise.all([
      MessageReaction.find({ messageId: { $in: ids }, isActive: 1 })
        .select("messageId userId emoji createdAt")
        .lean(),
      Message.find({ _id: { $in: replyIds } })
        .select("senderUserName body attachments")
        .lean(),
    ]);
    const reactionsByMessage = reactions.reduce<
      Record<string, typeof reactions>
    >((acc, reaction) => {
      const key = String(reaction.messageId);
      acc[key] = acc[key] || [];
      acc[key].push(reaction);
      return acc;
    }, {});
    const repliesById = new Map(
      replies.map((reply) => [String(reply._id), reply]),
    );

    return messages.map((message) => this.filterMessageForUser({
      ...message,
      reactions: reactionsByMessage[String(message._id)] || [],
      replyTo: message.replyToMessageId
        ? repliesById.get(String(message.replyToMessageId))
        : null,
    }, actor.id));
  }

  async listMembers(actor: AuthUser, conversationId: string) {
    await this.assertMember(conversationId, actor.id);
    const members = await ConversationMember.find({
      conversationId,
      isActive: 1,
    })
      .select("userId userName role lastReadMessageId lastReadAt")
      .sort({ role: 1, userName: 1 })
      .lean();
    const profiles = await findExistingUsersByIds(
      members.map((member) => member.userId),
    );
    const profileById = new Map(
      profiles.map((profile) => [profile.id, profile]),
    );

    return members.map((member) => {
      const profile = profileById.get(member.userId);
      return profile
        ? { ...member, userName: this.displayName(profile) }
        : member;
    });
  }

  async sendMessage(
    actor: AuthUser,
    input: {
      conversationId: string;
      clientMessageId: string;
      body?: string;
      type?: "text" | "image" | "file" | "voice" | "video";
      replyToMessageId?: string;
      attachments?: unknown[];
      mentionedUserIds?: string[];
    },
  ) {
    await this.assertMember(input.conversationId, actor.id);

    const message = await Message.create({
      conversationId: input.conversationId,
      clientMessageId: input.clientMessageId,
      senderId: actor.id,
      senderUserName: this.displayName(actor),
      body: input.body ?? "",
      type: input.type ?? "text",
      replyToMessageId: input.replyToMessageId,
      attachments: input.attachments ?? [],
      mentionedUserIds: input.mentionedUserIds ?? [],
    });

    await Conversation.updateOne(
      { _id: input.conversationId },
      {
        $set: { lastMessageId: message._id, lastMessageAt: message.createdAt },
      },
    );

    return message.toObject();
  }

  private async createAiAssistantMessage(conversationId: string, body: string) {
    const message = await Message.create({
      conversationId,
      clientMessageId: `ai-${new Types.ObjectId().toString()}`,
      senderId: AI_USER_ID,
      senderUserName: env.AI_ASSISTANT_NAME,
      body,
      type: "text",
      mentionedUserIds: [],
    });

    await Conversation.updateOne(
      { _id: conversationId },
      {
        $set: { lastMessageId: message._id, lastMessageAt: message.createdAt },
      },
    );

    return message.toObject();
  }

  private async buildAiHistory(conversationId: string) {
    const history = await Message.find({
      conversationId,
      isActive: 1,
      recalledAt: { $exists: false },
      body: { $ne: "" },
    })
      .select("senderId body createdAt")
      .sort({ _id: -1 })
      .limit(env.AI_MAX_HISTORY_MESSAGES)
      .lean();

    return history.reverse().map((message) => ({
      role: message.senderId === AI_USER_ID ? "assistant" as const : "user" as const,
      content: String(message.body ?? ""),
    }));
  }

  async sendAiMessage(
    actor: AuthUser,
    input: {
      conversationId?: string;
      clientMessageId?: string;
      body?: string;
    },
  ) {
    const conversation = input.conversationId
      ? await Conversation.findOne({
          _id: input.conversationId,
          type: "ai",
          createdBy: actor.id,
          isActive: 1,
        }).lean()
      : await this.ensureAiConversation(actor);

    if (!conversation) throw new Error("AI conversation not found.");
    const body = String(input.body ?? "").trim();
    if (!body) throw new Error("Message body is required.");
    if (!this.ai.isEnabled()) throw new Error("AI provider is not configured.");

    const userMessage = await this.sendMessage(actor, {
      conversationId: String(conversation._id),
      clientMessageId: input.clientMessageId || `ai-user-${new Types.ObjectId().toString()}`,
      body,
      type: "text",
      mentionedUserIds: [],
      attachments: [],
    });

    const history = await this.buildAiHistory(String(conversation._id));
    const reply = await this.ai.generateReply({ messages: history });
    const assistantMessage = await this.createAiAssistantMessage(String(conversation._id), reply);

    return {
      conversationId: String(conversation._id),
      userMessage,
      assistantMessage,
    };
  }

  async sendSystemMessage(
    conversationId: string,
    body: string,
    actor?: AuthUser,
  ) {
    const message = await Message.create({
      conversationId,
      clientMessageId: `system-${new Types.ObjectId().toString()}`,
      senderId: actor?.id || "system",
      senderUserName: actor ? this.displayName(actor) : "System",
      body,
      type: "system",
    });

    await Conversation.updateOne(
      { _id: conversationId },
      {
        $set: { lastMessageId: message._id, lastMessageAt: message.createdAt },
      },
    );

    return message.toObject();
  }

  async updateConversation(
    actor: AuthUser,
    conversationId: string,
    input: { title?: string; avatarUrl?: string },
  ) {
    await this.assertManager(conversationId, actor.id);
    const patch: Record<string, unknown> = {};
    if (typeof input.title === "string") {
      const title = input.title.trim();
      if (title.length < 3)
        throw new Error("Group name must be at least 3 characters.");
      patch.title = title.slice(0, 80);
    }
    if (typeof input.avatarUrl === "string")
      patch.avatarUrl = input.avatarUrl.trim();
    await Conversation.updateOne(
      { _id: conversationId, type: "group", isActive: 1 },
      { $set: patch },
    );
    return Conversation.findById(conversationId).lean();
  }

  async addMembers(
    actor: AuthUser,
    conversationId: string,
    members: Array<{ userId: string; userName: string }>,
  ) {
    await this.assertManager(conversationId, actor.id);
    const conversation = await Conversation.findOne({
      _id: conversationId,
      type: "group",
      isActive: 1,
    }).lean();
    if (!conversation) throw new Error("Group not found.");
    const uniqueMembers = Array.from(
      new Map(
        members.filter((m) => m.userId).map((m) => [m.userId, m]),
      ).values(),
    );
    await Promise.all(
      uniqueMembers.map((member) =>
        ConversationMember.updateOne(
          { conversationId, userId: member.userId },
          {
            $set: {
              userName: member.userName,
              role: "member",
              isActive: 1,
              removedAt: null,
              joinedAt: new Date(),
            },
          },
          { upsert: true },
        ),
      ),
    );
    return this.listMembers(actor, conversationId);
  }

  async removeMember(actor: AuthUser, conversationId: string, userId: string) {
    const manager = await this.assertManager(conversationId, actor.id);
    const target = await ConversationMember.findOne({
      conversationId,
      userId,
      isActive: 1,
    }).lean();
    if (!target) throw new Error("Member not found.");
    if (target.role === "owner") throw new Error("Cannot remove group owner.");
    if (manager.role !== "owner" && target.role === "admin")
      throw new Error("Only owner can remove an admin.");
    await ConversationMember.updateOne(
      { conversationId, userId },
      { $set: { isActive: 0, removedAt: new Date() } },
    );
    return this.listMembers(actor, conversationId);
  }

  async leaveGroup(actor: AuthUser, conversationId: string) {
    const conversation = await Conversation.findOne({
      _id: conversationId,
      type: "group",
      isActive: 1,
    }).lean();
    if (!conversation) throw new Error("Group not found.");

    const member = await ConversationMember.findOne({
      conversationId,
      userId: actor.id,
      isActive: 1,
    });
    if (!member) throw new Error("You are not a member of this group.");

    const activeMembers = await ConversationMember.find({
      conversationId,
      isActive: 1,
    })
      .sort({ role: 1, joinedAt: 1 })
      .lean();
    const remaining = activeMembers.filter((item) => item.userId !== actor.id);
    const leftAt = new Date();

    if (member.role === "owner" && remaining.length > 0) {
      const nextOwner =
        remaining.find((item) => item.role === "admin") || remaining[0];
      await ConversationMember.updateOne(
        { conversationId, userId: nextOwner.userId },
        { $set: { role: "owner" } },
      );
    }

    await ConversationMember.updateOne(
      { conversationId, userId: actor.id },
      { $set: { isActive: 0, removedAt: leftAt } },
    );

    if (!remaining.length) {
      await Conversation.updateOne(
        { _id: conversationId },
        { $set: { isActive: 0, deletedAt: leftAt } },
      );
    }

    const systemMessage = remaining.length
      ? await this.sendSystemMessage(
          conversationId,
          `${this.displayName(actor)} đã rời nhóm`,
          actor,
        )
      : null;

    const members = await ConversationMember.find({
      conversationId,
      isActive: 1,
    })
      .select("userId userName role lastReadMessageId lastReadAt")
      .sort({ role: 1, userName: 1 })
      .lean();
    const profiles = await findExistingUsersByIds(
      members.map((member) => member.userId),
    );
    const profileById = new Map(
      profiles.map((profile) => [profile.id, profile]),
    );
    const displayMembers = members.map((member) => {
      const profile = profileById.get(member.userId);
      return profile
        ? { ...member, userName: this.displayName(profile) }
        : member;
    });
    const updatedConversation =
      await Conversation.findById(conversationId).lean();

    return {
      conversation: updatedConversation,
      members: displayMembers,
      leftUserId: actor.id,
      leftUserName: actor.userName,
      conversationId,
      isConversationActive: Boolean(updatedConversation?.isActive),
      systemMessage,
    };
  }

  async changeMemberRole(
    actor: AuthUser,
    conversationId: string,
    userId: string,
    role: "admin" | "member",
  ) {
    const manager = await this.assertManager(conversationId, actor.id);
    if (manager.role !== "owner")
      throw new Error("Only owner can change member roles.");
    if (!["admin", "member"].includes(role)) throw new Error("Invalid role.");
    await ConversationMember.updateOne(
      { conversationId, userId, role: { $ne: "owner" } },
      { $set: { role } },
    );
    return this.listMembers(actor, conversationId);
  }

  async deleteMessage(
    actor: AuthUser,
    conversationId: string,
    messageId: string,
    scope: "me" | "everyone",
  ) {
    await this.assertMember(conversationId, actor.id);
    const message = await Message.findOne({
      _id: messageId,
      conversationId,
      isActive: 1,
    });
    if (!message) throw new Error("Message not found.");
    if (scope === "everyone") {
      const member = await this.assertMember(conversationId, actor.id);
      const canDelete =
        message.senderId === actor.id ||
        ["owner", "admin"].includes(String(member.role)) ||
        this.isSystemAdmin(actor);
      if (!canDelete) throw new Error("Cannot recall this message.");
      const recalledAt = new Date();
      await Message.updateOne(
        { _id: messageId },
        { $set: { recalledAt, body: "", attachments: [] } },
      );
      await Promise.all(
        (message.attachments as MessageAttachment[]).map((attachment) =>
          this.deleteLocalUploadIfUnused(attachment.url),
        ),
      );
      const recalled = await Message.findById(messageId).lean();
      return { scope, message: recalled };
    }
    await Message.updateOne(
      { _id: messageId },
      { $addToSet: { deletedForUserIds: actor.id } },
    );
    return { scope, messageId };
  }

  async deleteMessageAttachment(
    actor: AuthUser,
    conversationId: string,
    messageId: string,
    input: {
      attachmentUrl?: string;
      attachmentIndex?: number;
      scope: "me" | "everyone";
    },
  ) {
    await this.assertMember(conversationId, actor.id);
    const message = await Message.findOne({
      _id: messageId,
      conversationId,
      isActive: 1,
    });
    if (!message) throw new Error("Message not found.");
    if (message.recalledAt) throw new Error("Cannot delete attachment from a recalled message.");

    const attachments = ((message.attachments ?? []) as MessageAttachment[]).map((attachment) => ({
      ...attachment,
      deletedForUserIds: attachment.deletedForUserIds ?? [],
    }));
    const normalizedUrl = this.normalizeAttachmentUrl(input.attachmentUrl);
    const attachmentIndex = typeof input.attachmentIndex === "number" ? input.attachmentIndex : -1;
    const targetIndex = attachmentIndex >= 0
      ? attachmentIndex
      : attachments.findIndex((attachment) => this.attachmentMatches(attachment, normalizedUrl));
    const target = attachments[targetIndex];
    if (!target) throw new Error("Attachment not found.");

    if (input.scope === "everyone") {
      const member = await this.assertMember(conversationId, actor.id);
      const canDelete =
        message.senderId === actor.id ||
        ["owner", "admin"].includes(String(member.role)) ||
        this.isSystemAdmin(actor);
      if (!canDelete) throw new Error("Cannot recall this attachment.");

      const remainingAttachments = attachments.filter((_, index) => index !== targetIndex);
      const patch: Record<string, unknown> = { attachments: remainingAttachments };
      const shouldRecallMessage = !String(message.body ?? "").trim() && remainingAttachments.length === 0;
      if (shouldRecallMessage) {
        patch.recalledAt = new Date();
      }

      await Message.updateOne({ _id: messageId }, { $set: patch });
      await this.deleteLocalUploadIfUnused(target.url);
    } else {
      const deletedForUserIds = Array.from(new Set([...(target.deletedForUserIds ?? []), actor.id]));
      attachments[targetIndex] = { ...target, deletedForUserIds };
      await Message.updateOne({ _id: messageId }, { $set: { attachments } });
    }

    const updated = await Message.findById(messageId).lean();
    return {
      scope: input.scope,
      attachmentUrl: target.url,
      attachmentIndex: targetIndex,
      message: updated ? this.filterMessageForUser(updated, actor.id) : updated,
    };
  }

  async deleteConversationForMe(actor: AuthUser, conversationId: string) {
    await this.assertMember(conversationId, actor.id);
    const hiddenAt = new Date();
    await Promise.all([
      ConversationMember.updateOne(
        { conversationId, userId: actor.id, isActive: 1 },
        { $set: { hiddenAt } },
      ),
      Message.updateMany(
        {
          conversationId: new Types.ObjectId(conversationId),
          isActive: 1,
          createdAt: { $lte: hiddenAt },
        },
        { $addToSet: { deletedForUserIds: actor.id } },
      ),
    ]);

    return { conversationId, hiddenAt };
  }

  async editMessage(
    actor: AuthUser,
    conversationId: string,
    messageId: string,
    body: string,
  ) {
    await this.assertMember(conversationId, actor.id);
    const message = await Message.findOne({
      _id: messageId,
      conversationId,
      senderId: actor.id,
      isActive: 1,
    });
    if (!message) throw new Error("Message not found or not editable.");
    if (message.recalledAt) throw new Error("Cannot edit a recalled message.");
    message.body = String(body || "").trim();
    message.editedAt = new Date();
    await message.save();
    return message.toObject();
  }

  async reactMessage(
    actor: AuthUser,
    conversationId: string,
    messageId: string,
    emoji: string,
  ) {
    await this.assertMember(conversationId, actor.id);
    const allowed = ["👍", "❤️", "😂", "😮", "😢", "😡"];
    if (!allowed.includes(emoji)) throw new Error("Unsupported reaction.");
    await MessageReaction.updateOne(
      { messageId, conversationId, userId: actor.id, emoji },
      { $set: { isActive: 1 } },
      { upsert: true },
    );
    const reactions = await MessageReaction.find({ messageId, isActive: 1 })
      .select("messageId userId emoji createdAt")
      .lean();
    return { reactions };
  }

  async pinMessage(
    actor: AuthUser,
    conversationId: string,
    messageId: string,
    pinned: boolean,
  ) {
    await this.assertManager(conversationId, actor.id);
    const pinnedAt = pinned ? new Date() : null;
    const message = await Message.findOneAndUpdate(
      { _id: messageId, conversationId, isActive: 1 },
      { $set: { pinnedAt } },
      { new: true },
    ).lean();
    if (!message) throw new Error("Message not found.");
    return message;
  }

  async forwardMessage(
    actor: AuthUser,
    sourceConversationId: string,
    messageId: string,
    targetConversationId: string,
  ) {
    await this.assertMember(sourceConversationId, actor.id);
    await this.assertMember(targetConversationId, actor.id);
    const source = await Message.findOne({
      _id: messageId,
      conversationId: sourceConversationId,
      isActive: 1,
    }).lean();
    if (!source || source.recalledAt) throw new Error("Message not found.");
    const attachments = this.visibleAttachments(source.attachments as MessageAttachment[], actor.id);
    const type = ["text", "image", "file", "voice", "video"].includes(
      String(source.type),
    )
      ? (source.type as "text" | "image" | "file" | "voice" | "video")
      : "text";
    return this.sendMessage(actor, {
      conversationId: targetConversationId,
      clientMessageId: `forward-${new Types.ObjectId().toString()}`,
      body: source.body,
      type,
      attachments,
      mentionedUserIds: [],
    });
  }

  async markSeen(actor: AuthUser, conversationId: string, messageId: string) {
    await this.assertMember(conversationId, actor.id);
    await MessageRead.updateOne(
      { conversationId, messageId, userId: actor.id },
      { $set: { readAt: new Date() } },
      { upsert: true },
    );
    await ConversationMember.updateOne(
      { conversationId, userId: actor.id },
      { $set: { lastReadMessageId: messageId, lastReadAt: new Date() } },
    );
  }

  async setConversationNotifications(
    actor: AuthUser,
    conversationId: string,
    enabled: boolean,
  ) {
    await this.assertMember(conversationId, actor.id);
    const mutedUntil = enabled ? null : MUTED_FOREVER;

    await ConversationMember.updateOne(
      { conversationId, userId: actor.id, isActive: 1 },
      { $set: { mutedUntil } },
    );

    return {
      conversationId,
      notificationsEnabled: enabled,
      mutedUntil,
    };
  }

  async activeMemberIds(conversationId: string) {
    const members = await ConversationMember.find({
      conversationId,
      isActive: 1,
    })
      .select("userId")
      .lean();
    return members.map((m) => m.userId);
  }
}
