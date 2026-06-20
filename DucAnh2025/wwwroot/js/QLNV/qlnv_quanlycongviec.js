var allNhomNhanVien = [];
var nhomSelected;
var congViecDataSet = [];
var selectNhanVien = "";

$(function () {
    LoadNhomNhanVienByTaiKhoan();

    var isCalendar = false;
    $('#toggle-view-btn').on('click', function () {
        isCalendar = !isCalendar;
        if (isCalendar) {
            $('#tab-table').hide();
            $('#tab-calender').show();
            $('#toggle-view-btn').html('<i class="header-icon lnr-laptop-phone icon-gradient bg-plum-plate"></i> Bảng');
            renderCalendar(congViecDataSet);
        } else {
            $('#tab-calender').hide();
            $('#tab-table').show();
            $('#toggle-view-btn').html('<i class="header-icon pe-7s-id icon-gradient bg-plum-plate"></i> Lịch');
        }
    });
});


function onNhomClick(nhom) {
    $('.nav.flex-column .nav-item.cv-active').removeClass('cv-active');
    $(`.nav.flex-column .nav-item[data-nhom-id="${nhom.id}"]`).addClass('cv-active');
    nhomSelected = nhom;
    LoadNhanVienByNhom(nhom.groupId, nhom.companyId, nhom.id);
    loadCongViec(nhom.id, selectNhanVien);
}

function LoadNhomNhanVienByTaiKhoan() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: `/api/NhomNhanVien/GetNhomNhanVienByTaiKhoanAsync?groupId=${resUser.userInfor.groupId}&companyId=${resUser.userInfor.companyId}&taiKhoan=${resUser.userInfor.userName}`,
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (res) {
            if (res.success && res.data && res.data.length) {
                allNhomNhanVien = res.data;
                let liHtml = '';
                res.data.forEach((nhom, idx) => {
                    if (idx === 0) nhomSelected = nhom;
                    liHtml += `
                            <li class="nav-item${idx === 0 ? ' cv-active' : ''}" data-nhom-id="${nhom.id}">
                                <a href="javascript:void(0);" class="nav-link" onclick='onNhomClick(${JSON.stringify(nhom)})'>
                                    <i class="nav-link-icon pe-7s-chat"></i>
                                    <span>${nhom.tenNhom}</span>
                                    <div class="ml-auto badge badge-pill badge-info">${nhom.total || 0}</div>
                                </a>
                            </li>
                        `;
                });
                const $dsCvHeader = $('.nav-item-header:contains("Danh sách công việc")');
                if ($dsCvHeader.length) {
                    $dsCvHeader.nextAll('.nav-item.cv-active, .nav-item:not(.nav-item-header):not(.nav-item-divider)').remove();
                    $(liHtml).insertAfter($dsCvHeader);
                }
                // Load nhân viên nhóm đầu tiên mặc định
                if (nhomSelected) {
                    LoadNhanVienByNhom(nhomSelected.groupId, nhomSelected.companyId, nhomSelected.id);
                    loadCongViec(nhomSelected.id, selectNhanVien);
                }
            } else {
                nhomSelected = null;
                allNhomNhanVien = [];
                $('#nhanvientrongnhom').empty();
            }
        }).fail(function (xhr) {
            nhomSelected = null;
            allNhomNhanVien = [];
            showToastError(xhr.responseJSON?.message || "Lỗi khi tải nhóm nhân viên.");
            $('#nhanvientrongnhom').empty();
        });
    });
}

function LoadNhanVienByNhom(groupId, companyId, idNhomNhanVien) {
    getJwtToken().then(function (resUser) {
        if (!groupId || !companyId || !idNhomNhanVien) {
            $('#nhanvientrongnhom').empty();
            return;
        }
        $.ajax({
            url: `/api/NhanVien/GetNhanVienByNhom?groupId=${groupId}&companyId=${companyId}&Id_NhomNhanVien=${idNhomNhanVien}`,
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (res) {
            let html = '';
            if (res.success && Array.isArray(res.data)) {
                res.data.forEach(function (nv) {
                    // Xác định nếu nhân viên này là người thực hiện đang được chọn thì thêm class 'cv-active'
                    const isActive = selectNhanVien === `${nv.tenNhanVien} (${nv.taiKhoan})`;
                    html += `
                        <div class="card flex-shrink-0 shadow-sm border-0 p-2 nhan-vien-item${isActive ? ' cv-active' : ''}" 
                             style="min-width: 250px; cursor:pointer;" 
                             data-ten-nhan-vien="${nv.tenNhanVien || ''}" 
                             data-tai-khoan="${nv.taiKhoan || ''}">
                            <div class="row align-items-center no-gutters">
                                <div class="col-4 text-center">
                                    <div class="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center" style="width: 50px; height: 50px;">
                                        <i class="fas fa-user fa-2x"></i>
                                    </div>
                                </div>
                                <div class="col-8">
                                    <h6 class="mb-1 text-primary font-weight-bold">${nv.tenNhanVien || ''}</h6>
                                    <p class="mb-0 text-muted small">${nv.taiKhoan || ''}</p>
                                </div>
                            </div>
                        </div>
                    `;
                });
            }
            $('#nhanvientrongnhom').html(html);
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || "Lỗi khi tải nhân viên.");
            $('#nhanvientrongnhom').empty();
        });
    });
}

$(document).off('click', '#nhanvientrongnhom .nhan-vien-item').on('click', '#nhanvientrongnhom .nhan-vien-item', function () {
    const tenNhanVien = $(this).data('ten-nhan-vien');
    const taiKhoan = $(this).data('tai-khoan');
    chonNhanVien(tenNhanVien, taiKhoan);

    // Hiệu ứng chọn
    $('#nhanvientrongnhom .nhan-vien-item').removeClass('cv-active');
    if (selectNhanVien) {
        $(this).addClass('cv-active');
    }
});

