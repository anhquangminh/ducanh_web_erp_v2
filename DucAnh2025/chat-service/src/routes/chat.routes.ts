import { Router } from 'express';
import fs from 'fs';
import path from 'path';
import crypto from 'crypto';
import multer from 'multer';
import { AuthError, authenticateInternalHeaders, authenticateToken, redactToken } from '../auth/jwt.ts';
import { env } from '../config/env.ts';
import { searchExistingUsers } from '../db/sql.ts';
import { ChatService } from '../services/chat.service.ts';
import { sendChatNotification } from '../services/notification.service.ts';
import { enrichAttachmentUrls, enrichMessageAttachmentUrls } from '../utils/attachmentUrls.ts';
import { logger, serializeError } from '../utils/logger.ts';

const safeParseInt = (value: unknown, defaultValue: number, min = 1, max = 100): number => {
  if (typeof value === 'string') {
    const num = parseInt(value, 10);
    if (!isNaN(num) && num >= min && num <= max) {
      return num;
    }
  }
  return defaultValue;
};

const router = Router();
const chat = new ChatService();
const uploadRoot = path.resolve(env.MEDIA_ROOT);

const asyncRoute = (handler: any) => (req: any, res: any, next: any) => {
  Promise.resolve(handler(req, res, next)).catch(next);
};

function requestBaseUrl(req: any) {
  return env.CHAT_PUBLIC_BASE_URL || `${req.protocol}://${req.get('host')}`;
}

function enrichAttachment(req: any, attachment: any) {
  return enrichAttachmentUrls(requestBaseUrl(req), attachment);
}

function enrichMessageFiles(req: any, message: any) {
  return enrichMessageAttachmentUrls(requestBaseUrl(req), message);
}

const storage = multer.diskStorage({
  destination: (_, __, cb) => {
    fs.mkdirSync(uploadRoot, { recursive: true });
    cb(null, uploadRoot);
  },
  filename: (_, file, cb) => {
    const ext = path.extname(file.originalname).toLowerCase();
    const safeBase = path.basename(file.originalname, ext).replace(/[^a-zA-Z0-9-_]/g, '-').slice(0, 60);
    cb(null, `${Date.now()}-${crypto.randomUUID()}-${safeBase}${ext}`);
  }
});

const uploader = multer({
  storage,
  limits: { fileSize: 100 * 1024 * 1024 },
  fileFilter: (_, file, cb) => {
    const allowed = /^(image\/|video\/|application\/pdf|text\/plain|application\/zip|application\/msword|application\/vnd\.ms-|application\/vnd\.openxmlformats|application\/octet-stream)/;
    cb(null, allowed.test(file.mimetype));
  }
});

async function auth(req: any, res: any, next: any) {
  try {
    const internalUser = authenticateInternalHeaders(req.headers);
    if (internalUser) {
      req.user = internalUser;
      next();
      return;
    }

    const token = String(req.headers.authorization ?? '').replace(/^Bearer\s+/i, '');
    logger.info('Chat REST auth attempt', {
      method: req.method,
      path: req.originalUrl,
      origin: req.headers.origin,
      hasAuthorizationHeader: Boolean(req.headers.authorization),
      authorizationScheme: req.headers.authorization ? String(req.headers.authorization).split(/\s+/)[0] : '',
      token: redactToken(token),
      hasCookie: Boolean(req.headers.cookie)
    });
    req.user = await authenticateToken(token);
    req.authToken = token;
    next();
  } catch (error) {
    logger.warn('Chat REST authentication failed', { error: serializeError(error) });
    const code = error instanceof AuthError ? error.code : 'AUTH_REQUIRED';
    res.status(401).json({
      success: false,
      message: env.NODE_ENV === 'production'
        ? 'Unauthorized'
        : (error instanceof Error ? error.message : 'Unauthorized'),
      code
    });
  }
}

router.use(auth);

router.get('/users/search', asyncRoute(async (req: any, res: any) => {
  const keyword = String(req.query.q ?? '').trim();
  if (keyword.length < 2) return res.json({ success: true, data: [] });

  const limit = safeParseInt(req.query.limit, 20);
  const data = await searchExistingUsers(req.user, keyword, limit);
  res.json({ success: true, data });
}));

router.get('/conversations', asyncRoute(async (req: any, res: any) => {
  const data = await chat.listConversations(req.user, Number(req.query.limit ?? 30), req.query.before as string | undefined);
  res.json({ success: true, data });
}));

