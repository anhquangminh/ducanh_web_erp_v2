$(function () {
    loadDataQLN();
    loadDataQLNV();
    loadChiNhanhAll();

    loadAndApplyPermissions_qln("249ff511-8f10-45e8-bf8f-29b0ada5ab84", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba");
    loadAndApplyPermissions_qlnv("249ff511-8f10-45e8-bf8f-29b0ada5ab84", "fcf752d9-c19a-496d-bba9-f0864928f32b");
});
function applyPermissions_qln(permissions) {
    $('[data-permission-qln]').hide();
    if (!Array.isArray(permissions) || permissions.length === 0) return;
    const permissionMap = {
        3: "Thêm_qln",
        4: "Sửa_qln",
        5: "Xóa_qln"
    };

    permissions.forEach(perm => {
        if (perm.isActive === 1 && permissionMap[perm.permissionType]) {
            $(`[data-permission-qln="${permissionMap[perm.permissionType]}"]`).show();
        }
    });
}
// Gọi API lấy quyền
function loadAndApplyPermissions_qln(parentMajorId, majorId) {
    getJwtToken().then(async function (resUser) {
        await $.ajax({
            url: "/api/phanquyen/GetAllPermissionByGroupId?groupId=" + resUser.userInfor.groupId
                + "&userId=" + resUser.userInfor.id
                + "&parentMajorId=" + parentMajorId
                + "&majorId=" + majorId,
            method: "GET",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (response) {
            window.currentPermissions_qln = (response.success && Array.isArray(response.data)) ? response.data : [];
            applyPermissions_qln(window.currentPermissions_qln);
        });
    });
}

function applyPermissions_qlnv(permissions) {
    $('[data-permission-qlnv]').hide();
    if (!Array.isArray(permissions) || permissions.length === 0) return;
    const permissionMap = {
        3: "Thêm_qlnv",
        4: "Sửa_qlnv",
        5: "Xóa_qlnv"
    };
    permissions.forEach(perm => {
        if (perm.isActive === 1 && permissionMap[perm.permissionType]) {
            $(`[data-permission-qlnv="${permissionMap[perm.permissionType]}"]`).show();
        }
    });
}
// Gọi API lấy quyền
function loadAndApplyPermissions_qlnv(parentMajorId, majorId) {
    getJwtToken().then(async function (resUser) {
        await $.ajax({
            url: "/api/phanquyen/GetAllPermissionByGroupId?groupId=" + resUser.userInfor.groupId
                + "&userId=" + resUser.userInfor.id
                + "&parentMajorId=" + parentMajorId
                + "&majorId=" + majorId,
            method: "GET",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (response) {
            window.currentPermissions_qlnv = (response.success && Array.isArray(response.data)) ? response.data : [];
            applyPermissions_qlnv(window.currentPermissions_qlnv);
        });
    });
}


// ==== MODAL QLN ====
function showModalQLN(item) {
    item = item || {};
    var isEdit = !!item.id;
    var modalTitle = isEdit ? "Cập nhật nhóm" : "Thêm nhóm";
    var btnAdd = `<button type="button" class="btn btn-success" id="btnAddQLNModal">Thêm nhóm</button>`;
    var btnUpdate = `<button type="button" class="btn btn-primary" id="btnUpdateQLNModal">Cập nhật nhóm</button>`;
    var btnCancel = `<button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy bỏ</button>`;

    var bodyHtml = `
        <div class="row g-3 mb-3">
            <div class="col-12">
                <div id="formErrorQLN" class="alert alert-danger d-none"></div>
                <input type="hidden" id="IdQLN" value="${item.id || ''}" />
            </div>
            <div class="col-md-6">
                <div class="mb-2">
                    <label for="CompanyIdQLN" class="form-label">Chi nhánh <span class="text-danger">*</span></label>
                    <select id="CompanyIdQLN" class="form-control" required>
                        <option value="">-- Chọn chi nhánh --</option>
                    </select>
                </div>
                <div class="mb-2">
                    <label for="TenNhomQLN" class="form-label">Tên nhóm <span class="text-danger">*</span></label>
                    <input type="text" id="TenNhomQLN" class="form-control" placeholder="Tên nhóm" value="${item.tenNhom || ''}" required />
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-2">
                    <label for="Id_QuanLy" class="form-label">Quản lý<span class="text-danger">*</span></label>
                    <select id="Id_QuanLy" class="form-control" required>
                        <option value="">-- Chọn quản lý --</option>
                    </select>
                </div>
            </div>
        </div>
    `;

    var footerHtml = `
        ${isEdit ? btnUpdate : btnAdd}
        ${btnCancel}
    `;

    showGlobalModal(modalTitle, bodyHtml, footerHtml);

    getJwtToken().then(function (resUser) {
        loadChiNhanhQLN(item.companyId, resUser);
        if (item.companyId) {
            loadNhanViensQLN(item.companyId, item.id_QuanLy, resUser);
        }
    });

    setTimeout(function () {
        $('#CompanyIdQLN').off('change').on('change', function () {
            var companyId = $(this).val();
            resetDropdowns(['#Id_QuanLy']);
            getJwtToken().then(function (resUser) {
                if (companyId) loadNhanViensQLN(companyId, null, resUser);
            });
        });

        $('#btnAddQLNModal').off('click').on('click', function () {
            AddQLNModal();
        });
        $('#btnUpdateQLNModal').off('click').on('click', function () {
            UpdateQLNModal();
        });
    }, 200);
}

// ==== MODAL QLNV ====
function showModalQLNV(item) {
    item = item || {};
    var modalTitle = "Thêm nhân viên vào nhóm";
    var btnAdd = `<button type="button" class="btn btn-success" id="btnAddQLNVModal">Thêm vào nhóm</button>`;
    var btnCancel = `<button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy bỏ</button>`;

    var bodyHtml = `
        <div class="row g-3 mb-3">
            <div class="col-12">
                <div id="formErrorQLNV" class="alert alert-danger d-none"></div>
                <input type="hidden" id="IdQLNV" value="${item.id || ''}" />
            </div>
            <div class="row col-12">
                <div class="col-md-4">
                    <label for="CompanyIdQLNV" class="form-label">Chi nhánh <span class="text-danger">*</span></label>
                    <select id="CompanyIdQLNV" class="form-control" required>
                        <option value="">-- Chọn chi nhánh --</option>
                    </select>
                </div>
                <div class="col-md-4">
                    <label for="Id_NhomNhanVien" class="form-label">Nhóm <span class="text-danger">*</span></label>
                    <select id="Id_NhomNhanVien" class="form-control" required>
                        <option value="">-- Chọn nhóm --</option>
                    </select>
                </div>
                <div class="col-md-4">
                    <label for="Id_NhanVien" class="form-label">Nhân viên <span class="text-danger">*</span></label>
                    <select id="Id_NhanVien" class="form-control" required>
                        <option value="">-- Chọn nhân viên --</option>
                    </select>
                </div>
            </div>
        </div>
    `;

    var footerHtml = `
        ${btnAdd}
        ${btnCancel}
    `;

    showGlobalModal(modalTitle, bodyHtml, footerHtml);

    getJwtToken().then(function (resUser) {
        loadChiNhanhQLNV(item.companyId, resUser);
        if (item.companyId) {
            loadNhomNhanVienQLNV(item.companyId, item.id_NhomNhanVien, resUser);
        }
        if (item.id_NhomNhanVien) {
            loadNhanVienByCompanyQLNV(item.companyId, item.id_NhanVien, resUser);
        }
    });

    setTimeout(function () {
        $('#CompanyIdQLNV').off('change').on('change', function () {
            var companyId = $(this).val();
            resetDropdowns(['#Id_NhomNhanVien', '#Id_NhanVien']);
            getJwtToken().then(function (resUser) {
                if (companyId) loadNhomNhanVienQLNV(companyId, null, resUser);
            });
        });
        $('#Id_NhomNhanVien').off('change').on('change', function () {
            var nhomId = $(this).val();
            resetDropdowns(['#Id_NhanVien']);
            getJwtToken().then(function (resUser) {
                if (nhomId) loadNhanVienByCompanyQLNV($('#CompanyIdQLNV').val(), null, resUser);
            });
        });

        $('#btnAddQLNVModal').off('click').on('click', function () {
            AddQLNVModal();
        });
    }, 200);
}

// ==== DROPDOWN LOADERS ====
function loadChiNhanhAll(selectedId) {
    getJwtToken().then(async function (resUser) {
        await $.ajax({
            url: "/api/chinhanh/getall?groupId=" + resUser.userInfor.groupId,
            method: "GET",
            headers: { "Authorization": "Bearer " + resUser.token }
        }).done(function (response) {
            const $dropdowns = [$('#CompanyIdQLN'), $('#CompanyIdQLNV')];
            $dropdowns.forEach($dropdown => {
                $dropdown.empty().append('<option value="">-- Chọn chi nhánh --</option>');
                if (response?.data?.length) {
                    response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.tenChiNhanh}</option>`));
                    if (selectedId) $dropdown.val(selectedId).trigger('change');
                }
            });
        });
    });
}
function loadChiNhanhQLN(selectedId, resUser) {
    $.ajax({
        url: "/api/chinhanh/getall?groupId=" + resUser.userInfor.groupId,
        method: "GET",
        headers: { "Authorization": "Bearer " + resUser.token }
    }).done(function (response) {
        var $dropdown = $('#CompanyIdQLN');
        $dropdown.empty().append('<option value="">-- Chọn chi nhánh --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.tenChiNhanh}</option>`));
            if (selectedId) $dropdown.val(selectedId).trigger('change');
        }
    });
}
function loadNhanViensQLN(companyId, selectedId, resUser) {
    $.ajax({
        url: "/api/nhanvien/GetByVM?groupId=" + resUser.userInfor.groupId,
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + resUser.token
        },
        data: JSON.stringify({
            id: "",
            TenNhanVien: "",
            TaiKhoan: "",
            GroupId: resUser.userInfor.groupId,
            CompanyId: companyId || "",
            DepartmentId: "",
            ChucVuId: "",
            ChuyenMonId: "",
            createAt: new Date().toISOString(),
            CreateBy: "",
            IsActive: 1
        })
    }).done(function (response) {
        var $dropdown = $('#Id_QuanLy');
        $dropdown.empty().append('<option value="">-- Chọn quản lý --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => {
                $dropdown.append(`<option value="${item.id}">${item.tenNhanVien} (${item.taiKhoan})</option>`);
            });
            if (selectedId) $dropdown.val(selectedId);
        }
    });
}
function loadChiNhanhQLNV(selectedId, resUser) {
    $.ajax({
        url: "/api/chinhanh/getall?groupId=" + resUser.userInfor.groupId,
        method: "GET",
        headers: { "Authorization": "Bearer " + resUser.token }
    }).done(function (response) {
        var $dropdown = $('#CompanyIdQLNV');
        $dropdown.empty().append('<option value="">-- Chọn chi nhánh --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.tenChiNhanh}</option>`));
            if (selectedId) $dropdown.val(selectedId).trigger('change');
        }
    });
}
function loadNhomNhanVienQLNV(companyId, selectedId, resUser) {
    $.ajax({
        url: "/api/NhomNhanVien/GetByVM?groupId=" + resUser.userInfor.groupId,
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + resUser.token
        },
        data: JSON.stringify({
            Id: "",
            id_QuanLy: "",
            companyId: companyId,
            tenNhom: "",
            groupId: resUser.userInfor.groupId,
            createAt: new Date().toISOString(),
            createBy: "",
            isActive: 1
        })
    }).done(function (response) {
        var $dropdown = $('#Id_NhomNhanVien');
        $dropdown.empty().append('<option value="">-- Chọn nhóm --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => {
                $dropdown.append(`<option value="${item.id}">${item.tenNhom}</option>`);
            });
            if (selectedId) $dropdown.val(selectedId).trigger('change');
        }
    });
}
function loadNhanVienByCompanyQLNV(companyId, selectedId, resUser) {
    $.ajax({
        url: "/api/nhanvien/GetByVM?groupId=" + resUser.userInfor.groupId,
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + resUser.token
        },
        data: JSON.stringify({
            id: "",
            tenNhanVien: "",
            taiKhoan: "",
            groupId: resUser.userInfor.groupId,
            companyId: companyId,
            departmentId: "",
            chucVuId: "",
            chuyenMonId: "",
            createAt: new Date().toISOString(),
            createBy: "",
            isActive: 1
        })
    }).done(function (response) {
        var $dropdown = $('#Id_NhanVien');
        $dropdown.empty().append('<option value="">-- Chọn nhân viên --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => {
                $dropdown.append(`<option value="${item.id}">${item.tenNhanVien}-${item.taiKhoan}</option>`);
            });
            if (selectedId) $dropdown.val(selectedId);
        }
    });
}