function chonNhanVien(tenNhanVien, taiKhoan) {
    var nguoiThucHien = `${tenNhanVien} (${taiKhoan})`;
    if (selectNhanVien === nguoiThucHien) {
        selectNhanVien = "";
    } else {
        selectNhanVien = nguoiThucHien;
    }
    loadCongViec(nhomSelected.id, selectNhanVien);
}
function loadCongViec(nhomCongViec, nguoiThucHien) {
    getJwtToken().then(function (resUser) {
        // Lấy công việc con trước, sau đó mới load công việc cha
        $.ajax({
            url: "/api/CongViec/GetAllCVC",
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (resCVC) {
            var allCVC = Array.isArray(resCVC.data) ? resCVC.data : [];

            // Sau khi có công việc con, load công việc cha
            $.ajax({
                url: "/api/CongViec/GetByVM?groupId=" + resUser.userInfor.groupId,
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + resUser.token
                },
                data: JSON.stringify({
                    Id: "",
                    Id_Task: "",
                    Id_NguoiGiaoViec: resUser.userInfor.userName,
                    NguoiThucHien: nguoiThucHien,
                    NhomCongViec: nhomCongViec,
                    TenNhom: "",
                    NgayBatDau: null,
                    NgayKetThuc: null,
                    MucDoUuTien: "",
                    TuDanhGia: "",
                    TienDo: 0,
                    LapLai: "",
                    TenCongViec: "",
                    NoiDungCongViec: "",
                    FileDinhKem: "",
                    GroupId: resUser.userInfor.groupId,
                    CreateAt: "2023-10-10T00:00:00",
                    CreateBy: "",
                    IsActive: 1
                })
            }).done(function (res) {
                var dataSet = [];
                if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                    dataSet = res.data.map(function (cv, idx) {
                        var cvcList = allCVC.filter(cvc => cvc.id_CongViec === cv.id);
                        return {
                            id: cv.id,
                            stt: idx + 1,
                            noiDungCongViec: cv.noiDungCongViec || '',
                            tenCongViec: cv.tenCongViec || '',
                            nhomCongViec: cv.nhomCongViec || '',
                            tenNhom: cv.tenNhom || '',
                            nguoiThucHien: cv.nguoiThucHien || '',
                            ngayBatDau: cv.ngayBatDau ? moment(cv.ngayBatDau).format('DD-MM-YYYY') : '',
                            ngayKetThuc: cv.ngayKetThuc ? moment(cv.ngayKetThuc).format('DD-MM-YYYY') : '',
                            mucDoUuTien: cv.mucDoUuTien || '',
                            lapLai: cv.lapLai || '',
                            tuDanhGia: cv.tuDanhGia || '',
                            fileDinhKem: cv.fileDinhKem || '',
                            cvcList: cvcList
                        };
                    });
                }

                congViecDataSet = dataSet;

                // Destroy DataTable nếu đã tồn tại
                var $table = $('#table_congviec');
                if ($.fn.DataTable.isDataTable($table)) {
                    $table.DataTable().clear().destroy();
                }

                // Khởi tạo DataTable mới
                var table = $table.DataTable({
                    data: dataSet,
                    columns: [
                        {
                            className: 'dt-control text-center align-middle col-stt',
                            orderable: false,
                            data: null,
                            defaultContent: '<button class="btn btn-sm btn-info btn-expand-cvc" title="Xem công việc con"><i class="fas fa-chevron-right"></i></button>',
                        },
                        { data: 'stt', className: 'text-center align-middle'},
                        {
                            data: null,
                            className: 'align-middle',
                            width: "30%",
                            render: function (data, type, row) {
                                 return `<span style="text-transform:uppercase;">${row.tenCongViec || ''}</span><br/><span>${row.noiDungCongViec || ''}</span>`;
                            }
                        },
                        { data: 'nguoiThucHien', className: 'align-middle', width: "20%" },
                        { data: 'ngayBatDau', className: 'align-middle' },
                        { data: 'ngayKetThuc', className: 'align-middle' },
                        { data: 'mucDoUuTien', className: 'align-middle' },
                        { data: 'tuDanhGia', className: 'align-middle', width: "100px", },
                        {
                            data: null,
                            className: 'text-center align-middle',
                            orderable: false,
                            width: "120px" ,
                            render: function (data, type, row) {
                                return `
                                    <div class="btn-group">
                                        <button class="btn btn-sm btn-primary btn-edit" title="Chỉnh sửa" data-id="${row.id}" onclick="editCongViec(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(row))}')))"><i class="fas fa-edit"></i></button>
                                        <button class="btn btn-sm btn-danger btn-delete" title="Xóa" data-id="${row.id}" onclick=deleteCongViec("${row.id}")><i class="fas fa-trash"></i></button>
                                        <button class="btn btn-sm btn-success btn-add-child" title="Thêm công việc con" data-id="${row.id}" onclick= addChildCongViec(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(row))}')))><i class="fas fa-plus"></i></button>
                                        <button class="btn btn-sm btn-warning" title="Bình luận / timeline" data-id="${row.id}" onclick="qlnvCollab.openModal(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(row))}')))"><i class="fas fa-comments"></i></button>
                                        ${row.fileDinhKem && row.fileDinhKem !== "" ? `<a href="${row.fileDinhKem}" class="btn btn-sm btn-info" title="Tải file" download><i class="fas fa-download"></i></a>` : ""}
                                    </div>
                                `;
                            }
                        }
                    ],
                    autoWidth: false,
                    ordering: true,
                    searching: true,
                    paging: true,
                    pageLength: 10,
                    info: true,
                    language: {
                        lengthMenu: "Hiển thị _MENU_ bản ghi mỗi trang",
                        zeroRecords: "Không tìm thấy dữ liệu",
                        info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                        infoEmpty: "Không có bản ghi nào",
                        infoFiltered: "(lọc từ _MAX_ bản ghi)",
                        search: "Tìm kiếm:",
                        paginate: {
                            first: "Đầu",
                            last: "Cuối",
                            next: "Sau",
                            previous: "Trước"
                        }
                    },
                    createdRow: function (row, data) {
                        if (data.mucDoUuTien === 'Khẩn cấp' || data.mucDoUuTien === 'Cao') {
                            $('td', row).eq(6).addClass('bg-danger text-white');
                            $('td', row).eq(5).addClass('bg-danger text-white');
                        } else if (data.mucDoUuTien === 'Trung bình') {
                            $('td', row).eq(6).addClass('bg-primary text-white');
                            $('td', row).eq(5).addClass('bg-primary text-white');
                        } else if (data.mucDoUuTien === 'Thấp') {
                            $('td', row).eq(6).addClass('bg-success text-white');
                            $('td', row).eq(5).addClass('bg-success text-white');
                        }

                    }
                });

                // Sự kiện mở rộng công việc con
                $table.find('tbody').off('click', 'button.btn-expand-cvc').on('click', 'button.btn-expand-cvc', function () {
                    var tr = $(this).closest('tr');
                    var row = table.row(tr);
                    var $icon = $(this).find('i');

                    if (row.child.isShown()) {
                        row.child.hide();
                        $icon.removeClass('fa-chevron-down').addClass('fa-chevron-right');
                    } else {
                        var cvcList = row.data().cvcList;
                        if (cvcList && cvcList.length > 0) {
                            var childHtml = '<table class="table table-sm table-bordered mb-0" style="border: none;"><thead><tr>' +
                                '<th class="col-stt">STT</th><th>Nội dung công việc con</th><th class="text-center">Trạng thái</th><th></th></tr></thead><tbody>';
                            cvcList.forEach(function (cvc, idx) {
                                childHtml += `<tr class="bg-light">
                                        <td class="align-middle text-center">${idx + 1}</td>
                                        <td class="align-middle">${cvc.noiDungCongViec || ''}</td>
                                        <td class="align-middle" style="width:100px">${cvc.hoanThanh ==1 ?"Hoàn thành": ""}</td>
                                        <td class="text-center align-middle" style="width:120px">
                                            <div class="btn-group">
                                                <button class="btn btn-sm btn-primary" onclick= editCongViecCon(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(cvc))}'))) title="Chỉnh sửa"><i class="fas fa-edit"></i></button>
                                                <button class="btn btn-sm btn-danger" onclick=deleteCongViecCon("${cvc.id}") title="Xóa"><i class="fas fa-trash"></i></button>
                                                ${cvc.fileDinhKem && cvc.fileDinhKem !== "" ? `<a href="${cvc.fileDinhKem}" class="btn btn-sm btn-info" title="Tải file" download><i class="fas fa-download"></i></a>` : ""}
                                            </div>
                                        </td>
                                    </tr>`;
                            });
                            childHtml += '</tbody></table>';
                            row.child(childHtml, 'p-0').show();
                            $icon.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                        } else {
                            row.child('<div class="text-muted px-3 py-2">Không có công việc con.</div>').show();
                            $icon.removeClass('fa-chevron-right').addClass('fa-chevron-down');
                        }
                    }
                });

                // Nếu đang ở tab lịch thì render lại lịch
                if ($('#tab-calender').is(':visible')) {
                    renderCalendar(congViecDataSet);
                }

               
            }).fail(function (xhr) {
                showToastError(xhr.responseJSON?.message || "Lỗi khi load công việc.");
                $('#table_congviec tbody').html('<tr><td colspan="9" class="text-center text-danger">Lỗi khi tải dữ liệu.</td></tr>');
            });
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || "Lỗi khi load công việc con.");
        });
    });
}

