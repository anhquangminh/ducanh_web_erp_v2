(function (window) {
    'use strict';

    const U = window.DAChatUtils;

    function avatar(entity, name, className) {
        const url = U.avatarUrl(entity);
        return `<div class="da-avatar ${className || ''}">${url ? `<img src="${U.escapeHtml(url)}" alt="">` : U.escapeHtml(U.initials(name))}</div>`;
    }

    class Toasts {
        constructor(root) {
            this.root = root;
        }

        show(title, body, timeout) {
            const item = document.createElement('div');
            item.className = 'da-toast';
            item.innerHTML = `<strong>${U.escapeHtml(title)}</strong><div>${U.escapeHtml(body || '')}</div>`;
            this.root.appendChild(item);
            window.setTimeout(() => item.remove(), timeout || 4200);
        }
    }

    class ConfirmDialog {
        static ask({ title, body, actions }) {
            return new Promise((resolve) => {
                const backdrop = document.createElement('div');
                backdrop.className = 'da-modal-backdrop da-confirm-backdrop';
                const isMessageDeleteDialog = (actions || []).some((action) => action.value === 'me')
                    && false;
                const normalizedTitle = isMessageDeleteDialog ? 'Xóa tin nhắn?' : title;
                const normalizedBody = isMessageDeleteDialog ? 'Tin nhắn sẽ hiển thị là đã bị xóa với tất cả thành viên trong phòng.' : body;
                const normalizedActions = (actions || [])
                    .filter((action) => true)
                    .map((action) => isMessageDeleteDialog && action.value === 'everyone' ? Object.assign({}, action, { label: 'Xóa tin nhắn' }) : action);
                backdrop.innerHTML = `
                    <div class="da-modal da-confirm" role="dialog" aria-modal="true">
                        <div class="da-modal-header">
                            <div><h2>${U.escapeHtml(normalizedTitle)}</h2><p>${U.escapeHtml(normalizedBody || '')}</p></div>
                            <button type="button" class="da-icon-btn" data-value=""><i class="bi bi-x-lg"></i></button>
                        </div>
                        <div class="da-modal-footer">
                            ${normalizedActions.map((action) => `<button type="button" class="${U.escapeHtml(action.className || 'da-secondary-btn')}" data-value="${U.escapeHtml(action.value)}">${U.escapeHtml(action.label)}</button>`).join('')}
                        </div>
                    </div>`;
                document.body.appendChild(backdrop);
                backdrop.querySelectorAll('[data-value]').forEach((button) => {
                    button.addEventListener('click', () => {
                        const value = button.dataset.value || '';
                        backdrop.remove();
                        resolve(value);
                    });
                });
            });
        }
    }

    class ConversationSidebar {
        constructor(root, onSelect, onDelete) {
            this.root = root;
            this.onSelect = onSelect;
            this.onDelete = onDelete;
            this.query = '';
            this.filter = 'all';

            this.bindConversationMenu();
        }

        bindConversationMenu() {
            this.menu = document.getElementById('chat-conversation-menu');

            if (!this.menu) {
                console.warn('chat-conversation-menu not found');
                return;
            }

            // Click nút xóa
            this.menu
                .querySelector('[data-action="delete"]')
                ?.addEventListener('click', (event) => {

                    event.stopPropagation();

                    const conversationId =
                        this.menu.dataset.conversationId;

                    if (conversationId) {
                        this.onDelete?.(conversationId);
                    }

                    this.menu.hidden = true;
                });

            // Click bên trong menu không đóng
            this.menu.addEventListener('click', (event) => {
                event.stopPropagation();
            });

            // Click ngoài menu => đóng
            document.addEventListener('click', () => {
                this.menu.hidden = true;
            });
        }

        setQuery(query) {
            this.query = String(query || '').toLowerCase();
        }

        setFilter(filter) {
            this.filter = filter || 'all';
        }

        render(state) {

            const items = state.conversations.filter((conversation) => {

                const name =
                    this.title(conversation, state).toLowerCase();

                const matchesQuery =
                    !this.query || name.includes(this.query);

                const matchesFilter =
                    this.filter === 'unread'
                        ? Number(conversation.unreadCount || 0) > 0
                        : this.filter === 'group'
                            ? conversation.type === 'group'
                            : true;

                return matchesQuery && matchesFilter;
            });

            if (!items.length) {
                this.root.innerHTML =
                    '<div class="da-chat-state">Chưa có cuộc trò chuyện phù hợp.</div>';
                return;
            }

            this.root.innerHTML = items.map((conversation) => {

                const id = String(conversation._id);

                const name =
                    this.title(conversation, state);

                const active =
                    id === state.activeConversationId
                        ? ' is-active'
                        : '';

                const isOnline =
                    this.isPrivateOnline(conversation, state);

                const last =
                    conversation.lastMessageBody ||
                    (conversation.type === 'group'
                        ? 'Nhóm chat'
                        : 'Nhắn tin 1-1');

                const unread =
                    Number(conversation.unreadCount || 0);

                return `
                <div class="da-conversation-item ${active}"
                     data-conversation-id="${U.escapeHtml(id)}"
                     role="listitem">

                    <button type="button"
                            class="da-conversation-open"
                            data-conversation-open="${U.escapeHtml(id)}">

                        <div class="da-conversation-avatar">
                            ${avatar(conversation, name)}
                            <span class="da-status-dot ${isOnline ? 'is-online' : ''}"></span>
                        </div>

                        <div class="da-conversation-copy">

                            <div class="da-conversation-name">
                                ${U.escapeHtml(name)}
                            </div>

                            <div class="da-conversation-meta">
                                ${U.escapeHtml(last)}
                            </div>

                        </div>

                        <div class="da-conversation-side">

                            <div class="da-conversation-meta-side">

                                <span class="da-time">
                                    ${U.escapeHtml(
                    U.formatTime(
                        conversation.lastMessageAt
                    )
                )}
                                </span>

                                ${unread
                        ? `
                                        <span class="da-unread-badge">
                                            ${unread > 99 ? '99+' : unread}
                                        </span>
                                      `
                        : ''}

                            </div>

                            <button type="button"
                                    class="da-conversation-more"
                                    data-conversation-more="${U.escapeHtml(id)}">

                                <i class="bi bi-three-dots"></i>

                            </button>

                        </div>

                    </button>

                </div>
            `;
            }).join('');

            // Chọn conversation
            this.root
                .querySelectorAll('[data-conversation-open]')
                .forEach((button) => {

                    button.addEventListener('click', () => {
                        this.onSelect(
                            button.dataset.conversationOpen
                        );
                    });

                });

            // Mở menu ...
            this.root
                .querySelectorAll('[data-conversation-more]')
                .forEach((button) => {

                    button.addEventListener('click', (event) => {

                        event.preventDefault();
                        event.stopPropagation();

                        if (!this.menu) return;

                        this.menu.hidden = false;

                        this.menu.style.left =
                            `${event.pageX}px`;

                        this.menu.style.top =
                            `${event.pageY}px`;

                        this.menu.dataset.conversationId =
                            button.dataset.conversationMore;
                    });

                });
        }

        title(conversation, state) {

            if (conversation.title)
                return conversation.title;

            const members =
                state.membersByConversation[
                String(conversation._id)
                ] || [];

            const other =
                members.find(
                    member =>
                        member.userId !==
                        state.currentUser?.id
                );

            return other?.userName ||
                (
                    conversation.type === 'group'
                        ? 'Nhóm chưa đặt tên'
                        : 'Tin nhắn riêng'
                );
        }

        isPrivateOnline(conversation, state) {

            if (conversation.type !== 'private')
                return false;

            const members =
                state.membersByConversation[
                String(conversation._id)
                ] || [];

            const other =
                members.find(
                    member =>
                        member.userId !==
                        state.currentUser?.id
                );

            return other
                ? state.onlineUserIds.has(other.userId)
                : false;
        }
    }
    class MessagePanel {
        constructor(root, callbacks) {
            this.root = root;
            this.callbacks = callbacks;
            this.lastConversationId = '';
            this.lastSignature = '';
            this.lastIds = [];
            this.bound = false;
        }

        render(state) {
            const conversationId = state.activeConversationId;
            if (!conversationId) {
                this.root.innerHTML = '';
                this.lastConversationId = '';
                this.lastSignature = '';
                this.lastIds = [];
                return;
            }

            const messages = state.messagesByConversation[conversationId] || [];
            const previousBottom = this.root.scrollHeight - this.root.scrollTop - this.root.clientHeight;
            const visible = messages.length > 180 ? messages.slice(-180) : messages;
            const shouldStickToBottom = this.lastConversationId !== conversationId || previousBottom < 120;
            const ids = visible.map((message) => this.messageKey(message));
            const signature = visible.map((message) => this.messageSignature(message)).join('|');

            if (this.lastConversationId === conversationId && signature === this.lastSignature) return;

            if (this.lastConversationId === conversationId && this.canAppend(this.lastIds, ids)) {
                this.appendMessages(visible.slice(this.lastIds.length), visible, state);
                this.lastIds = ids;
                this.lastSignature = signature;
                if (shouldStickToBottom) this.scrollToBottom();
                return;
            }

            this.renderAll(visible, state);
            this.lastConversationId = conversationId;
            this.lastIds = ids;
            this.lastSignature = signature;
            if (shouldStickToBottom) this.scrollToBottom();
        }

        renderAll(visible, state) {
            let lastDay = '';
            this.root.innerHTML = visible.map((message, index) => {
                const day = U.formatDate(message.createdAt);
                const sep = day && day !== lastDay ? `<div class="da-date-separator"><span>${U.escapeHtml(day)}</span></div>` : '';
                lastDay = day || lastDay;
                const previous = visible[index - 1];
                const compact = previous && previous.senderId === message.senderId && (new Date(message.createdAt) - new Date(previous.createdAt) < 180000);
                return sep + this.messageHtml(message, state, compact);
            }).join('');
            this.bindActions();
        }

        appendMessages(messagesToAppend, visible, state) {
            if (!messagesToAppend.length) return;
            const start = visible.length - messagesToAppend.length;
            let html = '';
            let lastDay = start > 0 ? U.formatDate(visible[start - 1].createdAt) : '';
            messagesToAppend.forEach((message, offset) => {
                const index = start + offset;
                const day = U.formatDate(message.createdAt);
                const sep = day && day !== lastDay ? `<div class="da-date-separator"><span>${U.escapeHtml(day)}</span></div>` : '';
                lastDay = day || lastDay;
                const previous = visible[index - 1];
                const compact = previous && previous.senderId === message.senderId && (new Date(message.createdAt) - new Date(previous.createdAt) < 180000);
                html += sep + this.messageHtml(message, state, compact);
            });
            this.root.insertAdjacentHTML('beforeend', html);
            this.bindActions();
        }

        canAppend(previousIds, nextIds) {
            return previousIds.length > 0
                && nextIds.length > previousIds.length
                && previousIds.every((id, index) => id === nextIds[index]);
        }

        scrollToBottom() {
            requestAnimationFrame(() => {
                this.root.scrollTop = this.root.scrollHeight;
            });
        }

        messageKey(message) {
            return String(message._id || message.clientMessageId || '');
        }

        messageSignature(message) {
            return [
                this.messageKey(message),
                message.body || '',
                message.editedAt || '',
                message.recalledAt || '',
                message.pinnedAt || '',
                message.isOptimistic ? '1' : '0',
                message.failed ? '1' : '0',
                JSON.stringify(message.reactions || []),
                JSON.stringify(message.attachments || [])
            ].join('~');
        }

        bindActions() {
            if (this.bound) return;
            this.bound = true;
            this.root.addEventListener('click', (event) => {
                const button = event.target.closest('[data-message-action]');
                if (!button || !this.root.contains(button)) return;
                event.stopPropagation();
                const row = button.closest('[data-message-id]');
                this.callbacks.onAction(button.dataset.messageAction, row?.dataset.messageId);
            });
            this.root.addEventListener('contextmenu', (event) => {
                const row = event.target.closest('.da-message-row');
                if (!row || !this.root.contains(row)) return;
                event.preventDefault();
                this.callbacks.onContext(row.dataset.messageId, event.clientX, event.clientY);
            });
        }

        messageHtml(message, state, compact) {
            const isMe = message.senderId === state.currentUser?.id;
            const isDeleted = Boolean(message.recalledAt);
            const isSystem = message.type === 'system';
            if (isSystem) {
                return `<div class="da-system-message" data-message-id="${U.escapeHtml(message._id || message.clientMessageId)}"><span>${U.escapeHtml(message.body || '')}</span></div>`;
            }
            const reactions = this.reactionHtml(message.reactions || []);
            const attachments = (message.attachments || []).map((file) => this.attachmentHtml(file)).join('');
            const reply = message.replyTo ? `<div class="da-reply-quote"><strong>${U.escapeHtml(message.replyTo.senderUserName || '')}</strong><span>${U.escapeHtml(message.replyTo.body || '[File]')}</span></div>` : '';
            const linkPreview = !isDeleted && message.body ? this.linkPreviewHtml(message.body) : '';
            const pinned = message.pinnedAt ? '<span class="da-pin"><i class="bi bi-pin-angle-fill"></i> Đã ghim</span>' : '';
            const status = isMe ? (message.isOptimistic ? 'Đang gửi' : 'Đã gửi') : '';

            const currentMember = (state.membersByConversation[String(message.conversationId)] || []).find((member) => member.userId === state.currentUser?.id);
            const isConversationAdmin = ['owner', 'admin'].includes(String(currentMember?.role || ''));
            const isSystemAdmin = (state.currentUser?.roles || []).some((role) => /admin/i.test(role));
            const canDelete = isMe || isConversationAdmin || isSystemAdmin;

            return `
                <div class="da-message-row ${isMe ? 'is-me' : ''} ${compact ? 'is-compact' : ''} ${message.isOptimistic ? 'is-pending' : ''}" data-message-id="${U.escapeHtml(message._id || message.clientMessageId)}">
                    ${isMe || compact ? '<div class="da-avatar-spacer"></div>' : avatar({ userName: message.senderUserName }, message.senderUserName)}
                    <div class="da-message-wrap">
                        <div class="da-message-bubble">
                            ${!isMe && !compact ? `<div class="da-message-author">${U.escapeHtml(message.senderUserName)}</div>` : ''}
                            ${pinned}
                            ${isDeleted ? '<div class="da-message-body da-message-deleted">Tin nhắn đã được thu hồi</div>' : `
                                ${reply}
                                ${message.body ? `<div class="da-message-body">${this.linkMentions(U.escapeHtml(message.body))}</div>` : ''}
                                ${linkPreview}
                                ${attachments}
                            `}
                            <div class="da-message-time">${U.escapeHtml(U.formatTime(message.createdAt))}${message.editedAt ? ' · đã sửa' : ''}${status ? ` · ${status}` : ''}</div>
                        </div>
                        ${reactions}
                        ${isDeleted ? '' : this.actionsHtml(isMe, true)}
                    </div>
                </div>`;
        }

        actionsHtml(isMe, canDelete) {
            return `
                <div class="da-message-actions">
                    ${canDelete ? '<button type="button" data-message-action="delete" title="Delete"><i class="bi bi-trash"></i></button>' : ''}
                    <button type="button" data-message-action="quick-react" title="Thả tim">❤️</button>
                    <button type="button" data-message-action="reply" title="Trả lời"><i class="bi bi-reply"></i></button>
                    <button type="button" data-message-action="copy" title="Sao chép"><i class="bi bi-copy"></i></button>
                    ${isMe ? '<button type="button" data-message-action="edit" title="Sửa"><i class="bi bi-pencil"></i></button>' : ''}
                    <button type="button" data-message-action="more" title="Thêm"><i class="bi bi-three-dots"></i></button>
                </div>`;
        }

        reactionHtml(reactions) {
            if (!reactions.length) return '';
            const groups = reactions.reduce((acc, item) => {
                acc[item.emoji] = (acc[item.emoji] || 0) + 1;
                return acc;
            }, {});
            return `<div class="da-reactions">${Object.entries(groups).map(([emoji, count]) => `<span>${emoji}${count > 1 ? ` ${count}` : ''}</span>`).join('')}</div>`;
        }

        attachmentHtml(file) {
            const url = file.url?.startsWith('http') ? file.url : `${window.DAChatRuntime.api.baseUrl}${file.url}`;
            if (file.type === 'image') return `<a class="da-attachment" href="${U.escapeHtml(url)}" target="_blank" rel="noopener"><img src="${U.escapeHtml(url)}" alt="${U.escapeHtml(file.fileName)}" /></a>`;
            if (file.type === 'video') return `<video class="da-attachment-video" src="${U.escapeHtml(url)}" controls preload="metadata"></video>`;
            return `
                <a class="da-attachment da-attachment-file" href="${U.escapeHtml(url)}" target="_blank" rel="noopener">
                    <span class="da-file-icon"><i class="bi bi-file-earmark-text"></i></span>
                    <span class="da-file-copy"><strong>${U.escapeHtml(file.fileName)}</strong><small>${U.escapeHtml(U.formatBytes(file.size))}</small></span>
                    <span class="da-file-download"><i class="bi bi-download"></i></span>
                </a>`;
        }

        linkMentions(html) {
            return html.replace(/(^|\s)@([\w.@-]+)/g, '$1<strong>@$2</strong>');
        }

        linkPreviewHtml(body) {
            const match = String(body || '').match(/https?:\/\/[^\s]+/i);
            if (!match) return '';
            let url;
            try {
                url = new URL(match[0]);
            } catch {
                return '';
            }
            return `
                <a class="da-link-preview" href="${U.escapeHtml(url.href)}" target="_blank" rel="noopener">
                    <div class="da-link-thumb"><i class="bi bi-link-45deg"></i></div>
                    <div>
                        <strong>${U.escapeHtml(url.hostname.replace(/^www\./, ''))}</strong>
                        <span>${U.escapeHtml(url.href)}</span>
                        <small>${U.escapeHtml(url.hostname)}</small>
                    </div>
                </a>`;
        }
    }

    class RightPanel {
        constructor(root) {
            this.root = root;
        }

        render(state) {
            const conversationId = state.activeConversationId;
            const conversation = state.conversations.find((item) => String(item._id) === conversationId);
            if (!conversation) {
                this.root.innerHTML = '<div class="da-right-empty">Chọn cuộc trò chuyện để xem thông tin.</div>';
                return;
            }

            const members = state.membersByConversation[conversationId] || [];
            const messages = state.messagesByConversation[conversationId] || [];
            const title = conversation.title || members.find((member) => member.userId !== state.currentUser?.id)?.userName || 'Tin nhắn';
            const me = members.find((member) => member.userId === state.currentUser?.id);
            const canManage = conversation.type === 'group' && ['owner', 'admin'].includes(me?.role);
            const attachments = messages.flatMap((message) => message.attachments || []);
            const media = attachments.filter((file) => file.type === 'image' || file.type === 'video').slice(-6).reverse();
            const files = attachments.filter((file) => file.type !== 'image' && file.type !== 'video').slice(-5).reverse();
            const links = messages.flatMap((message) => String(message.body || '').match(/https?:\/\/[^\s]+/ig) || []).slice(-5).reverse();

            this.root.innerHTML = `
                <div class="da-right-profile">
                    ${avatar(conversation, title, 'da-avatar-lg')}
                    <strong>${U.escapeHtml(title)}</strong>
                    <span>${conversation.type === 'group' ? `${members.length} thành viên` : 'Cuộc trò chuyện riêng'}</span>
                    ${conversation.type === 'group' ? `
                        <div class="da-right-actions">
                            ${canManage ? `<button type="button" class="da-secondary-btn" data-right-action="add-members"><i class="bi bi-person-plus"></i> Thêm người</button>` : ''}
                            <button type="button" class="da-secondary-btn da-danger-text" data-right-action="leave-group"><i class="bi bi-box-arrow-right"></i> Rời nhóm</button>
                        </div>` : ''}
                </div>
                <section class="da-right-section">
                    <h3>Thành viên</h3>
                    <div class="da-right-members">
                        ${members.slice(0, 12).map((member) => `
                            <div class="da-right-member">
                                <div class="da-avatar da-avatar-sm">${U.escapeHtml(U.initials(member.userName))}</div>
                                <div><strong>${U.escapeHtml(member.userName)}</strong><span>${U.escapeHtml(member.role || 'member')}</span></div>
                                ${canManage && member.userId !== state.currentUser?.id && member.role !== 'owner'
                                    ? `<button type="button" class="da-icon-btn da-member-remove" data-right-action="remove-member" data-user-id="${U.escapeHtml(member.userId)}" title="Xóa thành viên"><i class="bi bi-person-dash"></i></button>`
                                    : ''}
                            </div>`).join('') || '<div class="da-list-empty">Chưa có dữ liệu.</div>'}
                    </div>
                </section>
                <section class="da-right-section">
                    <h3>Ảnh và video</h3>
                    <div class="da-media-grid">
                        ${media.map((file) => this.mediaItem(file)).join('') || '<div class="da-list-empty">Chưa có media.</div>'}
                    </div>
                </section>
                <section class="da-right-section">
                    <h3>File đã chia sẻ</h3>
                    <div class="da-right-list">
                        ${files.map((file) => this.fileItem(file)).join('') || '<div class="da-list-empty">Chưa có file.</div>'}
                    </div>
                </section>
                <section class="da-right-section">
                    <h3>Liên kết</h3>
                    <div class="da-right-list">
                        ${links.map((link) => `<a href="${U.escapeHtml(link)}" target="_blank" rel="noopener"><i class="bi bi-link-45deg"></i><span>${U.escapeHtml(link)}</span></a>`).join('') || '<div class="da-list-empty">Chưa có liên kết.</div>'}
                    </div>
                </section>`;
        }

        mediaItem(file) {
            const url = file.url?.startsWith('http') ? file.url : `${window.DAChatRuntime.api.baseUrl}${file.url}`;
            if (file.type === 'video') return `<a class="da-media-tile" href="${U.escapeHtml(url)}" target="_blank" rel="noopener"><i class="bi bi-play-circle"></i></a>`;
            return `<a class="da-media-tile" href="${U.escapeHtml(url)}" target="_blank" rel="noopener"><img src="${U.escapeHtml(url)}" alt=""></a>`;
        }

        fileItem(file) {
            const url = file.url?.startsWith('http') ? file.url : `${window.DAChatRuntime.api.baseUrl}${file.url}`;
            return `<a href="${U.escapeHtml(url)}" target="_blank" rel="noopener"><i class="bi bi-file-earmark"></i><span>${U.escapeHtml(file.fileName)}</span></a>`;
        }
    }

    class UserPicker {
        constructor(elements, api, toasts, onChange) {
            this.elements = elements;
            this.api = api;
            this.toasts = toasts;
            this.onChange = onChange;
            this.selected = [];
            this.lastUsers = [];
            this.bind();
        }

        bind() {
            this.elements.search.addEventListener('input', U.debounce(() => this.search(), 220));
        }

        async search() {
            const keyword = this.elements.search.value.trim();
            if (keyword.length < 2) {
                this.elements.results.innerHTML = '<div class="da-list-empty">Nhập ít nhất 2 ký tự để tìm kiếm.</div>';
                return;
            }
            try {
                const response = await this.api.searchUsers(keyword);
                this.lastUsers = response.data || [];
                this.renderResults();
            } catch (error) {
                this.toasts.show('Không tìm được user', error.message);
            }
        }

        renderResults() {
            const selectedIds = new Set(this.selected.map((user) => user.id));
            this.elements.results.innerHTML = this.lastUsers.length ? this.lastUsers.map((user) => `
                <button type="button" class="da-user-option ${selectedIds.has(user.id) ? 'is-selected' : ''}" data-user-id="${U.escapeHtml(user.id)}">
                    ${avatar(user, U.displayName(user))}
                    <div>
                        <div class="da-conversation-name">${U.escapeHtml(U.displayName(user))}</div>
                        <div class="da-conversation-meta">${U.escapeHtml(user.userName || user.email || '')}</div>
                    </div>
                    ${selectedIds.has(user.id) ? '<i class="bi bi-check-circle-fill"></i>' : ''}
                </button>
            `).join('') : '<div class="da-list-empty">Không có kết quả phù hợp.</div>';

            this.elements.results.querySelectorAll('[data-user-id]').forEach((button) => {
                button.addEventListener('click', () => {
                    const user = this.lastUsers.find((item) => item.id === button.dataset.userId);
                    if (!user) return;
                    if (selectedIds.has(user.id)) this.selected = this.selected.filter((item) => item.id !== user.id);
                    else this.selected.push(user);
                    this.renderSelected();
                    this.onChange?.();
                    this.renderResults();
                });
            });
        }

        renderSelected() {
            this.elements.selected.innerHTML = this.selected.length ? this.selected.map((user) => `
                <span class="da-user-chip">
                    ${avatar(user, U.displayName(user), 'da-avatar-xs')}
                    ${U.escapeHtml(U.displayName(user))}
                    <button type="button" class="da-chip-remove" data-remove-user="${U.escapeHtml(user.id)}" aria-label="Bỏ chọn"><i class="bi bi-x"></i></button>
                </span>
            `).join('') : '<span class="da-selected-placeholder">Chưa chọn thành viên</span>';
            this.elements.selected.querySelectorAll('[data-remove-user]').forEach((button) => {
                button.addEventListener('click', () => {
                    this.selected = this.selected.filter((user) => user.id !== button.dataset.removeUser);
                    this.renderSelected();
                    this.onChange?.();
                    this.renderResults();
                });
            });
        }

        reset() {
            this.selected = [];
            this.lastUsers = [];
            this.elements.search.value = '';
            this.elements.results.innerHTML = '<div class="da-list-empty">Tìm theo tên, username hoặc email.</div>';
            this.renderSelected();
            this.onChange?.();
        }
    }

    class GroupModal {
        constructor(elements, api, onCreate, toasts) {
            this.elements = elements;
            this.api = api;
            this.onCreate = onCreate;
            this.toasts = toasts;
            this.picker = new UserPicker(elements, api, toasts, () => this.validate(false));
            this.bind();
            this.picker.reset();
        }

        bind() {
            this.elements.name.addEventListener('input', () => this.validate(false));
            this.elements.create.addEventListener('click', async () => {
                if (!this.validate(true)) return;
                this.elements.create.disabled = true;
                try {
                    await this.onCreate(this.elements.name.value.trim(), this.picker.selected);
                    this.close();
                } finally {
                    this.elements.create.disabled = false;
                }
            });
        }

        validate(showToast) {
            const title = this.elements.name.value.trim();
            let message = '';
            if (title.length < 3) message = 'Tên nhóm cần tối thiểu 3 ký tự.';
            else if (!this.picker.selected.length) message = 'Vui lòng chọn ít nhất một thành viên.';
            this.elements.error.textContent = message;
            this.elements.create.disabled = Boolean(message);
            if (message && showToast) this.toasts.show('Chưa thể tạo nhóm', message);
            return !message;
        }

        open() {
            this.elements.modal.hidden = false;
            this.elements.name.focus();
            this.validate(false);
        }

        close() {
            this.elements.modal.hidden = true;
            this.elements.name.value = '';
            this.elements.error.textContent = '';
            this.picker.reset();
        }
    }

    class PrivateModal {
        constructor(elements, api, onSelect, toasts) {
            this.elements = elements;
            this.api = api;
            this.onSelect = onSelect;
            this.toasts = toasts;
            this.bind();
        }

        bind() {
            this.elements.search.addEventListener('input', U.debounce(async () => {
                const keyword = this.elements.search.value.trim();
                if (keyword.length < 2) {
                    this.elements.results.innerHTML = '<div class="da-list-empty">Nhập ít nhất 2 ký tự để tìm kiếm.</div>';
                    return;
                }
                try {
                    const response = await this.api.searchUsers(keyword);
                    this.renderResults(response.data || []);
                } catch (error) {
                    this.toasts.show('Không tìm được user', error.message);
                }
            }, 220));
        }

        open() {
            this.elements.modal.hidden = false;
            this.elements.search.focus();
            this.elements.results.innerHTML = '<div class="da-list-empty">Tìm theo tên, username hoặc email.</div>';
        }

        close() {
            this.elements.modal.hidden = true;
            this.elements.search.value = '';
            this.elements.results.innerHTML = '';
        }

        renderResults(users) {
            this.elements.results.innerHTML = users.length ? users.map((user) => `
                <button type="button" class="da-user-option" data-user-id="${U.escapeHtml(user.id)}">
                    ${avatar(user, U.displayName(user))}
                    <div>
                        <div class="da-conversation-name">${U.escapeHtml(U.displayName(user))}</div>
                        <div class="da-conversation-meta">${U.escapeHtml(user.userName || user.email || '')}</div>
                    </div>
                </button>
            `).join('') : '<div class="da-list-empty">Không có kết quả phù hợp.</div>';
            this.elements.results.querySelectorAll('[data-user-id]').forEach((button) => {
                button.addEventListener('click', async () => {
                    const user = users.find((item) => item.id === button.dataset.userId);
                    if (!user) return;
                    await this.onSelect(user);
                    this.close();
                });
            });
        }
    }

    window.DAChatComponents = { Toasts, ConfirmDialog, ConversationSidebar, MessagePanel, RightPanel, GroupModal, PrivateModal, UserPicker };
})(window);
