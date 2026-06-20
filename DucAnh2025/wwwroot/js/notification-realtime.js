
// Kết nối SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().then(function () {
    // Đăng ký userId lên server nếu cần (tuỳ server implement)
    // connection.invoke("Register", userId);
}).catch(function (err) {
    console.error(err.toString());
});

// Lắng nghe sự kiện nhận thông báo mới
connection.on("ReceiveNotification", function (notification) {
    // Hiển thị toast với nội dung thực tế
    showNotification(notification.title || "Thông báo mới", notification.body || "");

    // Cập nhật lại số lượng/thông tin thông báo ở header
    reloadNotifications();

});


//Xử lý thông báo
// --- Paging state ---
let systemCurrentPage = 0;
let qlnvCurrentPage = 0;
const pageSize = 10;
let systemHasMore = true;
let qlnvHasMore = true;

function apiRequest(options) {
    return getJwtToken().then(function (resUser) {
        options.headers = options.headers || {};
        options.headers["Authorization"] = "Bearer " + resUser.token;
        return $.ajax(options);
    });
}

function renderNotificationItem(item, type, isHtml = false) {
    const isUnread = item.isRead === 0;
    const notiId = item.id;
    const title = type === 'system' ? (item.subject || '(Không tiêu đề)') : (item.title || '(Không tiêu đề)');
    const content = type === 'system' ? (item.content || '') : (item.body || '');
    const time = type === 'system'
        ? (item.createAt ? moment(item.createAt).format('DD/MM/YYYY HH:mm') : '')
        : (item.createdAt ? moment(item.createdAt).format('DD/MM/YYYY HH:mm') : '');

    return `
                 <div class="notification-item custom-noti ${type}-noti${isUnread ? ' unread' : ''}" data-id="${notiId}"
                 style="
                     border-bottom: 1px solid #e3e6f0;
                     cursor: pointer;
                     position: relative;
                 ">
                     <div class="noti-header d-flex align-items-center" style="padding: 14px 18px;">
                         <div class="noti-dot${isUnread ? '' : ' read'}" style="
                             width: 10px; height: 10px; border-radius: 50%; margin-right: 12px;
                             background: ${isUnread ? '#ff5252' : '#cfd8dc'};
                             flex-shrink: 0;
                             transition: background 0.2s;
                         "></div>
                         <div class="noti-title font-weight-bold" style="font-size: 1.05em; flex: 1; color: #222;">
                             ${title}
                         </div>
                         <div class="noti-time text-muted small" style="margin-left: 12px; white-space: nowrap;">
                             ${time}
                         </div>
                         <span class="noti-toggle-icon" style="margin-left: 10px; color: #888; font-size: 1.1em;">
                             <i class="fa fa-chevron-down"></i>
                         </span>
                     </div>
                     <div class="noti-content" style="
                         display: none;
                         padding: 0 18px 14px 40px;
                         color: #444;
                         font-size: 0.98em;
                         border-top: 1px solid #f1f1f1;
                         animation: fadeInNoti 0.3s;
                     ">
                         ${content}
                     </div>
                 </div>
                 <style>

                     .notification-item.unread { border-left: 4px solid #ff5252; }
                     .notification-item:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.08); }
                 </style>
             `;
}

function handleNotificationItemClick(e) {
    // Nếu click vào link hoặc button bên trong thì bỏ qua
    if ($(e.target).is('a, button, .close')) return;

    const $item = $(this);
    const $content = $item.find('.noti-content');
    const $icon = $item.find('.noti-toggle-icon i');
    const id = $item.data('id');
    const isSystem = $item.hasClass('system-noti');
    const isQLNV = $item.hasClass('qlnv-noti');
    const wasUnread = $item.hasClass('unread');

    // Toggle content with animation
    $content.slideToggle(180);
    $icon.toggleClass('fa-chevron-down fa-chevron-up');

    // Only mark as read if it was unread
    if (wasUnread) {
        $item.removeClass('unread');
        $item.find('.noti-dot').css('background', '#cfd8dc');
        getJwtToken().then(function (resUser) {
            const apiCall = isSystem
                ? apiRequest({ url: `/api/fcm/IsReadNotificationSystem?Id=${id}`, method: "PUT" })
                : apiRequest({ url: `/api/fcm/IsReadFireBaseId?Id=${id}`, method: "PUT" });

            apiCall
                .then(function () {
                    loadTotalUnreadNoti();
                })
                .catch(function (err) {
                    console.error("Lỗi gọi API đánh dấu đã đọc:", err);
                });
        });

    }
}
// Load tổng số thông báo chưa đọc (cộng cả 2 loại)
function loadTotalUnreadNoti() {
    getJwtToken().then(function (resUser) {
        let count1 = 0, count2 = 0;
        let done1 = false, done2 = false;

        apiRequest({
            url: `/api/fcm/GetUnreadNotiByUserId?Id=${encodeURIComponent(resUser.userInfor.id)}`,
            method: "GET"
        }).then(function (res) {
            count1 = res.success ? (res.data || 0) : 0;
        }).catch(function () {
            count1 = 0;
        }).finally(function () {
            done1 = true; update();
        });

        apiRequest({
            url: `/api/fcm/GetUnreadNotiFireBaseByUserId?Id=${encodeURIComponent(resUser.userInfor.id)}`,
            method: "GET"
        }).then(function (res) {
            count2 = res.success ? (res.data || 0) : 0;
        }).catch(function () {
            count2 = 0;
        }).finally(function () {
            done2 = true; update();
        });

        function update() {
            if (done1 && done2) {
                const total = count1 + count2;
                $('#header-notification-count').text(total > 0 ? total : '');
                $('.menu-header-subtitle').html(`Bạn có <b>${total}</b> thông báo chưa đọc`);
                // Cập nhật số lượng cho từng tab
                $('#hethong-unread-count').text(count1 > 0 ? count1 : 0);
                $('#qlnv-unread-count').text(count2 > 0 ? count2 : 0);

            }
        }
    });
}

