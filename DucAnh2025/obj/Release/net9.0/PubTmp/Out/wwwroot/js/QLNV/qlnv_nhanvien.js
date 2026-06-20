$(function () {
    loadData();
    loadAndApplyPermissions("249ff511-8f10-45e8-bf8f-29b0ada5ab84","2105f7e7-1d45-4369-85a9-fdd185c3490b");
    // Nút thêm mới nhân viên (giả sử có nút #btnShowAddModal)
    $('#btnShowAddModal').on('click', function () {
        showNhanVienModal();
    });
});

function applyPermissions(permissions) {
    $('[data-permission]').hide();
    if (!permissions || !permissions.length) {
        return;
    }
    permissions.forEach(function (perm) {
        if (perm.isActive !== 1) return;
        switch (perm.permissionType) {
            case 3: $('[data-permission="Thêm"]').show(); break;
            case 4: $('[data-permission="Sửa"]').show(); break;
            case 5: $('[data-permission="Xóa"]').show(); break;
        }
    });
}
// Gọi API lấy quyền
 function loadAndApplyPermissions(parentMajorId, majorId) {
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
            window.currentPermissions = (response.success && Array.isArray(response.data)) ? response.data : [];
            applyPermissions(window.currentPermissions);
        });
    });
}


// Reset dropdown helper
function resetDropdowns(selectors) {
    selectors.forEach(sel => $(sel).empty().append(`<option value="">-- Chọn ${$(sel).attr('id').replace('Id', '').toLowerCase().replace(/([A-Z])/g, ' $1').trim()} --</option>`));
}

// Hiển thị modal thêm/sửa nhân viên (tối ưu)
function showNhanVienModal(nhanVien = {}) {
    const isEdit = !!nhanVien.id;
    const modalTitle = isEdit ? "Cập nhật nhân viên" : "Thêm nhân viên";
    const btnAddtk = `<button type="button" class="btn btn-success" id="btnAddNVTK">Thêm nhân viên + Tài khoản</button>`;
    const btnAdd = `<button type="button" class="btn btn-success" id="btnAddNV">Thêm nhân viên</button>`;
    const btnUpdate = `<button type="button" class="btn btn-primary" id="btnUpdateNV">Cập nhật</button>`;
    const btnCancel = `<button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy bỏ</button>`;

    const bodyHtml = `
        <div class="row g-3 mb-3">
            <div class="col-12">
                <div id="formError" class="alert alert-danger d-none"></div>
                <input type="hidden" id="NhanVienId" value="${nhanVien.id || ''}" />
            </div>
            <div class="col-md-6">
                <div class="mb-2">
                    <label for="CompanyId" class="form-label">Chi nhánh <span class="text-danger">*</span></label>
                    <select id="CompanyId" class="form-control" required>
                        <option value="">-- Chọn chi nhánh --</option>
                    </select>
                </div>
                <div class="mb-2">
                    <label for="ChucVuId" class="form-label">Chức vụ <span class="text-danger">*</span></label>
                    <select id="ChucVuId" class="form-control" required>
                        <option value="">-- Chọn chức vụ --</option>
                    </select>
                </div>
                <div class="mb-2">
                    <label for="TenNhanVien" class="form-label">Tên nhân viên <span class="text-danger">*</span></label>
                    <input type="text" id="TenNhanVien" class="form-control" placeholder="Tên nhân viên" value="${nhanVien.tenNhanVien || ''}" required />
                </div>
            </div>
            <div class="col-md-6">
                <div class="mb-2">
                    <label for="DepartmentId" class="form-label">Phòng ban <span class="text-danger">*</span></label>
                    <select id="DepartmentId" class="form-control" required>
                        <option value="">-- Chọn phòng ban --</option>
                    </select>
                </div>
                <div class="mb-2">
                    <label for="ChuyenMonId" class="form-label">Chuyên môn <span class="text-danger">*</span></label>
                    <select id="ChuyenMonId" class="form-control" required>
                        <option value="">-- Chọn chuyên môn --</option>
                    </select>
                </div>
                <div class="mb-2">
                    <label for="TaiKhoan" class="form-label">Tài khoản (email) <span class="text-danger">*</span></label>
                    <input type="email" id="TaiKhoan" class="form-control" placeholder="Tài khoản" value="${nhanVien.taiKhoan || ''}" ${isEdit ? 'disabled' : ''} required />
                </div>
            </div>
        </div>
    `;

    const footerHtml = isEdit ? btnUpdate + btnCancel : btnAddtk + btnAdd + btnCancel;
    showGlobalModal(modalTitle, bodyHtml, footerHtml);

    // Load dropdowns và set giá trị nếu là sửa
    getJwtToken().then(async function (resUser) {
        await loadChiNhanh(nhanVien.companyId, resUser);
        if (nhanVien.companyId) {
           await loadDepartments(nhanVien.companyId, nhanVien.departmentId, resUser);
        }
        if (nhanVien.departmentId) {
            loadChucVu(nhanVien.departmentId, nhanVien.chucVuId, resUser);
            await loadChuyenMon(nhanVien.departmentId, nhanVien.chuyenMonId, resUser);
        }
    });

    // Bind sự kiện sau khi modal render xong
    $('#CompanyId').off('change').on('change', function () {
        const companyId = $(this).val();
        resetDropdowns(['#DepartmentId', '#ChucVuId', '#ChuyenMonId']);
        getJwtToken().then(async function (resUser) {
            if (companyId) await loadDepartments(companyId, null, resUser);
        });
    });
    $('#DepartmentId').off('change').on('change', function () {
        const departmentId = $(this).val();
        resetDropdowns(['#ChucVuId', '#ChuyenMonId']);
        getJwtToken().then(async function (resUser) {
            if (departmentId) {
                await loadChucVu(departmentId, null, resUser);
                await loadChuyenMon(departmentId, null, resUser);
            }
        });
    });

    $('#btnAddNVTK').off('click').on('click', AddNhanVienTKModal);
    $('#btnAddNV').off('click').on('click', AddNhanVienModal);
    $('#btnUpdateNV').off('click').on('click', UpdateNhanVienModal);
}

