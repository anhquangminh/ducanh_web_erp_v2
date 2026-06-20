

//Xử lý thông báo
// Hàm hiển thị toast
function showNotification(title, message) {
    // Tạo toast container nếu chưa có
    if ($('#custom-toast-container').length === 0) {
        $('body').append('<div id="custom-toast-container" style="position:fixed;top:20px;right:20px;z-index:9999;"></div>');
    }

    // Tạo toast HTML
    const toastId = 'toast-' + Date.now();
    const toastHtml = `
        <div id="${toastId}" class="custom-toast shadow" style="
            min-width:300px;
            max-width:400px;
            background:#fff;
            border-radius:8px;
            box-shadow:0 2px 8px rgba(0,0,0,0.15);
            margin-bottom:12px;
            padding:16px 20px 16px 16px;
            position:relative;
            display:flex;
            flex-direction:column;
            animation:fadeIn 0.3s;
        ">
            <div style="font-weight:bold;font-size:1.1em;margin-bottom:4px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">
                ${title}
            </div>
            <div style="
                color:#333;
                font-size:1em;
                display:-webkit-box;
                -webkit-line-clamp:4;
                -webkit-box-orient:vertical;
                overflow:hidden;
                text-overflow:ellipsis;
                margin-bottom:8px;
                line-height:1.4;
                max-height:5.6em;
            ">
                ${message}
            </div>
            <button type="button" class="close" style="
                position:absolute;
                top:8px;
                right:8px;
                background:transparent;
                border:none;
                font-size:1.2em;
                color:#888;
                cursor:pointer;
            " aria-label="Đóng">&times;</button>
        </div>
        <style>
            @keyframes fadeIn { from { opacity:0; transform:translateY(-10px);} to { opacity:1; transform:none; } }
        </style>
    `;

    // Thêm toast vào container
    $('#custom-toast-container').append(toastHtml);

    // Xử lý nút tắt
    $(`#${toastId} .close`).on('click', function () {
        $(`#${toastId}`).fadeOut(200, function () { $(this).remove(); });
    });

    // Tự động tắt sau 5 giây
    setTimeout(function () {
        $(`#${toastId}`).fadeOut(200, function () { $(this).remove(); });
    }, 5000);
}

function showToastSuccess(message) {
    toastr.success(message, "Thành công", { timeOut: 3000 });
}

function showToastError(message) {
    toastr.error(message, "Lỗi", { timeOut: 3000 });
}