// Hàm render lịch bằng FullCalendar
function renderCalendar(dataSet) {
    var calendarEl = document.getElementById('calendar-congviec');
    if (!calendarEl) return;

    // Xóa calendar cũ nếu có
    if (calendarEl._fullCalendar) {
        calendarEl._fullCalendar.destroy();
    }

    $(calendarEl).css({
        width: '100%',
        height: '100%',
        minHeight: '400px'
    });

    // Chuyển đổi dữ liệu sang dạng event cho FullCalendar
    var events = (dataSet || []).map(function (cv) {
        // Chuyển ngày về dạng YYYY-MM-DD
        let start = cv.ngayBatDau ? moment(cv.ngayBatDau, 'DD-MM-YYYY').format('YYYY-MM-DD') : null;
        let end = cv.ngayKetThuc ? moment(cv.ngayKetThuc, 'DD-MM-YYYY').format('YYYY-MM-DD') : null;

        // Nếu ngày kết thúc trùng ngày bắt đầu hoặc nhỏ hơn, cộng thêm 1 ngày cho end
        if (start && end && (start === end || moment(end).isSameOrBefore(start))) {
            end = moment(start).add(1, 'days').format('YYYY-MM-DD');
        } else if (end) {
            // FullCalendar không bao gồm ngày end, nên cần cộng thêm 1 ngày để bao trọn ngày kết thúc
            end = moment(end).add(1, 'days').format('YYYY-MM-DD');
        }

        return {
            id: cv.id,
            title: cv.TenCongViec,
            start: start,
            end: end,
            description: cv.nguoiThucHien,
            backgroundColor: getEventColor(cv.mucDoUuTien)
        };
    });

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'vi',
        height: '100%',
        contentHeight: 'auto',
        aspectRatio: 1.7, // Có thể điều chỉnh cho phù hợp
        expandRows: true, // Đảm bảo các hàng luôn vừa container
        events: events,
        eventClick: function (info) {
            var cv = (congViecDataSet || []).find(x => x.id === info.event.id);
            if (cv) {
                editCongViec(cv);
            }
        },
        eventDidMount: function (info) {
            if (info.event.extendedProps.description) {
                $(info.el).tooltip({
                    title: info.event.extendedProps.description,
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body'
                });
            }
        }
    });
    calendar.render();
    calendarEl._fullCalendar = calendar;
}
function getEventColor(priority) {
    switch (priority) {
        case 'Khẩn cấp':
        case 'Cao':
            return '#dc3545'; // đỏ
        case 'Trung bình':
            return '#007bff'; // xanh dương
        case 'Thấp':
            return '#28a745'; // xanh lá
        default:
            return '#6c757d'; // xám
    }
}
function showModalCongViec() {
    getJwtToken().then(function (resUser) {
        var today = new Date();
        var dd = String(today.getDate()).padStart(2, '0');
        var mm = String(today.getMonth() + 1).padStart(2, '0'); // Months are zero-based
        var yyyy = today.getFullYear();
        var todayStr = yyyy + '-' + mm + '-' + dd;

        var endDate = new Date(today);
        endDate.setDate(endDate.getDate() + 7);
        var dd2 = String(endDate.getDate()).padStart(2, '0');
        var mm2 = String(endDate.getMonth() + 1).padStart(2, '0');
        var yyyy2 = endDate.getFullYear();
        var endDateStr = yyyy2 + '-' + mm2 + '-' + dd2;


        var nhomOptions = allNhomNhanVien.map(nhom =>
            `<option value="${nhom.id}">${nhom.tenNhom}</option>`
        ).join('');

        var bodyHtml = `
                <form id="formAddCongViec" novalidate>
                    <div class="row">
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="Id_NguoiGiaoViec">Người giao việc <span class="text-danger">*</span></label>
                             <input id="Id_NguoiGiaoViec" name="Input.Id_NguoiGiaoViec" class="form-control" readonly value="${resUser.userInfor.userName}" />
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="nhomCongViec">Tên nhóm <span class="text-danger">*</span></label>
                            <select id="nhomCongViec" class="form-control" required>
                                <option value="">-- Chọn nhóm --</option>
                                ${nhomOptions}
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn nhóm.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="nhanVienThucHien">Nhân viên thực hiện <span class="text-danger">*</span></label>
                            <select id="nhanVienThucHien" class="form-control multiselect-dropdown" multiple="multiple" style="width:100%" required>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn nhân viên thực hiện.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="addNgayBatDau">Ngày bắt đầu</label>
                            <input type="date" class="form-control" id="addNgayBatDau" value="${todayStr}">
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="addNgayKetThuc">Ngày kết thúc</label>
                            <input type="date" class="form-control" id="addNgayKetThuc" value ="${endDateStr}">
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="MucDoUuTien">Mức độ ưu tiên <span class="text-danger">*</span></label>
                            <select id="MucDoUuTien" class="form-control select2-single" required>
                                <option value="">Chọn</option>
                                <option value="Thấp">Thấp</option>
                                <option value="Trung bình">Trung bình</option>
                                <option value="Cao">Cao</option>
                                <option value="Khẩn cấp">Khẩn cấp</option>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn mức độ ưu tiên.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="LapLai">Lặp lại <span class="text-danger">*</span></label>
                            <select id="LapLai" class="form-control select2-single" required>
                                <option value="">Chọn</option>
                                <option value="Hàng ngày">Hàng ngày</option>
                                <option value="Hàng tuần">Hàng tuần</option>
                                <option value="Hàng tháng">Hàng tháng</option>
                                <option value="Không lặp lại">Không lặp lại</option>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn kiểu lặp lại.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="FileDinhKem">File đính kèm</label>
                            <input id="FileDinhKem" class="form-control" style="border:none" type="file">
                        </div>
                        <div class="col-sm-6 col-md-6 mb-3">
                            <label for="addTenCongViec">Tên công việc <span class="text-danger">*</span></label>
                            <input class="form-control" id="addTenCongViec" name="addTenCongViec" placeholder="Tên công việc" required />
                            <div class="invalid-feedback">Vui lòng nhập tên công việc.</div>
                        </div>
                        <div class="col-sm-6 col-md-6 mb-3">
                            <label for="addNoiDungCongViec">Chi tiết công việc <span class="text-danger">*</span></label>
                            <textarea class="form-control" id="addNoiDungCongViec" name="addNoiDungCongViec" placeholder="Nội dung công việc" required></textarea>
                            <div class="invalid-feedback">Vui lòng nhập nội dung công việc.</div>
                        </div>
                    
                        <div id="themNgayContainer" class="form-floating col-sm-12 col-md-6 mb-3" style="display:none"></div>

                    </div>
                </form>
            `;
        var footerHtml = `
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
                <button type="button" class="btn btn-primary" id="btnSaveAddCongViec">Lưu</button>
            `;
        showGlobalModal('Thêm công việc', bodyHtml, footerHtml);

        setTimeout(function () {
            $('#nhanVienThucHien').select2({
                theme: "bootstrap4",
                placeholder: "Chọn nhân viên thực hiện",
                width: '100%'
            });
        }, 200);

        // Load công việc cho select2 single và input số ngày ngay khi show modal
        $.ajax({
            url: `/api/CongViec/GetByVM?groupId=${resUser.userInfor.groupId}`,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify({
                Id: "",
                Id_Task: "",
                Id_NguoiGiaoViec: resUser.userInfor.userName,
                NguoiThucHien: "",
                NhomCongViec: "",
                TenNhom: "",
                NgayBatDau: null,
                NgayKetThuc: null,
                MucDoUuTien: "",
                TuDanhGia: "",
                TienDo: 0,
                LapLai: "",
                TenCongViec: "",
                NoiDungCongViec: "",
                FileDinhKem: "",
                GroupId: resUser.userInfor.groupId,
                CreateAt: new Date().toISOString(),
                CreateBy: "",
                IsActive: 1
            })
        }).done(function (res) {
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                var options = res.data.map(cv =>
                    `<option value="${cv.id}">${cv.noiDungCongViec}</option>`
                ).join('');
                var html = `
                        <div class="row">
                            <div class=" col-sm-8 mb-3">
                                <label for="id_CongViec_themngay">Công việc cần thêm ngày</label>
                                <select id="id_CongViec_themngay" class="form-control" style="width:100%">
                                    <option value=""> -- Chọn công việc --</option>
                                    ${options}
                                </select>
                            </div>
                            <div class=" col-sm-4 mb-3">
                                <label for="soNgay">Số ngày thêm</label>
                                <input type="number" min="1" class="form-control" id="soNgay" placeholder="Nhập số ngày">
                            </div>
                        </div>
                    `;
                $('#themNgayContainer').html(html).show();
                setTimeout(function () {
                    $('#id_CongViec_themngay').select2({
                        theme: "bootstrap4",
                        placeholder: "Chọn công việc cần thêm ngày",
                        width: '100%'
                    });
                }, 100);
            }
        });


        // Khi thay đổi nhóm, load nhân viên thực hiện
        $(document).off('change', '#nhomCongViec').on('change', '#nhomCongViec', function () {
            var nhomId = $(this).val();
            $('#nhanVienThucHien').empty().trigger('change');
            if (!nhomId) return;
            var nhom = allNhomNhanVien.find(x => x.id === nhomId);
            if (!nhom) return;
            LoadNhanVienByIdNhom(nhom.groupId, nhom.companyId, nhomId, "nhanVienThucHien");
        });

        $('#FileDinhKem').off('change').on('change', function () {
            var fileInput = this;
            if (fileInput.files && fileInput.files.length > 0) {
                uploadFile(fileInput, function (fileUrl) {
                    $('#FileDinhKem').data('uploaded-url', fileUrl);
                    showToastSuccess('Tải file thành công!');
                });
            } else {
                $('#FileDinhKem').removeData('uploaded-url');
            }
        });

        $(document).off('click', '#btnSaveAddCongViec').on('click', '#btnSaveAddCongViec', function () {
            var noiDung = $('#addNoiDungCongViec').val().trim();
            var tenCongViec = $('#addTenCongViec').val().trim();
            var ngayBatDau = $('#addNgayBatDau').val();
            var ngayKetThuc = $('#addNgayKetThuc').val();
            var idNguoiGiaoViec = $('#Id_NguoiGiaoViec').val();
            var nhomCongViec = $('#nhomCongViec').val();
            var nhanVienThucHien = $('#nhanVienThucHien').val() || [];
            var mucDoUuTien = $('#MucDoUuTien').val();
            var lapLai = $('#LapLai').val();
            var fileDinhKem = $('#FileDinhKem').data('uploaded-url') || "";

            var id_CongViec_themngay = $('#id_CongViec_themngay').val() || "";
            var soNgay = parseInt($('#soNgay').val() || "0");


            var isValid = true;
            if (!nhomCongViec) {
                $('#nhomCongViec').addClass('is-invalid');
                isValid = false;
            } else {
                $('#nhomCongViec').removeClass('is-invalid');
            }
            if (!noiDung) {
                $('#addNoiDungCongViec').addClass('is-invalid');
                isValid = false;
            } else {
                $('#addNoiDungCongViec').removeClass('is-invalid');
            }
            if (!tenCongViec) {
                $('#addTenCongViec').addClass('is-invalid');
                isValid = false;
            } else {
                $('#addTenCongViec').removeClass('is-invalid');
            }
            if (!nhanVienThucHien.length) {
                $('#nhanVienThucHien').addClass('is-invalid');
                isValid = false;
            } else {
                $('#nhanVienThucHien').removeClass('is-invalid');
            }
            if (!mucDoUuTien) {
                $('#MucDoUuTien').addClass('is-invalid');
                isValid = false;
            } else {
                $('#MucDoUuTien').removeClass('is-invalid');
            }
            if (!lapLai) {
                $('#LapLai').addClass('is-invalid');
                isValid = false;
            } else {
                $('#LapLai').removeClass('is-invalid');
            }
            if ($('#themNgayContainer').is(':visible')) {
                if (id_CongViec_themngay) {
                    if (!soNgay || soNgay < 1) {
                        $('#soNgay').addClass('is-invalid');
                        isValid = false;
                    } else {
                        $('#soNgay').removeClass('is-invalid');
                    }
                } else {
                    $('#soNgay').removeClass('is-invalid');
                    $('#id_CongViec_themngay').removeClass('is-invalid');
                }
                var ngayBatDau = $('#addNgayBatDau').val();
                var ngayKetThuc = $('#addNgayKetThuc').val();

                // Validate ngày bắt đầu và ngày kết thúc
                if (!ngayBatDau) {
                    $('#addNgayBatDau').addClass('is-invalid');
                    isValid = false;
                } else {
                    $('#addNgayBatDau').removeClass('is-invalid');
                }
                if (!ngayKetThuc) {
                    $('#addNgayKetThuc').addClass('is-invalid');
                    isValid = false;
                } else {
                    $('#addNgayKetThuc').removeClass('is-invalid');
                }
                if (ngayBatDau && ngayKetThuc && new Date(ngayBatDau) >= new Date(ngayKetThuc)) {
                    $('#addNgayBatDau').addClass('is-invalid');
                    $('#addNgayKetThuc').addClass('is-invalid');
                    showToastError('Ngày bắt đầu phải nhỏ hơn ngày kết thúc!');
                    isValid = false;
                } else {
                    if (ngayBatDau) $('#addNgayBatDau').removeClass('is-invalid');
                    if (ngayKetThuc) $('#addNgayKetThuc').removeClass('is-invalid');
                }
            }
            if (!isValid) return;

            var themNgayData = {};
            if ($('#themNgayContainer').is(':visible')) {
                themNgayData = {
                    id_CongViecThemNgay: id_CongViec_themngay,
                    soNgay: soNgay
                };
            }

            var congViecData = {
                tenCongViec: tenCongViec,
                noiDungCongViec: noiDung,
                ngayBatDau: ngayBatDau,
                ngayKetThuc: ngayKetThuc,
                id_NguoiGiaoViec: idNguoiGiaoViec,
                nhomCongViec: nhomCongViec,
                mucDoUuTien: mucDoUuTien,
                lapLai: lapLai,
                fileDinhKem: fileDinhKem
            };
            createCongViec(congViecData, nhanVienThucHien, themNgayData);

            $('#globalModal').modal('hide');
        });
    });
}

