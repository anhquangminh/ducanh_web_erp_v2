# Chat Service API Summary

## Endpoints

### Conversations

1. **GET /conversations**
   - Description: List conversations for the authenticated user
   - Headers: Authorization: Bearer <token>
   - Query Parameters:
     - limit (optional, default 30, max 50): Number of conversations to return
     - before (optional): ISO date string to get conversations before this date
   - Response: { success: true, data: [Conversation[]] }

2. **POST /conversations/private**
   - Description: Create a private conversation with another user
   - Headers: Authorization: Bearer <token>, Content-Type: application/json
   - Body:
     ```json
     {
       "targetUserId": "string",
       "targetUserName": "string"
     }
     ```
   - Response: { success: true, data: { conversationId: "string" } }

3. **POST /conversations/group**
   - Description: Create a group conversation
   - Headers: Authorization: Bearer <token>, Content-Type: application/json
   - Body:
     ```json
     {
       "title": "string",
       "members": [
         {
           "userId": "string",
           "userName": "string"
         }
       ]
     }
     ```
   - Response: { success: true, data: Conversation }

### Messages

4. **GET /conversations/:id/messages**
   - Description: List messages in a conversation
   - Headers: Authorization: Bearer <token>
   - Path Parameters:
     - id: Conversation ID
   - Query Parameters:
     - limit (optional, default 30, max 50): Number of messages to return
     - before (optional): Message ID to get messages before this ID
   - Response: { success: true, data: [Message[]] }

## Postman Collection

A Postman collection has been generated at `chat-service.postman_collection.json` containing:
- Pre-configured requests for all endpoints
- Variables for base URL and auth token
- Example request bodies with placeholder data
- Ready to import into Postman or Newman

## Usage

1. Import the Postman collection into Postman
2. Set the `authToken` variable with a valid JWT token
3. Update the example IDs in requests with actual values from your database
4. Run the requests to test the API

## Notes

- All routes are protected by the `authenticateToken` middleware
- The service uses MongoDB/Mongoose for data persistence
- Conversations can be private (2 users) or group (2+ users)
- Messages support text, image, file, and voice types