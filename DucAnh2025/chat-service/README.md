# DucAnh Chat Service

NodeJS realtime chat service for the existing DucAnh2025 system.

This service does not create a new auth system. It validates the JWT produced by the ASP.NET Core app and loads users from the existing SQL Server `ApplicationUsers`, `UserRoles`, and `Roles` tables.

## Local MongoDB

Start MongoDB local on Windows port `2717`:

```powershell
New-Item -ItemType Directory -Force C:\data\ducanh-chat-mongo
mongod --dbpath C:\data\ducanh-chat-mongo --port 2717 --bind_ip 127.0.0.1
```

Or with Docker:

```powershell
docker run --name ducanh-chat-mongo -p 2717:27017 -v ducanh-chat-mongo:/data/db -d mongo:7
```

## Run chat-service

```bash
npm install
npm run dev
```

Before testing authenticated REST/socket APIs, set `JWT_ISSUER`, `JWT_AUDIENCE`, and `JWT_KEY` in `.env` to the same values as the ASP.NET Core `appsettings.json`.

`MONGODB_URI` is configured for local MongoDB:

```text
mongodb://localhost:2717
```

Database name is configured by `MONGODB_DB_NAME=ducanh_chat`.

Redis is optional for single-node development and required for horizontal Socket.IO scaling.

## Test commands

Health check:

```powershell
Invoke-RestMethod http://localhost:4100/health
```

REST auth check:

```powershell
$token = "<JWT_FROM_EXISTING_ASPNET_APP>"
Invoke-RestMethod "http://localhost:4100/api/chat/conversations" -Headers @{ Authorization = "Bearer $token" }
```

Socket.IO smoke test:

```powershell
$env:CHAT_TOKEN="<JWT_FROM_EXISTING_ASPNET_APP>"
node -e "const { io } = require('socket.io-client'); const s = io('http://localhost:4100/chat', { auth: { token: process.env.CHAT_TOKEN }, transports: ['websocket'] }); s.on('connect', () => { console.log('socket connected', s.id); s.close(); }); s.on('connect_error', e => { console.error('socket error', e.message); process.exit(1); });"
```
