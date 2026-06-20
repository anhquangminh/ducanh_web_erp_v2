type LogMeta = Record<string, unknown>;

export function serializeError(error: unknown) {
  if (error instanceof Error) {
    return {
      name: error.name,
      message: error.message,
      stack: error.stack,
      cause: error.cause instanceof Error
        ? { name: error.cause.name, message: error.cause.message, stack: error.cause.stack }
        : error.cause
    };
  }

  return {
    name: typeof error,
    message: String(error),
    raw: error
  };
}

function write(level: 'info' | 'warn' | 'error' | 'debug', message: string, meta?: LogMeta) {
  const payload = {
    ts: new Date().toISOString(),
    level,
    service: 'chat-service',
    message,
    ...(meta ? { meta } : {})
  };

  const line = JSON.stringify(payload);
  if (level === 'error') {
    console.error(line);
    return;
  }
  if (level === 'warn') {
    console.warn(line);
    return;
  }
  if (level === 'debug') {
    console.debug(line);
    return;
  }
  console.log(line);
}

export const logger = {
  info: (message: string, meta?: LogMeta) => write('info', message, meta),
  warn: (message: string, meta?: LogMeta) => write('warn', message, meta),
  error: (message: string, meta?: LogMeta) => write('error', message, meta),
  debug: (message: string, meta?: LogMeta) => write('debug', message, meta)
};