// Hàm load nhân viên theo nhóm và render vào select2 multiselect
function LoadNhanVienByIdNhom(groupId, companyId, idNhomNhanVien, idHtml) {
    getJwtToken().then(function (resUser) {
        if (!groupId || !companyId || !idNhomNhanVien || !idHtml) {
            $('#' + idHtml).empty().trigger('change');
            return;
        }
        $.ajax({
            url: `/api/NhanVien/GetNhanVienByNhom?groupId=${groupId}&companyId=${companyId}&Id_NhomNhanVien=${idNhomNhanVien}`,
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (res) {
            let html = '';
            if (res.success && Array.isArray(res.data)) {
                res.data.forEach(function (nv) {
                    html += `<option value="${nv.id}">${nv.tenNhanVien || ''} (${nv.taiKhoan || ''})</option>`;
                });
            }
            $('#' + idHtml).html(html).trigger('change');
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || "Lỗi khi tải nhân viên.");
            $('#' + idHtml).empty().trigger('change');
        });
    });
}

function createCongViec(congViecData, nhanVienThucHien, themNgayData) {
    getJwtToken().then(function (resUser) {
        const data = {
            congViec: {
                id: "",
                id_NguoiGiaoViec: resUser.userInfor.userName,
                nguoiThucHien: "",
                nhomCongViec: congViecData.nhomCongViec,
                tenNhom: "",
                ngayBatDau: congViecData.ngayBatDau,
                ngayKetThuc: congViecData.ngayKetThuc,
                mucDoUuTien: congViecData.mucDoUuTien || "Thấp",
                tuDanhGia: "",
                tienDo: 0,
                lapLai: congViecData.lapLai || "Không lặp lại",
                tenCongViec: congViecData.tenCongViec,
                noiDungCongViec: congViecData.noiDungCongViec,
                fileDinhKem: congViecData.fileDinhKem || "",
                groupId: resUser.userInfor.groupId,
                companyId: resUser.userInfor.companyId,
                createAt: new Date().toISOString(),
                createBy: resUser.userInfor.userName,
                isActive: 1,
                page_number: 1,
                pageSize: 10
            },
            nhanVienThucHien: nhanVienThucHien,
            themNgay: {
                id: "",
                id_CongViec: "",
                id_CongViecThemNgay: themNgayData?.id_CongViec_themngay || "",
                soNgay: themNgayData?.soNgay || 0,
                groupId: resUser.userInfor.groupId,
                createAt: new Date().toISOString(),
                createBy: resUser.userInfor.userName,
                isActive: 1
            }
        };

        $.ajax({
            url: "/api/CongViec/CreateCongViec",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function (response) {
            if (response.success) {
                showToastSuccess("Tạo công việc thành công!");
                LoadNhomNhanVienByTaiKhoan();
            } else {
                showToastError(response.message);
            }
        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi khi tạo công việc.";
            showToastError(message);
        });
    });
}

function editCongViec(row) {
    getJwtToken().then(function (resUser) {
        // Lấy dữ liệu công việc hiện tại
        var nhomOptions = allNhomNhanVien.map(nhom =>
            `<option value="${nhom.id}" ${nhom.id == row.nhomCongViec ? 'selected' : ''}>${nhom.tenNhom}</option>`
        ).join('');

        var todayStr = row.ngayBatDau ? moment(row.ngayBatDau, 'DD-MM-YYYY').format('YYYY-MM-DD') : '';
        var endDateStr = row.ngayKetThuc ? moment(row.ngayKetThuc, 'DD-MM-YYYY').format('YYYY-MM-DD') : '';

        var fileLinkHtml = row.fileDinhKem
            ? `<div class="mb-2"><a href="${row.fileDinhKem}" target="_blank"><i class="fas fa-paperclip"></i> File hiện tại</a></div>`
            : '';

        var bodyHtml = `
                <form id="formEditCongViec" novalidate>
                    <div class="row">
                        <input type="hidden" id="editIdCongViec" value="${row.id}">
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="Id_NguoiGiaoViec">Người giao việc <span class="text-danger">*</span></label>
                             <input id="Id_NguoiGiaoViec" name="Input.Id_NguoiGiaoViec" class="form-control" readonly value="${resUser.userInfor.userName}" />
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editNhomCongViec">Tên nhóm <span class="text-danger">*</span></label>
                            <select id="editNhomCongViec" class="form-control" required>
                                <option value="">-- Chọn nhóm --</option>
                                ${nhomOptions}
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn nhóm.</div>
                        </div>
                         <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editNhanVienThucHien">Nhân viên thực hiện <span class="text-danger">*</span></label>
                             <select id="editNhanVienThucHien" class="form-control multiselect-dropdown" multiple="multiple" style="width:100%" required>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn nhân viên thực hiện.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editNgayBatDau">Ngày bắt đầu</label>
                            <input type="date" class="form-control" id="editNgayBatDau" value="${todayStr}">
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editNgayKetThuc">Ngày kết thúc</label>
                            <input type="date" class="form-control" id="editNgayKetThuc" value="${endDateStr}">
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editMucDoUuTien">Mức độ ưu tiên <span class="text-danger">*</span></label>
                            <select id="editMucDoUuTien" class="form-control select2-single" required>
                                <option value="">Chọn</option>
                                <option value="Thấp" ${row.mucDoUuTien === 'Thấp' ? 'selected' : ''}>Thấp</option>
                                <option value="Trung bình" ${row.mucDoUuTien === 'Trung bình' ? 'selected' : ''}>Trung bình</option>
                                <option value="Cao" ${row.mucDoUuTien === 'Cao' ? 'selected' : ''}>Cao</option>
                                <option value="Khẩn cấp" ${row.mucDoUuTien === 'Khẩn cấp' ? 'selected' : ''}>Khẩn cấp</option>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn mức độ ưu tiên.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editLapLai">Lặp lại <span class="text-danger">*</span></label>
                            <select id="editLapLai" class="form-control select2-single" required>
                                <option value="">Chọn</option>
                                <option value="Hàng ngày" ${row.lapLai === 'Hàng ngày' ? 'selected' : ''}>Hàng ngày</option>
                                <option value="Hàng tuần" ${row.lapLai === 'Hàng tuần' ? 'selected' : ''}>Hàng tuần</option>
                                <option value="Hàng tháng" ${row.lapLai === 'Hàng tháng' ? 'selected' : ''}>Hàng tháng</option>
                                <option value="Không lặp lại" ${row.lapLai === 'Không lặp lại' ? 'selected' : ''}>Không lặp lại</option>
                            </select>
                            <div class="invalid-feedback">Vui lòng chọn kiểu lặp lại.</div>
                        </div>
                        <div class="form-floating col-sm-6 col-md-3 mb-3">
                            <label for="editFileDinhKem">File đính kèm</label>
                            ${fileLinkHtml}
                            <input id="editFileDinhKem" class="form-control" style="border:none" type="file">
                        </div>
                         <div class=" col-sm-6 col-md-6 mb-3">
                            <label for="editTenCongViec">Tên công việc <span class="text-danger">*</span></label>
                            <input class="form-control" id="editTenCongViec" name="editTenCongViec" value="${row.tenCongViec || ''}" placeholder="Tên công việc" required />
                            <div class="invalid-feedback">Vui lòng nhập tên công việc.</div>
                        </div>
                        <div class="col-sm-6 col-md-6 mb-3">
                            <label for="editNoiDungCongViec">Nội dung công việc <span class="text-danger">*</span></label>
                            <textarea class="form-control" id="editNoiDungCongViec" required>${row.noiDungCongViec || ''}</textarea>
                            <div class="invalid-feedback">Vui lòng nhập nội dung công việc.</div>
                        </div>
                    </div>
                </form>
            `;
        var footerHtml = `
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
                <button type="button" class="btn btn-primary" id="btnUpdateCongViec">Cập nhật</button>
            `;
        showGlobalModal('Chỉnh sửa công việc', bodyHtml, footerHtml);

        // Khởi tạo select2 nếu cần
        setTimeout(function () {
            $('#editNhomCongViec').select2({ theme: "bootstrap4", width: '100%' });
            $('#editMucDoUuTien').select2({ theme: "bootstrap4", width: '100%' });
            $('#editLapLai').select2({ theme: "bootstrap4", width: '100%' });
        }, 200);

        // Bắt sự kiện upload file khi chọn file mới
        $('#editFileDinhKem').off('change').on('change', function () {
            var fileInput = this;
            if (fileInput.files && fileInput.files.length > 0) {
                uploadFile(fileInput, function (fileUrl) {
                    $('#editFileDinhKem').data('uploaded-url', fileUrl);
                    showToastSuccess('Tải file thành công!');
                });
            } else {
                $('#editFileDinhKem').removeData('uploaded-url');
            }
        });


        // Khi thay đổi nhóm, load nhân viên thực hiện
        $(document).off('change', '#editNhomCongViec').on('change', '#editNhomCongViec', function () {
            var nhomId = $(this).val();
            $('#nhanVienThucHien').empty().trigger('change');
            $('#editNhanVienThucHien').empty().trigger('change'); // Xóa option khi đổi nhóm

            if (!nhomId) return;
            var nhom = allNhomNhanVien.find(x => x.id === nhomId);
            if (!nhom) {
                $('#editNhanVienThucHien').empty().trigger('change'); // Đảm bảo xóa option nếu không tìm thấy nhóm
                return;
            }
            LoadNhanVienByIdNhom(nhom.groupId, nhom.companyId, nhomId, "editNhanVienThucHien");
        });
        getAllNVTH(row.nhomCongViec, row.id);
        // Sự kiện cập nhật công việc
        $(document).off('click', '#btnUpdateCongViec').on('click', '#btnUpdateCongViec', function () {
            var id = $('#editIdCongViec').val();
            var id_NguoiGiaoViec = $('#Id_NguoiGiaoViec').val();
            var nhomCongViec = $('#editNhomCongViec').val();
            var nhanVienThucHien = $('#editNhanVienThucHien').val();
            var ngayBatDau = $('#editNgayBatDau').val();
            var ngayKetThuc = $('#editNgayKetThuc').val();
            var mucDoUuTien = $('#editMucDoUuTien').val();
            var lapLai = $('#editLapLai').val();
            var noiDung = $('#editNoiDungCongViec').val().trim();
            var tenCongViec = $('#editTenCongViec').val().trim();
            var fileDinhKem = $('#editFileDinhKem').data('uploaded-url') || row.fileDinhKem || "";

            // Validate đơn giản
            var isValid = true;
            if (!nhomCongViec) { $('#editNhomCongViec').addClass('is-invalid'); isValid = false; } else { $('#editNhomCongViec').removeClass('is-invalid'); }
            if (!nhanVienThucHien.length > 0) { $('#editNhanVienThucHien').addClass('is-invalid'); isValid = false; } else { $('#editNhanVienThucHien').removeClass('is-invalid'); }
            if (!noiDung) { $('#editNoiDungCongViec').addClass('is-invalid'); isValid = false; } else { $('#editNoiDungCongViec').removeClass('is-invalid'); }
            if (!tenCongViec) { $('#editTenCongViec').addClass('is-invalid'); isValid = false; } else { $('#editTenCongViec').removeClass('is-invalid'); }
            if (!mucDoUuTien) { $('#editMucDoUuTien').addClass('is-invalid'); isValid = false; } else { $('#editMucDoUuTien').removeClass('is-invalid'); }
            if (!lapLai) { $('#editLapLai').addClass('is-invalid'); isValid = false; } else { $('#editLapLai').removeClass('is-invalid'); }
            if (!ngayBatDau) { $('#editNgayBatDau').addClass('is-invalid'); isValid = false; } else { $('#editNgayBatDau').removeClass('is-invalid'); }
            if (!ngayKetThuc) { $('#editNgayKetThuc').addClass('is-invalid'); isValid = false; } else { $('#editNgayKetThuc').removeClass('is-invalid'); }
            if (ngayBatDau && ngayKetThuc && new Date(ngayBatDau) >= new Date(ngayKetThuc)) {
                $('#editNgayBatDau').addClass('is-invalid');
                $('#editNgayKetThuc').addClass('is-invalid');
                showToastError('Ngày bắt đầu phải nhỏ hơn ngày kết thúc!');
                isValid = false;
            }
            if (!isValid) return;

            // Gửi dữ liệu lên API cập nhật
            $.ajax({
                url: "/api/CongViec/UpdateCongViec?id=" + id,
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + resUser.token
                },
                data: JSON.stringify({
                    CongViec: {
                        Id: id,
                        id_NguoiGiaoViec: resUser.userInfor.userName,
                        NhomCongViec: nhomCongViec,
                        NgayBatDau: ngayBatDau,
                        NgayKetThuc: ngayKetThuc,
                        MucDoUuTien: mucDoUuTien,
                        LapLai: lapLai,
                        NoiDungCongViec: noiDung,
                        TenCongViec: tenCongViec,
                        FileDinhKem: fileDinhKem,
                        GroupId: resUser.userInfor.groupId,
                        CompanyId: resUser.userInfor.companyId,
                        CreateAt: new Date().toISOString(),
                        CreateBy: resUser.userInfor.userName,
                        IsActive: 1
                    },
                    NhanVienThucHien: nhanVienThucHien,
                    Themngay: {
                        "id": "",
                        "id_CongViec": "",
                        "id_CongViecThemNgay": "",
                        "soNgay": 1,
                        "groupId": "",
                        "createAt": "2025-04-09T03:27:24.194984",
                        "createBy": "",
                        "isActive": 1
                    }
                })
            }).done(function (response) {
                showToastSuccess("Cập nhật công việc thành công!");
                $('#globalModal').modal('hide');
                LoadNhomNhanVienByTaiKhoan();
            }).fail(function (xhr) {
                const message = xhr.responseJSON?.message || "Lỗi khi cập nhật công việc.";
                showToastError(message);
            });
        });
    });
}

