import sql from 'mssql';
import { env } from '../config/env.ts';
import { logger, serializeError } from '../utils/logger.ts';

let pool: any | null = null;

// Global query logger middleware
sql.on('query', (query: any) => {
  logger.debug('SQL Query executed', {
    text: query.text,
    parameters: query.parameters,
    duration: query.duration?.toString()
  });
});

export async function getSqlPool() {
  if (pool?.connected) return pool;

  const [serverName, instanceName] = env.SQL_SERVER.split('\\');

  logger.info('Connecting to SQL Server', {
    server: serverName,
    database: env.SQL_DATABASE,
    user: env.SQL_USER,
    instanceName: instanceName ?? undefined,
    encrypt: env.SQL_ENCRYPT
  });

  try {
    pool = await sql.connect({
      server: serverName,
      database: env.SQL_DATABASE,
      user: env.SQL_USER,
      password: env.SQL_PASSWORD,
      options: {
        encrypt: env.SQL_ENCRYPT,
        trustServerCertificate: true,
        ...(instanceName ? { instanceName } : {})
      },
      pool: {
        max: 20,
        min: 0,
        idleTimeoutMillis: 30000
      }
    });

    logger.info('SQL Server connected successfully');
    return pool;
  } catch (error) {
    logger.error('SQL Server connection failed', {
      error: serializeError(error),
      instanceName: instanceName,
      server: serverName,
      database: env.SQL_DATABASE,
      user: env.SQL_USER
    });
    throw error;
  }
}

export type ExistingUser = {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  companyId: string;
  groupId: string;
  departmentId: string;
  roles: string[];
};

export async function sqlHealth() {
  try {
    const db = await getSqlPool();
    await db.request().query('SELECT 1 as health');
    return { connected: true };
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    return { connected: false, error: message };
  }
}

export async function findExistingUserByUserName(userName: string): Promise<ExistingUser | null> {
  const db = await getSqlPool();
  const userResult = await db.request()
    .input('userName', sql.NVarChar, userName)
    .query(`
      SELECT TOP 1 Id, UserName, FirstName, LastName, Email, CompanyId, GroupId, DepartmentId
      FROM ApplicationUsers
      WHERE (UserName = @userName OR Email = @userName) AND ISNULL(IsActive, 0) <> 100
    `);

  const row = userResult.recordset[0];
  if (!row) return null;

  let roles: string[] = [];
  try {
    const roleResult = await db.request()
      .input('userId', sql.NVarChar, row.Id)
      .query(`
        SELECT r.Name
        FROM UserRoles ur
        INNER JOIN Roles r ON ur.RoleId = r.Id
        WHERE ur.UserId = @userId
      `);
    roles = roleResult.recordset.map((r: { Name: string }) => r.Name);
  } catch {
    roles = [];
  }

  return {
    id: row.Id,
    userName: row.UserName,
    firstName: row.FirstName ?? '',
    lastName: row.LastName ?? '',
    email: row.Email ?? '',
    companyId: row.CompanyId ?? '',
    groupId: row.GroupId ?? '',
    departmentId: row.DepartmentId ?? '',
    roles
  };
}

export async function findExistingUsersByIds(userIds: string[]): Promise<ExistingUser[]> {
  const ids = Array.from(new Set(userIds.filter(Boolean)));
  if (!ids.length) return [];

  const db = await getSqlPool();
  const request = db.request();
  ids.forEach((id, index) => request.input(`id${index}`, sql.NVarChar, id));
  const placeholders = ids.map((_, index) => `@id${index}`).join(',');

  const result = await request.query(`
    SELECT Id, UserName, FirstName, LastName, Email, CompanyId, GroupId, DepartmentId
    FROM ApplicationUsers
    WHERE ISNULL(IsActive, 0) <> 100
      AND Id IN (${placeholders})
  `);

  return result.recordset.map((row: any) => ({
    id: row.Id,
    userName: row.UserName,
    firstName: row.FirstName ?? '',
    lastName: row.LastName ?? '',
    email: row.Email ?? '',
    companyId: row.CompanyId ?? '',
    groupId: row.GroupId ?? '',
    departmentId: row.DepartmentId ?? '',
    roles: []
  }));
}

export async function searchExistingUsers(actor: ExistingUser, keyword: string, limit = 20): Promise<ExistingUser[]> {
  const db = await getSqlPool();
  const safeLimit = Math.min(Math.max(limit, 1), 30);
  const q = `%${keyword.trim()}%`;

  const result = await db.request()
    .input('q', sql.NVarChar, q)
    .input('groupId', sql.NVarChar, actor.groupId)
    .input('actorId', sql.NVarChar, actor.id)
    .input('limit', sql.Int, safeLimit)
    .query(`
      SELECT TOP (@limit) Id, UserName, FirstName, LastName, Email, CompanyId, GroupId, DepartmentId
      FROM ApplicationUsers
      WHERE ISNULL(IsActive, 0) <> 100
        AND GroupId = @groupId
        AND Id <> @actorId
        AND (
          UserName LIKE @q OR Email LIKE @q OR FirstName LIKE @q OR LastName LIKE @q
        )
      ORDER BY FirstName, LastName, UserName
    `);

  return result.recordset.map((row: any) => ({
    id: row.Id,
    userName: row.UserName,
    firstName: row.FirstName ?? '',
    lastName: row.LastName ?? '',
    email: row.Email ?? '',
    companyId: row.CompanyId ?? '',
    groupId: row.GroupId ?? '',
    departmentId: row.DepartmentId ?? '',
    roles: []
  }));
}
