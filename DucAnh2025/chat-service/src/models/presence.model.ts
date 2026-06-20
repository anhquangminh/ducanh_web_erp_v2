import mongoose, { Schema } from 'mongoose';

const onlineUserSchema = new Schema({
  userId: { type: String, required: true, index: true },
  socketId: { type: String, required: true, unique: true },
  companyId: { type: String, required: true, index: true },
  groupId: { type: String, required: true, index: true },
  lastSeenAt: { type: Date, default: Date.now, index: true },
  expiresAt: { type: Date, required: true, index: { expires: 0 } }
}, { timestamps: true });

export const OnlineUser = mongoose.model('OnlineUser', onlineUserSchema);
