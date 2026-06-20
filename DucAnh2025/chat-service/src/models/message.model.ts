import mongoose, { Schema } from 'mongoose';

const attachmentSchema = new Schema({
  type: { type: String, enum: ['image', 'file', 'voice', 'video'], required: true },
  url: { type: String, required: true },
  fileName: { type: String, default: '' },
  mimeType: { type: String, default: '' },
  size: { type: Number, default: 0 },
  durationMs: { type: Number },
  recalledAt: { type: Date },
  deletedForUserIds: { type: [String], default: [] }
}, { _id: false });

const messageSchema = new Schema({
  conversationId: { type: Schema.Types.ObjectId, required: true, index: true, ref: 'Conversation' },
  clientMessageId: { type: String, required: true },
  senderId: { type: String, required: true, index: true },
  senderUserName: { type: String, required: true },
  body: { type: String, default: '' },
  type: { type: String, enum: ['text', 'image', 'file', 'voice', 'video', 'system'], default: 'text', index: true },
  replyToMessageId: { type: Schema.Types.ObjectId, ref: 'Message' },
  attachments: { type: [attachmentSchema], default: [] },
  mentionedUserIds: { type: [String], default: [], index: true },
  pinnedAt: { type: Date },
  editedAt: { type: Date },
  recalledAt: { type: Date },
  deletedForUserIds: { type: [String], default: [] },
  isActive: { type: Number, default: 1, index: true }
}, { timestamps: true });

messageSchema.index({ conversationId: 1, createdAt: -1, _id: -1 });
messageSchema.index({ conversationId: 1, clientMessageId: 1, senderId: 1 }, { unique: true });
messageSchema.index({ body: 'text' }, { default_language: 'none' });

const reactionSchema = new Schema({
  messageId: { type: Schema.Types.ObjectId, required: true, index: true, ref: 'Message' },
  conversationId: { type: Schema.Types.ObjectId, required: true, index: true, ref: 'Conversation' },
  userId: { type: String, required: true, index: true },
  emoji: { type: String, required: true },
  isActive: { type: Number, default: 1 }
}, { timestamps: true });

reactionSchema.index({ messageId: 1, userId: 1, emoji: 1 }, { unique: true });

const messageReadSchema = new Schema({
  conversationId: { type: Schema.Types.ObjectId, required: true, index: true, ref: 'Conversation' },
  messageId: { type: Schema.Types.ObjectId, required: true, index: true, ref: 'Message' },
  userId: { type: String, required: true, index: true },
  readAt: { type: Date, default: Date.now }
}, { timestamps: true });

messageReadSchema.index({ conversationId: 1, userId: 1, messageId: 1 }, { unique: true });

export const Message = mongoose.model('Message', messageSchema);
export const MessageReaction = mongoose.model('MessageReaction', reactionSchema);
export const MessageRead = mongoose.model('MessageRead', messageReadSchema);
