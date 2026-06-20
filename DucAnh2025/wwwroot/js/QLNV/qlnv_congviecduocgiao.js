

var allNhomNhanVien = [];
var congViecSelectd;
var nhomSelected;
var congViecDataSet = [];

$(function () {
    GetNhomNhanVienByCVDG();

    var isCalendar = false;
    $('#toggle-dg').on('click', function () {
        isCalendar = !isCalendar;
        if (isCalendar) {
            $('#tab-table-dg').hide();
            $('#tab-calender-dg').show();
            $('#toggle-dg').html('<i class="header-icon lnr-laptop-phone icon-gradient bg-plum-plate"></i> Bảng');
            renderCalendarCongViecDuocGiao(congViecDataSet);
        } else {
            $('#tab-calender-dg').hide();
            $('#tab-table-dg').show();
            $('#toggle-dg').html('<i class="header-icon pe-7s-id icon-gradient bg-plum-plate"></i> Lịch');
        }
    });
});

function GetNhomNhanVienByCVDG() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: `/api/nhomnhanvien/GetNhomNhanVienByCVDGAsync?groupId=${resUser.userInfor.groupId}&taiKhoan=${resUser.userInfor.userName}`,
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

                if (nhomSelected) {
                    loadCongViec( nhomSelected.id);
                }
            } else {
                nhomSelected = null;
                allNhomNhanVien = [];
                $('#nhanvientrongnhom').empty();
            }
        }).fail(function (xhr) {
            nhomSelected = null;
            allNhomNhanVien = [];
            showToastError(xhr.responseJSON?.message || "Lỗi khi tải nhóm nhân viên theo công việc đánh giá.");
            $('#nhanvientrongnhom').empty();
        });
    });
}

function onNhomClick(nhom) {
    $('.nav.flex-column .nav-item.cv-active').removeClass('cv-active');
    $(`.nav.flex-column .nav-item[data-nhom-id="${nhom.id}"]`).addClass('cv-active');
    nhomSelected = nhom;
    loadCongViec( nhom.id);
}

function loadCongViec(nhomCongViec) {
    getJwtToken().then(function (resUser) {
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
                Id_NguoiGiaoViec: "",
                NguoiThucHien: resUser.userInfor.nguoiThucHien,
                NhomCongViec: nhomCongViec,
                TenNhom: "",
                NgayBatDau: null,
                NgayKetThuc: null,
                MucDoUuTien: "",
                TuDanhGia: "",
                TienDo: 0,
                LapLai: "",
                NoiDungCongViec: "",
                TenCongViec: "",
                FileDinhKem: "",
                GroupId: resUser.userInfor.groupId,
                companyId: resUser.userInfor.companyId,
                CreateAt: new Date().toISOString(),
                CreateBy: "",
                IsActive: 1
            })
        }).done(function (res) {
            var dataSet = [];
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                dataSet = res.data.map(function (cv, idx) {
                    return {
                        stt: idx + 1,
                        id: cv.id,
                        id_NguoiGiaoViec: cv.id_NguoiGiaoViec,
                        nhomCongViec: cv.nhomCongViec,
                        tenNhom: cv.tenNhom,
                        nguoiThucHien: cv.nguoiThucHien,
                        tienDo: cv.TienDo,
                        lapLai: cv.lapLai,
                        fileDinhKem: cv.fileDinhKem,
                        noiDungCongViec: cv.noiDungCongViec || '',
                        tenCongViec: cv.tenCongViec || '',
                        groupId: cv.groupId,
                        companyId: cv.companyId,
                        ngayBatDau: cv.ngayBatDau ? moment(cv.ngayBatDau).format('DD-MM-YYYY') : '',
                        ngayKetThuc: cv.ngayKetThuc ? moment(cv.ngayKetThuc).format('DD-MM-YYYY') : '',
                        mucDoUuTien: cv.mucDoUuTien || '',
                        createBy: cv.createBy,
                        isActive: cv.isActive,
                    };
                });
            }

            congViecDataSet = dataSet; 

            var $table = $('#table_congviec');
            if ($.fn.DataTable.isDataTable($table)) {
                $table.DataTable().clear().destroy();
            }

            $table.DataTable({
                data: dataSet,
                columns: [
                    { data: 'stt', className: 'text-center align-middle', width: "5%" },
                    { data: 'tenCongViec', className: 'align-middle readonly-cell' },
                    { data: 'ngayBatDau', className: 'align-middle readonly-cell' },
                    { data: 'ngayKetThuc', className: 'align-middle readonly-cell' }
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
                createdRow: function (row, data, dataIndex) {
                    // Cột "Ngày kết thúc" là cột thứ 3 (index 3)
                    var $td = $('td', row).eq(3);
                    $td.removeClass('bg-danger bg-primary bg-success text-white');
                    if (data.mucDoUuTien === 'Khẩn cấp' || data.mucDoUuTien === 'Cao') {
                        $td.addClass('bg-danger text-white');
                    } else if (data.mucDoUuTien === 'Trung bình') {
                        $td.addClass('bg-primary text-white');
                    } else if (data.mucDoUuTien === 'Thấp') {
                        $td.addClass('bg-success text-white');
                    }
                    row.setAttribute(
                        'onclick',
                        `showSidebar(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(data))}')))`
                    );
                }
            });

            if ($('#tab-calender-dg').is(':visible')) {
                renderCalendarCongViecDuocGiao(congViecDataSet);
            }
        }).fail(function (xhr) {
            $('#table_congviec tbody').html('<tr><td colspan="4" class="text-center text-danger">Lỗi khi tải dữ liệu.</td></tr>');
        });
    });
}