router.get('/ai/status', asyncRoute(async (_req: any, res: any) => {
  res.json({
    success: true,
    data: {
      enabled: Boolean(env.AI_PROVIDER),
      provider: env.AI_PROVIDER ?? null,
      assistantName: env.AI_ASSISTANT_NAME
    }
  });
}));

router.get('/ai/conversation', asyncRoute(async (req: any, res: any) => {
  const data = await chat.ensureAiConversation(req.user);
  res.json({ success: true, data });
}));

router.post('/ai/messages', asyncRoute(async (req: any, res: any) => {
  const data = await chat.sendAiMessage(req.user, {
    conversationId: req.body.conversationId,
    clientMessageId: req.body.clientMessageId,
    body: req.body.body
  });
  res.json({
    success: true,
    data: {
      ...data,
      userMessage: enrichMessageFiles(req, data.userMessage),
      assistantMessage: enrichMessageFiles(req, data.assistantMessage)
    }
  });
}));

router.post('/conversations/private', asyncRoute(async (req: any, res: any) => {
  const conversationId = await chat.createPrivateConversation(req.user, req.body.targetUserId, req.body.targetUserName);
  res.json({ success: true, data: { conversationId } });
}));

router.post('/conversations/group', asyncRoute(async (req: any, res: any) => {
  const data = await chat.createGroupConversation(req.user, req.body.title, req.body.members ?? []);
  res.json({ success: true, data });
}));

router.patch('/conversations/:id', asyncRoute(async (req: any, res: any) => {
  const conversation = await chat.updateConversation(req.user, req.params.id, {
    title: req.body.title,
    avatarUrl: req.body.avatarUrl
  });
  const members = await chat.listMembers(req.user, req.params.id);
  res.json({ success: true, data: { conversation, members } });
}));

router.delete('/conversations/:id', asyncRoute(async (req: any, res: any) => {
  const data = await chat.deleteConversationForMe(req.user, req.params.id);
  res.json({ success: true, data });
}));

router.patch('/conversations/:id/notifications', asyncRoute(async (req: any, res: any) => {
  if (typeof req.body.enabled !== 'boolean') {
    return res.status(400).json({ success: false, message: 'enabled must be a boolean.' });
  }

  const data = await chat.setConversationNotifications(
    req.user,
    req.params.id,
    req.body.enabled
  );
  res.json({ success: true, data });
}));

router.get('/conversations/:id/messages', asyncRoute(async (req: any, res: any) => {
  const data = await chat.listMessages(req.user, req.params.id, Number(req.query.limit ?? 30), req.query.before as string | undefined);
  res.json({ success: true, data: data.map((message: any) => enrichMessageFiles(req, message)) });
}));

router.post('/conversations/:id/messages', asyncRoute(async (req: any, res: any) => {
  if (await chat.isAiConversation(req.params.id, req.user.id)) {
    const data = await chat.sendAiMessage(req.user, {
      conversationId: req.params.id,
      clientMessageId: req.body.clientMessageId,
      body: req.body.body
    });
    return res.json({
      success: true,
      data: {
        ...data,
        userMessage: enrichMessageFiles(req, data.userMessage),
        assistantMessage: enrichMessageFiles(req, data.assistantMessage)
      }
    });
  }

  const data = await chat.sendMessage(req.user, {
    conversationId: req.params.id,
    clientMessageId: req.body.clientMessageId,
    body: req.body.body,
    type: req.body.type,
    replyToMessageId: req.body.replyToMessageId,
    attachments: req.body.attachments,
    mentionedUserIds: req.body.mentionedUserIds
  });
  res.json({ success: true, data: enrichMessageFiles(req, data) });

  void sendChatNotification({
    conversationId: String(data.conversationId),
    senderId: data.senderId,
    senderUserName: data.senderUserName,
    body: data.body,
    type: data.type,
    mentionedUserIds: data.mentionedUserIds,
    authorizationBearerToken: req.authToken
  });
}));

router.patch('/conversations/:id/messages/:messageId', asyncRoute(async (req: any, res: any) => {
  const data = await chat.editMessage(req.user, req.params.id, req.params.messageId, req.body.body);
  res.json({ success: true, data });
}));

router.delete('/conversations/:id/messages/:messageId', asyncRoute(async (req: any, res: any) => {
  const scope = req.body.scope === 'everyone' ? 'everyone' : 'me';
  const data = await chat.deleteMessage(req.user, req.params.id, req.params.messageId, scope);
  res.json({ success: true, data });
}));