// Thêm nhân viên + tài khoản (tối ưu)
function AddNhanVienTKModal() {
    submitNhanVienModal("/api/nhanvien", "POST", "Thêm nhân viên thành công!", false);
}

// Thêm nhân viên không tài khoản (tối ưu)
function AddNhanVienModal() {
    submitNhanVienModal("/api/nhanvien/CreateNhanVienNotTaiKhoan", "POST", "Thêm nhân viên thành công!", false);
}

// Hàm dùng chung cho submit modal nhân viên
function submitNhanVienModal(apiUrl, method, successMsg, isUpdate) {
    $('#formError').addClass('d-none').html('');
    if (!validateNhanVienForm()) return;
    getJwtToken().then(function (resUser) {
        const id = isUpdate ? $('#NhanVienId').val() : "";
        const data = getNhanVienFormData(id, resUser);
        $.ajax({
            url: apiUrl + (apiUrl.includes("?") ? "" : "?userName=" + encodeURIComponent(resUser.userInfor.userName)),
            method: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function () {
            showToastSuccess(successMsg);
            $('#formError').addClass('d-none').html('');
            loadData();
            $('#globalModal').modal('hide');
        }).fail(function (xhr) {
            $('#formError').removeClass('d-none').html(xhr.responseJSON?.message || "Lỗi khi xử lý nhân viên");
        });
    });
}

// Cập nhật nhân viên từ modal
function UpdateNhanVienModal() {
    $('#formError').addClass('d-none').html('');
    if (!validateNhanVienForm()) return;
    const id = $('#NhanVienId').val();
    getJwtToken().then(function (resUser) {
        const data = getNhanVienFormData(id, resUser);
        $.ajax({
            url: "/api/NhanVien/" + id + "?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
            data: JSON.stringify(data)
        }).done(function () {
            showToastSuccess('Cập nhật nhân viên thành công!');
            $('#formError').addClass('d-none').html('');
            loadData();
            $('#globalModal').modal('hide');
        }).fail(function (xhr) {
            $('#formError').removeClass('d-none').html(xhr.responseJSON?.message || "Lỗi khi cập nhật nhân viên");
        });
    });
}

// Lấy dữ liệu form nhân viên
function getNhanVienFormData(id, resUser) {
    return {
        id: id,
        TenNhanVien: $('#TenNhanVien').val().trim(),
        TaiKhoan: $('#TaiKhoan').val().trim(),
        GroupId: resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412",
        CompanyId: $('#CompanyId').val(),
        DepartmentId: $('#DepartmentId').val(),
        ChucVuId: $('#ChucVuId').val(),
        ChuyenMonId: $('#ChuyenMonId').val(),
        createAt: new Date().toISOString(),
        CreateBy: resUser ? resUser.userInfor.userName : "qminh97ictu@gmail.com",
        IsActive: 1
    };
}

// Xóa nhân viên
function DeleteNhanVien(id) {
    if (!id) {
        $('#formError').removeClass('d-none').html('Không xác định được nhân viên cần xóa.');
        return;
    }
    if (!confirm('Bạn có chắc chắn muốn xóa nhân viên này?')) return;
    $('#formError').addClass('d-none').html('');
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: "/api/NhanVien/" + id + "?userName=" + encodeURIComponent(resUser.userInfor.userName),
            method: "DELETE",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function () {
            $('#formError').addClass('d-none').html('');
            loadData();
        }).fail(function (xhr) {
            $('#formError').removeClass('d-none').html(xhr.responseJSON?.message || "Lỗi khi xóa nhân viên");
        });
    });
}

