import mongoose, { Schema, Types } from 'mongoose';

export type ConversationType = 'private' | 'group' | 'ai';
export type MemberRole = 'owner' | 'admin' | 'member';

const conversationSchema = new Schema({
  type: { type: String, enum: ['private', 'group', 'ai'], required: true, index: true },
  title: { type: String, default: '' },
  avatarUrl: { type: String, default: '' },
  companyId: { type: String, required: true, index: true },
  groupId: { type: String, required: true, index: true },
  createdBy: { type: String, required: true, index: true },
  lastMessageId: { type: Schema.Types.ObjectId, ref: 'Message' },
  lastMessageAt: { type: Date, index: true },
  isActive: { type: Number, default: 1, index: true },
  deletedAt: { type: Date }
}, { timestamps: true });

conversationSchema.index({ companyId: 1, groupId: 1, lastMessageAt: -1 });
conversationSchema.index({ type: 1, createdBy: 1, isActive: 1 });

const conversationMemberSchema = new Schema({
  conversationId: { type: Schema.Types.ObjectId, required: true, ref: 'Conversation', index: true },
  userId: { type: String, required: true, index: true },
  userName: { type: String, required: true },
  role: { type: String, enum: ['owner', 'admin', 'member'], default: 'member', index: true },
  mutedUntil: { type: Date },
  joinedAt: { type: Date, default: Date.now },
  lastReadMessageId: { type: Schema.Types.ObjectId },
  lastReadAt: { type: Date },
  hiddenAt: { type: Date },
  isActive: { type: Number, default: 1, index: true },
  removedAt: { type: Date }
}, { timestamps: true });

conversationMemberSchema.index({ conversationId: 1, userId: 1 }, { unique: true });
conversationMemberSchema.index({ userId: 1, isActive: 1, updatedAt: -1 });

export const Conversation = mongoose.model('Conversation', conversationSchema);
export const ConversationMember = mongoose.model('ConversationMember', conversationMemberSchema);
export type ConversationDocument = mongoose.Document<unknown, object, { type: ConversationType }> & { _id: Types.ObjectId };
export type ConversationMemberDocument = mongoose.InferSchemaType<typeof conversationMemberSchema>;