// ==== RESET DROPDOWNS ====
function resetDropdowns(selectors) {
    selectors.forEach(sel => $(sel).empty().append(`<option value="">-- Chọn ${$(sel).attr('id').replace('Id', '').toLowerCase().replace(/([A-Z])/g, ' $1').trim()} --</option>`));
}

// ==== VALIDATE ====
function validateQLNForm() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    let isValid = true;
    const requiredFields = [
        { selector: '#CompanyIdQLN', message: 'Vui lòng chọn chi nhánh.' },
        { selector: '#Id_QuanLy', message: 'Vui lòng chọn quản lý.' },
        { selector: '#TenNhomQLN', message: 'Vui lòng nhập tên nhóm.' }
    ];
    requiredFields.forEach(f => {
        if (!$(f.selector).val() || !$(f.selector).val().trim()) {
            showErrorQLN(f.selector, f.message);
            isValid = false;
        }
    });
    return isValid;
}
function showErrorQLN(selector, message) {
    const $input = $(selector);
    $input.addClass('is-invalid');
    if ($input.next('.invalid-feedback').length === 0) {
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }
}
function validateQLNVForm() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    let isValid = true;
    const requiredFields = [
        { selector: '#CompanyIdQLNV', message: 'Vui lòng chọn chi nhánh.' },
        { selector: '#Id_NhomNhanVien', message: 'Vui lòng chọn nhóm.' },
        { selector: '#Id_NhanVien', message: 'Vui lòng chọn nhân viên.' }
    ];
    requiredFields.forEach(f => {
        if (!$(f.selector).val() || !$(f.selector).val().trim()) {
            const $input = $(f.selector);
            $input.addClass('is-invalid');
            if ($input.next('.invalid-feedback').length === 0) {
                $input.after(`<div class="invalid-feedback">${f.message}</div>`);
            }
            isValid = false;
        }
    });
    return isValid;
}