// Hàm render FullCalendar cho Công việc được giao
function renderCalendarCongViecDuocGiao(dataSet) {
    var calendarEl = document.getElementById('calendar-congviecduocgiao');
    if (!calendarEl) return;

    // Xóa calendar 
    if (calendarEl._fullCalendar) {
        calendarEl._fullCalendar.destroy();
    }

    $(calendarEl).css({
        width: '100%',
        height: '100%',
        minHeight: '400px'
    });

    var events = (dataSet || []).map(function (cv) {
        let start = cv.ngayBatDau ? moment(cv.ngayBatDau, 'DD-MM-YYYY').format('YYYY-MM-DD') : null;
        let end = cv.ngayKetThuc ? moment(cv.ngayKetThuc, 'DD-MM-YYYY').format('YYYY-MM-DD') : null;
        if (start && end && (start === end || moment(end).isSameOrBefore(start))) {
            end = moment(start).add(1, 'days').format('YYYY-MM-DD');
        } else if (end) {
            end = moment(end).add(1, 'days').format('YYYY-MM-DD');
        }
        return {
            id: cv.id,
            title: cv.tenCongViec,
            start: start,
            end: end,
            description: cv.nguoiThucHien,
            backgroundColor: getEventColor(cv.mucDoUuTien)
        };
    });

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

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        locale: 'vi',
        height: '100%',
        contentHeight: 'auto',
        aspectRatio: 1.7,
        expandRows: true,
        events: events,
        eventClick: function (info) {
            var cv = (congViecDataSet || []).find(x => x.id === info.event.id);
            if (cv) {
                showSidebar(cv); 
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

function showSidebar(row) {
    getJwtToken().then(function (resUser) {
        // Gán dữ liệu vào các trường trong sidebar
        $('#Id_NguoiGiaoViec').val(row.id_NguoiGiaoViec || '');
        $('#NgayBatDau').text(row.ngayBatDau || '');
        $('#LapLai').val(row.lapLai || '');
        $('#TenNhom').val(row.tenNhom || '');
        $('#MucDoUuTien').val(row.mucDoUuTien || '');
        $('#NgayKetThuc').text(row.ngayKetThuc || '');
        $('#NguoiThucHien').val(row.nguoiThucHien || '');
        $('#TenCongViec').val(row.tenCongViec || '');
        $('#NoiDungCongViec').val(row.noiDungCongViec || '');

        // Gọi API lấy đánh giá nếu có
        $.ajax({
            url: `/api/danhgia/GetByIdCongViec?idCongViec=${encodeURIComponent(row.id)}`,
            method: "GET",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            if (res && res.success && res.data) {
                $('#duocDanhGia').val(res.data.danhGia ?? '');
                $('#ghiChuDuocDanhGia').val(res.data.ghiChu ?? '');
            } else {
                $('#duocDanhGia').val('');
                $('#ghiChuDuocDanhGia').val('');
            }
        }).fail(function (xhr) {
            $('#duocDanhGia').val('');
            $('#ghiChuDuocDanhGia').val('');
        });
        // Gọi API lấy công việc con
        $.ajax({
            url: '/api/CongViec/GetByIdCongViecCVC/' + (row.id),
            method: 'GET',
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            if (res.success && Array.isArray(res.data)) {
                if (res.data.length > 0) {
                    // Hiển thị bảng và render dữ liệu
                    $('#table_cvc').closest('.table-responsive').show();
                    var html = '';
                    res.data.forEach(function (item, idx) {
                        html += `<tr>
                                <td>${idx + 1}</td>
                                <td>${item.noiDungCongViec || ''}</td>
                                <td>${item.fileDinhKem ? `<a href="${item.fileDinhKem}" target="_blank">Tải file</a>` : '<span class="text-muted">Không có</span>'}</td>
                                <td>
                                     <input type="checkbox"
                                         ${item.hoanThanh == 1 ? 'checked' : ''}
                                         onchange="updateCongViecCon('${item.id}', '${item.id_CongViec}', '${item.noiDungCongViec ? item.noiDungCongViec.replace(/'/g, "\\'") : ''}', '${item.fileDinhKem ? item.fileDinhKem.replace(/'/g, "\\'") : ''}', this.checked ? 1 : 0)"/>
                                 </td>
                            </tr>`;
                    });
                    $('#table_cvc tbody').html(html);
                } else {
                    // Ẩn bảng nếu không có công việc con
                    $('#table_cvc').closest('.table-responsive').hide();
                }
            } else {
                // Ẩn bảng nếu không có dữ liệu
                $('#table_cvc').closest('.table-responsive').hide();
            }
        }).fail(function (xhr) {
            $('#table_cvc').closest('.table-responsive').hide();
        });

        // Mở sidebar và overlay
        $('.control-sidebar').addClass('open');
        $('.control-sidebar-overlay').show();
        setupTienDoAndTuDanhGiaListener(row);
        qlnvCollab.mountInline('#qlnv-task-collaboration-panel', row);
    });

}


function setupTienDoAndTuDanhGiaListener(row) {
    getJwtToken().then(function (resUser) {
        const $tienDo = $('#TienDo');
        const $tuDanhGia = $('#TuDanhGia');
        let lastTienDo = $tienDo.val();
        let lastTuDanhGia = $tuDanhGia.val();

        function clearError($input) {
            $input.removeClass('is-invalid');
            $input.next('.invalid-feedback').remove();
        }

        function showError($input, message) {
            $input.addClass('is-invalid');
            if ($input.next('.invalid-feedback').length === 0) {
                $input.after(`<div class="invalid-feedback">${message}</div>`);
            }
        }

        function toISODateString(dateStr) {
            if (!dateStr) return null;
            if (dateStr.includes('T')) return dateStr;
            const parts = dateStr.split('-');
            if (parts.length === 3) {
                // dd-MM-yyyy
                return `${parts[2]}-${parts[1]}-${parts[0]}`;
            }
            return dateStr;
        }

        function updateCongViecIfNeeded() {
            clearError($tienDo);
            const tienDo = parseFloat($tienDo.val());
            const tuDanhGia = $tuDanhGia.val();

            if (isNaN(tienDo) || tienDo <= 0 || tienDo >= 11) {
                showError($tienDo, 'Tiến độ phải lớn hơn 0 và nhỏ hơn hoặc bằng 10.');
                return;
            }

            if (tienDo == lastTienDo && tuDanhGia == lastTuDanhGia) return;

            lastTienDo = tienDo;
            lastTuDanhGia = tuDanhGia;

            $.ajax({
                url: "/api/CongViec/UpdateCongViec?id=" + row.id,
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + resUser.token
                },
                data: JSON.stringify({
                    CongViec: {
                        Id: row.id,
                        Id_NguoiGiaoViec: row.id_NguoiGiaoViec,
                        NhomCongViec: row.nhomCongViec,
                        NgayBatDau: toISODateString(row.ngayBatDau),
                        NgayKetThuc: toISODateString(row.ngayKetThuc),
                        MucDoUuTien: row.mucDoUuTien,
                        LapLai: row.lapLai,
                        NoiDungCongViec: row.noiDungCongViec,
                        TenCongViec: row.tenCongViec,
                        FileDinhKem: row.fileDinhKem,
                        GroupId: resUser.userInfor.groupId,
                        CompanyId: resUser.userInfor.companyId,
                        TienDo: tienDo,
                        TuDanhGia: tuDanhGia,
                        CreateAt: new Date().toISOString(),
                        CreateBy: row.id_NguoiGiaoViec,
                        IsActive: 1
                    },
                    NhanVienThucHien: [],
                    Themngay: {
                        "id": "",
                        "id_CongViec": "",
                        "id_CongViecThemNgay": "",
                        "soNgay": 1,
                        "groupId": "",
                        "createAt": new Date().toISOString(),
                        "createBy": "",
                        "isActive": 1
                    }
                })
            }).done(function (response) {
                showToastSuccess("Cập nhật công việc thành công!");
                $('#globalModal').modal('hide');
            }).fail(function (xhr) {
                const message = xhr.responseJSON?.message || "Lỗi khi cập nhật công việc.";
                showToastError(message);
            });
        }

        $tienDo.off('change.td').on('change.td', updateCongViecIfNeeded);
        $tuDanhGia.off('change.tdg').on('change.tdg', updateCongViecIfNeeded);
    });
}

function onCloseSidebar() {
    $('.control-sidebar').removeClass('open');
    $('.control-sidebar-overlay').hide();
}
function updateCongViecCon(id, id_CongViec, noiDungCVC, fileDinhKem, hoanThanh) {
    getJwtToken().then(function (resUser) {
        const data = {
            id: id,
            Id_CongViec: id_CongViec,
            NoiDungCongViec: noiDungCVC,
            FileDinhKem: fileDinhKem || "",
            HoanThanh: hoanThanh,
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

        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi khi cập nhật công việc con.";
            showToastError(message);
        });
     });
}
