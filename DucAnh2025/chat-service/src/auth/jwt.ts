import jwt from 'jsonwebtoken';
import { Socket } from 'socket.io';
import { env } from '../config/env.ts';
import type { ExistingUser } from '../db/sql.ts';
import { findExistingUserByUserName } from '../db/sql.ts';
import { logger, serializeError } from '../utils/logger.ts';

const nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';

export type AuthUser = ExistingUser;

export class AuthError extends Error {
  status = 401;
  code: string;

  constructor(code: string, message: string) {
    super(message);
    this.name = 'AuthError';
    this.code = code;
  }
}

export function redactToken(token: string) {
  if (!token) return '';
  if (token.length <= 16) return '[redacted]';
  return `${token.slice(0, 10)}...[redacted]...${token.slice(-6)}`;
}

export function authenticateInternalHeaders(headers: Record<string, unknown>): AuthUser | null {
  const secret = String(headers['x-chat-internal-secret'] ?? '');
  if (!secret || secret !== env.INTERNAL_AUTH_SECRET) return null;

  const id = String(headers['x-chat-user-id'] ?? '');
  const userName = String(headers['x-chat-user-name'] ?? '');
  if (!id || !userName) {
    throw new AuthError('AUTH_REQUIRED', 'Missing forwarded chat user headers.');
  }

  const user: AuthUser = {
    id,
    userName,
    firstName: String(headers['x-chat-user-first-name'] ?? ''),
    lastName: String(headers['x-chat-user-last-name'] ?? ''),
    email: String(headers['x-chat-user-email'] ?? userName),
    companyId: String(headers['x-chat-company-id'] ?? ''),
    groupId: String(headers['x-chat-group-id'] ?? ''),
    departmentId: String(headers['x-chat-department-id'] ?? ''),
    roles: String(headers['x-chat-roles'] ?? '').split(',').map((x) => x.trim()).filter(Boolean)
  };

  logger.info('Internal chat auth accepted', {
    userId: user.id,
    userName: user.userName,
    companyId: user.companyId,
    groupId: user.groupId
  });

  return user;
}

export async function authenticateToken(token: string): Promise<AuthUser> {
  if (!token || token === 'null' || token === 'undefined') {
    throw new AuthError('AUTH_REQUIRED', 'Missing authorization bearer token.');
  }

  let payload: jwt.JwtPayload;
  try {
    payload = jwt.verify(token, env.JWT_KEY, {
      issuer: env.JWT_ISSUER,
      audience: env.JWT_AUDIENCE
    }) as jwt.JwtPayload;
  } catch (error) {
    if (error instanceof jwt.TokenExpiredError) {
      throw new AuthError('TOKEN_EXPIRED', 'JWT token has expired.');
    }
    if (error instanceof jwt.JsonWebTokenError) {
      throw new AuthError('TOKEN_INVALID', error.message);
    }
    throw error;
  }

  const userName = String(payload[nameClaim] ?? payload.unique_name ?? payload.email ?? payload[emailClaim] ?? payload.sub ?? '');
  if (!userName) throw new AuthError('TOKEN_INVALID', 'JWT does not contain a user name claim.');

  logger.info('JWT verified for chat request', {
    subject: payload.sub,
    userName,
    issuer: payload.iss,
    audience: payload.aud,
    expiresAt: payload.exp ? new Date(payload.exp * 1000).toISOString() : null
  });

  const user = await findExistingUserByUserName(userName);
  if (!user) throw new AuthError('USER_NOT_FOUND', 'User does not exist or is inactive.');

  return user;
}

export async function socketAuth(socket: Socket, next: (err?: Error) => void) {
  try {
    const raw = socket.handshake.auth?.token || socket.handshake.headers.authorization;
    const token = String(raw ?? '').replace(/^Bearer\s+/i, '');
    logger.info('Socket auth attempt', {
      socketId: socket.id,
      origin: socket.handshake.headers.origin,
      hasToken: Boolean(token),
      token: redactToken(token)
    });
    socket.data.user = await authenticateToken(token);
    socket.data.authToken = token;
    next();
  } catch (error) {
    logger.warn('Socket auth failed', { socketId: socket.id, error: serializeError(error) });
    next(error instanceof Error ? error : new Error('Unauthorized'));
  }
}