// Load danh sách nhân viên
function loadData() {
    getJwtToken().then(async function (resUser) {
        await $.ajax({
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
                companyId: resUser.userInfor.companyId,
                departmentId: "",
                chucVuId: "",
                chuyenMonId: "",
                createAt: new Date().toISOString(),
                createBy: "",
                isActive: 1
            })
        }).done(function (response) {
            let html = '';
            if (response?.data?.length) {
                response.data.forEach((item, index) => {
                    html += `<tr>
                        <td>${index + 1}</td>
                        <td>${item.companyName || ''}</td>
                        <td>${item.departmentName || ''}</td>
                        <td>${item.chucVu || ''}</td>
                        <td>${item.chuyenMon || ''}</td>
                        <td>${item.tenNhanVien || ''}</td>
                        <td>${item.taiKhoan || ''}</td>
                        <td><button data-permission="Sửa" class="btn btn-warning btn-sm" onclick="showNhanVienModal(JSON.parse(decodeURIComponent('${encodeURIComponent(JSON.stringify(item))}')))">Sửa</button></td>
                        <td><button data-permission="Xóa" class="btn btn-danger btn-sm btn-delete" onclick="DeleteNhanVien('${item.id}')">Xóa</button></td>
                    </tr>`;
                });
            }
            if ($.fn.DataTable.isDataTable('#table_nhanvien')) {
                $('#table_nhanvien').DataTable().destroy();
            }
            $('#tbody').html(html);
            $('#table_nhanvien').DataTable({
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
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );
                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }
            });
            if (window.currentPermissions) {
                applyPermissions(window.currentPermissions);
            }
        }).fail(xhr => {
            $('#formError').removeClass('d-none').html('Lỗi khi tải dữ liệu: ' + (xhr.responseJSON?.message || xhr.statusText));
        })
    });
}