function loadSystemNotifications(append = false) {
    if (!append) {
        systemCurrentPage = 0;
        systemHasMore = true;
    }
    if (!systemHasMore && append) return;

    getJwtToken().then(function (resUser) {
        apiRequest({
            url: `/api/fcm/GetAllNotiByUserId?userId=${encodeURIComponent(resUser.userInfor.id)}&currentPage=${systemCurrentPage}&pageSize=${pageSize}`,
            method: "GET"
        }).then(function (res) {
            let html = '';
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                res.data.forEach(function (item) {
                    html += renderNotificationItem(item, 'system');
                });
            } else if (!append) {
                html = '<div class="text-center text-muted py-2">Không có thông báo hệ thống.</div>';
            }

            // Chỉ hiển thị nút "Tải thêm" nếu còn dữ liệu (tức là đủ pageSize)
            if (res.data && res.data.length >= pageSize) {
                html += '<div class="text-center mt-2 mb-2"><button id="loadMoreSystemBtn" class="btn btn-outline-primary">Tải thêm</button></div>';
                systemHasMore = true;
            } else {
                systemHasMore = false;
            }

            if (append) {
                // Xóa nút cũ trước khi append để tránh lặp nút
                $('#tab-hethong-header .scroll-area-sm #loadMoreSystemBtn').parent().remove();
                $('#tab-hethong-header .scroll-area-sm').append(html);
            } else {
                $('#tab-hethong-header .scroll-area-sm').html(html);
            }

            // Gán lại sự kiện click cho nút
            $('#loadMoreSystemBtn').off('click').on('click', function () {
                systemCurrentPage++;
                loadSystemNotifications(true);
            });

            $('#tab-hethong-header .scroll-area-sm .notification-item').off('click').on('click', handleNotificationItemClick);
        });
    });
}

function loadQLNVNotifications(append = false) {
    if (!append) {
        qlnvCurrentPage = 0;
        qlnvHasMore = true;
    }
    if (!qlnvHasMore && append) return;

    getJwtToken().then(function (resUser) {
        apiRequest({
            url: `/api/fcm/GetAllNotiFireBaseByUserId?reciverId=${encodeURIComponent(resUser.userInfor.id)}&currentPage=${qlnvCurrentPage}&pageSize=${pageSize}`,
            method: "GET"
        }).then(function (res) {
            let html = '';
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                res.data.forEach(function (item) {
                    html += renderNotificationItem(item, 'qlnv');
                });
            } else if (!append) {
                html = '<div class="text-center text-muted py-2">Không có thông báo QLNV.</div>';
            }

            // Chỉ hiển thị nút "Tải thêm" nếu còn dữ liệu (tức là đủ pageSize)
            if (res.data && res.data.length >= pageSize) {
                html += '<div class="text-center mt-2 mb-2"><button id="loadMoreQLNVBtn" class="btn btn-outline-primary">Tải thêm</button></div>';
                qlnvHasMore = true;
            } else {
                qlnvHasMore = false;
            }

            if (append) {
                $('#tab-qlnv-header .scroll-area-sm #loadMoreQLNVBtn').parent().remove();
                $('#tab-qlnv-header .scroll-area-sm').append(html);
            } else {
                $('#tab-qlnv-header .scroll-area-sm').html(html);
            }

            // Gán lại sự kiện click cho nút
            $('#loadMoreQLNVBtn').off('click').on('click', function () {
                qlnvCurrentPage++;
                loadQLNVNotifications(true);
            });

            $('#tab-qlnv-header .scroll-area-sm .notification-item').off('click').on('click', handleNotificationItemClick);
        });
    });
}

// Khi mở dropdown thông báo hoặc khi cần reload
function reloadNotifications() {
    loadTotalUnreadNoti();
    loadSystemNotifications();
    loadQLNVNotifications();
}

$(document).ready(function () {
    // Use event delegation for dynamically rendered buttons
    $('#tab-hethong-header .scroll-area-sm').off('click', '#loadMoreSystemBtn').on('click', '#loadMoreSystemBtn', function () {
        systemCurrentPage++;
        loadSystemNotifications(true);
    });
    $('#tab-qlnv-header .scroll-area-sm').off('click', '#loadMoreQLNVBtn').on('click', '#loadMoreQLNVBtn', function () {
        qlnvCurrentPage++;
        loadQLNVNotifications(true);
    });
});
