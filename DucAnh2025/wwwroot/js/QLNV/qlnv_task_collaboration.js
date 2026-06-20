window.qlnvCollab = (function () {
    const userCache = {};
    const mentionTimers = {};

    function esc(value) {
        return String(value ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function toTaskId(task) {
        return task?.id || task?.Id || '';
    }

    function api(options) {
        return getJwtToken().then(function (resUser) {
            options.headers = options.headers || {};
            options.headers.Authorization = 'Bearer ' + resUser.token;
            if (options.json !== false) {
                options.headers['Content-Type'] = options.headers['Content-Type'] || 'application/json';
            }
            return $.ajax(options).then(function (res) {
                return { res: res, resUser: resUser };
            });
        });
    }

    function renderShell(task, mode) {
        const taskId = esc(toTaskId(task));
        const isInline = mode === 'inline';
        return `
            <div class="qlnv-collab" data-task-id="${taskId}">
                <ul class="nav nav-tabs qlnv-collab-tabs" role="tablist">
                    <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#qlnv-comments-${taskId}" role="tab">Bình luận</a></li>
                    <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#qlnv-watchers-${taskId}" role="tab">Theo dõi</a></li>
                    <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#qlnv-timeline-${taskId}" role="tab">Timeline</a></li>
                    <li class="nav-item"><a class="nav-link" data-toggle="tab" href="#qlnv-events-${taskId}" role="tab">Events</a></li>
                </ul>
                <div class="tab-content qlnv-collab-body ${isInline ? '' : 'qlnv-collab-modal-body'}">
                    <div class="tab-pane fade show active" id="qlnv-comments-${taskId}" role="tabpanel">
                        <div class="qlnv-comment-list"></div>
                        <div class="qlnv-comment-composer">
                            <div class="qlnv-reply-target alert alert-light py-2 px-3 mb-2" style="display:none;">
                                <span class="qlnv-reply-text"></span>
                                <button type="button" class="close qlnv-cancel-reply" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            </div>
                            <div class="qlnv-comment-input-wrap">
                                <textarea class="form-control qlnv-comment-input" rows="3" placeholder="Nhập bình luận..."></textarea>
                                <div class="qlnv-mention-suggestions" style="display:none;"></div>
                            </div>
                            <div class="qlnv-selected-mentions"></div>
                            <div class="d-flex justify-content-between align-items-center mt-2">
                                <small class="text-muted">Reply: bấm nút trả lời ở từng bình luận.</small>
                                <button type="button" class="btn btn-primary btn-sm qlnv-send-comment" style="min-width:100px">Gửi bình luận</button>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="qlnv-watchers-${taskId}" role="tabpanel">
                        <div class="qlnv-watcher-actions mb-2">
                            <button type="button" class="btn btn-outline-primary btn-sm qlnv-watch-me">Theo dõi tôi</button>
                        </div>
                        <div class="mb-3">
                            <select class="form-control qlnv-watch-user-select"></select>
                            <button type="button" class="btn btn-primary btn-sm qlnv-add-watcher mt-2">Thêm người theo dõi</button>
                        </div>
                        <div class="qlnv-watcher-list"></div>
                    </div>
                    <div class="tab-pane fade" id="qlnv-timeline-${taskId}" role="tabpanel">
                        <div class="qlnv-timeline-list"></div>
                    </div>
                    <div class="tab-pane fade" id="qlnv-events-${taskId}" role="tabpanel">
                        <div class="qlnv-event-list"></div>
                    </div>
                </div>
            </div>
        `;
    }

    function mentionIds($root) {
        return $root.data('mentionIds') || [];
    }

    function getDropdownParent($root) {
        const $modal = $root.closest('.modal');
        return $modal.length ? $modal : $(document.body);
    }

    function userLabel(user) {
        const fullName = `${user.firstName || ''} ${user.lastName || ''}`.trim();
        return fullName ? `${fullName} (${user.userName})` : (user.userName || user.id);
    }

    function displayCommentBody(value) {
        return String(value || '').replace(/@reply\s+[0-9a-f-]{36}\s*/gi, '').trim();
    }

    function getCommentScrollContainer($root) {
        const $modalBody = $root.closest('.qlnv-collab-modal-body');
        if ($modalBody.length) return $modalBody;

        const $controlBody = $root.closest('.control-sidebar-body');
        if ($controlBody.length) return $controlBody;

        return $root.find('.qlnv-comment-list');
    }

    function isNearBottom($container) {
        const el = $container.get(0);
        if (!el) return true;
        return el.scrollHeight - el.scrollTop - el.clientHeight < 80;
    }

    function preserveScroll($root, mutate, shouldStickToBottom) {
        const $container = getCommentScrollContainer($root);
        const el = $container.get(0);
        if (!el) {
            mutate();
            return;
        }

        const oldHeight = el.scrollHeight;
        const oldTop = el.scrollTop;
        const stickToBottom = typeof shouldStickToBottom === 'boolean' ? shouldStickToBottom : isNearBottom($container);

        mutate();

        requestAnimationFrame(function () {
            if (stickToBottom) {
                el.scrollTop = el.scrollHeight;
            } else {
                el.scrollTop = oldTop + (el.scrollHeight - oldHeight);
            }
        });
    }

    function fetchTaskUsers($root, keyword) {
        const taskId = $root.data('task-id');
        const q = (keyword || '').trim();
        const cacheKey = `${taskId}|${q.toLowerCase()}`;
        if (userCache[cacheKey]) {
            return $.Deferred().resolve(userCache[cacheKey]).promise();
        }

        return api({
            url: `/api/QLNVTaskCollaboration/users?idCongViec=${encodeURIComponent(taskId)}&keyword=${encodeURIComponent(q)}&take=20`,
            method: 'GET',
            json: false
        }).then(function ({ res }) {
            const users = res.success && Array.isArray(res.data) ? res.data : [];
            userCache[cacheKey] = users;
            return users;
        }).catch(function () {
            return [];
        });
    }

    function loadUsers($root) {
        return fetchTaskUsers($root, '').then(function (users) {
            fillUserSelects($root, users);
            return users;
        });
    }
    function fillUserSelects($root, users) {
        $root.data('users', users);

        const options = ['<option></option>'].concat(users.map(user =>
            `<option value="${esc(user.id)}">${esc(userLabel(user))}</option>`
        )).join('');

        const $watcher = $root.find('.qlnv-watch-user-select');

        $watcher.html(options);

        if ($.fn.select2) {
            if ($watcher.hasClass('select2-hidden-accessible')) $watcher.select2('destroy');

            $watcher.select2({
                theme: 'bootstrap4',
                width: '100%',
                placeholder: 'Chọn người theo dõi',
                allowClear: true,
                dropdownParent: getDropdownParent($root),
                minimumInputLength: 0,
                ajax: {
                    delay: 250,
                    transport: function (params, success, failure) {
                        fetchTaskUsers($root, params.data?.term || '')
                            .then(function (items) {
                                success({
                                    results: items.map(user => ({
                                        id: user.id,
                                        text: userLabel(user)
                                    }))
                                });
                            })
                            .catch(failure);
                    },
                    processResults: function (data) {
                        return data;
                    }
                }
            });
        }
    }
    function currentMentionQuery(text, caret) {
        const left = text.slice(0, caret);
        const match = left.match(/(^|\s)@([\p{L}\p{N}._-]*)$/u);
        if (!match) return null;
        return {
            query: match[2] || '',
            start: caret - match[2].length - 1,
            end: caret
        };
    }

    function showMentionSuggestions($root) {
        const $input = $root.find('.qlnv-comment-input');
        const input = $input.get(0);
        if (!input) return;

        const text = $input.val() || '';
        const mention = currentMentionQuery(text, input.selectionStart || 0);
        const $box = $root.find('.qlnv-mention-suggestions');
        if (!mention) {
            $box.hide().empty();
            return;
        }

        const q = mention.query.toLowerCase();
        const taskId = $root.data('task-id');
        clearTimeout(mentionTimers[taskId]);
        mentionTimers[taskId] = setTimeout(function () {
            fetchTaskUsers($root, q).then(function (users) {
                const inputNow = $input.get(0);
                const current = currentMentionQuery($input.val() || '', inputNow?.selectionStart || 0);
                if (!current || current.query.toLowerCase() !== q) return;

                const existing = $root.data('users') || [];
                const merged = existing.concat(users).filter((user, index, arr) =>
                    arr.findIndex(x => x.id === user.id) === index
                );
                $root.data('users', merged);

                const matches = users.slice(0, 8);
                if (!matches.length) {
                    $box.hide().empty();
                    return;
                }

                $box.html(matches.map(user => `
                    <button type="button" class="qlnv-mention-option" data-user-id="${esc(user.id)}" data-label="${esc(userLabel(user))}">
                        <span class="qlnv-mention-avatar">${esc((user.firstName || user.userName || '?').charAt(0).toUpperCase())}</span>
                        <span>
                            <strong>${esc(userLabel(user))}</strong>
                            <small>${esc(user.role || user.email || user.userName || '')}</small>
                        </span>
                    </button>
                `).join('')).show();
            });
        }, 220);
    }

    function selectMention($root, userId, label) {
        const $input = $root.find('.qlnv-comment-input');
        const input = $input.get(0);
        if (!input) return;

        const text = $input.val() || '';
        const caret = input.selectionStart || 0;
        const mention = currentMentionQuery(text, caret);
        if (!mention) return;

        const safeLabel = label.replace(/\s*\([^)]*\)\s*$/, '').trim();
        const replacement = `@${safeLabel} `;
        const nextText = text.slice(0, mention.start) + replacement + text.slice(mention.end);
        $input.val(nextText);

        const ids = new Set($root.data('mentionIds') || []);
        ids.add(userId);
        $root.data('mentionIds', Array.from(ids));
        renderSelectedMentions($root);

        const nextCaret = mention.start + replacement.length;
        input.focus();
        input.setSelectionRange(nextCaret, nextCaret);
        $root.find('.qlnv-mention-suggestions').hide().empty();
    }

    function renderSelectedMentions($root) {
        const ids = $root.data('mentionIds') || [];
        const users = $root.data('users') || [];
        const html = ids.map(id => {
            const user = users.find(x => x.id === id);
            return `<span class="qlnv-selected-mention" data-user-id="${esc(id)}">@${esc(user ? userLabel(user) : 'người dùng')} <button type="button" class="qlnv-remove-mention">&times;</button></span>`;
        }).join('');
        $root.find('.qlnv-selected-mentions').html(html);
    }

    function removeMention($root, userId) {
        const ids = ($root.data('mentionIds') || []).filter(id => id !== userId);
        $root.data('mentionIds', ids);
        renderSelectedMentions($root);
    }

    function resetMentions($root) {
        $root.data('mentionIds', []);
        $root.find('.qlnv-selected-mentions').empty();
        $root.find('.qlnv-mention-suggestions').hide().empty();
    }

    function loadComments($root) {
        const taskId = $root.data('task-id');
        const $list = $root.find('.qlnv-comment-list').html('<div class="text-muted py-2">Đang tải bình luận...</div>');
        return api({
            url: `/api/QLNVTaskCollaboration/comments?idCongViec=${encodeURIComponent(taskId)}&take=100`,
            method: 'GET',
            json: false
        }).then(function ({ res }) {
            const payload = res.data || {};
            const data = Array.isArray(payload.comments) ? payload.comments : (Array.isArray(res.data) ? res.data : []);
            const policy = payload.policy || {};
            if (!data.length) {
                $list.html('<div class="qlnv-comment-empty text-muted py-2">Chưa có bình luận.</div>');
            } else {
                $list.html(renderCommentTree(data));
            }

            $root.data('comments', data);

            const canComment = Boolean(policy.canComment);
            $root.data('canComment', canComment);

            const $composer = $root.find('.qlnv-comment-composer');
            if (!canComment) {
                $composer.hide();
                if (!$root.find('.qlnv-comment-no-permission').length) {
                    $root.find('.qlnv-comment-list').after('<div class="qlnv-comment-no-permission text-muted small py-2">Bạn không có quyền bình luận trong công việc này.</div>');
                }
            } else {
                $composer.show();
                $root.find('.qlnv-comment-no-permission').remove();
            }

            $root.find('.qlnv-reply-comment').toggle(canComment);

            return data;
        }).catch(function () {
            $list.html('<div class="text-danger py-2">Không tải được bình luận.</div>');
            $root.find('.qlnv-comment-composer').hide();
            $root.data('canComment', false);
        });
    }

    function renderCommentTree(items) {
        const byParent = {};
        items.forEach(item => {
            const key = item.parentCommentId || '';
            byParent[key] = byParent[key] || [];
            byParent[key].push(item);
        });

        function renderBranch(parentId, level) {
            return (byParent[parentId || ''] || [])
                .map(item => renderComment(item, level) + renderBranch(item.id, level + 1))
                .join('');
        }

        return renderBranch('', 0);
    }

    function renderComment(item, level) {
        const time = item.createAt ? moment(item.createAt).format('DD/MM/YYYY HH:mm') : '';
        const mentions = Array.isArray(item.mentions) && item.mentions.length
            ? `<div class="qlnv-comment-mentions">${item.mentions.map(x => `<span>@${esc(x.userName || x.userId)}</span>`).join('')}</div>`
            : '';
        const replyLabel = item.parentCommentId ? '<span class="badge badge-light mr-1">Trả lời</span>' : '';
        const deleteButton = item.canDelete
            ? '<button type="button" class="btn btn-link btn-sm text-danger qlnv-delete-comment">Xóa</button>'
            : '';
        const margin = Math.min(level, 4) * 28;
        return `
            <div class="qlnv-comment-item ${level > 0 ? 'qlnv-comment-reply' : ''}" data-comment-id="${esc(item.id)}" data-parent-comment-id="${esc(item.parentCommentId || '')}" data-level="${level}" data-author="${esc(item.createBy || 'Người dùng')}" style="margin-left:${margin}px">
                <div class="d-flex justify-content-between">
                    <div class="font-weight-bold">${replyLabel}${esc(item.createBy || 'Người dùng')}</div>
                    <small class="text-muted">${esc(time)}${item.isEdited ? ' · đã sửa' : ''}</small>
                </div>
                <div class="qlnv-comment-content">${esc(displayCommentBody(item.noiDung))}</div>
                ${mentions}
                <div class="mt-1">
                    <button type="button" class="btn btn-link btn-sm p-0 qlnv-reply-comment">Trả lời</button>
                    ${deleteButton}
                </div>
            </div>
        `;
    }

    function updateLocalCommentState($root, comment) {
        const items = $root.data('comments') || [];
        const index = items.findIndex(x => x.id === comment.id);
        if (index >= 0) {
            items[index] = comment;
        } else {
            items.push(comment);
        }
        $root.data('comments', items);
    }

    function insertCommentNode($root, comment, parentCommentId) {
        if (!comment || !comment.id) return;
        if ($root.find(`.qlnv-comment-item[data-comment-id="${comment.id}"]`).length) return;

        updateLocalCommentState($root, comment);

        const $list = $root.find('.qlnv-comment-list');
        const $parent = parentCommentId
            ? $list.find(`.qlnv-comment-item[data-comment-id="${parentCommentId}"]`).first()
            : $();
        const parentLevel = $parent.length ? Number($parent.attr('data-level') || 0) : -1;
        const level = parentCommentId ? parentLevel + 1 : 0;
        const html = renderComment(comment, level);
        const shouldStick = !parentCommentId && isNearBottom(getCommentScrollContainer($root));

        preserveScroll($root, function () {
            $list.find('.qlnv-comment-empty').remove();

            if (!$parent.length) {
                $list.append(html);
                return;
            }

            let $insertAfter = $parent;
            let $cursor = $parent.next('.qlnv-comment-item');
            while ($cursor.length) {
                const cursorLevel = Number($cursor.attr('data-level') || 0);
                if (cursorLevel <= parentLevel) break;
                $insertAfter = $cursor;
                $cursor = $cursor.next('.qlnv-comment-item');
            }
            $insertAfter.after(html);
        }, shouldStick);

        $root.find('.qlnv-reply-comment').toggle(Boolean($root.data('canComment')));
    }

    function sendComment($root, parentCommentId) {
        if (!$root.data('canComment')) {
            showToastError('Bạn không có quyền gửi bình luận.');
            return;
        }

        const taskId = $root.data('task-id');
        const $input = $root.find('.qlnv-comment-input');
        const body = ($input.val() || '').replace(/@reply\s+[0-9a-f-]{36}\s*/gi, '').trim();
        if (!body) {
            showToastError('Vui lòng nhập nội dung bình luận.');
            return;
        }

        const $button = $root.find('.qlnv-send-comment');
        if ($button.data('sending')) return;
        $button.data('sending', true).prop('disabled', true).text('Đang gửi...');

        return api({
            url: '/api/QLNVTaskCollaboration/comments',
            method: 'POST',
            data: JSON.stringify({
                id_CongViec: taskId,
                parentCommentId: parentCommentId || null,
                noiDung: body,
                mentionUserIds: mentionIds($root)
            })
        }).then(function ({ res }) {
            if (!res.success) {
                showToastError(res.message || 'Không gửi được bình luận.');
                return;
            }
            $input.val('');
            resetMentions($root);
            if (!parentCommentId) {
                clearReply($root);
            } else {
                $input.attr('data-parent-comment-id', parentCommentId);
                $root.find('.qlnv-reply-target').show();
            }
            showToastSuccess('Đã gửi bình luận.');
            insertCommentNode($root, res.data, parentCommentId || null);
            $root.data('timelineDirty', true);
            $root.data('eventsDirty', true);
            $input.focus();
        }).always(function () {
            $button.data('sending', false).prop('disabled', false).text('Gửi bình luận');
        });
    }

    function deleteComment($root, id) {
        if (!confirm('Bạn có chắc chắn muốn xóa bình luận này?')) return;
        return api({
            url: `/api/QLNVTaskCollaboration/comments/${encodeURIComponent(id)}`,
            method: 'DELETE',
            json: false
        }).then(function ({ res }) {
            if (!res.success) {
                showToastError(res.message || 'Không xóa được bình luận.');
                return;
            }
            showToastSuccess('Đã xóa bình luận.');
            preserveScroll($root, function () {
                const $target = $root.find(`.qlnv-comment-item[data-comment-id="${id}"]`).first();
                const targetLevel = Number($target.attr('data-level') || 0);
                let $cursor = $target.next('.qlnv-comment-item');
                while ($cursor.length) {
                    const cursorLevel = Number($cursor.attr('data-level') || 0);
                    if (cursorLevel <= targetLevel) break;
                    const $next = $cursor.next('.qlnv-comment-item');
                    $cursor.remove();
                    $cursor = $next;
                }
                $target.remove();
                const items = ($root.data('comments') || []).filter(x => x.id !== id && x.parentCommentId !== id);
                $root.data('comments', items);
                if (!$root.find('.qlnv-comment-item').length) {
                    $root.find('.qlnv-comment-list').html('<div class="qlnv-comment-empty text-muted py-2">Chưa có bình luận.</div>');
                }
            }, false);
            $root.data('timelineDirty', true);
            $root.data('eventsDirty', true);
        });
    }

    function loadWatchers($root) {
        const taskId = $root.data('task-id');
        const $list = $root.find('.qlnv-watcher-list').html('<div class="text-muted py-2">Đang tải người theo dõi...</div>');
        return api({
            url: `/api/QLNVTaskCollaboration/watchers?idCongViec=${encodeURIComponent(taskId)}`,
            method: 'GET',
            json: false
        }).then(function ({ res }) {
            const data = Array.isArray(res.data) ? res.data : [];
            if (!data.length) {
                $list.html('<div class="text-muted py-2">Chưa có người theo dõi.</div>');
                return;
            }
            $list.html(data.map(x => `
                <div class="qlnv-watcher-item" data-user-id="${esc(x.userId)}">
                    <div>
                        <div class="font-weight-bold">${esc(x.userName || x.userId)}</div>
                        <small class="text-muted">Đang theo dõi</small>
                    </div>
                    <button type="button" class="btn btn-outline-danger btn-sm qlnv-remove-watcher">Xóa</button>
                </div>
            `).join(''));
        }).catch(function () {
            $list.html('<div class="text-danger py-2">Không tải được người theo dõi.</div>');
        });
    }

    function addWatcher($root, userId, userName) {
        const taskId = $root.data('task-id');
        if (!userId) {
            showToastError('Vui lòng chọn người dùng.');
            return;
        }
        return api({
            url: '/api/QLNVTaskCollaboration/watchers',
            method: 'POST',
            data: JSON.stringify({
                id_CongViec: taskId,
                userId: userId,
                userName: userName || ''
            })
        }).then(function ({ res }) {
            if (!res.success) {
                showToastError(res.message || 'Không thêm được người theo dõi.');
                return;
            }
            $root.find('.qlnv-watch-user-select').val(null).trigger('change');
            showToastSuccess('Đã thêm người theo dõi.');
            loadWatchers($root);
            loadTimeline($root);
            loadEvents($root);
        });
    }

    function removeWatcher($root, userId) {
        const taskId = $root.data('task-id');
        return api({
            url: `/api/QLNVTaskCollaboration/watchers?idCongViec=${encodeURIComponent(taskId)}&userId=${encodeURIComponent(userId)}`,
            method: 'DELETE',
            json: false
        }).then(function ({ res }) {
            if (!res.success) {
                showToastError(res.message || 'Không xóa được người theo dõi.');
                return;
            }
            showToastSuccess('Đã xóa người theo dõi.');
            loadWatchers($root);
            loadTimeline($root);
            loadEvents($root);
        });
    }

    function loadTimeline($root) {
        const taskId = $root.data('task-id');
        const $list = $root.find('.qlnv-timeline-list').html('<div class="text-muted py-2">Đang tải timeline...</div>');
        return api({
            url: `/api/QLNVTaskCollaboration/timeline?idCongViec=${encodeURIComponent(taskId)}&take=100`,
            method: 'GET',
            json: false
        }).then(function ({ res }) {
            const data = Array.isArray(res.data) ? res.data : [];
            if (!data.length) {
                $list.html('<div class="text-muted py-2">Chưa có activity.</div>');
                return;
            }
            $list.html(data.map(x => {
                const time = x.createAt ? moment(x.createAt).format('DD/MM/YYYY HH:mm') : '';
                const diff = x.oldValue || x.newValue
                    ? `<div class="small text-muted">Trước: ${esc(x.oldValue || '')}<br>Sau: ${esc(x.newValue || '')}</div>`
                    : '';
                return `
                    <div class="qlnv-timeline-item">
                        <div class="d-flex justify-content-between">
                            <strong>${esc(x.eventType)}</strong>
                            <small class="text-muted">${esc(time)}</small>
                        </div>
                        <div>${esc(x.description)}</div>
                        <small class="text-muted">${esc(x.actorUserName || '')}</small>
                        ${diff}
                    </div>
                `;
            }).join(''));
        }).catch(function () {
            $list.html('<div class="text-danger py-2">Không tải được timeline.</div>');
        });
    }

    function loadEvents($root) {
        const taskId = $root.data('task-id');
        const $list = $root.find('.qlnv-event-list').html('<div class="text-muted py-2">Đang tải events...</div>');
        return api({
            url: `/api/QLNVTaskCollaboration/events?idCongViec=${encodeURIComponent(taskId)}&take=100`,
            method: 'GET',
            json: false
        }).then(function ({ res }) {
            const data = Array.isArray(res.data) ? res.data : [];
            if (!data.length) {
                $list.html('<div class="text-muted py-2">Chưa có event.</div>');
                return;
            }
            $list.html(data.map(x => {
                const time = x.createAt ? moment(x.createAt).format('DD/MM/YYYY HH:mm') : '';
                return `
                    <div class="qlnv-event-item">
                        <div class="d-flex justify-content-between">
                            <strong>${esc(x.eventName)}</strong>
                            <small class="text-muted">${esc(time)}</small>
                        </div>
                        <pre>${esc(x.payloadJson || '')}</pre>
                    </div>
                `;
            }).join(''));
        }).catch(function () {
            $list.html('<div class="text-danger py-2">Không tải được events.</div>');
        });
    }

    function bind($root) {
        $root.off('click.qlnvCollab');
        $root.on('click.qlnvCollab', '.qlnv-send-comment', function () {
            const parentCommentId = $root.find('.qlnv-comment-input').attr('data-parent-comment-id') || null;
            sendComment($root, parentCommentId);
            $root.find('.qlnv-comment-input').removeAttr('data-parent-comment-id');
        });
        $root.on('click.qlnvCollab', '.qlnv-reply-comment', function () {
            const id = $(this).closest('.qlnv-comment-item').data('comment-id');
            const author = $(this).closest('.qlnv-comment-item').data('author') || 'bình luận';
            $root.find('.qlnv-comment-input').focus().attr('data-parent-comment-id', id);
            $root.find('.qlnv-reply-text').text(`Đang trả lời ${author}`);
            $root.find('.qlnv-reply-target').show();
        });
        $root.on('click.qlnvCollab', '.qlnv-cancel-reply', function () {
            clearReply($root);
        });
        $root.on('input.qlnvCollab keyup.qlnvCollab click.qlnvCollab', '.qlnv-comment-input', function () {
            showMentionSuggestions($root);
        });
        $root.on('keydown.qlnvCollab', '.qlnv-comment-input', function (e) {
            const $options = $root.find('.qlnv-mention-option:visible');
            if (e.key === 'Escape') {
                $root.find('.qlnv-mention-suggestions').hide().empty();
            }
            if (e.key === 'Enter' && $options.length) {
                e.preventDefault();
                const $first = $options.first();
                selectMention($root, $first.data('user-id'), $first.data('label'));
            }
        });
        $root.on('mousedown.qlnvCollab', '.qlnv-mention-option', function (e) {
            e.preventDefault();
            selectMention($root, $(this).data('user-id'), $(this).data('label'));
        });
        $root.on('click.qlnvCollab', '.qlnv-remove-mention', function () {
            removeMention($root, $(this).closest('.qlnv-selected-mention').data('user-id'));
        });
        $root.on('click.qlnvCollab', '.qlnv-delete-comment', function () {
            deleteComment($root, $(this).closest('.qlnv-comment-item').data('comment-id'));
        });
        $root.on('click.qlnvCollab', '.qlnv-add-watcher', function () {
            const $select = $root.find('.qlnv-watch-user-select');
            addWatcher($root, $select.val(), $select.find('option:selected').text());
        });
        $root.on('click.qlnvCollab', '.qlnv-watch-me', function () {
            getJwtToken().then(function (resUser) {
                addWatcher($root, resUser.userInfor.id, resUser.userInfor.userName);
            });
        });
        $root.on('click.qlnvCollab', '.qlnv-remove-watcher', function () {
            removeWatcher($root, $(this).closest('.qlnv-watcher-item').data('user-id'));
        });
        $root.find('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            const target = $(e.target).attr('href') || '';
            if (target.includes('watchers')) loadWatchers($root);
            if (target.includes('timeline') && $root.data('timelineDirty')) {
                loadTimeline($root);
                $root.data('timelineDirty', false);
            }
            if (target.includes('events') && $root.data('eventsDirty')) {
                loadEvents($root);
                $root.data('eventsDirty', false);
            }
        });
    }

    function init($root) {
        bind($root);
        loadUsers($root);
        loadComments($root);
        loadWatchers($root);
        loadTimeline($root);
        loadEvents($root);
    }

    function clearReply($root) {
        $root.find('.qlnv-comment-input').removeAttr('data-parent-comment-id');
        $root.find('.qlnv-reply-text').text('');
        $root.find('.qlnv-reply-target').hide();
    }

    function openModal(task) {
        const footer = `<button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>`;
        showGlobalModal(`Trao đổi công việc: ${esc(task.tenCongViec || task.TenCongViec || '')}`, renderShell(task, 'modal'), footer);
        setTimeout(function () {
            init($('.qlnv-collab[data-task-id="' + toTaskId(task) + '"]').last());
        }, 0);
    }

    function mountInline(selector, task) {
        const $container = $(selector);
        if (!$container.length || !toTaskId(task)) return;
        $container.html(renderShell(task, 'inline'));
        init($container.find('.qlnv-collab').first());
    }

    return {
        openModal: openModal,
        mountInline: mountInline
    };
})();
