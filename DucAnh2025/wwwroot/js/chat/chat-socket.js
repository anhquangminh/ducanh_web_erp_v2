(function (window) {
    'use strict';

    class ChatSocket {
        constructor(baseUrl) {
            this.baseUrl = baseUrl.replace(/\/$/, '');
            this.socket = null;
            this.handlers = new Map();
        }

        connect(token) {
            if (!window.io) throw new Error('Socket.IO client chưa được load.');
            this.socket = window.io(`${this.baseUrl}/chat`, {
                auth: { token },
                transports: ['websocket', 'polling'],
                reconnection: true,
                reconnectionAttempts: Infinity,
                reconnectionDelay: 600,
                timeout: 12000
            });

            [
                'connect',
                'disconnect',
                'connect_error',
                'message:new',
                'message:seen',
                'message:typing',
                'message:deleted',
                'message:edited',
                'message:reaction',
                'message:pinned',
                'conversation:updated',
                'conversation:left',
                'presence:online',
                'presence:offline',
                'notification:new'
            ].forEach((eventName) => this.socket.on(eventName, (payload) => this.emit(eventName, payload)));
        }

        on(eventName, handler) {
            const list = this.handlers.get(eventName) || [];
            list.push(handler);
            this.handlers.set(eventName, list);
            return () => this.handlers.set(eventName, list.filter((item) => item !== handler));
        }

        emit(eventName, payload) {
            (this.handlers.get(eventName) || []).forEach((handler) => handler(payload));
        }

        send(eventName, payload) {
            return new Promise((resolve, reject) => {
                if (!this.socket?.connected) return reject(new Error('Socket chưa kết nối.'));
                this.socket.emit(eventName, payload, (ack) => {
                    if (ack?.success) resolve(ack.data || ack);
                    else reject(new Error(ack?.message || 'Socket event failed.'));
                });
            });
        }

        join(conversationId) {
            return this.send('conversation:join', { conversationId });
        }

        sendMessage(payload) {
            return this.send('message:send', payload);
        }

        seen(conversationId, messageId) {
            return this.send('message:seen', { conversationId, messageId });
        }

        typing(conversationId, isTyping) {
            if (this.socket?.connected) this.socket.emit('message:typing', { conversationId, isTyping });
        }
    }

    window.DAChatSocket = ChatSocket;
})(window);
