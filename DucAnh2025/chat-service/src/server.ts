import http from 'http';
import path from 'path';
import cors from 'cors';
import express from 'express';
import helmet from 'helmet';
import { Server } from 'socket.io';
import { env, isAllowedCorsOrigin } from './config/env.ts';
import { connectMongo, mongoHealth } from './db/mongo.ts';
import chatRoutes from './routes/chat.routes.ts';
import { configureChatGateway } from './socket/chat.gateway.ts';
import { logger, serializeError } from './utils/logger.ts';
import { requestLogger } from './middleware/requestLogger.ts';
import { sqlHealth } from './db/sql.ts';

async function main() {
  logger.info('Starting chat service', { port: env.PORT, mongoUri: env.MONGODB_URI });
  await connectMongo();

  const app = express();
  app.use(helmet());
  app.use(requestLogger);
  app.use(cors({
    origin(origin, callback) {
      callback(null, isAllowedCorsOrigin(origin) ? (origin || true) : false);
    },
    credentials: true
  }));
  app.use(express.json({ limit: '1mb' }));
  app.use('/uploads/chat', express.static(path.resolve(env.MEDIA_ROOT), {
    maxAge: env.NODE_ENV === 'production' ? '7d' : 0,
    immutable: env.NODE_ENV === 'production'
  }));

  app.get('/health', async (_, res) => {
    const mongo = mongoHealth();
    const sql = await sqlHealth();
    const healthy = mongo.connected && sql.connected;
    res.status(healthy ? 200 : 503).json({
      ok: healthy,
      mongo,
      sql
    });
  });
  app.use('/api/chat', chatRoutes);

  const server = http.createServer(app);
  const io = new Server(server, {
    cors: {
      origin(origin, callback) {
        callback(null, isAllowedCorsOrigin(origin));
      },
      credentials: true
    },
    transports: ['websocket', 'polling'],
    pingInterval: 25000,
    pingTimeout: 20000
  });

  await configureChatGateway(io);

  server.listen(env.PORT, () => {
    logger.info('Chat service listening', { port: env.PORT });
  });
}

main().catch((error) => {
  logger.error('Chat service startup failed', {
    error: serializeError(error)
  });
  process.exit(1);
});
