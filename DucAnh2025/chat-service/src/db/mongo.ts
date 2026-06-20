import mongoose from 'mongoose';
import { env } from '../config/env.ts';
import { logger, serializeError } from '../utils/logger.ts';

let listenersRegistered = false;

function registerMongoLifecycleLogs() {
  if (listenersRegistered) return;
  listenersRegistered = true;

  mongoose.connection.on('connected', () => {
    logger.info('MongoDB connected', {
      host: mongoose.connection.host,
      port: mongoose.connection.port,
      database: mongoose.connection.name
    });
  });

  mongoose.connection.on('reconnected', () => {
    logger.info('MongoDB reconnected', {
      host: mongoose.connection.host,
      port: mongoose.connection.port,
      database: mongoose.connection.name
    });
  });

  mongoose.connection.on('disconnected', () => {
    logger.warn('MongoDB disconnected');
  });

  mongoose.connection.on('error', (error) => {
    logger.error('MongoDB connection error', {
      error: serializeError(error)
    });
  });
}

export async function connectMongo() {
  mongoose.set('strictQuery', true);
  registerMongoLifecycleLogs();

  try {
    await mongoose.connect(env.MONGODB_URI, {
      dbName: env.MONGODB_DB_NAME,
      autoIndex: env.NODE_ENV !== 'production',
      serverSelectionTimeoutMS: 10000,
      heartbeatFrequencyMS: 10000,
      maxPoolSize: 30,
      minPoolSize: 2
    });
  } catch (error) {
    logger.error('MongoDB initial connection failed', {
      uri: env.MONGODB_URI,
      database: env.MONGODB_DB_NAME,
      error: serializeError(error)
    });
    throw error;
  }
}

export function mongoHealth() {
  return {
    readyState: mongoose.connection.readyState,
    connected: mongoose.connection.readyState === 1,
    host: mongoose.connection.host,
    port: mongoose.connection.port,
    database: mongoose.connection.name
  };
}