// ==== ADD/UPDATE QLN ====
function AddQLNModal() {
    $('#formErrorQLN').addClass('d-none').html('');
    if (!validateQLNForm()) return;
    getJwtToken().then(function (resUser) {
        var data = {
            Id_QuanLy: $('#Id_QuanLy').val(),
            TenNhom: $('#TenNhomQLN').val().trim(),
            CompanyId: $('#CompanyIdQLN').val(),
            GroupId: resUser.userInfor.groupId,
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };
        $.ajax({
            url: "/api/NhomNhanVien?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function (res) {
            if (res.success) {
                $('#formErrorQLN').addClass('d-none').html('');
                loadDataQLN();
                $('#globalModal').modal('hide');
                showToastSuccess(res.message || 'Thêm nhóm thành công!');
            } else {
                $('#formErrorQLN').removeClass('d-none').html(res.message || 'Có lỗi xảy ra.');
            }
        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi khi thêm nhóm";
            $('#formErrorQLN').removeClass('d-none').html(message);
        });
    });
}
function UpdateQLNModal() {
    $('#formErrorQLN').addClass('d-none').html('');
    if (!validateQLNForm()) return;
    const id = $('#IdQLN').val();
    getJwtToken().then(function (resUser) {
        var data = {
            id: id,
            Id_QuanLy: $('#Id_QuanLy').val().trim(),
            TenNhom: $('#TenNhomQLN').val().trim(),
            GroupId: resUser.userInfor.groupId,
            CompanyId: $('#CompanyIdQLN').val(),
            createAt: new Date().toISOString(),
            CreateBy: resUser.userInfor.userName,
            IsActive: 1
        };
        $.ajax({
            url: "/api/NhomNhanVien/" + id + "?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function (res) {
            $('#formErrorQLN').addClass('d-none').html('');
            loadDataQLN();
            $('#globalModal').modal('hide');
            showToastSuccess(res.message || 'Cập nhật nhóm thành công!');
        }).fail(function (xhr) {
            $('#formErrorQLN').removeClass('d-none').html(xhr.responseJSON?.message || "Lỗi khi cập nhật nhóm");
        });
    });
}

