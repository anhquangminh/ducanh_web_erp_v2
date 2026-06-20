import { Types } from 'mongoose';
import { env } from '../config/env.ts';
import { Conversation, ConversationMember } from '../models/conversation.model.ts';
import { logger, serializeError } from '../utils/logger.ts';

type ChatMessageType = 'text' | 'image' | 'file' | 'voice' | 'video' | 'system';

export type ChatNotificationMessageData = {
  conversationId: string;
  senderId: string;
  senderUserName: string;
  body?: string;
  type?: ChatMessageType;
  mentionedUserIds?: string[];
  activeConversationUserIds?: string[];
  authorizationBearerToken?: string;
};

function notificationBody(messageData: ChatNotificationMessageData) {
  switch (messageData.type) {
    case 'image':
      return '\u0110\u00e3 g\u1eedi m\u1ed9t \u1ea3nh';
    case 'file':
      return '\u0110\u00e3 g\u1eedi m\u1ed9t t\u1ec7p';
    case 'voice':
      return '\u0110\u00e3 g\u1eedi m\u1ed9t tin nh\u1eafn tho\u1ea1i';
    case 'video':
      return '\u0110\u00e3 g\u1eedi m\u1ed9t video';
    default:
      return messageData.body?.trim() || 'Tin nh\u1eafn m\u1edbi';
  }
}

async function sendViaNotificationApi(input: {
  userIds: string[];
  title: string;
  body: string;
  targetPage: string;
  targetId: string;
  clientBearerToken?: string;
}) {
  if (!env.FCM_NOTIFICATION_API_URL) return false;

  const { clientBearerToken, ...payload } = input;
  const headers: Record<string, string> = {
    'Content-Type': 'application/json'
  };
  if (clientBearerToken) {
    headers.Authorization = `Bearer ${clientBearerToken}`;
  }

  const response = await fetch(env.FCM_NOTIFICATION_API_URL, {
    method: 'POST',
    headers,
    body: JSON.stringify(payload),
    signal: env.FCM_NOTIFICATION_API_TIMEOUT_MS > 0
      ? AbortSignal.timeout(env.FCM_NOTIFICATION_API_TIMEOUT_MS)
      : undefined
  });

  const responseBody = await response.text();
  if (!response.ok) {
    throw new Error(`Notification API failed with ${response.status}: ${responseBody}`);
  }

  logger.info('Chat notification API called', {
    targetId: payload.targetId,
    userCount: payload.userIds.length,
    status: response.status
  });

  return true;
}

export async function sendChatNotification(messageData: ChatNotificationMessageData) {
  if (!Types.ObjectId.isValid(messageData.conversationId)) return;

  try {
    const conversation = await Conversation.findOne({
      _id: messageData.conversationId,
      isActive: 1
    }).lean();
    if (!conversation) return;

    const activeConversationUserIds = new Set(messageData.activeConversationUserIds ?? []);
    const members = await ConversationMember.find({
      conversationId: messageData.conversationId,
      isActive: 1,
      userId: { $ne: messageData.senderId },
      $or: [{ mutedUntil: null }, { mutedUntil: { $lte: new Date() } }]
    })
      .select('userId userName')
      .lean();

    const memberIds = new Set(members.map((member) => member.userId));
    const mentionedUserIds = new Set(messageData.mentionedUserIds ?? []);
    const shouldPrioritizeMentions = conversation.type === 'group' && mentionedUserIds.size > 0;
    const recipientIds = members
      .map((member) => member.userId)
      .filter((userId) => !activeConversationUserIds.has(userId))
      .filter((userId) => !shouldPrioritizeMentions || mentionedUserIds.has(userId))
      .filter((userId) => memberIds.has(userId));

    if (!recipientIds.length) return;

    const title = conversation.type === 'group'
      ? (conversation.title || messageData.senderUserName)
      : messageData.senderUserName;

    await sendViaNotificationApi({
      userIds: recipientIds,
      title,
      body: notificationBody(messageData),
      targetPage: 'chat',
      targetId: messageData.conversationId,
      clientBearerToken: messageData.authorizationBearerToken
    });
  } catch (error) {
    logger.error('Cannot send chat notification', {
      conversationId: messageData.conversationId,
      error: serializeError(error)
    });
  }
}
