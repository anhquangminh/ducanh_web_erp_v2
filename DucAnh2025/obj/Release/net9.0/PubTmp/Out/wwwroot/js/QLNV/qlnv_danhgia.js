$(function () {
    loadDanhGia();
});

let originalTableHtml = $('#table_danhgia').parent().html();

function showError(selector, message) {
    const $input = $(selector);
    $input.addClass('is-invalid');
    if ($input.next('.invalid-feedback').length === 0) {
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }
}

function clearError(selector) {
    $(selector).removeClass('is-invalid').next('.invalid-feedback').remove();
}

function reloadDataTable($table, html) {
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
        $table.parent().html(html);
        return $('#table_danhgia');
    }
    return $table;
}

function loadDanhGia() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: `/api/danhgia/GetByVM?groupId=${resUser.userInfor.groupId}&Id_NguoiGiaoViec=${resUser.userInfor.userName}`,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify({
                Id: "",
                Id_CongViec: "",
                Id_NguoiGiaoViec: resUser.userInfor.userName,
                NoiDungCongViec: "",
                TenCongViec: "",
                NguoiThucHien: "",
                TienDo: 0,
                DanhGia: 0,
                GhiChu: "",
                GroupId: "",
                createAt: "2025-04-09T00:00:00",
                CreateBy: "",
                IsActive: 1,
                IsEdited: false
            })
        }).done(function (res) {
            let dataSet = [];
            if (res.success && Array.isArray(res.data)) {
                dataSet = res.data.map((dg, idx) => ({
                    id: dg.id,
                    stt: idx + 1,
                    id_CongViec: dg.id_CongViec,
                    id_NguoiGiaoViec: dg.id_NguoiGiaoViec,
                    tienDo: dg.tienDo,
                    companyId: dg.companyId,
                    groupId: dg.groupId,
                    createBy: dg.createBy,
                    noiDungCongViec: dg.noiDungCongViec || '',
                    tenCongViec: dg.tenCongViec || '',
                    nguoiThucHien: dg.nguoiThucHien || '',
                    tienDoText: dg.tienDo === 0 ? 'Chưa thực hiện' : dg.tienDo === 10 ? 'Hoàn thành' : 'Đang thực hiện',
                    tienDoClass: dg.tienDo === 0 ? 'text-danger' : dg.tienDo === 10 ? 'text-success' : 'text-warning',
                    danhGia: dg.danhGia ?? '',
                    ghiChu: dg.ghiChu ?? ''
                }));
            }

            let $table = $('#table_danhgia');
            $table = reloadDataTable($table, originalTableHtml);

            $table.DataTable({
                data: dataSet,
                columns: [
                    { data: 'stt', className: 'text-center align-middle' },
                    { data: 'tenCongViec', className: 'align-middle' },
                    { data: 'noiDungCongViec', className: 'align-middle' },
                    { data: 'nguoiThucHien', className: 'align-middle' },
                    {
                        data: 'tienDoText',
                        className: 'align-middle text-center',
                        render: (data, type, row) => `<span class="${row.tienDoClass}">${data}</span>`
                    },
                    {
                        data: 'danhGia',
                        className: 'align-middle text-center',
                        render: (data, type, row) =>
                            `<input step="any" type="number" class="input-custom input-danhgia" value="${data}" data-id="${row.id}" data-id-congviec="${row.id_CongViec}">`
                    },
                    {
                        data: 'ghiChu',
                        className: 'align-middle',
                        render: (data, type, row) =>
                            `<input class="input-custom input-ghichu" value="${data}" data-id="${row.id}" data-id-congviec="${row.id_CongViec}">`
                    },
                    {
                        data: null,
                        className: 'text-center align-middle',
                        orderable: false,
                        render: (data, type, row) =>
                            `<div class="btn-group-custom">
                                <button class="btn btn-sm btn-success btn-luu" style="display:none" onclick="DanhGia(this)" data-id="${row.id}" data-id-congviec="${row.id_CongViec}">
                                    <i class="pe-7s-cloud-download"></i>
                                </button>
                                <button class="btn btn-sm btn-primary" onclick="showDetail('${row.id}','${row.id_CongViec}','${encodeURIComponent(row.nguoiThucHien)}')" title="Chi tiết">
                                    <i class="pe-7s-ribbon"></i>
                                </button>
                            </div>`
                    }
                ],
                autoWidth: false,
                scrollX: true,
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
                drawCallback: function () {
                    $('#table_danhgia tbody')
                        .off('input', '.input-danhgia, .input-ghichu')
                        .on('input', '.input-danhgia, .input-ghichu', function () {
                            $(this).closest('tr').find('.btn-luu').show();
                        });
                },
                initComplete: function () {
                    this.api().columns().every(function () {
                        const column = this;
                        const select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                const val = $.fn.dataTable.util.escapeRegex($(this).val());
                                column.search(val ? '^' + val + '$' : '', true, false).draw();
                            });
                        column.data().unique().sort().each(function (d) {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });
                    });
                }
            });
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || "Lỗi khi load đánh giá.");
            $('#table_danhgia tbody').html('<tr><td colspan="8" class="text-center text-danger">Lỗi khi tải dữ liệu đánh giá.</td></tr>');
        });
    });
}