router.delete('/conversations/:id/messages/:messageId/attachments', asyncRoute(async (req: any, res: any) => {
  const scope = req.body.scope === 'everyone' ? 'everyone' : 'me';
  const attachmentIndex = Number.isInteger(req.body.attachmentIndex)
    ? req.body.attachmentIndex
    : undefined;
  const data = await chat.deleteMessageAttachment(req.user, req.params.id, req.params.messageId, {
    attachmentUrl: req.body.attachmentUrl,
    attachmentIndex,
    scope
  });
  res.json({
    success: true,
    data: {
      ...data,
      message: data.message ? enrichMessageFiles(req, data.message) : data.message
    }
  });
}));

router.post('/conversations/:id/messages/:messageId/reactions', asyncRoute(async (req: any, res: any) => {
  const data = await chat.reactMessage(req.user, req.params.id, req.params.messageId, req.body.emoji);
  res.json({ success: true, data });
}));

router.post('/conversations/:id/messages/:messageId/pin', asyncRoute(async (req: any, res: any) => {
  const data = await chat.pinMessage(req.user, req.params.id, req.params.messageId, Boolean(req.body.pinned));
  res.json({ success: true, data });
}));

router.post('/conversations/:id/messages/:messageId/forward', asyncRoute(async (req: any, res: any) => {
  const data = await chat.forwardMessage(req.user, req.params.id, req.params.messageId, req.body.targetConversationId);
  res.json({ success: true, data });
}));

router.get('/conversations/:id/members', asyncRoute(async (req: any, res: any) => {
  const data = await chat.listMembers(req.user, req.params.id);
  res.json({ success: true, data });
}));

router.post('/conversations/:id/members', asyncRoute(async (req: any, res: any) => {
  const data = await chat.addMembers(req.user, req.params.id, req.body.members ?? []);
  res.json({ success: true, data });
}));

router.delete('/conversations/:id/members/:userId', asyncRoute(async (req: any, res: any) => {
  const data = await chat.removeMember(req.user, req.params.id, req.params.userId);
  res.json({ success: true, data });
}));

router.post('/conversations/:id/leave', asyncRoute(async (req: any, res: any) => {
  const data = await chat.leaveGroup(req.user, req.params.id);
  res.json({ success: true, data });
}));

router.patch('/conversations/:id/members/:userId/role', asyncRoute(async (req: any, res: any) => {
  const data = await chat.changeMemberRole(req.user, req.params.id, req.params.userId, req.body.role);
  res.json({ success: true, data });
}));

router.post('/conversations/:id/seen', asyncRoute(async (req: any, res: any) => {
  await chat.markSeen(req.user, req.params.id, req.body.messageId);
  res.json({ success: true, data: { conversationId: req.params.id, messageId: req.body.messageId } });
}));

router.post('/uploads', uploader.single('file'), async (req: any, res) => {
  if (!req.file) return res.status(400).json({ success: false, message: 'File is required or not allowed.' });

  const isImage = req.file.mimetype.startsWith('image/');
  const isVideo = req.file.mimetype.startsWith('video/');
  const attachment = {
    type: isImage ? 'image' : isVideo ? 'video' : 'file',
    url: `/uploads/chat/${req.file.filename}`,
    fileName: req.file.originalname,
    mimeType: req.file.mimetype,
    size: req.file.size
  };

  res.json({
    success: true,
    data: enrichAttachment(req, attachment)
  });
});

router.get('/uploads/:fileName/download', asyncRoute(async (req: any, res: any) => {
  const fileName = path.basename(String(req.params.fileName ?? ''));
  const filePath = path.resolve(uploadRoot, fileName);
  const downloadName = req.query.name
    ? path.basename(String(req.query.name))
    : undefined;

  if (!filePath.startsWith(`${uploadRoot}${path.sep}`)) {
    return res.status(400).json({ success: false, message: 'Invalid file path.' });
  }

  if (!fs.existsSync(filePath)) {
    return res.status(404).json({ success: false, message: 'File not found.' });
  }

  res.download(filePath, downloadName);
}));

router.use((error: any, req: any, res: any, _next: any) => {
  const message = error instanceof Error ? error.message : 'Chat API error.';
  const status = /not a member|forbidden|permission|cannot recall/i.test(message) ? 403 : 400;
  logger.warn('Chat REST request failed', {
    method: req.method,
    path: req.originalUrl,
    userId: req.user?.id,
    status,
    error: serializeError(error)
  });
  res.status(status).json({ success: false, message });
});

export default router;