function getAllNVTH(idNhomNhanVien, id_CongViec) {
    getJwtToken().then(function (resUser) {
        if (!idNhomNhanVien) {
            $('#editNhanVienThucHien').empty().trigger('change');
            return;
        }

        // Bước 1: Lấy danh sách nhân viên theo nhóm
        $.ajax({
            url: `/api/NhanVien/GetNhanVienByNhom?groupId=${resUser.userInfor.groupId}&companyId=${resUser.userInfor.companyId}&Id_NhomNhanVien=${idNhomNhanVien}`,
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (resNhanVien) {
            if (!resNhanVien.success || !Array.isArray(resNhanVien.data)) {
                showToastError("Không lấy được danh sách nhân viên.");
                return;
            }

            const nhanVienList = resNhanVien.data;

            // Bước 2: Gọi API lấy danh sách nhân viên thực hiện
            $.ajax({
                url: `/api/congviec/GetAllNVTH?groupId=${resUser.userInfor.groupId}`,
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + resUser.token
                },
                data: JSON.stringify({
                    id_CongViec: id_CongViec,
                    id_NhanVien: "",
                    groupId: resUser.userInfor.groupId,
                    CreateBy: resUser.userInfor.userName
                })
            }).done(function (resNVTH) {
                if (!resNVTH.success || !Array.isArray(resNVTH.data)) {
                    showToastError("Không lấy được danh sách nhân viên thực hiện.");
                    return;
                }

                const selectedIds = resNVTH.data.map(x => x.id_NhanVien);
                let html = '';
                nhanVienList.forEach(nv => {
                    const selected = selectedIds.includes(nv.id) ? 'selected' : '';
                    html += `<option value="${nv.id}" ${selected}>${nv.tenNhanVien || ''} (${nv.taiKhoan || ''})</option>`;
                });
                $('#editNhanVienThucHien').html(html);

                // Destroy select2 nếu đã khởi tạo
                if ($('#editNhanVienThucHien').hasClass("select2-hidden-accessible")) {
                    $('#editNhanVienThucHien').select2('destroy');
                }

                // Khởi tạo lại select2
                $('#editNhanVienThucHien').select2({
                    theme: "bootstrap4",
                    width: '100%',
                    placeholder: "Chọn nhân viên thực hiện"
                });

                // Set lại giá trị đã chọn
                $('#editNhanVienThucHien').val(selectedIds).trigger('change');

            }).fail(xhr => {
                const message = xhr.responseJSON?.message || "Lỗi khi tải nhân viên thực hiện.";
                showToastError(message);
            });

        }).fail(function () {
            showToastError("Lỗi khi tải danh sách nhân viên.");
            $('#editNhanVienThucHien').empty().trigger('change');
        });
    });
}