function DanhGia(btn) {
    getJwtToken().then(function (resUser) {
        const $row = $(btn).closest('tr');
        const id = $(btn).data('id');
        const id_CongViec = $(btn).data('id-congviec');
        const $inputDanhGia = $row.find('.input-danhgia');
        const danhGia = parseFloat($inputDanhGia.val());
        const ghiChu = $row.find('.input-ghichu').val();

        clearError($inputDanhGia);

        if (isNaN(danhGia) || danhGia < 0 || danhGia > 10) {
            showError($inputDanhGia, 'Đánh giá phải từ 0 đến 10.');
            $inputDanhGia.focus();
            return;
        }

        const data = {
            Id: id || "",
            Id_CongViec: id_CongViec,
            DanhGia: danhGia,
            GhiChu: ghiChu,
            GroupId: resUser.userInfor.groupId,
            createAt: new Date().toISOString(),
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };

        const ajaxSettings = {
            url: id && id.trim() !== ""
                ? `/api/danhgia/${id}?userName=${encodeURIComponent(resUser.userInfor.userName)}`
                : `/api/danhgia?userName=${encodeURIComponent(resUser.userInfor.userName)}`,
            method: id && id.trim() !== "" ? "PUT" : "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        };

        $.ajax(ajaxSettings).done(function () {
            showToastSuccess(id ? 'Cập nhật đánh giá thành công!' : 'Thêm đánh giá thành công!');
            $('#globalModal').modal('hide');
            loadDanhGia();
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || (id ? 'Lỗi khi cập nhật đánh giá.' : 'Lỗi khi thêm đánh giá.'));
        });
    });
}

