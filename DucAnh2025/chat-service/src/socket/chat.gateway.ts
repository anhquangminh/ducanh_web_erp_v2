import { Server } from 'socket.io';
import { createAdapter } from '@socket.io/redis-adapter';
import { createClient } from 'redis';
import { env } from '../config/env.ts';
import { socketAuth } from '../auth/jwt.ts';
import { OnlineUser } from '../models/presence.model.ts';
import { ChatService } from '../services/chat.service.ts';
import { sendChatNotification } from '../services/notification.service.ts';
import { enrichMessageAttachmentUrls } from '../utils/attachmentUrls.ts';
import { logger, serializeError } from '../utils/logger.ts';

const chat = new ChatService();

function conversationRoom(id: string) {
  return `conversation:${id}`;
}

function userRoom(userId: string) {
  return `user:${userId}`;
}

export async function configureChatGateway(io: Server) {
  if (env.REDIS_URL) {
    try {
      const pub = createClient({ url: env.REDIS_URL });
      const sub = pub.duplicate();
      pub.on('error', (error) => logger.error('Redis pub client error', { error: serializeError(error) }));
      sub.on('error', (error) => logger.error('Redis sub client error', { error: serializeError(error) }));
      await Promise.all([pub.connect(), sub.connect()]);
      io.adapter(createAdapter(pub, sub));
      logger.info('Socket.IO Redis adapter connected', { redisUrl: env.REDIS_URL });
    } catch (error) {
      logger.warn('Redis adapter disabled because Redis connection failed', {
        redisUrl: env.REDIS_URL,
        error: serializeError(error)
      });
    }
  } else {
    logger.info('Socket.IO Redis adapter disabled for local single-node mode');
  }

  const nsp = io.of('/chat');
  nsp.use(socketAuth);

  nsp.on('connection', async (socket) => {
    const user = socket.data.user;
    socket.join(userRoom(user.id));

    await OnlineUser.updateOne(
      { socketId: socket.id },
      {
        $set: {
          userId: user.id,
          socketId: socket.id,
          companyId: user.companyId,
          groupId: user.groupId,
          lastSeenAt: new Date(),
          expiresAt: new Date(Date.now() + 60_000)
        }
      },
      { upsert: true }
    );

    socket.broadcast.emit('presence:online', { userId: user.id });

    socket.on('conversation:join', async ({ conversationId }, ack) => {
      try {
        await chat.assertMember(conversationId, user.id);
        socket.join(conversationRoom(conversationId));
        ack?.({ success: true });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Forbidden' });
      }
    });

    socket.on('message:send', async (payload, ack) => {
      try {
        const message = await chat.sendMessage(user, payload);
        const messagePayload = env.CHAT_PUBLIC_BASE_URL
          ? enrichMessageAttachmentUrls(env.CHAT_PUBLIC_BASE_URL, message)
          : message;
        const members = await chat.activeMemberIds(payload.conversationId);
        nsp.to(conversationRoom(payload.conversationId)).emit('message:new', messagePayload);
        members.forEach((userId) => {
          nsp.to(userRoom(userId)).emit('notification:new', {
            type: 'chat.message',
            conversationId: payload.conversationId,
            messageId: message._id
          });
        });
        ack?.({ success: true, data: messagePayload });

        void (async () => {
          const socketsInConversation = await nsp.in(conversationRoom(payload.conversationId)).fetchSockets();
          const activeConversationUserIds = Array.from(new Set(
            socketsInConversation
              .map((item) => item.data.user?.id)
              .filter((id): id is string => Boolean(id))
          ));

          await sendChatNotification({
            conversationId: String(message.conversationId),
            senderId: message.senderId,
            senderUserName: message.senderUserName,
            body: message.body,
            type: message.type,
            mentionedUserIds: message.mentionedUserIds,
            activeConversationUserIds,
            authorizationBearerToken: socket.data.authToken
          });
        })();
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot send message' });
      }
    });

    socket.on('message:seen', async ({ conversationId, messageId }, ack) => {
      try {
        await chat.markSeen(user, conversationId, messageId);
        nsp.to(conversationRoom(conversationId)).emit('message:seen', {
          conversationId,
          messageId,
          userId: user.id,
          seenAt: new Date().toISOString()
        });
        ack?.({ success: true });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot mark seen' });
      }
    });

    socket.on('message:delete', async ({ conversationId, messageId, scope }, ack) => {
      try {
        const result = await chat.deleteMessage(user, conversationId, messageId, scope === 'everyone' ? 'everyone' : 'me');
        const eventPayload = {
          conversationId,
          messageId,
          scope: result.scope,
          userId: user.id,
          deletedAt: new Date().toISOString(),
          message: result.message
        };

        if (result.scope === 'everyone') {
          nsp.to(conversationRoom(conversationId)).emit('message:deleted', eventPayload);
        } else {
          nsp.to(userRoom(user.id)).emit('message:deleted', eventPayload);
        }
        ack?.({ success: true, data: result });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot delete message' });
      }
    });

    socket.on('message:attachment:delete', async ({ conversationId, messageId, attachmentUrl, attachmentIndex, scope }, ack) => {
      try {
        const result = await chat.deleteMessageAttachment(user, conversationId, messageId, {
          attachmentUrl,
          attachmentIndex: Number.isInteger(attachmentIndex) ? attachmentIndex : undefined,
          scope: scope === 'everyone' ? 'everyone' : 'me'
        });
        const message = env.CHAT_PUBLIC_BASE_URL && result.message
          ? enrichMessageAttachmentUrls(env.CHAT_PUBLIC_BASE_URL, result.message)
          : result.message;
        const eventPayload = {
          conversationId,
          messageId,
          scope: result.scope,
          userId: user.id,
          attachmentUrl: result.attachmentUrl,
          attachmentIndex: result.attachmentIndex,
          deletedAt: new Date().toISOString(),
          message
        };

        if (result.scope === 'everyone') {
          nsp.to(conversationRoom(conversationId)).emit('message:attachment:deleted', eventPayload);
        } else {
          nsp.to(userRoom(user.id)).emit('message:attachment:deleted', eventPayload);
        }
        ack?.({ success: true, data: { ...result, message } });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot delete attachment' });
      }
    });

    socket.on('message:edit', async ({ conversationId, messageId, body }, ack) => {
      try {
        const message = await chat.editMessage(user, conversationId, messageId, body);
        nsp.to(conversationRoom(conversationId)).emit('message:edited', {
          conversationId,
          messageId,
          message
        });
        ack?.({ success: true, data: message });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot edit message' });
      }
    });

    socket.on('message:reaction', async ({ conversationId, messageId, emoji }, ack) => {
      try {
        const result = await chat.reactMessage(user, conversationId, messageId, emoji);
        nsp.to(conversationRoom(conversationId)).emit('message:reaction', {
          conversationId,
          messageId,
          reactions: result.reactions
        });
        ack?.({ success: true, data: result });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot react message' });
      }
    });

    socket.on('message:pin', async ({ conversationId, messageId, pinned }, ack) => {
      try {
        const message = await chat.pinMessage(user, conversationId, messageId, Boolean(pinned));
        nsp.to(conversationRoom(conversationId)).emit('message:pinned', {
          conversationId,
          messageId,
          pinnedAt: message.pinnedAt
        });
        ack?.({ success: true, data: message });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot pin message' });
      }
    });

    socket.on('conversation:update', async ({ conversationId, title, avatarUrl }, ack) => {
      try {
        const conversation = await chat.updateConversation(user, conversationId, { title, avatarUrl });
        const members = await chat.listMembers(user, conversationId);
        nsp.to(conversationRoom(conversationId)).emit('conversation:updated', {
          conversationId,
          conversation,
          members
        });
        ack?.({ success: true, data: { conversation, members } });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot update conversation' });
      }
    });

    socket.on('conversation:members:add', async ({ conversationId, members }, ack) => {
      try {
        const updatedMembers = await chat.addMembers(user, conversationId, members ?? []);
        nsp.to(conversationRoom(conversationId)).emit('conversation:updated', {
          conversationId,
          members: updatedMembers
        });
        (members ?? []).forEach((member: { userId?: string }) => {
          if (member.userId) {
            nsp.to(userRoom(member.userId)).emit('notification:new', {
              type: 'chat.member_added',
              conversationId
            });
          }
        });
        ack?.({ success: true, data: updatedMembers });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot add members' });
      }
    });

    socket.on('conversation:leave', async ({ conversationId }, ack) => {
      try {
        const result = await chat.leaveGroup(user, conversationId);
        nsp.in(userRoom(user.id)).socketsLeave(conversationRoom(conversationId));
        nsp.to(conversationRoom(conversationId)).emit('conversation:updated', {
          conversationId,
          conversation: result.conversation,
          members: result.members,
          leftUserId: result.leftUserId
        });
        if (result.systemMessage) {
          nsp.to(conversationRoom(conversationId)).emit('message:new', result.systemMessage);
        }
        nsp.to(userRoom(user.id)).emit('conversation:left', result);
        ack?.({ success: true, data: result });
      } catch (error) {
        ack?.({ success: false, message: error instanceof Error ? error.message : 'Cannot leave group' });
      }
    });

    socket.on('message:typing', async ({ conversationId, isTyping }) => {
      try {
        await chat.assertMember(conversationId, user.id);
        socket.to(conversationRoom(conversationId)).emit('message:typing', {
          conversationId,
          userId: user.id,
          isTyping: Boolean(isTyping)
        });
      } catch {
        // Ignore unauthorized typing probes.
      }
    });

    socket.on('disconnect', async () => {
      await OnlineUser.deleteOne({ socketId: socket.id });
      const stillOnline = await OnlineUser.exists({ userId: user.id });
      if (!stillOnline) socket.broadcast.emit('presence:offline', { userId: user.id, lastSeenAt: new Date().toISOString() });
    });
  });
}