function showGlobalModal(title, bodyHtml, footerHtml) {
    $('#globalModalLabel').html(title);
    $('#globalModalBody').html(bodyHtml);
    if (footerHtml) {
        $('#globalModalFooter').html(footerHtml);
    }
    /*$('#globalModal').modal('show');*/
    $('#globalModal').modal({
        backdrop: 'static',
        keyboard: false
    });
}
function roundToDecimal(value, decimals) {
    if (isNaN(value) || isNaN(decimals)) return NaN;
    const factor = Math.pow(10, decimals);
    return Number((Math.round(value * factor) / factor).toFixed(decimals));
}
function generateUUID() {
    var d = new Date().getTime();
    var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;
        if (d > 0) {
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}


document.addEventListener('DOMContentLoaded', function () {
    getJwtToken().then(function (resUser) {
        if (resUser && resUser.userInfor && resUser.userInfor.name) {
            const fullnameDiv = document.getElementById('fullname');
            if (fullnameDiv) {
                fullnameDiv.textContent = resUser.userInfor.name;
            }
            const fullnameheader = document.getElementById('header-user-info');
            if (fullnameheader) {
                fullnameheader.textContent = resUser.userInfor.name;
            }
        }
    });
});

// Hỗ trợ back/forward
window.onpopstate = function (event) {
    if (event.state && event.state.url) {
        $.get(event.state.url, function (data) {
            var $data = $('<div>').html(data);
            var body = $data.find('#main-content').length ? $data.find('#main-content').html() : data;
            $('#main-content').html(body);
        });
    }
};
function getJwtToken() {
    // Lấy dữ liệu token đã lưu (nếu có)
    const tokenData = JSON.parse(sessionStorage.getItem('jwtTokenData') || '{}');
    const now = Date.now();

    //Kiểm tra token còn hạn không
    if (tokenData.token && tokenData.expireAt && now < tokenData.expireAt) {
        // Trả về Promise resolve với token và userInfor đã lưu
        return Promise.resolve({
            token: tokenData.token,
            userInfor: tokenData.userInfor
        });
    }

    // Nếu chưa có hoặc hết hạn, gọi API để lấy token mới
    return $.ajax({
        url: '/api/user/GetJwtToken',
        method: 'GET'
    }).then(function (res) {
        // Ước lượng thời gian hết hạn (60 phút, trừ 1 phút dự phòng)
        const expireAt = now + 60 * 60 * 1000 - 60 * 1000;
        sessionStorage.setItem('jwtTokenData', JSON.stringify({
            token: res.token,
            userInfor: res.userInfor,
            expireAt: expireAt
        }));
        return {
            token: res.token,
            userInfor: res.userInfor
        };
    });
}

document.addEventListener('DOMContentLoaded', function () {
    var logoutForm = document.getElementById('logoutForm');
    if (logoutForm) {
        logoutForm.addEventListener('submit', function () {
            sessionStorage.clear();
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {

        // Kích hoạt tab Bootstrap khi click
        $('a[data-toggle="tab"]').on('click', function (e) {
            e.preventDefault();
            $(this).tab('show');
        });

        $(".vertical-nav-menu").metisMenu();
        $('.search-icon').click(function () {
            $(this).parent().parent().addClass('active');
        });

        $('.search-wrapper .close').click(function () {
            $(this).parent().removeClass('active');
        });
        $('[data-toggle="popover"]').popover();
        $('[data-toggle="tooltip"]').tooltip();
        $('[data-toggle="tooltip-light"]').tooltip({
            template: '<div class="tooltip tooltip-light"><div class="tooltip-inner"></div></div>'
        });
        $('[data-toggle="popover-custom"]').each(function () {
            var $container = $(this).parent().find('.rm-max-width');
            $(this).popover({
                html: true,
                container: $container.length ? $container.get(0) : false, // Use DOM element or false
                content: function () {
                    return $(this).next('.rm-max-width').find('.popover-custom-content').html();
                }
            });
        });
        $('.dropdown-menu').on('click', function (event) {
            event.stopPropagation();
        });
        $('.open-right-drawer').click(function () {
            $(this).addClass('is-active');
            $('.app-drawer-wrapper').addClass('drawer-open');
            $('.app-drawer-overlay').removeClass('d-none');
        });

        $('.drawer-nav-btn, .app-drawer-overlay').click(function () {
            $('.app-drawer-wrapper').removeClass('drawer-open');
            $('.app-drawer-overlay').addClass('d-none');
            $('.open-right-drawer').removeClass('is-active');
        });

        $('.mobile-toggle-nav').click(function () {
            $(this).toggleClass('is-active');
            $('.app-container').toggleClass('sidebar-mobile-open');
        });

        $('.mobile-toggle-header-nav').click(function () {
            $(this).toggleClass('active');
            $('.app-header__content').toggleClass('header-mobile-open');
        });

        $('.mobile-app-menu-btn').click(function () {
            $('.hamburger', this).toggleClass('is-active');
            $('.app-inner-layout').toggleClass('open-mobile-menu');
        });

        $(window).on('resize', function () {
            if ($(this).width() < 1250) {
                $('.app-container').addClass('closed-sidebar-mobile closed-sidebar');
            } else {
                $('.app-container').removeClass('closed-sidebar-mobile closed-sidebar');
            }
        }).trigger('resize'); // Chạy luôn khi load trang

        // Demo Theme Options
        $('.btn-open-options').click(function () {
            $('.ui-theme-settings').toggleClass('settings-open');
        });

        $('.close-sidebar-btn').click(function () {
            var classToSwitch = $(this).attr('data-class');
            $('.app-container').toggleClass(classToSwitch);
            $(this).toggleClass('is-active');
        });

        $('.switch-container-class').on('click', function () {
            var classToSwitch = $(this).attr('data-class');
            $('.app-container').toggleClass(classToSwitch);
            $(this).parent().find('.switch-container-class').removeClass('active');
            $(this).addClass('active');
        });

        $('.switch-theme-class').on('click', function () {
            var classToSwitch = $(this).attr('data-class');
            var containerElement = '.app-container';

            if (classToSwitch === 'app-theme-white') {
                $(containerElement).removeClass('app-theme-gray').addClass(classToSwitch);
            } else if (classToSwitch === 'app-theme-gray') {
                $(containerElement).removeClass('app-theme-white').addClass(classToSwitch);
            } else if (classToSwitch === 'body-tabs-line') {
                $(containerElement).removeClass('body-tabs-shadow').addClass(classToSwitch);
            } else if (classToSwitch === 'body-tabs-shadow') {
                $(containerElement).removeClass('body-tabs-line').addClass(classToSwitch);
            }

            $(this).parent().find('.switch-theme-class').removeClass('active');
            $(this).addClass('active');
        });

        $('.switch-header-cs-class').on('click', function () {
            var classToSwitch = $(this).attr('data-class');
            $('.app-header').attr('class', 'app-header header-shadow ' + classToSwitch);
            $('.switch-header-cs-class').removeClass('active');
            $(this).addClass('active');
        });

        $('.switch-sidebar-cs-class').on('click', function () {
            var classToSwitch = $(this).attr('data-class');
            $('.app-sidebar').attr('class', 'app-sidebar sidebar-shadow ' + classToSwitch);
            $('.switch-sidebar-cs-class').removeClass('active');
            $(this).addClass('active');
        });

    }, 500); // Đợi Blazor tải xong
});

function isValidEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}
function isValidPhoneNumber(phone) {
    const regex = /^(0|\+84)(3[2-9]|5[2689]|7[06-9]|8[1-9]|9[0-9])[0-9]{7}$/;
    return regex.test(phone);
}