function showDetail(id, id_CongViec, nguoiThucHien) {
    getJwtToken().then(function (resUser) {
        const congViecPromise = $.ajax({
            url: '/api/congviec/' + encodeURIComponent(id_CongViec),
            method: 'GET',
            headers: { "Authorization": "Bearer " + resUser.token }
        });

        const danhGiaPromise = $.ajax({
            url: '/api/danhgia/GetByIdCongViec?idCongViec=' + encodeURIComponent(id_CongViec),
            method: 'GET',
            headers: { "Authorization": "Bearer " + resUser.token }
        }).then(
            res => res,
            () => ({ success: false, data: null })
        );

        Promise.all([congViecPromise, danhGiaPromise]).then(function ([res, resDanhGia]) {
            if (!(res && res.success && res.data)) {
                showToastError("Không thể tải chi tiết công việc.");
                return;
            }
            const row = res.data;
            let id_danhGia = '';
            let danhGiaValue = '';
            let ghiChuValue = '';
            if (resDanhGia && resDanhGia.success && resDanhGia.data && resDanhGia.data.id) {
                id_danhGia = resDanhGia.data.id;
                danhGiaValue = resDanhGia.data.danhGia ?? '';
                ghiChuValue = resDanhGia.data.ghiChu ?? '';
            }

            $.ajax({
                url: '/api/CongViec/GetByIdCongViecCVC/' + encodeURIComponent(id_CongViec),
                method: 'GET',
                headers: { "Authorization": "Bearer " + resUser.token }
            }).done(function (resCVC) {
                let cvcRows = '';
                if (resCVC.success && Array.isArray(resCVC.data) && resCVC.data.length > 0) {
                    resCVC.data.forEach(function (item, idx) {
                        cvcRows += `
                            <tr>
                                <td>${idx + 1}</td>
                                <td>${item.noiDungCongViec || ''}</td>
                                <td>
                                    ${item.fileDinhKem
                                ? `<a href="${item.fileDinhKem}" title="${item.fileDinhKem}" download><i class="fa fa-download"></i> Tải về</a>`
                                : '<span class="text-muted">Không có</span>'}
                                </td>
                                <td><input type="checkbox" disabled ${item.hoanThanh == 1 ? 'checked' : ''}></td>
                            </tr>
                        `;
                    });
                } else {
                    cvcRows = `<tr><td colspan="4" class="text-center text-muted">Không có công việc con</td></tr>`;
                }

                let tienDoText = '';
                let tienDoClass = '';
                if (row.tienDo === 0) {
                    tienDoText = 'Chưa thực hiện';
                    tienDoClass = 'text-danger';
                } else if (row.tienDo === 10) {
                    tienDoText = 'Hoàn thành';
                    tienDoClass = 'text-success';
                } else {
                    tienDoText = 'Đang thực hiện';
                    tienDoClass = 'text-warning';
                }

                const bodyHtml = `<div id="editFormId" autocomplete="off">
                    <div class="container mt-4">
                        <table class="table table-hover">
                            <tbody>
                                <tr><th style="width: 200px;">Người giao việc</th><td>${row.id_NguoiGiaoViec || ''}</td></tr>
                                <tr><th>Người thực hiện</th><td>${decodeURIComponent(nguoiThucHien) || ''}</td></tr>
                                <tr><th>Mức độ ưu tiên</th><td>${row.mucDoUuTien || ''}</td></tr>
                                <tr><th>Tên công việc</th><td>${row.tenCongViec || ''}</td></tr>
                                <tr><th>Nội dung công việc</th><td>${row.noiDungCongViec || ''}</td></tr>
                                <tr><th>Tự đánh giá</th><td>${row.tuDanhGia || ''}</td></tr>
                                <tr><th>Ngày bắt đầu - Ngày kết thúc</th>
                                    <td>${row.ngayBatDau || ''} - ${row.ngayKetThuc || ''}</td></tr>
                                <tr><th>Tiến độ</th><td class="${tienDoClass}">${tienDoText}</td></tr>
                                <tr><th>Tệp đính kèm</th>
                                    <td>
                                        ${row.fileDinhKem
                        ? `<a href="${row.fileDinhKem}" title="${row.fileDinhKem}" download><i class="fa fa-download"></i> Tải về</a>`
                        : '<span class="text-muted">Không có</span>'}
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <h5 class="mt-4 mb-2">Công việc con</h5>
                        <table class="table table-striped table-hover">
                            <thead class="table-info">
                                <tr>
                                    <th>STT</th>
                                    <th>Nội dung công việc</th>
                                    <th>Tệp đính kèm</th>
                                    <th>Hoàn thành</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${cvcRows}
                            </tbody>
                        </table>
                        <div class="row">
                            <div class="col-12 col-sm-12">
                                <div class="form-group">
                                    <label for="DanhGia">Đánh giá<span class="text-danger">*</span></label>
                                    <input step="any" id="DanhGia" type="number" name="Input.DanhGia" class="form-control valid" value="${danhGiaValue}">
                                    <div id="DanhGiaError" class="invalid-feedback" style="display:none"></div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12 col-sm-12">
                                <div class="form-group">
                                    <label for="GhiChu">Ghi chú<span class="text-danger"></span></label>
                                    <textarea id="GhiChu" name="Input.GhiChu" class="form-control valid">${ghiChuValue}</textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`;

                const footerHtml = `
                    <button class="btn btn-primary mr-2" type="button" onclick=updateDanhGia('${id_danhGia}','${row.id}','${row.groupId}') form="editFormId">Lưu lại</button>
                    <button class="btn btn-secondary" type="button" onclick="$('#globalModal').modal('hide')">Hủy</button>
                `;

                showGlobalModal('Chi tiết công việc', bodyHtml, footerHtml);
            });
        }).catch(function (xhr) {
            showToastError(xhr.responseJSON?.message || "Không thể tải chi tiết công việc.");
        });
    });
}

function updateDanhGia(id, id_CongViec, groupId) {
    getJwtToken().then(function (resUser) {
        const danhGia = parseFloat($('#DanhGia').val());
        const ghiChu = $('#GhiChu').val();

        clearError('#DanhGia');
        $('#DanhGiaError').hide().text('');

        if (isNaN(danhGia) || danhGia <= 0 || danhGia > 10) {
            $('#DanhGia').addClass('is-invalid');
            $('#DanhGiaError').text('Đánh giá phải lớn hơn 0 và nhỏ hơn hoặc bằng 10.').show();
            $('#DanhGia').focus();
            return;
        }

        const data = {
            Id: id || "",
            Id_CongViec: id_CongViec,
            DanhGia: danhGia,
            GhiChu: ghiChu,
            GroupId: groupId,
            createAt: new Date().toISOString(),
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };

        const ajaxSettings = {
            url: id && id.trim() !== ""
                ? `/api/danhgia/${id}?userName=${encodeURIComponent(resUser.userInfor.userName)}`
                : `/api/danhgia?userName=${encodeURIComponent(resUser.userInfor.userName)}`,
            method: id && id.trim() !== "" ? "PUT" : "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        };

        $.ajax(ajaxSettings).done(function () {
            showToastSuccess(id ? 'Cập nhật đánh giá thành công!' : 'Thêm đánh giá thành công!');
            $('#globalModal').modal('hide');
            loadDanhGia();
        }).fail(function (xhr) {
            showToastError(xhr.responseJSON?.message || (id ? 'Lỗi khi cập nhật đánh giá.' : 'Lỗi khi thêm đánh giá.'));
        });
    });
}