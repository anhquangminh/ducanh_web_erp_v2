(function (window) {
    'use strict';

    class ChatApi {
        constructor(options) {
            this.baseUrl = options.baseUrl.replace(/\/$/, '');
            this.tokenUrl = options.tokenUrl;
            this.token = '';
            this.currentUser = null;
        }

        async bootstrap() {
            const sessionResponse = await fetch('/api/chat/session', { credentials: 'include' });
            const sessionPayload = await sessionResponse.json().catch(() => ({}));

            if (sessionResponse.status === 401) {
                this.redirectToLogin();
                throw new Error(sessionPayload.message || 'Unauthorized');
            }
            if (sessionResponse.status === 403) throw new Error(sessionPayload.message || 'Account is inactive or locked.');
            if (!sessionResponse.ok || sessionPayload.success === false) throw new Error(sessionPayload.message || 'Cannot get chat session.');

            this.currentUser = sessionPayload.data;

            try {
                const tokenResponse = await fetch(this.tokenUrl, { credentials: 'include' });
                if (tokenResponse.ok) {
                    const tokenPayload = await tokenResponse.json();
                    if (tokenPayload?.token && tokenPayload.token !== 'null' && tokenPayload.token !== 'undefined') this.token = tokenPayload.token;
                }
            } catch {
                this.token = '';
            }

            return { userInfor: this.currentUser, token: this.token };
        }

        async request(path, options) {
            const url = `${this.baseUrl}${path}`;
            const headers = Object.assign({}, options?.headers || {});

            if (window.axios && !options?.bodyIsFormData) {
                try {
                    const axiosResponse = await window.axios({
                        url,
                        method: options?.method || 'GET',
                        data: options?.body,
                        headers: Object.assign({ 'Content-Type': 'application/json' }, headers),
                        withCredentials: true
                    });
                    return axiosResponse.data;
                } catch (error) {
                    const status = error?.response?.status;
                    const message = error?.response?.data?.message || error?.message || 'Chat API error.';
                    if (status === 401) this.redirectToLogin();
                    throw new Error(message);
                }
            }

            const response = await fetch(url, {
                method: options?.method || 'GET',
                credentials: 'include',
                headers: options?.bodyIsFormData ? headers : Object.assign({ 'Content-Type': 'application/json' }, headers),
                body: options?.bodyIsFormData ? options.body : options?.body ? JSON.stringify(options.body) : undefined
            });
            const payload = await response.json().catch(() => ({}));
            if (response.status === 401) this.redirectToLogin();
            if (!response.ok || payload.success === false) throw new Error(payload.message || 'Chat API error.');
            return payload;
        }

        redirectToLogin() {
            if (window.location.pathname.toLowerCase().includes('/account/login')) return;
            const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = `/Account/Login?ReturnUrl=${returnUrl}`;
        }

        listConversations() {
            return this.request('/api/chat/conversations?limit=50');
        }

        createPrivateConversation(user) {
            return this.request('/api/chat/conversations/private', {
                method: 'POST',
                body: { targetUserId: user.id, targetUserName: window.DAChatUtils.displayName(user) }
            });
        }

        createGroupConversation(title, members) {
            return this.request('/api/chat/conversations/group', {
                method: 'POST',
                body: { title, members: members.map((m) => ({ userId: m.id, userName: window.DAChatUtils.displayName(m) })) }
            });
        }

        updateConversation(conversationId, payload) {
            return this.request(`/api/chat/conversations/${conversationId}`, { method: 'PATCH', body: payload });
        }

        deleteConversationForMe(conversationId) {
            return this.request(`/api/chat/conversations/${conversationId}`, { method: 'DELETE' });
        }

        addMembers(conversationId, members) {
            return this.request(`/api/chat/conversations/${conversationId}/members`, {
                method: 'POST',
                body: { members: members.map((m) => ({ userId: m.id, userName: window.DAChatUtils.displayName(m) })) }
            });
        }

        removeMember(conversationId, userId) {
            return this.request(`/api/chat/conversations/${conversationId}/members/${encodeURIComponent(userId)}`, { method: 'DELETE' });
        }

        leaveConversation(conversationId) {
            return this.request(`/api/chat/conversations/${conversationId}/leave`, { method: 'POST' });
        }

        changeMemberRole(conversationId, userId, role) {
            return this.request(`/api/chat/conversations/${conversationId}/members/${encodeURIComponent(userId)}/role`, {
                method: 'PATCH',
                body: { role }
            });
        }

        listMessages(conversationId, before) {
            const query = before ? `?limit=80&before=${encodeURIComponent(before)}` : '?limit=80';
            return this.request(`/api/chat/conversations/${conversationId}/messages${query}`);
        }

        sendMessage(conversationId, payload) {
            return this.request(`/api/chat/conversations/${conversationId}/messages`, {
                method: 'POST',
                body: payload
            });
        }

        listMembers(conversationId) {
            return this.request(`/api/chat/conversations/${conversationId}/members`);
        }

        searchUsers(keyword) {
            return this.request(`/api/chat/users/search?q=${encodeURIComponent(keyword)}&limit=20`);
        }

        deleteMessage(conversationId, messageId, scope) {
            return this.request(`/api/chat/conversations/${conversationId}/messages/${messageId}`, {
                method: 'DELETE',
                body: { scope }
            });
        }

        editMessage(conversationId, messageId, body) {
            return this.request(`/api/chat/conversations/${conversationId}/messages/${messageId}`, {
                method: 'PATCH',
                body: { body }
            });
        }

        reactMessage(conversationId, messageId, emoji) {
            return this.request(`/api/chat/conversations/${conversationId}/messages/${messageId}/reactions`, {
                method: 'POST',
                body: { emoji }
            });
        }

        pinMessage(conversationId, messageId, pinned) {
            return this.request(`/api/chat/conversations/${conversationId}/messages/${messageId}/pin`, {
                method: 'POST',
                body: { pinned }
            });
        }

        forwardMessage(conversationId, messageId, targetConversationId) {
            return this.request(`/api/chat/conversations/${conversationId}/messages/${messageId}/forward`, {
                method: 'POST',
                body: { targetConversationId }
            });
        }

        upload(file, onProgress) {
            const form = new FormData();
            form.append('file', file);

            if (window.axios) {
                return window.axios({
                    url: `${this.baseUrl}/api/chat/uploads`,
                    method: 'POST',
                    data: form,
                    withCredentials: true,
                    onUploadProgress(event) {
                        if (event.total && onProgress) onProgress(Math.round((event.loaded / event.total) * 100));
                    }
                }).then((response) => response.data).catch((error) => {
                    throw new Error(error?.response?.data?.message || error?.message || 'Upload failed.');
                });
            }

            return this.request('/api/chat/uploads', {
                method: 'POST',
                body: form,
                bodyIsFormData: true
            });
        }
    }

    window.DAChatApi = ChatApi;
})(window);