// ==== ADD QLNV ====
function AddQLNVModal() {
    $('#formErrorQLNV').addClass('d-none').html('');
    if (!validateQLNVForm()) return;
    getJwtToken().then(function (resUser) {
        const companyId = $('#CompanyIdQLNV').val().trim();
        const idNhom = $('#Id_NhomNhanVien').val().trim();
        const Id_NhanVien = $('#Id_NhanVien').val().trim();
        const data = {
            id_NhomNhanVien: idNhom,
            id_NhanVien: Id_NhanVien,
            companyId: companyId,
            groupId: resUser.userInfor.groupId,
            createAt: new Date().toISOString(),
            createBy: resUser.userInfor.userName,
            isActive: 1
        };
        $.ajax({
            url: "/api/QuanLyNhanVien?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function (res) {
            if (res.success) {
                showToastSuccess(res.message || 'Thêm vào nhóm thành công!');
                $('#formErrorQLNV').addClass('d-none').html('');
                loadDataQLNV();
                $('#globalModal').modal('hide');
            } else {
                $('#formErrorQLNV').removeClass('d-none').html(res.message || 'Có lỗi xảy ra.');
            }
        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi khi thêm quản lý nhân viên";
            $('#formErrorQLNV').removeClass('d-none').html(message);
        });
    });
}