function deleteCongViec(id) {
    getJwtToken().then(function (resUser) {
        if (!confirm("Bạn có chắc chắn muốn xóa công việc này?")) return;

        $.ajax({
            url: `/api/CongViec/${id}?userName=${resUser.userInfor.userName}`,
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (response) {
            if (response?.success) {
                showToastSuccess("Xóa công việc thành công!");
                LoadNhomNhanVienByTaiKhoan();
            } else {
                showToastError(response?.message || "Xóa công việc thất bại.");
            }
        }).fail(function (xhr) {
            const msg = xhr.responseJSON?.message || "Lỗi khi xóa công việc.";
            showToastError(msg);
        });
    });
}

function addChildCongViec(row) {
    // Tạo nội dung body cho modal
    var bodyHtml = `
    <form id="formAddCVC" novalidate>
        <div class="form-group">
            <label for="inputIdCongViec">Công việc cha</label>
            <input type="text" class="form-control" value="${row.tenCongViec}" readonly>
            <input type="text" class="form-control" id="inputIdCongViec" value="${row.id}" hidden>
        </div>
        <div class="form-group">
            <label for="inputNoiDungCVC">Nội dung công việc con</label>
            <input type="text" class="form-control" id="inputNoiDungCVC" placeholder="Nhập nội dung công việc con" required>
            <div class="invalid-feedback" id="inputNoiDungCVCFeedback"></div>
        </div>
        <div class="form-group">
           <label for="inputFileCVC">File đính kèm (tùy chọn)</label>
           <input type="file" class="form-control-file" id="inputFileCVC" accept="*/*">
           <div class="invalid-feedback" id="inputFileCVCFeedback"></div>
        </div>
    </form>`;
    // Tạo nội dung footer cho modal
    var footerHtml = `
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
            <button type="button" class="btn btn-primary" id="btnSaveCVC">Lưu</button>
            `;
    // Gọi hàm showGlobalModal đã đặt ở layout
    showGlobalModal('Thêm công việc con', bodyHtml, footerHtml);

    // Gán lại sự kiện lưu (xóa sự kiện cũ trước để tránh double event)
    $(document).off('click', '#btnSaveCVC').on('click', '#btnSaveCVC', function () {
        var idCongViec = $('#inputIdCongViec').val();
        var noiDungCVC = $('#inputNoiDungCVC').val().trim();
        var $input = $('#inputNoiDungCVC');
        var $feedback = $('#inputNoiDungCVCFeedback');
        $input.removeClass('is-invalid');
        $feedback.text('');

        var fileInput = document.getElementById('inputFileCVC');
        var file = fileInput.files[0];

        if (!noiDungCVC) {
            $input.addClass('is-invalid');
            $feedback.text('Vui lòng nhập nội dung công việc con.');
            $input.focus();
            return;
        }

        if (file) {
            uploadFile(fileInput, function (fileUrl) {
                insertCVC(idCongViec, noiDungCVC, fileUrl);
                $('#globalModal').modal('hide');
            });
        } else {
            insertCVC(idCongViec, noiDungCVC, "");
            $('#globalModal').modal('hide');
        }
    });

}
function insertCVC(idCongViec, noiDungCVC, fileDinhKem) {
    getJwtToken().then(function (resUser) {
        const data = {
            id: "",
            Id_CongViec: idCongViec,
            NoiDungCongViec: noiDungCVC,
            FileDinhKem: fileDinhKem || "",
            HoanThanh: 0,
            GroupId: resUser.userInfor.groupId,
            CreateAt: new Date().toISOString(),
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };

        $.ajax({
            url: "/api/CongViec/InsertCVC?userName=qminh97ictu@gmail.com",
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function (response) {
            var message = "Thêm công việc con thành công!";
            showToastSuccess(message);
            loadCongViec(nhomSelected.id, selectNhanVien);
        }).fail(xhr => {
            const message = xhr.responseJSON?.message || "Lỗi kết nối hoặc không thêm được.";
            showToastError(message);
        });
    });
}
function editCongViecCon(cvc) {
    // Nếu cvc là chuỗi JSON, parse lại
    if (typeof cvc === "string") {
        try {
            cvc = JSON.parse(cvc);
        } catch {
            showToastError("Dữ liệu công việc con không hợp lệ.");
            return;
        }
    }
    var fileLinkHtml = "";
    if (cvc.fileDinhKem && cvc.fileDinhKem !== "") {
        fileLinkHtml = `
                <div class="mb-2">
                    <a href="${cvc.fileDinhKem}" target="_blank" class="d-inline-block">
                        <i class="fas fa-paperclip"></i> File đính kèm hiện tại
                    </a>
                </div>
            `;
    }

    var bodyHtml = `
            <form id="formEditCVC" novalidate>
                <input type="hidden" id="editCVCId" value="${cvc.id || ""}">
                <div class="form-group">
                    <label for="editNoiDungCVC">Nội dung công việc con</label>
                    <input type="text" class="form-control" id="id_CongViec" value="${cvc.id_CongViec}" hidden>
                    <input type="text" class="form-control" id="editNoiDungCVC" value="${cvc.noiDungCongViec || ""}" required>
                    <div class="invalid-feedback" id="editNoiDungCVCFeedback"></div>
                </div>
                <div class="form-group">
                    <label for="inputFileCVC">File đính kèm (tùy chọn)</label>
                    ${fileLinkHtml}
                    <input type="file" class="form-control-file" id="inputFileCVC" accept="*/*">
                    <div class="invalid-feedback" id="inputFileCVCFeedback"></div>
                </div>
            </form>
            `;
    var footerHtml = `
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
            <button type="button" class="btn btn-primary" id="btnUpdateCVC">Cập nhật</button>
            `;
    showGlobalModal('Chỉnh sửa công việc con', bodyHtml, footerHtml);

    $(document).off('click', '#btnUpdateCVC').on('click', '#btnUpdateCVC', function () {
        var id = $('#editCVCId').val();
        var noiDung = $('#editNoiDungCVC').val().trim();
        var $input = $('#editNoiDungCVC');
        var $feedback = $('#editNoiDungCVCFeedback');
        var id_CongViec = $('#id_CongViec').val();
        $input.removeClass('is-invalid');
        $feedback.text('');

        var fileInput = document.getElementById('inputFileCVC');
        var file = fileInput.files[0];

        if (!noiDung) {
            $input.addClass('is-invalid');
            $feedback.text('Vui lòng nhập nội dung công việc con.');
            $input.focus();
            return;
        }

        // Nếu có file mới, upload file rồi cập nhật
        if (file) {
            uploadFile(fileInput, function (fileUrl) {
                updateCongViecCon(id, id_CongViec, noiDung, fileUrl);
                $('#globalModal').modal('hide');
            });
        } else {
            // Không chọn file mới, giữ file cũ
            updateCongViecCon(id, id_CongViec, noiDung, cvc.fileDinhKem || "");
            $('#globalModal').modal('hide');
        }
    });
}
function updateCongViecCon(id, id_CongViec, noiDungCVC, fileDinhKem) {
    getJwtToken().then(function (resUser) {
        const data = {
            id: id,
            Id_CongViec: id_CongViec,
            NoiDungCongViec: noiDungCVC,
            FileDinhKem: fileDinhKem || "",
            HoanThanh: 0,
            GroupId: resUser.userInfor.groupId,
            CreateAt: new Date().toISOString(),
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };

        $.ajax({
            url: `/api/congviec/UpdateCVC?userName=${resUser.userInfor.userName}`,
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function () {
            showToastSuccess("Cập nhật công việc con thành công!");
            loadCongViec(nhomSelected.id, selectNhanVien);

        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi khi cập nhật công việc con.";
            showToastError(message);
        });
    });
}
function uploadFile(fileInput, callback) {
    getJwtToken().then(function (resUser) {

        if (!fileInput.files.length) {
            alert("Vui lòng chọn file trước khi upload.");
            return;
        }

        const form = new FormData();
        form.append("file", fileInput.files[0]);

        $.ajax({
            url: "/api/CongViec/upload",
            method: "POST",
            headers: {
                "Authorization": "Bearer " + resUser.token
            },
            processData: false,
            contentType: false,
            mimeType: "multipart/form-data",
            data: form
        }).done(function (response) {
            const res = JSON.parse(response);
            if (res.success && res.data) {
                callback(res.data);
            } else {
                showToastError("Upload thất bại: " + (res.message || "Không rõ lỗi"));
            }
        }).fail(function () {
            showToastError("Lỗi kết nối khi upload file");
        });
    });
}
// Xóa công việc con
function deleteCongViecCon(id) {
    getJwtToken().then(function (resUser) {
        if (!confirm("Bạn có chắc chắn muốn xóa công việc con này?")) return;
        $.ajax({
            url: `/api/CongViec/DeleteByIdCVC/${id}?userName=${resUser.userInfor.userName}`,
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function () {
            showToastSuccess("Xóa công việc con thành công!");
            loadCongViec(nhomSelected.id, selectNhanVien);
        }).fail(xhr => {
            const message = xhr.responseJSON?.message || "Lỗi khi xóa công việc con.";
            showToastError(message);
        });
    });
}

