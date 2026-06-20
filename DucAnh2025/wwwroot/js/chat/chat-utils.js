(function (window) {
    'use strict';

    const emojiSet = ['👍', '❤️', '😂', '😮', '😢', '😡', '😊', '👏', '🙏', '📌', '✅', '🚀'];
    const reactionSet = ['👍', '❤️', '😂', '😮', '😢', '😡'];
    const maxUploadSize = 100 * 1024 * 1024;
    const allowedMime = [
        /^image\//,
        /^video\//,
        /^text\/plain$/,
        /^application\/pdf$/,
        /^application\/zip$/,
        /^application\/msword$/,
        /^application\/vnd\.ms-/,
        /^application\/vnd\.openxmlformats-/,
        /^application\/octet-stream$/
    ];

    function escapeHtml(value) {
        return String(value || '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function initials(name) {
        const clean = String(name || 'Chat').trim();
        return clean.split(/\s+/).slice(0, 2).map((part) => part[0]).join('').toUpperCase();
    }

    function displayName(user) {
        const fullName = `${user?.firstName || ''} ${user?.lastName || ''}`.trim();
        return fullName || user?.name || user?.userName || user?.email || 'Người dùng';
    }

    function avatarUrl(entity) {
        return entity?.avatarUrl || entity?.avatar || entity?.photoUrl || '';
    }

    function formatTime(value) {
        if (!value) return '';
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return '';
        return new Intl.DateTimeFormat('vi-VN', { hour: '2-digit', minute: '2-digit' }).format(date);
    }

    function formatDate(value) {
        if (!value) return '';
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) return '';
        return new Intl.DateTimeFormat('vi-VN', { weekday: 'short', day: '2-digit', month: '2-digit', year: 'numeric' }).format(date);
    }

    function formatBytes(bytes) {
        const size = Number(bytes || 0);
        if (size < 1024) return `${size} B`;
        if (size < 1024 * 1024) return `${Math.ceil(size / 1024)} KB`;
        return `${(size / 1024 / 1024).toFixed(size > 10 * 1024 * 1024 ? 0 : 1)} MB`;
    }

    function debounce(fn, wait) {
        let timer = 0;
        return function (...args) {
            window.clearTimeout(timer);
            timer = window.setTimeout(() => fn.apply(this, args), wait);
        };
    }

    function uid(prefix) {
        if (window.crypto?.randomUUID) return `${prefix}-${window.crypto.randomUUID()}`;
        return `${prefix}-${Date.now()}-${Math.random().toString(16).slice(2)}`;
    }

    function isAllowedFile(file) {
        return Boolean(file && file.size <= maxUploadSize && allowedMime.some((pattern) => pattern.test(file.type || 'application/octet-stream')));
    }

    function fileType(file) {
        if (file?.type?.startsWith('image/')) return 'image';
        if (file?.type?.startsWith('video/')) return 'video';
        return 'file';
    }

    window.DAChatUtils = {
        emojiSet,
        reactionSet,
        maxUploadSize,
        escapeHtml,
        initials,
        displayName,
        avatarUrl,
        formatTime,
        formatDate,
        formatBytes,
        debounce,
        uid,
        isAllowedFile,
        fileType
    };
})(window);