// Load chi nhánh
async function loadChiNhanh(selectedId, resUser) {
    await $.ajax({
        url: "/api/chinhanh/getall?groupId=" + (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
        method: "GET",
        headers: {
            "Authorization": "Bearer " + (resUser ? resUser.token : "")
        }
    }).done(function (response) {
        const $dropdown = $('#CompanyId');
        $dropdown.empty().append('<option value="">-- Chọn chi nhánh --</option>');
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.tenChiNhanh}</option>`));
            if (selectedId) $dropdown.val(selectedId).trigger('change');
        }
    }).fail(() => alert('Không thể tải danh sách chi nhánh'));
}

// Load phòng ban
async function loadDepartments(companyId, selectedId, resUser) {
    const $dropdown = $('#DepartmentId');
    $dropdown.empty().append('<option value="">-- Chọn phòng ban --</option>');
    if (!companyId) return;
    await $.ajax({
        url: "/api/NhanSu/DepartmentGetAllByVM?groupId=" + (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + (resUser ? resUser.token : "")
        },
        data: JSON.stringify({
            CompanyId: companyId,
            GroupId: (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
            IsActive: 1,
            PageNumber: 1,
            PageSize: 100
        })
    }).done(function (response) {
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.deptName}</option>`));
            if (selectedId) $dropdown.val(selectedId);
        }
    }).fail(() => alert('Không thể tải danh sách phòng ban'));
}

// Load chức vụ
async function loadChucVu(departmentId, selectedId, resUser) {
    const $dropdown = $('#ChucVuId');
    $dropdown.empty().append('<option value="">-- Chọn chức vụ --</option>');
    if (!departmentId) return;
    await $.ajax({
        url: "/api/nhansu/ChucVuGetAllByVM?groupId=" + (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + (resUser ? resUser.token : "")
        },
        data: JSON.stringify({
            DepartmentId: departmentId,
            GroupId: (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
            IsActive: 1,
            PageNumber: 1,
            PageSize: 100
        })
    }).done(function (response) {
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.chucVu}</option>`));
            if (selectedId) $dropdown.val(selectedId);
        }
    }).fail(() => alert('Không thể tải danh sách chức vụ'));
}

// Load chuyên môn
async function loadChuyenMon(departmentId, selectedId, resUser) {
    const $dropdown = $('#ChuyenMonId');
    $dropdown.empty().append('<option value="">-- Chọn chuyên môn --</option>');
    if (!departmentId) return;
    await $.ajax({
        url: "/api/nhansu/ChuyenMonGetAllByVM?groupId=" + (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + (resUser ? resUser.token : "")
        },
        data: JSON.stringify({
            DepartmentId: departmentId,
            GroupId: (resUser ? resUser.userInfor.groupId : "5a022928-fb56-49d8-bc8a-d69f2f3e2412"),
            IsActive: 1,
            PageNumber: 1,
            PageSize: 100
        })
    }).done(function (response) {
        if (response?.data?.length) {
            response.data.forEach(item => $dropdown.append(`<option value="${item.id}">${item.chuyenMon}</option>`));
            if (selectedId) $dropdown.val(selectedId);
        }
    }).fail(() => alert('Không thể tải danh sách chuyên môn'));
}

// Validate form nhân viên
function validateNhanVienForm() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    let isValid = true;
    const requiredFields = [
        { selector: '#CompanyId', message: 'Vui lòng chọn chi nhánh.' },
        { selector: '#DepartmentId', message: 'Vui lòng chọn phòng ban.' },
        { selector: '#ChucVuId', message: 'Vui lòng chọn chức vụ.' },
        { selector: '#ChuyenMonId', message: 'Vui lòng chọn chuyên môn.' },
        { selector: '#TenNhanVien', message: 'Vui lòng nhập tên nhân viên.' },
        { selector: '#TaiKhoan', message: 'Vui lòng nhập tài khoản.' }
    ];
    requiredFields.forEach(f => {
        if (!$(f.selector).val() || !$(f.selector).val().trim()) {
            showError(f.selector, f.message);
            isValid = false;
        }
    });
    return isValid;
}

// Hiển thị lỗi
function showError(selector, message) {
    const $input = $(selector);
    $input.addClass('is-invalid');
    if ($input.next('.invalid-feedback').length === 0) {
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }
}