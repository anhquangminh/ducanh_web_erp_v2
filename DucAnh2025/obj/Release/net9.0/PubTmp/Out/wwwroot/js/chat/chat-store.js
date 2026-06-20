(function (window) {
    'use strict';

    class ChatStore {
        constructor() {
            this.state = {
                currentUser: null,
                conversations: [],
                activeConversationId: null,
                messagesByConversation: {},
                membersByConversation: {},
                onlineUserIds: new Set(),
                typingByConversation: {},
                uploadProgress: 0,
                replyTo: null,
                pinnedByConversation: {},
                hasMoreByConversation: {},
                loading: false,
                error: ''
            };
            this.listeners = [];
        }

        subscribe(listener) {
            this.listeners.push(listener);
            listener(this.state);
            return () => {
                this.listeners = this.listeners.filter((item) => item !== listener);
            };
        }

        set(patch) {
            this.state = Object.assign({}, this.state, patch);
            this.listeners.forEach((listener) => listener(this.state));
        }

        setConversations(conversations) {
            this.set({ conversations });
        }

        upsertConversation(conversation) {
            const id = String(conversation._id || conversation.id);
            const conversations = this.state.conversations
                .filter((item) => String(item._id) !== id)
                .concat(conversation)
                .sort((a, b) => new Date(b.lastMessageAt || b.updatedAt || 0) - new Date(a.lastMessageAt || a.updatedAt || 0));
            this.set({ conversations });
        }

        removeConversation(conversationId) {
            const id = String(conversationId);
            const messagesByConversation = Object.assign({}, this.state.messagesByConversation);
            const membersByConversation = Object.assign({}, this.state.membersByConversation);
            delete messagesByConversation[id];
            delete membersByConversation[id];
            this.set({
                conversations: this.state.conversations.filter((item) => String(item._id) !== id),
                activeConversationId: this.state.activeConversationId === id ? null : this.state.activeConversationId,
                messagesByConversation,
                membersByConversation,
                replyTo: this.state.activeConversationId === id ? null : this.state.replyTo
            });
        }

        activate(conversationId) {
            this.set({ activeConversationId: conversationId, replyTo: null });
        }

        setMessages(conversationId, messages) {
            const normalized = messages.slice().reverse();
            const pinned = normalized.find((item) => item.pinnedAt && !item.recalledAt);
            this.set({
                messagesByConversation: Object.assign({}, this.state.messagesByConversation, { [conversationId]: normalized }),
                pinnedByConversation: Object.assign({}, this.state.pinnedByConversation, { [conversationId]: pinned || null }),
                hasMoreByConversation: Object.assign({}, this.state.hasMoreByConversation, { [conversationId]: messages.length >= 80 })
            });
        }

        prependMessages(conversationId, messages) {
            const normalized = messages.slice().reverse();
            const current = this.state.messagesByConversation[conversationId] || [];
            const known = new Set(current.map((item) => String(item._id || item.clientMessageId)));
            const next = normalized.filter((item) => !known.has(String(item._id || item.clientMessageId))).concat(current);
            const pinned = next.find((item) => item.pinnedAt && !item.recalledAt);
            this.set({
                messagesByConversation: Object.assign({}, this.state.messagesByConversation, { [conversationId]: next }),
                pinnedByConversation: Object.assign({}, this.state.pinnedByConversation, { [conversationId]: pinned || null }),
                hasMoreByConversation: Object.assign({}, this.state.hasMoreByConversation, { [conversationId]: messages.length >= 80 })
            });
        }

        appendMessage(message) {
            const conversationId = String(message.conversationId);
            const current = this.state.messagesByConversation[conversationId] || [];
            const existingIndex = current.findIndex((item) => item._id === message._id || item.clientMessageId === message.clientMessageId);
            const next = existingIndex >= 0
                ? current.map((item, index) => index === existingIndex ? Object.assign({}, item, message, { isOptimistic: false }) : item)
                : current.concat(message);

            const messagesByConversation = Object.assign({}, this.state.messagesByConversation, {
                [conversationId]: next.slice(-300)
            });
            const conversations = this.state.conversations
                .map((item) => String(item._id) === conversationId ? Object.assign({}, item, { lastMessageAt: message.createdAt, lastMessageId: message._id, lastMessageBody: message.body || '[File]' }) : item)
                .sort((a, b) => new Date(b.lastMessageAt || 0) - new Date(a.lastMessageAt || 0));

            this.set({ messagesByConversation, conversations });
        }

        bumpUnread(conversationId, message) {
            const conversations = this.state.conversations
                .map((item) => String(item._id) === String(conversationId)
                    ? Object.assign({}, item, {
                        unreadCount: Number(item.unreadCount || 0) + 1,
                        lastMessageAt: message?.createdAt || new Date().toISOString(),
                        lastMessageId: message?._id || item.lastMessageId,
                        lastMessageBody: message?.body || item.lastMessageBody || '[File]'
                    })
                    : item)
                .sort((a, b) => new Date(b.lastMessageAt || 0) - new Date(a.lastMessageAt || 0));
            this.set({ conversations });
        }

        clearUnread(conversationId) {
            const conversations = this.state.conversations.map((item) => String(item._id) === String(conversationId) ? Object.assign({}, item, { unreadCount: 0 }) : item);
            this.set({ conversations });
        }

        updateMessage(conversationId, messageId, patch) {
            const current = this.state.messagesByConversation[conversationId] || [];
            const messages = current.map((item) => String(item._id) === String(messageId) || String(item.clientMessageId) === String(messageId) ? Object.assign({}, item, patch) : item);
            this.set({ messagesByConversation: Object.assign({}, this.state.messagesByConversation, { [conversationId]: messages }) });
        }

        removeMessageForMe(conversationId, messageId) {
            const current = this.state.messagesByConversation[conversationId] || [];
            this.set({
                messagesByConversation: Object.assign({}, this.state.messagesByConversation, {
                    [conversationId]: current.filter((item) => String(item._id) !== String(messageId))
                })
            });
        }

        setMembers(conversationId, members) {
            this.set({ membersByConversation: Object.assign({}, this.state.membersByConversation, { [conversationId]: members }) });
        }

        setPresence(userId, isOnline) {
            const next = new Set(this.state.onlineUserIds);
            if (isOnline) next.add(userId);
            else next.delete(userId);
            this.set({ onlineUserIds: next });
        }

        setTyping(conversationId, userId, isTyping) {
            const current = Object.assign({}, this.state.typingByConversation[conversationId] || {});
            if (isTyping) current[userId] = Date.now();
            else delete current[userId];
            this.set({ typingByConversation: Object.assign({}, this.state.typingByConversation, { [conversationId]: current }) });
        }
    }

    window.DAChatStore = ChatStore;
})(window);