// ==== LOAD DATA TABLES ====
function loadDataQLN() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: "/api/NhomNhanVien/GetByVM?groupId=" + resUser.userInfor.groupId,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify({
                "Id": "",
                "id_QuanLy": "",
                "tenNhom": "",
                "groupId": resUser.userInfor.groupId,
                "createAt": new Date().toISOString(),
                "createBy": "",
                "isActive": 1
            })
        }).done(function (response) {
            let html = '';
            if (response?.data?.length) {
                response.data.forEach((item, index) => {
                    html += `<tr>
                        <td>${index + 1}</td>
                        <td>${item.companyName || ''}</td>
                        <td>${item.tenNhanVien || ''}</td>
                        <td>${item.tenNhom || ''}</td>
                        <td><button data-permission-qln="Sửa_qln" class="btn btn-warning btn-sm" onclick="showModalQLN(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(item))}')))">Sửa</button></td>
                        <td><button data-permission-qln="Sửa_qln" class="btn btn-danger btn-sm" onclick="deleteQLN('${item.id}')">Xóa</button></td>
                    </tr>`;
                });
            }
            if ($.fn.DataTable.isDataTable('#table_qln')) {
                $('#table_qln').DataTable().destroy();
            }
            $('#table_qln tbody').html(html);
            $('#table_qln').DataTable({
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d) {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });
                    });
                }
            });
            if (window.currentPermissions_qln) {
                applyPermissions_qln(window.currentPermissions_qln);
            }
        }).fail(function (xhr) {
            $('#formErrorQLN').removeClass('d-none').html('Lỗi khi tải dữ liệu: ' + (xhr.responseJSON?.message || xhr.statusText));
        });
    });
}
function loadDataQLNV() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: "/api/QuanLyNhanVien/GetByVM?groupId=" + resUser.userInfor.groupId,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify({
                "Id": "",
                "Id_NhomNhanVien": "",
                "TenNhom": "",
                "Id_NhanVien": "",
                "TenNhanVien": "",
                "TaiKhoan": "",
                "GroupId": resUser.userInfor.groupId,
                "CreateAt": new Date().toISOString(),
                "CreateBy": "",
                "IsActive": 1
            })
        }).done(function (response) {
            let html = '';
            if (response?.data?.length) {
                response.data.forEach((item, index) => {
                    html += `<tr>
                  <td>${index + 1}</td>
                  <td>${item.companyName || ''}</td>
                  <td>${item.tenNhom || ''}</td>
                  <td>${item.tenNhanVien || ''} (${item.taiKhoan || ''})</td>
                  <td>
                    <button data-permission-qlnv="Xóa_qlnv" class="btn btn-danger btn-sm" onclick="deleteQLNV('${item.id}')">Xóa</button>
                  </td>
                </tr>`;
                });
            }
            if ($.fn.DataTable.isDataTable('#table_qlnv')) {
                $('#table_qlnv').DataTable().destroy();
            }
            $('#table_qlnv tbody').html(html);
            $('#table_qlnv').DataTable({
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d) {
                            select.append('<option value="' + d + '">' + d + '</option>');
                        });
                    });
                }
            });

            if (window.currentPermissions_qlnv) {
                applyPermissions_qlnv(window.currentPermissions_qlnv);
            }
        }).fail(function (xhr) {
            $('#formErrorQLNV').removeClass('d-none').html('Lỗi khi tải dữ liệu: ' + (xhr.responseJSON?.message || xhr.statusText));
        });
    });
}

// ==== DELETE ====
function deleteQLN(id) {
    if (!id) {
        $('#formErrorQLN').removeClass('d-none').html('Không xác định được nhóm cần xóa.');
        return;
    }
    if (!confirm('Bạn có chắc chắn muốn xóa nhóm này?')) return;
    $('#formErrorQLN').addClass('d-none').html('');
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: "/api/NhomNhanVien/" + id + "?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            $('#formErrorQLN').addClass('d-none').html('');
            loadDataQLN();
            showToastSuccess(res.message || 'Xóa nhóm thành công!');
        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi kết nối hoặc không xoá được.";
            showToastError(message);
        });
    });
}
function deleteQLNV(id) {
    if (!confirm("Bạn có chắc chắn muốn xoá quản lý nhân viên này?")) return;
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: `/api/QuanLyNhanVien/${id}?userName=${encodeURIComponent(resUser.userInfor.userName)}`,
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            if (res.success) {
                showToastSuccess(res.message || "Xoá thành công!");
                loadDataQLNV();
            } else {
                showToastError(res.message || "Có lỗi xảy ra khi xoá.");
            }
        }).fail(function (xhr) {
            const message = xhr.responseJSON?.message || "Lỗi kết nối hoặc không xoá được.";
            showToastError(message);
        });
    });
}