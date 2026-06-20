import dotenv from 'dotenv';
import { z } from 'zod';

dotenv.config();

const emptyStringToUndefined = (value: unknown) => value === '' ? undefined : value;
const stringToBoolean = (value: unknown) => {
  if (typeof value === 'boolean') return value;
  if (typeof value === 'string') return ['true', '1', 'yes'].includes(value.trim().toLowerCase());
  return Boolean(value);
};

const schema = z.object({
  PORT: z.coerce.number().default(4100),
  NODE_ENV: z.string().default('development'),
  CORS_ORIGINS: z.string().default('http://localhost:5000'),
  JWT_ISSUER: z.string().min(1),
  JWT_AUDIENCE: z.string().min(1),
  JWT_KEY: z.string().min(16),
  SQL_SERVER: z.string().min(1),
  SQL_DATABASE: z.string().min(1),
  SQL_USER: z.string().min(1),
  SQL_PASSWORD: z.string().default(''),
  SQL_ENCRYPT: z.preprocess(stringToBoolean, z.boolean()).default(false),
  MONGODB_URI: z.string().min(1).default('mongodb://localhost:27017'),
  MONGODB_DB_NAME: z.string().min(1).default('chat-service'),
  REDIS_URL: z.preprocess(emptyStringToUndefined, z.string().url().optional()),
  MEDIA_ROOT: z.string().default('./uploads/chat'),
  CHAT_PUBLIC_BASE_URL: z.preprocess(emptyStringToUndefined, z.string().url().optional()),
  FCM_NOTIFICATION_API_URL: z.preprocess(emptyStringToUndefined, z.string().url().optional()),
  FCM_NOTIFICATION_API_TIMEOUT_MS: z.coerce.number().int().min(0).default(60_000),
  INTERNAL_AUTH_SECRET: z.string().min(16).default('dev-chat-internal-secret-change-me')
});

export const env = schema.parse(process.env);
export const corsOrigins = env.CORS_ORIGINS.split(',').map((x) => x.trim()).filter(Boolean);

export function isAllowedCorsOrigin(origin?: string) {
  if (!origin) return true;
  if (corsOrigins.includes(origin)) return true;

  if (env.NODE_ENV !== 'production') {
    try {
      const url = new URL(origin);
      return ['localhost', '127.0.0.1'].includes(url.hostname) && ['http:', 'https:'].includes(url.protocol);
    } catch {
      return false;
    }
  }

  return false;
}
