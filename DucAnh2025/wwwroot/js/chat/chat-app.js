(function (window, document) {
    'use strict';

    const U = window.DAChatUtils;
    const C = window.DAChatComponents;

    class ChatApp {
        constructor(root) {
            this.root = root;
            this.api = new window.DAChatApi({
                baseUrl: root.dataset.chatApiBase || '',
                tokenUrl: root.dataset.tokenUrl || '/api/User/GetJwtToken'
            });
            this.socket = new window.DAChatSocket(root.dataset.chatSocketBase || 'http://localhost:3000');
            this.store = new window.DAChatStore();
            this.pendingAttachment = null;
            this.mentionedUsers = new Map();
            this.typingTimer = 0;
            this.contextMessageId = '';
            this.loadingOlder = new Set();
            this.leavingConversationIds = new Set();
            window.DAChatRuntime = { api: this.api, socket: this.socket, store: this.store };
            this.cacheElements();
            this.initComponents();
            this.bind();
        }

        cacheElements() {
            this.els = {
                appHeaderBrand: document.getElementById('chat-app-header-brand'),
                sidebarState: document.getElementById('chat-sidebar-state'),
                conversationList: document.getElementById('chat-conversation-list'),
                search: document.getElementById('chat-conversation-search'),
                filterButtons: Array.from(document.querySelectorAll('[data-chat-filter]')),
                empty: document.getElementById('chat-empty-state'),
                thread: document.getElementById('chat-thread'),
                title: document.getElementById('chat-thread-title'),
                subtitle: document.getElementById('chat-thread-subtitle'),
                avatar: document.getElementById('chat-thread-avatar'),
                memberList: document.getElementById('chat-member-list'),
                pinnedBar: document.getElementById('chat-pinned-bar'),
                rightPanel: document.getElementById('chat-right-panel'),
                rightPanelContent: document.getElementById('chat-right-panel-content'),
                messageState: document.getElementById('chat-message-state'),
                messages: document.getElementById('chat-message-list') || document.getElementById('chat-dropzone'),
                typing: document.getElementById('chat-typing-indicator'),
                input: document.getElementById('chat-message-input'),
                send: document.getElementById('chat-send-button'),
                file: document.getElementById('chat-file-input'),
                dropzone: document.getElementById('chat-dropzone') || document.getElementById('chat-message-list'),
                uploadPreview: document.getElementById('chat-upload-preview'),
                uploadProgress: document.getElementById('chat-upload-progress'),
                replyPreview: document.getElementById('chat-reply-preview'),
                emoji: document.getElementById('chat-emoji-picker'),
                mention: document.getElementById('chat-mention-menu'),
                context: document.getElementById('chat-context-menu'),
                toast: document.getElementById('chat-toast-stack'),
                groupModal: document.getElementById('chat-group-modal'),
                groupName: document.getElementById('chat-group-name'),
                groupError: document.getElementById('chat-group-error'),
                userSearch: document.getElementById('chat-user-search'),
                selectedUsers: document.getElementById('chat-selected-users'),
                userResults: document.getElementById('chat-user-results'),
                createGroup: document.getElementById('chat-create-group-button'),
                privateModal: document.getElementById('chat-private-modal'),
                privateSearch: document.getElementById('chat-private-user-search'),
                privateResults: document.getElementById('chat-private-user-results'),
                addMemberModal: document.getElementById('chat-add-member-modal'),
                addMemberSearch: document.getElementById('chat-add-member-search'),
                addMemberSelected: document.getElementById('chat-add-member-selected'),
                addMemberResults: document.getElementById('chat-add-member-results'),
                addMemberButton: document.getElementById('chat-add-member-button')
            };
        }

        initComponents() {
            this.toasts = new C.Toasts(this.els.toast);
            this.sidebar = new C.ConversationSidebar(
                this.els.conversationList,
                (id) => this.openConversation(id),
                (id) => this.deleteConversationForMe(id)
            );
            this.panel = new C.MessagePanel(this.els.messages, {
                onAction: (action, messageId) => this.handleMessageAction(action, messageId),
                onContext: (messageId, x, y) => this.openContextMenu(messageId, x, y)
            });
            this.rightPanel = new C.RightPanel(this.els.rightPanelContent);
            this.groupModal = new C.GroupModal({
                modal: this.els.groupModal,
                name: this.els.groupName,
                error: this.els.groupError,
                search: this.els.userSearch,
                selected: this.els.selectedUsers,
                results: this.els.userResults,
                create: this.els.createGroup
            }, this.api, (title, members) => this.createGroup(title, members), this.toasts);
            this.privateModal = new C.PrivateModal({
                modal: this.els.privateModal,
                search: this.els.privateSearch,
                results: this.els.privateResults
            }, this.api, (user) => this.createPrivate(user), this.toasts);
            this.addMemberPicker = new C.UserPicker({
                search: this.els.addMemberSearch,
                selected: this.els.addMemberSelected,
                results: this.els.addMemberResults
            }, this.api, this.toasts, () => {
                this.els.addMemberButton.disabled = !this.addMemberPicker.selected.length;
            });
        }

        bind() {
            this.store.subscribe((state) => this.render(state));

            this.els.search.addEventListener('input', () => {
                this.sidebar.setQuery(this.els.search.value);
                this.sidebar.render(this.store.state);
            });
            this.els.filterButtons.forEach((button) => {
                button.addEventListener('click', () => {
                    this.els.filterButtons.forEach((item) => item.classList.toggle('is-active', item === button));
                    this.sidebar.setFilter(button.dataset.chatFilter);
                    this.sidebar.render(this.store.state);
                });
            });

            this.els.send.addEventListener('click', () => this.sendMessage());
            this.els.input.addEventListener('input', () => this.onInput());
            this.els.input.addEventListener('keydown', (event) => {
                if (event.key === 'Enter' && !event.shiftKey) {
                    event.preventDefault();
                    this.sendMessage();
                }
            });

            this.els.file.addEventListener('change', () => this.prepareUpload(this.els.file.files[0]));
            this.els.messages.addEventListener('scroll', () => this.onMessageScroll());
            this.bindDragDrop();
            document.querySelectorAll('[data-chat-action]').forEach((button) => button.addEventListener('click', () => this.handleAction(button.dataset.chatAction)));
            document.querySelectorAll('[data-chat-action="close-group-modal"]').forEach((button) => button.addEventListener('click', () => this.groupModal.close()));
            document.querySelectorAll('[data-chat-action="close-private-modal"]').forEach((button) => button.addEventListener('click', () => this.privateModal.close()));
            document.querySelectorAll('[data-chat-action="close-add-member-modal"]').forEach((button) => button.addEventListener('click', () => this.closeAddMemberModal()));
            this.els.addMemberButton.addEventListener('click', () => this.addMembersToActiveConversation());
            this.els.rightPanelContent.addEventListener('click', (event) => {
                const button = event.target.closest('[data-right-action]');
                if (!button) return;
                const action = button.dataset.rightAction;
                if (action === 'add-members') this.openAddMemberModal();
                if (action === 'leave-group') this.leaveActiveGroup();
                if (action === 'remove-member') this.removeMember(button.dataset.userId);
            });
            document.addEventListener('click', (event) => {
                if (!event.target.closest('#chat-context-menu')) this.els.context.hidden = true;
            });
            this.els.context.addEventListener('click', (event) => {
                const button = event.target.closest('[data-context-action]');
                if (!button) return;
                this.els.context.hidden = true;
                this.handleMessageAction(button.dataset.contextAction, this.contextMessageId);
            });

            this.socket.on('connect', () => this.toasts.show('Chat đã kết nối', 'Realtime đang hoạt động.'));
            this.socket.on('connect_error', (error) => this.toasts.show('Lỗi socket', error?.message || 'Không kết nối được chat-service.'));
            this.socket.on('message:new', (message) => this.onNewMessage(message));
            this.socket.on('message:seen', (payload) => this.onSeen(payload));
            this.socket.on('message:typing', (payload) => this.onTyping(payload));
            this.socket.on('message:deleted', (payload) => this.onMessageDeleted(payload));
            this.socket.on('message:edited', (payload) => this.store.updateMessage(String(payload.conversationId), payload.messageId, payload.message));
            this.socket.on('message:reaction', (payload) => this.store.updateMessage(String(payload.conversationId), payload.messageId, { reactions: payload.reactions || [] }));
            this.socket.on('message:pinned', (payload) => this.store.updateMessage(String(payload.conversationId), payload.messageId, { pinnedAt: payload.pinnedAt }));
            this.socket.on('conversation:updated', (payload) => this.onConversationUpdated(payload));
            this.socket.on('conversation:left', (payload) => this.onConversationLeft(payload));
            this.socket.on('presence:online', (payload) => this.store.setPresence(payload.userId, true));
            this.socket.on('presence:offline', (payload) => this.store.setPresence(payload.userId, false));
            this.socket.on('notification:new', (payload) => this.onNotification(payload));

            this.els.emoji.innerHTML = U.emojiSet.map((emoji) => `<button type="button" data-emoji="${emoji}">${emoji}</button>`).join('');
            this.els.emoji.querySelectorAll('[data-emoji]').forEach((button) => {
                button.addEventListener('click', () => {
                    this.insertAtCursor(button.dataset.emoji);
                    this.els.emoji.hidden = true;
                });
            });
        }

        bindDragDrop() {
            ['dragenter', 'dragover'].forEach((eventName) => {
                this.els.dropzone.addEventListener(eventName, (event) => {
                    event.preventDefault();
                    this.root.classList.add('is-dragging-file');
                });
            });
            ['dragleave', 'drop'].forEach((eventName) => {
                this.els.dropzone.addEventListener(eventName, (event) => {
                    event.preventDefault();
                    this.root.classList.remove('is-dragging-file');
                });
            });
            this.els.dropzone.addEventListener('drop', (event) => this.prepareUpload(event.dataTransfer.files[0]));
        }

        async start() {
            try {
                this.setSidebarState('Đang tải chat...');
                const auth = await this.api.bootstrap();
                this.store.set({ currentUser: auth.userInfor });
                if (auth.token) this.socket.connect(auth.token);
                else this.toasts.show('Realtime chưa kết nối', 'Không lấy được JWT socket từ phiên web hiện tại.');
                await this.loadConversations();
                this.setSidebarState('');
            } catch (error) {
                this.store.set({ error: error.message });
                this.setSidebarState(error.message);
            }
        }

        async loadConversations() {
            const response = await this.api.listConversations();
            this.store.setConversations(response.data || []);
            await Promise.all((response.data || []).slice(0, 20).map((conversation) => this.loadMembers(String(conversation._id), false)));
        }

        async loadMembers(conversationId, render) {
            const response = await this.api.listMembers(conversationId);
            this.store.setMembers(conversationId, response.data || []);
            if (render) this.renderHeader(this.store.state);
        }

        async openConversation(conversationId) {
            conversationId = String(conversationId);
            if (this.leavingConversationIds.has(conversationId)) return;
            this.root.classList.add('is-thread-open');
            this.store.activate(conversationId);
            this.els.empty.hidden = true;
            this.els.thread.hidden = false;
            this.setMessageState('Đang tải tin nhắn...');
            try {
                await this.socket.join(conversationId);
                if (this.store.state.activeConversationId !== conversationId || this.leavingConversationIds.has(conversationId)) return;
                if (!this.store.state.membersByConversation[conversationId]) await this.loadMembers(conversationId, true);
                if (this.store.state.activeConversationId !== conversationId || this.leavingConversationIds.has(conversationId)) return;
                const response = await this.api.listMessages(conversationId);
                if (this.store.state.activeConversationId !== conversationId || this.leavingConversationIds.has(conversationId)) return;
                this.store.setMessages(conversationId, response.data || []);
                this.store.clearUnread(conversationId);
                this.setMessageState('');
                this.markLatestSeen(conversationId);
            } catch (error) {
                if (/not a member|forbidden|permission/i.test(error.message || '')) {
                    this.store.removeConversation(conversationId);
                    this.els.thread.hidden = true;
                    this.els.empty.hidden = false;
                    this.root.classList.remove('is-thread-open');
                    return;
                }
                this.setMessageState(error.message);
            }
        }

        async onMessageScroll() {
            const conversationId = this.store.state.activeConversationId;
            if (this.leavingConversationIds.has(String(conversationId))) return;
            if (!conversationId || this.els.messages.scrollTop > 90) return;
            if (this.loadingOlder.has(conversationId)) return;
            if (this.store.state.hasMoreByConversation[conversationId] === false) return;

            const messages = this.store.state.messagesByConversation[conversationId] || [];
            const oldest = messages[0];
            if (!oldest?.createdAt) return;

            this.loadingOlder.add(conversationId);
            const previousHeight = this.els.messages.scrollHeight;
            const previousTop = this.els.messages.scrollTop;
            try {
                const response = await this.api.listMessages(conversationId, oldest.createdAt);
                this.store.prependMessages(conversationId, response.data || []);
                requestAnimationFrame(() => {
                    this.els.messages.scrollTop = this.els.messages.scrollHeight - previousHeight + previousTop;
                });
            } catch (error) {
                this.toasts.show('Không tải được tin nhắn cũ', error.message);
            } finally {
                this.loadingOlder.delete(conversationId);
            }
        }

        async createGroup(title, members) {
            const response = await this.api.createGroupConversation(title, members);
            await this.loadConversations();
            await this.openConversation(String(response.data._id));
        }

        async createPrivate(user) {
            const response = await this.api.createPrivateConversation(user);
            await this.loadConversations();
            await this.openConversation(String(response.data.conversationId));
        }

        async sendMessage() {
            const conversationId = this.store.state.activeConversationId;
            if (!conversationId) return;
            const body = this.els.input.value.trim();
            if (!body && !this.pendingAttachment) return;

            const attachment = this.pendingAttachment;
            const replyTo = this.store.state.replyTo;
            const clientMessageId = U.uid('web');
            const mentionedUserIds = Array.from(this.mentionedUsers.keys());
            const optimistic = {
                _id: clientMessageId,
                clientMessageId,
                conversationId,
                senderId: this.store.state.currentUser?.id,
                senderUserName: this.store.state.currentUser?.userName,
                body,
                replyTo,
                attachments: attachment ? [{ type: U.fileType(attachment), fileName: attachment.name, size: attachment.size }] : [],
                createdAt: new Date().toISOString(),
                isOptimistic: true
            };

            this.store.appendMessage(optimistic);
            this.els.input.value = '';
            this.pendingAttachment = null;
            this.mentionedUsers.clear();
            this.store.set({ replyTo: null, uploadProgress: 0 });
            this.renderUploadPreview();
            this.autoSizeInput();

            try {
                const uploaded = attachment ? await this.api.upload(attachment, (value) => this.store.set({ uploadProgress: value })) : null;
                const payload = {
                    conversationId,
                    clientMessageId,
                    body,
                    type: uploaded?.data?.type || 'text',
                    replyToMessageId: replyTo?._id,
                    attachments: uploaded?.data ? [uploaded.data] : [],
                    mentionedUserIds
                };
                const message = this.socket.socket?.connected
                    ? await this.socket.sendMessage(payload)
                    : (await this.api.sendMessage(conversationId, payload)).data;
                this.store.appendMessage(message);
                this.socket.typing(conversationId, false);
            } catch (error) {
                this.store.updateMessage(conversationId, clientMessageId, { failed: true, isOptimistic: false });
                this.toasts.show('Không gửi được tin nhắn', error.message);
            } finally {
                this.store.set({ uploadProgress: 0 });
            }
        }

        onNewMessage(message) {
            this.store.appendMessage(message);
            if (String(message.conversationId) === this.store.state.activeConversationId) {
                this.store.clearUnread(String(message.conversationId));
                this.markLatestSeen(String(message.conversationId));
            } else {
                this.store.bumpUnread(String(message.conversationId), message);
                this.toasts.show('Tin nhắn mới', message.body || 'Bạn nhận được file mới.');
            }
        }

        onSeen(payload) {
            if (payload.userId !== this.store.state.currentUser?.id) this.store.updateMessage(String(payload.conversationId), payload.messageId, { seenAt: payload.seenAt });
        }

        onTyping(payload) {
            if (payload.userId === this.store.state.currentUser?.id) return;
            this.store.setTyping(payload.conversationId, payload.userId, payload.isTyping);
            if (payload.isTyping) window.setTimeout(() => this.store.setTyping(payload.conversationId, payload.userId, false), 3500);
        }

        onMessageDeleted(payload) {
            const conversationId = String(payload.conversationId);
            if (payload.scope === 'me' && payload.userId === this.store.state.currentUser?.id) this.store.removeMessageForMe(conversationId, payload.messageId);
            else this.store.updateMessage(conversationId, payload.messageId, payload.message || { recalledAt: payload.deletedAt, body: '', attachments: [] });
        }

        onConversationUpdated(payload) {
            if (payload.conversation) this.store.upsertConversation(payload.conversation);
            if (payload.members && payload.conversationId) this.store.setMembers(String(payload.conversationId), payload.members);
            if (payload.leftUserId === this.store.state.currentUser?.id && payload.conversationId) this.store.removeConversation(String(payload.conversationId));
            this.renderHeader(this.store.state);
        }

        onConversationLeft(payload) {
            const conversationId = String(payload?.conversationId || '');
            if (!conversationId) return;
            this.store.removeConversation(conversationId);
            if (this.store.state.activeConversationId === conversationId || !this.store.state.activeConversationId) {
                this.els.thread.hidden = true;
                this.els.empty.hidden = false;
                this.root.classList.remove('is-thread-open');
            }
        }

        onNotification(payload) {
            if (payload.conversationId !== this.store.state.activeConversationId) {
                this.store.bumpUnread(String(payload.conversationId), { _id: payload.messageId });
                this.toasts.show('Thông báo chat', 'Có hoạt động mới trong cuộc trò chuyện.');
            }
        }

        markLatestSeen(conversationId) {
            const messages = this.store.state.messagesByConversation[conversationId] || [];
            const latest = messages[messages.length - 1];
            if (latest?._id && !latest.isOptimistic) this.socket.seen(conversationId, latest._id).catch(() => {});
        }

        prepareUpload(file) {
            if (!file) return;
            if (!U.isAllowedFile(file)) {
                this.toasts.show('File không hợp lệ', `Chỉ hỗ trợ ảnh, video, tài liệu và tối đa ${U.formatBytes(U.maxUploadSize)}.`);
                this.els.file.value = '';
                return;
            }
            this.pendingAttachment = file;
            this.renderUploadPreview();
        }

        renderUploadPreview() {
            if (!this.pendingAttachment) {
                this.els.uploadPreview.hidden = true;
                this.els.uploadPreview.innerHTML = '';
                this.els.file.value = '';
                return;
            }
            const file = this.pendingAttachment;
            const preview = file.type.startsWith('image/') ? `<img src="${URL.createObjectURL(file)}" alt="">` : `<i class="bi bi-file-earmark"></i>`;
            this.els.uploadPreview.hidden = false;
            this.els.uploadPreview.innerHTML = `
                <div class="da-upload-card">${preview}<div><strong>${U.escapeHtml(file.name)}</strong><span>${U.escapeHtml(U.formatBytes(file.size))}</span></div></div>
                <button type="button" class="da-icon-btn" data-remove-upload aria-label="Bỏ file"><i class="bi bi-x"></i></button>`;
            this.els.uploadPreview.querySelector('[data-remove-upload]').addEventListener('click', () => {
                this.pendingAttachment = null;
                this.renderUploadPreview();
            });
        }

        async onInput() {
            this.autoSizeInput();
            const conversationId = this.store.state.activeConversationId;
            if (conversationId) {
                this.socket.typing(conversationId, true);
                window.clearTimeout(this.typingTimer);
                this.typingTimer = window.setTimeout(() => this.socket.typing(conversationId, false), 1200);
            }
            await this.updateMentionMenu();
        }

        async updateMentionMenu() {
            const value = this.els.input.value;
            const cursor = this.els.input.selectionStart;
            const match = value.slice(0, cursor).match(/(^|\s)@([\w.@-]{1,30})$/);
            if (!match) {
                this.els.mention.hidden = true;
                return;
            }
            try {
                const response = await this.api.searchUsers(match[2]);
                const users = response.data || [];
                this.els.mention.hidden = !users.length;
                this.els.mention.innerHTML = users.map((user) => `
                    <button type="button" class="da-mention-option" data-user-id="${U.escapeHtml(user.id)}">
                        <div class="da-avatar">${U.escapeHtml(U.initials(U.displayName(user)))}</div>
                        <div><div class="da-conversation-name">${U.escapeHtml(U.displayName(user))}</div><div class="da-conversation-meta">${U.escapeHtml(user.userName || '')}</div></div>
                    </button>`).join('');
                this.els.mention.querySelectorAll('[data-user-id]').forEach((button) => {
                    button.addEventListener('click', () => {
                        const user = users.find((item) => item.id === button.dataset.userId);
                        if (!user) return;
                        this.mentionedUsers.set(user.id, user);
                        this.replaceMention(match[2], user.userName || U.displayName(user));
                        this.els.mention.hidden = true;
                    });
                });
            } catch {
                this.els.mention.hidden = true;
            }
        }

        replaceMention(query, userName) {
            const cursor = this.els.input.selectionStart;
            const value = this.els.input.value;
            const start = cursor - query.length - 1;
            this.els.input.value = `${value.slice(0, start)}@${userName} ${value.slice(cursor)}`;
            this.els.input.focus();
        }

        insertAtCursor(value) {
            const input = this.els.input;
            const start = input.selectionStart;
            const end = input.selectionEnd;
            input.value = `${input.value.slice(0, start)}${value}${input.value.slice(end)}`;
            input.selectionStart = input.selectionEnd = start + value.length;
            input.focus();
            this.autoSizeInput();
        }

        autoSizeInput() {
            this.els.input.style.height = 'auto';
            this.els.input.style.height = `${Math.min(this.els.input.scrollHeight, 150)}px`;
        }

        render(state) {
            this.sidebar.render(state);
            this.panel.render(state);
            this.renderHeader(state);
            this.renderTyping(state);
            this.renderReply(state);
            this.renderUploadProgress(state);
            this.renderPinned(state);
            this.rightPanel.render(state);
        }

        renderHeader(state) {
            const conversation = state.conversations.find((item) => String(item._id) === state.activeConversationId);
            if (!conversation) return;
            const members = state.membersByConversation[state.activeConversationId] || [];
            const title = conversation.title || members.find((member) => member.userId !== state.currentUser?.id)?.userName || 'Tin nhắn';
            const online = members.filter((member) => state.onlineUserIds.has(member.userId)).length;
            this.els.title.textContent = title;
            this.els.subtitle.textContent = conversation.type === 'group' ? `${members.length} thành viên · ${online} online` : (online ? 'Đang online' : 'Offline');
            this.els.avatar.textContent = U.initials(title);
            if (this.els.appHeaderBrand) this.els.appHeaderBrand.querySelector('strong').textContent = title;
            this.renderMembers(conversation, members, state.currentUser);
        }

        renderMembers(conversation, members, currentUser) {
            if (!this.els.memberList) return;
            const me = members.find((member) => member.userId === currentUser?.id);
            const canManage = conversation?.type === 'group' && ['owner', 'admin'].includes(me?.role);
            this.els.memberList.innerHTML = members.map((member) => `
                <div class="da-member-item">
                    <div class="da-avatar da-avatar-sm">${U.escapeHtml(U.initials(member.userName))}</div>
                    <div><strong>${U.escapeHtml(member.userName)}</strong><span>${U.escapeHtml(member.role)}</span></div>
                    ${canManage && member.role !== 'owner' ? `<button type="button" class="da-icon-btn" data-remove-member="${U.escapeHtml(member.userId)}"><i class="bi bi-person-dash"></i></button>` : ''}
                </div>`).join('');
            this.els.memberList.querySelectorAll('[data-remove-member]').forEach((button) => {
                button.addEventListener('click', () => this.removeMember(button.dataset.removeMember));
            });
        }

        renderTyping(state) {
            const activeId = state.activeConversationId;
            const typing = activeId ? Object.keys(state.typingByConversation[activeId] || {}) : [];
            this.els.typing.hidden = !typing.length;
            this.els.typing.innerHTML = typing.length ? '<span></span><span></span><span></span> Đang nhập...' : '';
        }

        renderReply(state) {
            const reply = state.replyTo;
            this.els.replyPreview.hidden = !reply;
            this.els.replyPreview.innerHTML = reply ? `
                <div><strong>Trả lời ${U.escapeHtml(reply.senderUserName || '')}</strong><span>${U.escapeHtml(reply.body || '[File]')}</span></div>
                <button type="button" class="da-icon-btn" data-cancel-reply><i class="bi bi-x"></i></button>` : '';
            this.els.replyPreview.querySelector('[data-cancel-reply]')?.addEventListener('click', () => this.store.set({ replyTo: null }));
        }

        renderUploadProgress(state) {
            this.els.uploadProgress.hidden = !state.uploadProgress;
            this.els.uploadProgress.querySelector('span').style.width = `${state.uploadProgress || 0}%`;
        }

        renderPinned(state) {
            if (!this.els.pinnedBar) return;
            const conversationId = state.activeConversationId;
            const messages = conversationId ? (state.messagesByConversation[conversationId] || []) : [];
            const pinned = messages.find((message) => message.pinnedAt && !message.recalledAt);
            this.els.pinnedBar.hidden = !pinned;
            this.els.pinnedBar.innerHTML = pinned ? `<i class="bi bi-pin-angle-fill"></i><span>${U.escapeHtml(pinned.body || '[File đã ghim]')}</span>` : '';
        }

        messageById(messageId) {
            const conversationId = this.store.state.activeConversationId;
            return (this.store.state.messagesByConversation[conversationId] || []).find((item) => String(item._id) === String(messageId) || String(item.clientMessageId) === String(messageId));
        }

        async handleMessageAction(action, messageId) {
            const conversationId = this.store.state.activeConversationId;
            const message = this.messageById(messageId);
            if (!conversationId || !message) return;
            if (action === 'delete' && !this.canDeleteMessage(message)) return this.deleteMessageForMe(message);
            if (action === 'quick-react') return this.reactMessage(message, '❤️');
            if (action === 'reply') return this.store.set({ replyTo: message });
            if (action === 'copy') return navigator.clipboard?.writeText(message.body || '').then(() => this.toasts.show('Đã sao chép', 'Nội dung tin nhắn đã được copy.'));
            if (action === 'edit') return this.editMessage(message);
            if (action === 'delete') return this.deleteMessage(message);
            if (action === 'pin') return this.pinMessage(message);
            if (action === 'more') return this.openContextMenu(messageId, window.innerWidth / 2, window.innerHeight / 2);
            if (U.reactionSet.includes(action)) return this.reactMessage(message, action);
        }

        canDeleteMessage(message) {
            const conversationId = this.store.state.activeConversationId;
            const currentUser = this.store.state.currentUser;
            const members = this.store.state.membersByConversation[conversationId] || [];
            const me = members.find((member) => member.userId === currentUser?.id);
            const isConversationAdmin = ['owner', 'admin'].includes(String(me?.role || ''));
            const isSystemAdmin = (currentUser?.roles || []).some((role) => /admin/i.test(role));
            return message.senderId === currentUser?.id || isConversationAdmin || isSystemAdmin;
        }

        openContextMenu(messageId, x, y) {
            this.contextMessageId = messageId;
            const message = this.messageById(messageId);
            const deleteButton = this.els.context.querySelector('[data-context-action="delete"]');
            if (deleteButton) deleteButton.hidden = !message;
            this.els.context.hidden = false;
            this.els.context.style.left = `${Math.min(x, window.innerWidth - 220)}px`;
            this.els.context.style.top = `${Math.min(y, window.innerHeight - 260)}px`;
        }

        async reactMessage(message, emoji) {
            const conversationId = this.store.state.activeConversationId;
            const current = message.reactions || [];
            this.store.updateMessage(conversationId, message._id, { reactions: current.concat({ userId: this.store.state.currentUser?.id, emoji }) });
            try {
                const response = this.socket.socket?.connected
                    ? await this.socket.send('message:reaction', { conversationId, messageId: message._id, emoji })
                    : (await this.api.reactMessage(conversationId, message._id, emoji)).data;
                this.store.updateMessage(conversationId, message._id, { reactions: response.reactions || [] });
            } catch (error) {
                this.toasts.show('Không thả được reaction', error.message);
            }
        }

        async editMessage(message) {
            const body = window.prompt('Sửa tin nhắn', message.body || '');
            if (body === null || body.trim() === message.body) return;
            const conversationId = this.store.state.activeConversationId;
            this.store.updateMessage(conversationId, message._id, { body: body.trim(), editedAt: new Date().toISOString() });
            try {
                if (this.socket.socket?.connected) await this.socket.send('message:edit', { conversationId, messageId: message._id, body: body.trim() });
                else await this.api.editMessage(conversationId, message._id, body.trim());
            } catch (error) {
                this.toasts.show('Không sửa được tin nhắn', error.message);
            }
        }

        async deleteMessageForMe(message) {
            const confirmed = await C.ConfirmDialog.ask({
                title: 'Xóa tin nhắn phía tôi?',
                body: 'Tin nhắn chỉ bị xóa khỏi tài khoản của bạn. Thành viên khác vẫn nhìn thấy tin nhắn này.',
                actions: [
                    { label: 'Hủy', value: '', className: 'da-secondary-btn' },
                    { label: 'Xóa phía tôi', value: 'me', className: 'da-primary-btn da-danger-btn' }
                ]
            });
            if (!confirmed) return;
            const conversationId = this.store.state.activeConversationId;
            this.store.removeMessageForMe(conversationId, message._id);
            try {
                if (this.socket.socket?.connected) await this.socket.send('message:delete', { conversationId, messageId: message._id, scope: 'me' });
                else await this.api.deleteMessage(conversationId, message._id, 'me');
            } catch (error) {
                this.toasts.show('Không xóa được tin nhắn', error.message);
            }
        }

        async deleteMessage(message) {
            let scope = await C.ConfirmDialog.ask({
                title: 'Xóa tin nhắn?',
                body: 'Bạn muốn xóa chỉ phía bạn hay thu hồi với mọi người?',
                actions: [
                    { label: 'Hủy', value: '', className: 'da-secondary-btn' },
                    { label: 'Xóa phía tôi', value: 'me', className: 'da-secondary-btn' },
                    { label: 'Thu hồi', value: 'everyone', className: 'da-primary-btn da-danger-btn' }
                ]
            });
            if (!scope) return;
            const conversationId = this.store.state.activeConversationId;
            if (scope === 'me') this.store.removeMessageForMe(conversationId, message._id);
            else this.store.updateMessage(conversationId, message._id, { recalledAt: new Date().toISOString(), body: '', attachments: [] });
            try {
                if (this.socket.socket?.connected) await this.socket.send('message:delete', { conversationId, messageId: message._id, scope });
                else await this.api.deleteMessage(conversationId, message._id, scope);
            } catch (error) {
                this.toasts.show('Không xóa được tin nhắn', error.message);
            }
        }

        async pinMessage(message) {
            const conversationId = this.store.state.activeConversationId;
            try {
                if (this.socket.socket?.connected) await this.socket.send('message:pin', { conversationId, messageId: message._id, pinned: !message.pinnedAt });
                else await this.api.pinMessage(conversationId, message._id, !message.pinnedAt);
            } catch (error) {
                this.toasts.show('Không ghim được tin nhắn', error.message);
            }
        }

        async removeMember(userId) {
            const conversationId = this.store.state.activeConversationId;
            if (!conversationId || !userId) return;
            const confirmed = await C.ConfirmDialog.ask({
                title: 'Xóa thành viên?',
                body: 'Thành viên này sẽ không còn xem hoặc gửi tin nhắn trong nhóm.',
                actions: [
                    { label: 'Hủy', value: '', className: 'da-secondary-btn' },
                    { label: 'Xóa khỏi nhóm', value: 'remove', className: 'da-primary-btn da-danger-btn' }
                ]
            });
            if (!confirmed) return;
            try {
                await this.api.removeMember(conversationId, userId);
                await this.loadMembers(conversationId, true);
                this.toasts.show('Đã xóa thành viên', 'Danh sách thành viên nhóm đã được cập nhật.');
            } catch (error) {
                this.toasts.show('Không xóa được thành viên', error.message);
            }
        }

        openAddMemberModal() {
            const conversationId = this.store.state.activeConversationId;
            const conversation = this.store.state.conversations.find((item) => String(item._id) === conversationId);
            if (!conversation || conversation.type !== 'group') return;
            this.addMemberPicker.reset();
            this.els.addMemberButton.disabled = true;
            this.els.addMemberModal.hidden = false;
            this.els.addMemberSearch.focus();
        }

        closeAddMemberModal() {
            this.els.addMemberModal.hidden = true;
            this.addMemberPicker.reset();
        }

        async deleteConversationForMe(conversationId) {
            conversationId = String(conversationId || '');
            if (!conversationId) return;
            const conversation = this.store.state.conversations.find((item) => String(item._id) === conversationId);
            if (!conversation) return;

            const confirmed = await C.ConfirmDialog.ask({
                title: 'Xóa tin nhắn?',
                body: 'Cuộc trò chuyện và các tin nhắn hiện có chỉ bị xóa khỏi tài khoản của bạn.',
                actions: [
                    { label: 'Hủy', value: '', className: 'da-secondary-btn' },
                    { label: 'Xóa phía tôi', value: 'delete', className: 'da-primary-btn da-danger-btn' }
                ]
            });
            if (!confirmed) return;

            const wasActive = this.store.state.activeConversationId === conversationId;
            this.store.removeConversation(conversationId);
            if (wasActive) {
                this.els.thread.hidden = true;
                this.els.empty.hidden = false;
                this.root.classList.remove('is-thread-open');
            }

            try {
                await this.api.deleteConversationForMe(conversationId);
                this.toasts.show('Đã xóa phía bạn', 'Cuộc trò chuyện đã được xóa khỏi danh sách của bạn.');
            } catch (error) {
                await this.loadConversations().catch(() => {});
                this.toasts.show('Không xóa được cuộc trò chuyện', error.message);
            }
        }

        async addMembersToActiveConversation() {
            const conversationId = this.store.state.activeConversationId;
            const members = this.addMemberPicker.selected || [];
            if (!conversationId || !members.length) return;
            this.els.addMemberButton.disabled = true;
            try {
                const payload = members.map((member) => ({
                    userId: member.id,
                    userName: U.displayName(member)
                }));
                const updatedMembers = this.socket.socket?.connected
                    ? await this.socket.send('conversation:members:add', { conversationId, members: payload })
                    : (await this.api.addMembers(conversationId, members)).data;
                this.store.setMembers(conversationId, updatedMembers || []);
                this.closeAddMemberModal();
                this.renderHeader(this.store.state);
                this.toasts.show('Đã thêm thành viên', 'Nhóm chat đã được cập nhật.');
            } catch (error) {
                this.toasts.show('Không thêm được thành viên', error.message);
            } finally {
                this.els.addMemberButton.disabled = false;
            }
        }

        async leaveActiveGroup() {
            const conversationId = this.store.state.activeConversationId;
            const conversation = this.store.state.conversations.find((item) => String(item._id) === conversationId);
            if (!conversation || conversation.type !== 'group') return;
            const confirmed = await C.ConfirmDialog.ask({
                title: 'Rời nhóm?',
                body: 'Bạn sẽ không còn nhận tin nhắn mới trong nhóm này.',
                actions: [
                    { label: 'Hủy', value: '', className: 'da-secondary-btn' },
                    { label: 'Rời nhóm', value: 'leave', className: 'da-primary-btn da-danger-btn' }
                ]
            });
            if (!confirmed) return;
            this.leavingConversationIds.add(conversationId);
            this.store.removeConversation(conversationId);
            this.els.thread.hidden = true;
            this.els.empty.hidden = false;
            this.root.classList.remove('is-thread-open');
            try {
                if (this.socket.socket?.connected) await this.socket.send('conversation:leave', { conversationId });
                else await this.api.leaveConversation(conversationId);
                await this.loadConversations();
                this.toasts.show('Đã rời nhóm', 'Nhóm chat đã được gỡ khỏi danh sách của bạn.');
            } catch (error) {
                this.toasts.show('Không rời được nhóm', error.message);
            } finally {
                this.leavingConversationIds.delete(conversationId);
            }
        }

        async renameGroup() {
            const conversationId = this.store.state.activeConversationId;
            const conversation = this.store.state.conversations.find((item) => String(item._id) === conversationId);
            if (!conversation || conversation.type !== 'group') return;
            const title = window.prompt('Tên nhóm mới', conversation.title || '');
            if (!title || title.trim().length < 3) return;
            try {
                const response = this.socket.socket?.connected
                    ? await this.socket.send('conversation:update', { conversationId, title: title.trim() })
                    : (await this.api.updateConversation(conversationId, { title: title.trim() })).data;
                this.store.upsertConversation(response.conversation);
            } catch (error) {
                this.toasts.show('Không đổi được tên nhóm', error.message);
            }
        }

        handleAction(action) {
            if (action === 'toggle-theme') this.root.classList.toggle('is-dark');
            if (action === 'open-private-modal') this.privateModal.open();
            if (action === 'open-group-modal') this.groupModal.open();
            if (action === 'back-to-list') this.root.classList.remove('is-thread-open');
            if (action === 'refresh') this.loadConversations().catch((error) => this.toasts.show('Không tải lại được', error.message));
            if (action === 'toggle-emoji') this.els.emoji.hidden = !this.els.emoji.hidden;
            if (action === 'toggle-right-panel') this.root.classList.toggle('is-right-panel-collapsed');
            if (action === 'rename-group') this.renameGroup();
        }

        setSidebarState(message) {
            this.els.sidebarState.hidden = !message;
            this.els.sidebarState.textContent = message || '';
        }

        setMessageState(message) {
            this.els.messageState.hidden = !message;
            this.els.messageState.textContent = message || '';
        }
    }

    document.addEventListener('DOMContentLoaded', () => {
        const root = document.getElementById('ducanh-chat-root');
        if (!root) return;
        const app = new ChatApp(root);
        app.start();
    });
})(window, document);
