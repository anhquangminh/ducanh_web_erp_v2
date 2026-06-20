let danhMucTable = null;
$(function () {
    loadData();
});

function loadData() {
    getJwtToken().then(function (resUser) {
        $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/GetByVM?groupId=${resUser.userInfor.groupId}`,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + resUser.token
            },
        }).done(function (res) {
            let dataSet = [];
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                dataSet = res.data.map((item, idx) => ({
                    stt: idx + 1,
                    id: item.id,
                    id_TramTron: item.id_TramTron || '',
                    ngayNhapKho: formatVNDate(item.ngayNhapKho),
                    chotSoCoCay: item.chotSoCoCay || 0,
                    soPhieu: item.soPhieu || 0,
                    bienSo: item.bienSo || '',
                    soHopDong: item.soHopDong || '',
                    id_LoaiNhaCungCap: item.id_LoaiNhaCungCap || '',
                    id_NhaCungcap: item.id_NhaCungcap || '',
                    diaChi: item.diaChi || '',
                    maSoThue: item.maSoThue || '',
                    id_NhomNhienLieu: item.id_NhomNhienLieu || '',
                    id_LoaiNhienLieu: item.id_LoaiNhienLieu || '',
                    id_NhanHieu: item.id_NhanHieu || '',
                    id_DonVi: item.id_DonVi || '',
                    slNhapKhoCoThue: item.slNhapKhoCoThue || 0,
                    slNhapKhoKhongThue: item.slNhapKhoKhongThue || 0,
                    donViSauQuyDoi: item.donViSauQuyDoi || '',
                    soLuongThucCoThue: item.soLuongThucCoThue || 0,
                    soLuongThucKhongThue: item.soLuongThucKhongThue || 0,
                    tongSoLuong: item.tongSoLuong || 0,
                    donGiaCoThue: item.donGiaCoThue || 0,
                    donGiaKhongThue: item.donGiaKhongThue || 0,
                    thanhTienCoThue: item.thanhTienCoThue || 0,
                    thanhTienKhongThue: item.thanhTienKhongThue || 0,
                    tongThanhTien: item.tongThanhTien || 0,
                    bienSoXe: item.bienSoXe || '',
                    donGiaCuocCoThue: item.donGiaCuocCoThue || 0,
                    donGiaCuocKhongThue: item.donGiaCuocKhongThue || 0,
                    thanhTienCuocCoThue: item.thanhTienCuocCoThue || 0,
                    thanhTienCuocKhongThue: item.thanhTienCuocKhongThue || 0,
                    tongTienHangCoThue: item.tongTienHangCoThue || 0,
                    tongTienHangKhongThue: item.tongTienHangKhongThue || 0,
                    tongTienHang: item.tongTienHang || 0,
                    groupId: item.groupId || '',
                    ordinarily: item.ordinarily || 0,
                    createAt: formatDate(item.createAt) || null,
                    createBy: item.createBy || '',
                    isActive: item.isActive || 0,
                    approvalUserId: item.approvalUserId || '',
                    dateApproval: formatDate(item.dateApproval) || null,
                    approvalDept: item.approvalDept || '',
                    departmentId: item.departmentId || '',
                    departmentOrder: item.departmentOrder || 0,
                    approvalOrder: item.approvalOrder || 0,
                    approvalId: item.approvalId || '',
                    lastApprovalId: item.lastApprovalId || '',
                    isStatus: item.isStatus || '',
                }));
                // Lấy danh sách duy nhất id_TramTron
                const uniqueTramTron = [...new Set(res.data.map(x => x.id_TramTron).filter(Boolean))];

                // Render dropdown
                let $filter = $('#filterTramTron');
                if ($filter.length === 0) {
                    $('#filter').html(`
                        <select id="filterTramTron" class="custom-select" style="min-width:200px;">
                            <option value="Tất cả">-- Tất cả trạm trộn --</option>
                        </select>
                    `);
                    $filter = $('#filterTramTron');
                }
                $filter.empty().append('<option value="Tất cả">-- Tất cả trạm trộn --</option>');
                uniqueTramTron.forEach(val => {
                    $filter.append(`<option value="${val}">${val}</option>`);
                });
            }
            if (!danhMucTable) {
                danhMucTable = $('#table_data').DataTable({
                    data: dataSet,
                    columns: [
                        {
                            data: null,
                            className: 'text-center align-middle',
                            orderable: false,
                            width: '1%',
                            render: function (data, type, row) {
                                let buttons = `
                                    <div class="btn-group">
                                `;
                                if (row.isActive < 3) {
                                    buttons += `
                                        <button class="btn btn-sm btn-success btn-approve" title="Duyệt" data-id="${row.id}" onclick="duyet('${row.id}')" ><i class="fas fa-check"></i></button>
                                        <button class="btn btn-sm btn-warning btn-cancel-approve" title="Hủy duyệt" data-id="${row.id}" onclick="huyDuyet('${row.id}')" ><i class="fas fa-times"></i></button>
                                    `;
                                } else if (row.isActive == 3) {
                                    buttons += `
                                        <button class="btn btn-sm btn-danger btn-delete" title="Xóa" data-id="${row.id}" onclick="deleteCaiDat('${row.id}')" ><i class="fas fa-trash"></i></button>
                                    `;
                                }
                                buttons += `
                                    <button class="btn btn-sm btn-primary btn-edit" title="Chỉnh sửa" data-id="${row.id}" onclick="editCaiDat('${row.id}')" ><i class="fas fa-edit"></i></button>
                                    <button class="btn btn-sm btn-danger btn-delete" title="Chi tiết !" data-id="${row.id}" onclick="chiTiet('${row.id}')" ><i class="fas fa-eye"></i></button>
                                </div>
                                `;
                                return buttons;
                            }
                        },
                        { data: 'stt', className: 'text-center align-middle', width: '5%' },
                        { data: 'id_TramTron', className: 'align-middle' },
                        { data: 'ngayNhapKho', className: 'align-middle' },
                        { data: 'chotSoCoCay', className: 'align-middle' },
                        { data: 'soPhieu', className: 'align-middle' },
                        { data: 'bienSo', className: 'align-middle' },
                        { data: 'soHopDong', className: 'align-middle' },
                        { data: 'id_LoaiNhaCungCap', className: 'align-middle' },
                        { data: 'id_NhaCungcap', className: 'align-middle' },
                        { data: 'maSoThue', className: 'align-middle' },
                        { data: 'diaChi', className: 'align-middle' },
                        { data: 'id_NhomNhienLieu', className: 'align-middle' },
                        { data: 'id_LoaiNhienLieu', className: 'align-middle' },
                        { data: 'id_NhanHieu', className: 'align-middle' },
                        { data: 'id_DonVi', className: 'align-middle' },
                        { data: 'slNhapKhoCoThue', className: 'align-middle' },
                        { data: 'slNhapKhoKhongThue', className: 'align-middle' },
                        { data: 'donViSauQuyDoi', className: 'align-middle' },
                        { data: 'soLuongThucCoThue', className: 'align-middle' },
                        { data: 'soLuongThucKhongThue', className: 'align-middle' },
                        { data: 'tongSoLuong', className: 'align-middle' },
                        { data: 'donGiaCoThue', className: 'align-middle' },
                        { data: 'donGiaKhongThue', className: 'align-middle' },
                        { data: 'thanhTienCoThue', className: 'align-middle' },
                        { data: 'thanhTienKhongThue', className: 'align-middle' },
                        { data: 'tongThanhTien', className: 'align-middle' },
                        { data: 'bienSoXe', className: 'align-middle' },
                        { data: 'donGiaCuocCoThue', className: 'align-middle' },
                        { data: 'donGiaCuocKhongThue', className: 'align-middle' },
                        { data: 'thanhTienCuocCoThue', className: 'align-middle' },
                        { data: 'thanhTienCuocKhongThue', className: 'align-middle' },
                        { data: 'tongTienHangCoThue', className: 'align-middle' },
                        { data: 'tongTienHangKhongThue', className: 'align-middle' },
                        { data: 'tongTienHang', className: 'align-middle' },
                        { data: 'isStatus', className: 'align-middle' },
                    ],
                    autoWidth: false,
                    scrollX: false,
                    ordering: true,
                    searching: true,
                    paging: true,
                    pageLength: 10,
                    info: true,
                    language: {
                        lengthMenu: "Hiển thị _MENU_ bản ghi moí trang ",
                        zeroRecords: "Không tìm thấy dữ liệu !",
                        info: "Hiển thị của _TOTAL_ bản ghi !_START_ đến _END_ của _TOTAL_ bản ghi !",
                        infoEmpty: "Không có bản ghi nào ",
                        infoFiltered: "( Lọc từ )_MAX_( bản ghi)",
                        search: "Tìm kiếm :",
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
            } else {
                danhMucTable.clear();
                danhMucTable.rows.add(dataSet);
                danhMucTable.draw(false);
            }
        }).fail(function (xhr) {
            $('#table_data tbody').html('<tr><td colspan="7" class="text-center text-danger">Lỗi khi tải dữ liệu </td></tr>');
        });
    });
}
async function showModalTaoMoi(isEdit = false, data = null) {
    const modalTitle = isEdit ? "Cập nhật" : "Thêm mới";
    const resUser = await getJwtToken();
    const token = resUser.token;
    const groupId = resUser.userInfor.groupId;
    const userName = resUser.userInfor.userName;

    let selectOptions_Id_TramTron = '<option value="">Chọn Tên trạm trộn --</option>';
    try {
        const res_Id_TramTron = await $.ajax({
            url: `/api/ChiNhanh/GetAll?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_Id_TramTron.success && Array.isArray(res_Id_TramTron.data)) {
            res_Id_TramTron.data.forEach(item => {
                selectOptions_Id_TramTron += `<option value="${item.id}" ${data && data.id_TramTron === item.id ? 'selected' : ''}>${item.tenChiNhanh}</option>`;
            });
        }
    } catch {
        selectOptions_Id_TramTron += '<option value="">Lỗi tải dữ liệu </option>';
    }
    let selectOptions_SoHopDong = '<option value="">Chọn Số HĐ --</option>';
    try {
        const res_SoHopDong = await $.ajax({
            url: `/api/KhoHDMuaNhienLieu/GetAllModel?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_SoHopDong.success && Array.isArray(res_SoHopDong.data)) {
            res_SoHopDong.data.forEach(item => {
                selectOptions_SoHopDong += `
                  <option value="${item.id}" ${data && data.soHopDong === item.id ? 'selected' : ''}>
                    ${item.soHopDong} - ${item.id_NhaCungcap} | MST: ${item.maSoThue} | ${item.diaChi}
                  </option>
                `;
            });
        }

    } catch {
        selectOptions_SoHopDong += '<option value="">Lỗi tải dữ liệu </option>';
    }
    let selectOptions_Id_NhomNhienLieu = '<option value="">Chọn Nhóm nhiên liệu --</option>';
    try {
        const res_Id_NhomNhienLieu = await $.ajax({
            url: `/api/KhoDMNhomNhienLieu/GetAll?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_Id_NhomNhienLieu.success && Array.isArray(res_Id_NhomNhienLieu.data)) {
            res_Id_NhomNhienLieu.data.forEach(item => {
                selectOptions_Id_NhomNhienLieu += `<option value="${item.id}" ${data && data.id_NhomNhienLieu === item.id ? 'selected' : ''}>${item.tenNhienLieu}</option>`;
            });
        }
    } catch {
        selectOptions_Id_NhomNhienLieu += '<option value="">Lỗi tải dữ liệu </option>';
    }
    let selectOptions_Id_LoaiNhienLieu = '<option value="">Chọn Loại nhiên liệu --</option>';
    try {
        const res_Id_LoaiNhienLieu = await $.ajax({
            url: `/api/KhoDMLoaiNhienLieu/GetAll?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_Id_LoaiNhienLieu.success && Array.isArray(res_Id_LoaiNhienLieu.data)) {
            res_Id_LoaiNhienLieu.data.forEach(item => {
                selectOptions_Id_LoaiNhienLieu += `<option value="${item.id}" ${data && data.id_LoaiNhienLieu === item.id ? 'selected' : ''}>${item.loaiNhienLieu}</option>`;
            });
        }
    } catch {
        selectOptions_Id_LoaiNhienLieu += '<option value="">Lỗi tải dữ liệu </option>';
    }
    let selectOptions_Id_NhanHieu = '<option value="">Chọn Nhãn hiệu --</option>';
    try {
        const res_Id_NhanHieu = await $.ajax({
            url: `/api/KhoDMNhanHieu/GetAll?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_Id_NhanHieu.success && Array.isArray(res_Id_NhanHieu.data)) {
            res_Id_NhanHieu.data.forEach(item => {
                selectOptions_Id_NhanHieu += `<option value="${item.id}" ${data && data.id_NhanHieu === item.id ? 'selected' : ''}>${item.tenNhanHieu}</option>`;
            });
        }
    } catch {
        selectOptions_Id_NhanHieu += '<option value="">Lỗi tải dữ liệu </option>';
    }
    let selectOptions_Id_DonVi = '<option value="">Chọn Đơn vị --</option>';
    try {
        const res_Id_DonVi = await $.ajax({
            url: `/api/KhoDMDonVi/GetAll?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            }
        });
        if (res_Id_DonVi.success && Array.isArray(res_Id_DonVi.data)) {
            res_Id_DonVi.data.forEach(item => {
                selectOptions_Id_DonVi += `<option value="${item.id}" ${data && data.id_DonVi === item.id ? 'selected' : ''}>${item.tenDonVi}</option>`;
            });
        }
    } catch {
        selectOptions_Id_DonVi += '<option value="">Lỗi tải dữ liệu </option>';
    }
    const bodyHtml = `
            <form id="form_caidat" class="row">
                <div class="form-group col-md-3">
                    <label>Tên trạm trộn</label>
                    <select id="select-Id_TramTron" class="form-control">${selectOptions_Id_TramTron}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Ngày nhập kho <span class="text-danger">*</span></label>
                    <input type="date" id="input-NgayNhapKho" name="NgayNhapKho" value="${data ? formatDate(data.ngayNhapKho) : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Chốt số cơ cây dầu trước khi nhập nhiên liệu Diezel <span class="text-danger">*</span></label>
                    <input type="number" id="input-ChotSoCoCay" name="ChotSoCoCay" value="${data ? data.chotSoCoCay : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Số phiếu <span class="text-danger">*</span></label>
                    <input type="number" id="input-SoPhieu" name="SoPhieu" value="${data ? data.soPhieu : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Biển số xe <span class="text-danger">*</span></label>
                    <input type="text" id="input-BienSo" name="BienSo" value="${data ? data.bienSo : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Số HĐ</label>
                    <select id="select-SoHopDong" class="form-control">${selectOptions_SoHopDong}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Nhóm nhiên liệu</label>
                    <select id="select-Id_NhomNhienLieu" class="form-control">${selectOptions_Id_NhomNhienLieu}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Loại nhiên liệu</label>
                    <select id="select-Id_LoaiNhienLieu" class="form-control">${selectOptions_Id_LoaiNhienLieu}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Nhãn hiệu</label>
                    <select id="select-Id_NhanHieu" class="form-control">${selectOptions_Id_NhanHieu}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn vị</label>
                    <select id="select-Id_DonVi" class="form-control">${selectOptions_Id_DonVi}</select>
                </div>
                <div class="form-group col-md-3">
                    <label>Số lượng nhập kho có thuế</label>
                    <input type="number" id="input-SLNhapKhoCoThue" name="SLNhapKhoCoThue" value="${data ? data.slNhapKhoCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Số lượng nhập kho không thuế</label>
                    <input type="number" id="input-SLNhapKhoKhongThue" name="SLNhapKhoKhongThue" value="${data ? data.slNhapKhoKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn vị sau quy đổi</label>
                    <input type="text" readonly id="input-DonViSauQuyDoi" name="DonViSauQuyDoi" value="${data ? data.donViSauQuyDoi : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Số lượng thực tế có thuế</label>
                    <input type="number" readonly id="input-SoLuongThucCoThue" name="SoLuongThucCoThue" value="${data ? data.soLuongThucCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Số lượng thực tế không thuế</label>
                    <input type="number" readonly id="input-SoLuongThucKhongThue" name="SoLuongThucKhongThue" value="${data ? data.soLuongThucKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Tổng số lượng</label>
                    <input type="number" readonly id="input-TongSoLuong" name="TongSoLuong" value="${data ? data.tongSoLuong : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn giá có thuế</label>
                    <input type="number" id="input-DonGiaCoThue" name="DonGiaCoThue" value="${data ? data.donGiaCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn giá không thuế</label>
                    <input type="number" id="input-DonGiaKhongThue" name="DonGiaKhongThue" value="${data ? data.donGiaKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Thành tiền có thuế</label>
                    <input type="number" readonly id="input-ThanhTienCoThue" name="ThanhTienCoThue" value="${data ? data.thanhTienCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Thành tiền không thuế</label>
                    <input type="number" readonly id="input-ThanhTienKhongThue" name="ThanhTienKhongThue" value="${data ? data.thanhTienKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Tổng thành tiền</label>
                    <input type="number" readonly id="input-TongThanhTien" name="TongThanhTien" value="${data ? data.tongThanhTien : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Biển số xe (vận chuyển)</label>
                    <input type="text" id="input-BienSoXe" name="BienSoXe" value="${data ? data.bienSoXe : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn giá cước vận chuyển có thuế</label>
                    <input type="number" id="input-DonGiaCuocCoThue" name="DonGiaCuocCoThue" value="${data ? data.donGiaCuocCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Đơn giá cước vận chuyển không thuế</label>
                    <input type="number" id="input-DonGiaCuocKhongThue" name="DonGiaCuocKhongThue" value="${data ? data.donGiaCuocKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Thành tiền cước vận chuyển có thuế</label>
                    <input type="number" readonly id="input-ThanhTienCuocCoThue" name="ThanhTienCuocCoThue" value="${data ? data.thanhTienCuocCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Thành tiền cước vận chuyển không thuế</label>
                    <input type="number" readonly id="input-ThanhTienCuocKhongThue" name="ThanhTienCuocKhongThue" value="${data ? data.thanhTienCuocKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Tổng tiền hàng có thuế</label>
                    <input type="number" readonly id="input-TongTienHangCoThue" name="TongTienHangCoThue" value="${data ? data.tongTienHangCoThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Tổng tiền hàng không thuế</label>
                    <input type="number" readonly id="input-TongTienHangKhongThue" name="TongTienHangKhongThue" value="${data ? data.tongTienHangKhongThue : ''}" class="form-control" />
                </div>
                <div class="form-group col-md-3">
                    <label>Tổng tiền hàng</label>
                    <input type="number" readonly id="input-TongTienHang" name="TongTienHang" value="${data ? data.tongTienHang : ''}" class="form-control" />
                </div>
            </form>
            <div id="error_msg" class="text-danger font-weight-bold mb-2"></div>
        `;
    const footerHtml = `
            <button type="button" class="btn btn-primary" id="btnLuuCaiDat">${isEdit ? "Cập nhật" : "Lưu "}</button>
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Hủy</button>`;
    showGlobalModal(modalTitle, bodyHtml, footerHtml);
    $('#btnLuuCaiDat').data("isedit", isEdit).data("id", data?.id || "");
    $('#btnLuuCaiDat').off('click').on('click', async function () {
        $('#error_msg').text("");
        const resUser = await getJwtToken();
        const token = resUser.token;
        const isEdit = $(this).data("isedit") === true;
        const id = $(this).data("id");
        if (!validateForm())
            return;
        const dataSend = {
            Id: id,
            Id_TramTron: $('#select-Id_TramTron').val() || "",
            NgayNhapKho: $('#input-NgayNhapKho').val() || null,
            ChotSoCoCay: $('#input-ChotSoCoCay').val() || 0,
            SoPhieu: $('#input-SoPhieu').val() || 0,
            BienSo: $('#input-BienSo').val() || "",
            SoHopDong: $('#select-SoHopDong').val() || "",
            Id_NhomNhienLieu: $('#select-Id_NhomNhienLieu').val() || "",
            Id_LoaiNhienLieu: $('#select-Id_LoaiNhienLieu').val() || "",
            Id_NhanHieu: $('#select-Id_NhanHieu').val() || "",
            Id_DonVi: $('#select-Id_DonVi').val() || "",
            SLNhapKhoCoThue: $('#input-SLNhapKhoCoThue').val() || 0.0,
            SLNhapKhoKhongThue: $('#input-SLNhapKhoKhongThue').val() || 0.0,
            DonViSauQuyDoi: $('#input-DonViSauQuyDoi').val() || "",
            SoLuongThucCoThue: $('#input-SoLuongThucCoThue').val() || 0.0,
            SoLuongThucKhongThue: $('#input-SoLuongThucKhongThue').val() || 0.0,
            TongSoLuong: $('#input-TongSoLuong').val() || 0.0,
            DonGiaCoThue: $('#input-DonGiaCoThue').val() || 0.0,
            DonGiaKhongThue: $('#input-DonGiaKhongThue').val() || 0.0,
            ThanhTienCoThue: $('#input-ThanhTienCoThue').val() || 0.0,
            ThanhTienKhongThue: $('#input-ThanhTienKhongThue').val() || 0.0,
            TongThanhTien: $('#input-TongThanhTien').val() || 0.0,
            BienSoXe: $('#input-BienSoXe').val() || "",
            DonGiaCuocCoThue: $('#input-DonGiaCuocCoThue').val() || 0.0,
            DonGiaCuocKhongThue: $('#input-DonGiaCuocKhongThue').val() || 0.0,
            ThanhTienCuocCoThue: $('#input-ThanhTienCuocCoThue').val() || 0.0,
            ThanhTienCuocKhongThue: $('#input-ThanhTienCuocKhongThue').val() || 0.0,
            TongTienHangCoThue: $('#input-TongTienHangCoThue').val() || 0.0,
            TongTienHangKhongThue: $('#input-TongTienHangKhongThue').val() || 0.0,
            TongTienHang: $('#input-TongTienHang').val() || 0.0,
            GroupId: groupId,
            Ordinarily: 0,
            CreateAt: new Date().toISOString(),
            CreateBy: userName,
            IsActive: isEdit && data ? data.isActive : 0,
            ApprovalUserId: "",
            DateApproval: null,
            ApprovalDept: "",
            DepartmentId: "",
            DepartmentOrder: 0,
            ApprovalOrder: 0,
            ApprovalId: "",
            LastApprovalId: "",
            IsStatus: ""
        };
        try {
            const res = await $.ajax({
                url: `/api/KhoNhapkhoNhienLieu/Create?isEdit=${isEdit}`,
                method: "POST",
                contentType: "application/json",
                headers: { Authorization: `Bearer ${token}` },
                data: JSON.stringify(dataSend)
            });
            if (res.success) {
                showToastSuccess(isEdit ? "Cập nhật thành công " : "Lưu thành công");
                $('#globalModal').modal('hide');
                loadData();
            } else {
                $('#error_msg').text(res.message || "Có lỗi xảy ra khi lưu");
                showToastError(res.message || "Có lỗi xảy ra khi lưu");
            }
        } catch (err) {
            $('#error_msg').text("Có lỗi xảy ra khi lưu");
        }
    });
    setTimeout(function () {
        $('#globalModal select').select2({
            width: '100%',
            theme: 'bootstrap4',
            dropdownParent: $('#globalModal')
        });
    }, 100);
}

$(document).on('change', '#form_caidat input, #form_caidat select', function () {
    autoCalculate()
});

$(document).on('change', '#select-Id_DonVi', function () {
    const selectedText = $(this).find('option:selected').text();
    $('#input-DonViSauQuyDoi').val(selectedText);
});


$(document).on('input', '#form_caidat input', function () {
    debounce(autoCalculate, 400)
});

const calcMap = {
    "SLNhapKhoCoThue": {
        func: SLNhapKhoCoThue,
        inputs: ["SLNhapKhoCoThue"]
    },

    "SoLuongThucCoThue": {
        func: SoLuongThucCoThue,
        inputs: ["SLNhapKhoCoThue"]
    },

    "SoLuongThucKhongThue": {
        func: SoLuongThucKhongThue,
        inputs: ["SLNhapKhoKhongThue"]
    },

    "TongSoLuong": {
        func: TongSoLuong,
        inputs: ["SoLuongThucCoThue", "SoLuongThucKhongThue"]
    },

    "ThanhTienCoThue": {
        func: ThanhTienCoThue,
        inputs: ["SoLuongThucCoThue", "DonGiaCoThue"]
    },

    "ThanhTienKhongThue": {
        func: ThanhTienKhongThue,
        inputs: ["SoLuongThucKhongThue", "DonGiaKhongThue"]
    },

    "TongThanhTien": {
        func: TongThanhTien,
        inputs: ["ThanhTienCoThue", "ThanhTienKhongThue"]
    },

    "ThanhTienCuocCoThue": {
        func: ThanhTienCuocCoThue,
        inputs: ["SoLuongThucCoThue", "DonGiaCuocCoThue"]
    },

    "ThanhTienCuocKhongThue": {
        func: ThanhTienCuocKhongThue,
        inputs: ["SoLuongThucKhongThue", "DonGiaCuocKhongThue"]
    },

    "TongTienHangCoThue": {
        func: TongTienHangCoThue,
        inputs: ["ThanhTienCoThue", "ThanhTienCuocCoThue"]
    },

    "TongTienHangKhongThue": {
        func: TongTienHangKhongThue,
        inputs: ["ThanhTienKhongThue", "ThanhTienCuocKhongThue"]
    },

    "TongTienHang": {
        func: TongTienHang,
        inputs: ["TongTienHangCoThue", "TongTienHangKhongThue"]
    }
};

// Hàm tổng quát để gọi các phép tính và set lại giá trị
function autoCalculate() {
    try {
        Object.keys(calcMap).forEach(function (resultId) {
            const { func, inputs } = calcMap[resultId];
            const args = inputs.map(id => {
                const $el = $('#input-' + id);

                if ($el.is('select')) {
                    return $el.find(':selected').text();
                }

                const inputType = $el.attr('type');
                const val = $el.val();

                if (inputType === "text") {
                    return (val === null || typeof val === "undefined") ? "" : val;
                }

                if (val === "" || val === null || typeof val === "undefined") return 0;
                const num = parseFloat(val);
                return isNaN(num) ? 0 : num;
            });

            let result = func.apply(null, args);

            if (typeof result === "number" && isNaN(result)) result = "";
            $('#input-' + resultId).val(result);
        });
    } catch (err) {
        console.error('Lỗi tổng thể trong autoCalculate:', err);
    }
}

function debounce(fn, delay) {
    let timer = null;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), delay);
    };
}
function toDecimal(value) {
    return Math.round((Number(value) || 0) * 100) / 100;
}
function SLNhapKhoCoThue(SLNhapKhoCoThue) {
    return SLNhapKhoCoThue;
}

function SoLuongThucCoThue(SLNhapKhoCoThue) {
    return SLNhapKhoCoThue;
}

function SoLuongThucKhongThue(SLNhapKhoKhongThue) {
    return SLNhapKhoKhongThue;
}

function TongSoLuong(SoLuongThucCoThue, SoLuongThucKhongThue) {
    return toDecimal(SoLuongThucCoThue + SoLuongThucKhongThue);
}

function ThanhTienCoThue(SoLuongThucCoThue, DonGiaCoThue) {
    return toDecimal(SoLuongThucCoThue * DonGiaCoThue);
}

function ThanhTienKhongThue(SoLuongThucKhongThue, DonGiaKhongThue) {
    return toDecimal(SoLuongThucKhongThue * DonGiaKhongThue);
}

function TongThanhTien(ThanhTienCoThue, ThanhTienKhongThue) {
    return toDecimal(ThanhTienCoThue + ThanhTienKhongThue);
}

function ThanhTienCuocCoThue(SoLuongThucCoThue, DonGiaCuocCoThue) {
    return toDecimal(SoLuongThucCoThue * DonGiaCuocCoThue);
}

function ThanhTienCuocKhongThue(SoLuongThucKhongThue, DonGiaCuocKhongThue) {
    return toDecimal(SoLuongThucKhongThue * DonGiaCuocKhongThue);
}

function TongTienHangCoThue(ThanhTienCoThue, ThanhTienCuocCoThue) {
    return toDecimal(ThanhTienCoThue + ThanhTienCuocCoThue);
}

function TongTienHangKhongThue(ThanhTienKhongThue, ThanhTienCuocKhongThue) {
    return toDecimal(ThanhTienKhongThue + ThanhTienCuocKhongThue);
}

function TongTienHang(TongTienHangCoThue, TongTienHangKhongThue) {
    return toDecimal(TongTienHangCoThue + TongTienHangKhongThue);
}


function validateForm() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    let isValid = true;
    let firstInvalid = null;
    const requiredFields = [
        { selector: '#input-NgayNhapKho', message: 'Vui lòng nhâp Ngày nhập kho.' },
        { selector: '#input-ChotSoCoCay', message: 'Vui lòng nhâp Chốt số cơ cây dầu trước khi nhập nhiên liệu Diezel.' },
        { selector: '#input-SoPhieu', message: 'Vui lòng nhâp Số phiếu.' },
        { selector: '#input-BienSo', message: 'Vui lòng nhâp Biển số xe.' },
    ];

    requiredFields.forEach(f => {
        const $input = $(f.selector);
        if (!$input.val() || !$input.val().trim()) {
            showError(f.selector, f.message);
            if (!firstInvalid) firstInvalid = $input[0];
            isValid = false;
        }
    });

    if (!isValid && firstInvalid) {
        firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
        const $el = $(firstInvalid);
        if ($el.hasClass('select2-hidden-accessible')) {
            $el.next('.select2-container').find('.select2-selection').focus();
        } else {
            $el.focus();
        }
    }
    return isValid;
}

function showError(selector, message) {
    const $input = $(selector);
    $input.addClass('is-invalid');
    if ($input.next('.invalid-feedback').length === 0) {
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }
}
async function chiTiet(id) {
    const resUser = await getJwtToken();
    const token = resUser.token;

    try {
        const res = await $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/GetHistory?id=${id}`,
            method: "GET",
            headers: { Authorization: `Bearer ${token}` }
        });

        if (res.success && Array.isArray(res.data) && res.data.length > 0) {
            let tableRows = res.data.map((item, idx) => `
                    <tr>
                        <td class="text-center">${idx + 1}</td>
                        <td>${item.id_TramTron || ''}</td>
                        <td>${item.ngayNhapKho || ''}</td>
                        <td>${item.chotSoCoCay || ''}</td>
                        <td>${item.soPhieu || ''}</td>
                        <td>${item.bienSo || ''}</td>
                        <td>${item.soHopDong || ''}</td>
                        <td>${item.id_NhomNhienLieu || ''}</td>
                        <td>${item.id_LoaiNhienLieu || ''}</td>
                        <td>${item.id_NhanHieu || ''}</td>
                        <td>${item.id_DonVi || ''}</td>
                        <td>${item.slNhapKhoCoThue || ''}</td>
                        <td>${item.slNhapKhoKhongThue || ''}</td>
                        <td>${item.donViSauQuyDoi || ''}</td>
                        <td>${item.soLuongThucCoThue || ''}</td>
                        <td>${item.soLuongThucKhongThue || ''}</td>
                        <td>${item.tongSoLuong || ''}</td>
                        <td>${item.donGiaCoThue || ''}</td>
                        <td>${item.donGiaKhongThue || ''}</td>
                        <td>${item.thanhTienCoThue || ''}</td>
                        <td>${item.thanhTienKhongThue || ''}</td>
                        <td>${item.tongThanhTien || ''}</td>
                        <td>${item.bienSoXe || ''}</td>
                        <td>${item.donGiaCuocCoThue || ''}</td>
                        <td>${item.donGiaCuocKhongThue || ''}</td>
                        <td>${item.thanhTienCuocCoThue || ''}</td>
                        <td>${item.thanhTienCuocKhongThue || ''}</td>
                        <td>${item.tongTienHangCoThue || ''}</td>
                        <td>${item.tongTienHangKhongThue || ''}</td>
                        <td>${item.tongTienHang || ''}</td>
                        <td>${item.groupId || ''}</td>
                        <td>${item.ordinarily || ''}</td>
                        <td>${item.createAt || ''}</td>
                        <td>${item.createBy || ''}</td>
                        <td>${item.isActive || ''}</td>
                        <td>${item.approvalUserId || ''}</td>
                        <td>${item.dateApproval || ''}</td>
                        <td>${item.approvalDept || ''}</td>
                        <td>${item.departmentId || ''}</td>
                        <td>${item.departmentOrder || ''}</td>
                        <td>${item.approvalOrder || ''}</td>
                        <td>${item.approvalId || ''}</td>
                        <td>${item.lastApprovalId || ''}</td>
                        <td>${item.isStatus || ''}</td>
                        <td>${item.createBy || ''}</td>
                        <td>${item.createAt ? new Date(item.createAt).toLocaleString() : ''}</td>
                    </tr>`).join('');

            const bodyHtml = `
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover no-wrap-table">
                            <thead class="thead-light">
                                <tr>
                                    <th class="text-center">#</th>
                                    <th> Tên trạm trộn</th>
                                    <th> Ngày nhập kho</th>
                                    <th> Chốt số cơ cây dầu trước khi nhập nhiên liệu Diezel</th>
                                    <th> Số phiếu</th>
                                    <th> Biển số xe</th>
                                    <th> Số HĐ</th>
                                    <th> Nhóm nhiên liệu</th>
                                    <th> Loại nhiên liệu</th>
                                    <th> Nhãn hiệu</th>
                                    <th> Đơn vị</th>
                                    <th> Số lượng nhập kho có thuế</th>
                                    <th> Số lượng nhập kho không thuế</th>
                                    <th> Đơn vị sau quy đổi</th>
                                    <th> Số lượng thực tế có thuế</th>
                                    <th> Số lượng thực tế không thuế</th>
                                    <th> Tổng số lượng</th>
                                    <th> Đơn giá có thuế</th>
                                    <th> Đơn giá không thuế</th>
                                    <th> Thành tiền có thuế</th>
                                    <th> Thành tiền không thuế</th>
                                    <th> Tổng thành tiền</th>
                                    <th> Biển số xe (vận chuyển)</th>
                                    <th> Đơn giá cước vận chuyển có thuế</th>
                                    <th> Đơn giá cước vận chuyển không thuế</th>
                                    <th> Thành tiền cước vận chuyển có thuế</th>
                                    <th> Thành tiền cước vận chuyển không thuế</th>
                                    <th> Tổng tiền hàng có thuế</th>
                                    <th> Tổng tiền hàng không thuế</th>
                                    <th> Tổng tiền hàng</th>
                                    <th> GroupId</th>
                                    <th> Thứ tự</th>
                                    <th> Ngày tạo</th>
                                    <th> Người tạo</th>
                                    <th> IsActive</th>
                                    <th> Người duyệt</th>
                                    <th> Ngày duyệt</th>
                                    <th> Phòng ban duyệt</th>
                                    <th> Phòng ban duyệt tiếp</th>
                                    <th> Thứ tự phòng ban</th>
                                    <th> Thứ tự duyệt</th>
                                    <th> ID duyệt</th>
                                    <th> Id duyệt cuối</th>
                                    <th> Trạng thái</th>
                                    <th>Nguoi thao tác</th>
                                    <th>Thời gian</th>
                                </tr>
                            </thead>
                            <tbody>${tableRows}</tbody>
                        </table>
                    </div>`;

            showGlobalModal("Lịch sử", bodyHtml, `<button class="btn btn-secondary" data-dismiss="modal">Ðóng</button>`);
        } else {
            showGlobalModal("Lịch sử", `<div class="text-center text-muted">Không có lịch sử</div>`, `<button class="btn btn-secondary" data-dismiss="modal">Ðóng</button>`);
        }
    } catch (err) {
        showGlobalModal("Lịch sử", `<div class="text-center text-danger">Lỗi khi tải dữ liệu lịch sử</div>`, `<button class="btn btn-secondary" data-dismiss="modal">Ðóng</button>`);
    }
}
async function duyet(id) {
    if (!confirm('Bạn có chắc chắn muốn duyệt ')) return;
    const resUser = await getJwtToken();
    const token = resUser.token;
    try {
        const res = await $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/Duyet?id=${id}`,
            method: "POST",
            headers: { Authorization: `Bearer ${token}` }
        });
        if (res.success) {
            showToastSuccess("Duyệt thành công !");
            loadData();
        } else {
            showToastError(res.message || "Duyệt thất bại");
        }
    } catch (err) {
        showToastError("Lỗi hệ thống khi duyệt");
    }
}
async function huyDuyet(id) {
    if (!confirm('Bạn có chắc chắn muốn hủy duyẹt !')) return;
    const resUser = await getJwtToken();
    const token = resUser.token;
    try {
        const res = await $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/HuyDuyet?id=${id}`,
            method: "POST",
            headers: { Authorization: `Bearer ${token}` }
        });
        if (res.success) {
            showToastSuccess("Hủy duyệt thành công");
            loadData();
        } else {
            showToastError(res.message || "Hủy duyệt thất bại");
        }
    } catch (err) {
        showToastError("Lỗi hệ thống khi hủy duyệt");
    }
}
async function editCaiDat(id) {
    const resUser = await getJwtToken();
    const token = resUser.token;
    try {
        const res = await $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/GetById?id=${id}`,
            method: "GET",
            headers: { Authorization: `Bearer ${token}` }
        });
        if (res.success && res.data) {
            showModalTaoMoi(true, res.data);
        } else {
            showToastError(res.message || "Không lấy được dữ liệu");
        }
    } catch (err) {
        showToastError("Lỗi hệ thống khi chỉnh sửa ");
    }
}
async function deleteCaiDat(id) {
    if (!confirm('Bạn chác chắn muốn xóa ?')) return;
    const resUser = await getJwtToken();
    const token = resUser.token;
    try {
        const res = await $.ajax({
            url: `/api/KhoNhapkhoNhienLieu/${id}`,
            method: "DELETE",
            headers: { Authorization: `Bearer ${token}` }
        });
        if (res.success) {
            showToastSuccess("Xóa thành công !");
            loadData();
        } else {
            showToastError(res.message || "Xóa thất bại");
        }
    } catch (err) {
        showToastError("Lỗi hệ thống khi xóa");
    }
}
function showHuongDanNghiepVu() {
    const headerHtml =
        `<div><div class="card-header">
                <i class="header-icon lnr-license icon-gradient bg-plum-plate"></i>
                <form id="SearchInfo">
                    <select id="ChiNhanhInfo" class="form-control">
                        <option value="">-- Chọn danh mục --</option>
                    </select>
                </form>
                <div class="btn-actions-pane-right">
                    <div role="group" class="btn-group-sm nav btn-group">
                        <a data-toggle="tab" href="#tab-hdsd" class="btn-shadow active btn btn-primary">Hướng dẫn sử dụng </a>
                        <a data-toggle="tab" href="#tab-ttd" class="btn-shadow  btn btn-primary">Thứ tự duyệt</a>
                    </div>
                </div>
            </div>`;

    const tabsHtml = `
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane tabs-animation fade active show" id="tab-hdsd" role="tabpanel">
                        <div class="p-3">Nội dung hướng dẫn sư dụng... </div>
                    </div>
                    <div class="tab-pane tabs-animation fade" id="tab-ttd" role="tabpanel">
                        <div class="row">
                            <div class="col-md-12">
                                <div>
                                    <div class="mb-3">
                                        <div role="group" class="btn-group-sm nav btn-group">
                                            <a data-toggle="tab" href="#tab-them" class="btn-pill pl-3 btn btn-warning active show">Thêm</a>
                                            <a data-toggle="tab" href="#tab-sua" class="btn btn-warning show">S?a</a>
                                            <a data-toggle="tab" href="#tab-xoa" class="btn-pill pr-3 btn btn-warning show">Xóa</a>
                                        </div>
                                    </div>
                                    <div class="tab-content">
                                        <div class="tab-pane show active" id="tab-them" role="tabpanel">
                                            <div class="row">
                                                <div class="col-md-4">
                                                    <ul class="list-group" id="ul-them"></ul>
                                                </div>
                                                <div class="col-md-8">
                                                    <div class="card-body" id="content-them">Chọn phòng ban để xem chi tiết</div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane show" id="tab-sua" role="tabpanel">
                                            <div class="row">
                                                <div class="col-md-4">
                                                    <ul class="list-group" id="ul-sua"></ul>
                                                </div>
                                                <div class="col-md-8">
                                                    <div class="card-body" id="content-sua">Chọn phòng ban để xem chi tiết</div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane show" id="tab-xoa" role="tabpanel">
                                            <div class="row">
                                                <div class="col-md-4">
                                                    <ul class="list-group" id="ul-xoa"></ul>
                                                </div>
                                                <div class="col-md-8">
                                                    <div class="card-body" id="content-xoa">Chọn phòng ban để xem chi tiết</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    const bodyHtml = headerHtml + tabsHtml;
    const footerHtml = `<button type="button" class="btn btn-secondary" data-dismiss="modal">Ðóng</button>`;
    showGlobalModal("Thông tin nghi?p v?", bodyHtml, footerHtml);

    getJwtToken().then(function (resUser) {
        const groupId = resUser.userInfor.groupId;
        $.ajax({
            url: `/api/caidatsoluotduyet/GetChiNhanhsForCompanyId?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            const $ddl = $('#ChiNhanhInfo');
            $ddl.empty().append('<option value="">--- Chọn danh mục ---</option>');
            if (res.success && Array.isArray(res.data)) {
                res.data.forEach(function (item) {
                    $ddl.append(`<option value="${item.id}">${item.tenChiNhanh}</option>`);
                });
            }
        }).fail(function (err) {
            $('#ChiNhanhInfo').empty().append('<option value="">--- Lỗi tải danh mục ---</option>');
        });
        $.ajax({
            url: `/api/caidatphongbanduyet/ListDept?groupId=${groupId}`,
            method: "GET",
            headers: {
                "Authorization": "Bearer " + resUser.token
            }
        }).done(function (res) {
            let htmlUl = "";
            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                var stt = 1;
                res.data.forEach(function (item, idx) {
                    htmlUl += `<li class="list-group-item" data-idx="${idx}" style="cursor:pointer;">
                            <b>${stt + '.' + item.deptName}</b><br/>
                            <span>Trạng thái : : ${item.isActive ? "Hoạt động" : "--- Ngưng ---"}</span>
                        </li>`;
                    stt++;
                });
            } else {
                htmlUl = `<li class="list-group-item text-muted">không có dữ liêụ phòng ban.</li>`;
            }
            $('#ul-them').html(htmlUl);
            $('#ul-sua').html(htmlUl);
            $('#ul-xoa').html(htmlUl);

            function bindDeptClick(tab, data, groupId) {
                const permissionMap = {
                    'them': '5fe86889-5019-4cca-bec7-6a8baad3ed14',
                    'sua': '3488c9e9-3542-4340-b831-61a1621d73d7',
                    'xoa': 'ef236725-edd1-43ae-95b1-5d6fcdf41829'
                };
                $(`#ul-${tab}`).off('click').on('click', 'li', function () {
                    const idx = $(this).data('idx');
                    const dept = data[idx];
                    let content = '';
                    if (dept) {
                        $.ajax({
                            url: `/api/caidatsoluotduyet/ListStepSetting?groupId=${groupId}&majorId=523f812e-5bf5-4784-bbf4-d986816796d1&departmentId=${dept.id || ''}&permissionId=${permissionMap[tab]}`,
                            method: "GET",
                            headers: {
                                "Authorization": "Bearer " + resUser.token
                            }
                        }).done(function (res) {
                            if (res.success && Array.isArray(res.data) && res.data.length > 0) {
                                let stepsHtml = `<h5>${dept.deptName}</h5>`;
                                stepsHtml += `<ul class="list-group mb-2">`;
                                res.data.forEach((step, i) => {
                                    stepsHtml += `<li class="list-group-item">
                                            <b>Bu?c ${i + 1}:</b> ${step.content || ''}
                                            <span class="badge badge-info ml-2">${step.isStatus || ''}</span>
                                        </li>`;
                                });
                                stepsHtml += `</ul>`;
                                content = stepsHtml;
                            } else {
                                content = `<div class="text-muted">không có dữ liêụ duyệt.</div>`;
                            }
                            $(`#content-${tab}`).html(content);
                        }).fail(function (err) {
                            $(`#content-${tab}`).html('<div class="text-danger">Lỗi khi tải dữ liêụ duyệt.</div>');
                        });
                    } else {
                        $(`#content-${tab}`).html("không có dữ liêụ chi tiết.");
                    }
                    $(`#ul-${tab} li`).removeClass('active');
                    $(this).addClass('active');
                });
            }
            bindDeptClick('them', res.data, groupId);
            bindDeptClick('sua', res.data, groupId);
            bindDeptClick('xoa', res.data, groupId);
        }).fail(function (err) {
            $('#ul-them, #ul-sua, #ul-xoa').html('<li class="list-group-item text-danger">Lỗi khi tải dữ liệu phòng ban.</li>');
            $('#content-them, #content-sua, #content-xoa').html('');
        });
    });
    if (window.$ && $.fn.select2) {
        $('#ChiNhanhInfo').select2();
    }
    $('#ChiNhanhInfo').off('change').on('change', function () {
        if (typeof onSelectChange === 'function') {
            onSelectChange($(this).val());
        }
    });
}

function copyTable(idTable) {
    var range = document.createRange();
    range.selectNode(document.getElementById(idTable));
    window.getSelection().removeAllRanges();
    window.getSelection().addRange(range);
    try {
        var successful = document.execCommand('copy');
        alert(successful ? 'Đã copy vào clipboard!' : 'Copy thất bại!');
    } catch (err) {
        alert('Trình duyệt không hỗ trợ copy bằng document.execCommand.\nBạn có thể thử thủ công (Ctrl+C).');
    }
    window.getSelection().removeAllRanges();
}


async function exportExcel() {
    const type = document.getElementById("reportType").value;
    const filterTramTron = document.getElementById("filterTramTron").value;

    switch (type) {
        case "nknl_ncc_ct":
            await exportExcelBCNKVLtheoNCCCT("table_data", "BCNKVLtheoNCCCT.xlsx");
            break;
        case "nknl_ncc_th":
            await exportExcelBCNKVLtheoNCCTH("table_data", "BCNKVLtheoNCCTH.xlsx", filterTramTron);
            break;
        case "nknl_ncc_cuoc_ct":
            BCNKVLtheoNCCCuocCongTyCT1("table_data", "BCNKVLtheoNCCCuocCongTyCT.xlsx", filterTramTron);
            break;
        case "nknl_ncc_cuoc_th":
            await exportExcelBCNKVLtheoNCCCuocCongTyTH("table_data", "BCNKVLtheoNCCCuocCongTyTH.xlsx", filterTramTron);
            break;
        case "nknl_loai_ct":
            await exportExcelBCNKVLtheoLoaiVatLieuCT("table_data", "BCNKVLtheoLoaiVatLieuCT.xlsx", filterTramTron);
            break;
        case "nknl_loai_th":
            await exportExcelBCNKVLtheoLoaiVatLieuTH("table_data", "BCNKVLtheoLoaiVatLieuTH.xlsx", filterTramTron);
            break;
        case "nknl_caybom":
            await exportExcelBCNKVLtheoCayBom("table_data", "BCNKVLtheoCayBom.xlsx", filterTramTron);
            break;

        default:
            alert("Vui lòng chọn loại báo cáo");
    }
    //await exportExcelBCNKVLtheoNCCCT("table_data", "BCNKVLtheoNCCCT.xlsx");
    //await exportExcelBCNKVLtheoNCCTH("table_data", "BCNKVLtheoNCCTH.xlsx","Tất cả");
    //await BCNKVLtheoNCCCuocCongTyCT("table_data", "BCNKVLtheoNCCCuocCongTyCT.xlsx", "Tất cả");
    //await exportExcelBCNKVLtheoNCCCuocCongTyTH("table_data", "BCNKVLtheoNCCCuocCongTyTH.xlsx", "Tất cả");
    //await exportExcelBCNKVLtheoLoaiVatLieuCT("table_data", "BCNKVLtheoLoaiVatLieuCT.xlsx", "Tất cả");
    //await exportExcelBCNKVLtheoLoaiVatLieuTH("table_data", "BCNKVLtheoLoaiVatLieuTH.xlsx", "Tất cả");
    //await exportExcelBCNKVLtheoCayBom("table_data", "BCNKVLtheoCayBom.xlsx", "Tất cả");
    
}

// NKNL theo số cơ cây nhiên liệu
async function exportExcelBCNKVLtheoCayBom(tableId, fileName, chiNhanh) {
    const table = document.getElementById(tableId);
    if (!table) return;

    // 1. Đọc dữ liệu từ bảng HTML (giống VBA: B13:F..., M13:N..., R13:U...)
    const trs = Array.from(table.querySelectorAll("tbody tr"));
    let rows = [];
    for (let i = 0; i < trs.length; i++) {
        const tds = Array.from(trs[i].querySelectorAll("td"));
        // Kiểm tra các cột chính có dữ liệu (B, C, D tương ứng tds[1], tds[2], tds[3])
        if (
            !tds[1] || !tds[2] || !tds[3] ||
            !tds[1].innerText.trim() || !tds[2].innerText.trim() || !tds[3].innerText.trim()
        ) break;

        rows.push({
            chiNhanh: tds[2]?.innerText.trim(),
            ngayNhapKho: tds[2]?.innerText.trim(),
            chotSoCoCay: tds[3]?.innerText.trim(),
            soPhieu: tds[5]?.innerText.trim(),
            bienSo: tds[6]?.innerText.trim(),
            loaiNhienLieu: tds[14]?.innerText.trim(),
            nhanHieu: tds[15]?.innerText.trim(),
            donViSauQuyDoi: tds[19]?.innerText.trim(),
            slNhapKhoCoThue: parseFloat(tds[20]?.innerText.replace(/,/g, "")) || 0,
            slNhapKhoKhongThue: parseFloat(tds[21]?.innerText.replace(/,/g, "")) || 0,
            tongSoLuong: parseFloat(tds[22]?.innerText.replace(/,/g, "")) || 0,
            soNhienLieuDaBom: parseFloat(tds[25]?.innerText.replace(/,/g, "")) || 0,
            soCoLyThuyet: parseFloat(tds[26]?.innerText.replace(/,/g, "")) || 0,
            soCoThucTe: parseFloat(tds[29]?.innerText.replace(/,/g, "")) || 0,
        });
    }

    // 2. Sắp xếp theo chi nhánh, ngày nhập kho, biển số xe (giống VBA)
    rows.sort((a, b) => {
        if (a.chiNhanh !== b.chiNhanh) return a.chiNhanh.localeCompare(b.chiNhanh);
        if (a.ngayNhapKho !== b.ngayNhapKho) return new Date(a.ngayNhapKho) - new Date(b.ngayNhapKho);
        return a.bienSo.localeCompare(b.bienSo);
    });

    // 3. Gom nhóm theo chi nhánh, xuất sheet cho từng chi nhánh
    const chiNhanhMap = {};
    rows.forEach(r => {
        let key = r.chiNhanh || "Tất cả";
        if (!chiNhanhMap[key]) chiNhanhMap[key] = [];
        chiNhanhMap[key].push(r);
    });

    // 4. Tạo workbook và sheet
    const workbook = new ExcelJS.Workbook();
    workbook.creator = "Ứng dụng";

    for (const chiNhanhKey in chiNhanhMap) {
        let sheetName = chiNhanhKey.replace(/[\\/?*\[\]]/g, "");
        if (sheetName.length > 31) sheetName = sheetName.substring(0, 26) + Math.floor(Math.random() * 1000);
        const ws = workbook.addWorksheet(sheetName);

        // Header
        ws.mergeCells("A1:F1");
        ws.getCell("A1").value = "CÔNG TY CPXD ĐỨC ANH";
        ws.getCell("A1").font = { bold: true, size: 11 };
        ws.mergeCells("A2:F2");
        ws.getCell("A2").value = chiNhanhKey;
        ws.getCell("A2").font = { bold: true, size: 11 };
        ws.mergeCells("A3:Q4");
        ws.getCell("A3").value = "BẢNG THEO DÕI CÂY BƠM NHIÊN LIỆU";
        ws.getCell("A3").font = { bold: true, size: 14 };

        // Header dòng 6-7
        ws.mergeCells("A6:A7"); ws.getCell("A6").value = "STT";
        ws.mergeCells("B6:B7"); ws.getCell("B6").value = "Chi nhánh";
        ws.mergeCells("C6:C7"); ws.getCell("C6").value = "Ngày nhập kho";
        ws.mergeCells("D6:D7"); ws.getCell("D6").value = "Chốt số cơ cây dầu trước khi nhập nhiên liệu Diezel";
        ws.mergeCells("E6:E7"); ws.getCell("E6").value = "Số phiếu";
        ws.mergeCells("F6:F7"); ws.getCell("F6").value = "Biển số xe";
        ws.mergeCells("G6:H6"); ws.getCell("G6").value = "Thông tin nhiên liệu";
        ws.getCell("G7").value = "Loại nhiên liệu";
        ws.getCell("H7").value = "Nhãn hiệu";
        ws.mergeCells("I6:L6"); ws.getCell("I6").value = "Thông tin nhập kho sau qui đổi";
        ws.getCell("I7").value = "Đơn vị sau qui đổi";
        ws.getCell("J7").value = "Số lượng nhập kho có thuế";
        ws.getCell("K7").value = "Số lượng nhập kho không thuế";
        ws.getCell("L7").value = "Tổng số lượng nhập kho";
        ws.mergeCells("M6:Q6"); ws.getCell("M6").value = "So sánh số cơ cây bơm nhiên liệu";
        ws.getCell("M7").value = "Số nhiên liệu đã bơm cho các thiết bị trước khi nhập nhiên liệu tiếp theo";
        ws.getCell("N7").value = "Số cơ lý thuyết sau khi bơm nhiên liệu cho các thiết bị";
        ws.getCell("O7").value = "Số cơ thực tế trước khi nhập kho nhiên liệu";
        ws.getCell("P7").value = "Chênh (Số cơ hiện tại sau khi bơm cho các thiết bị - Số cơ thực tế nhập nhiên liệu tiếp theo)";
        ws.getCell("Q7").value = "Tồn lý thuyết trong téc";

        // Tô màu header
        function fillHeader(ws, r1, r2, c1, c2, color) {
            for (let r = r1; r <= r2; r++) {
                for (let c = c1; c <= c2; c++) {
                    const cell = ws.getCell(r, c);
                    cell.fill = {
                        type: "pattern",
                        pattern: "solid",
                        fgColor: { argb: color }
                    };
                    cell.alignment = { horizontal: "center", vertical: "middle", wrapText: true };
                    cell.font = { bold: true, name: "Times New Roman", size: 11 };
                }
            }
        }
        fillHeader(ws, 6, 7, 1, 17, "AEEFC2");

        // Ghi dữ liệu
        let rowIdx = 8, stt = 1;
        chiNhanhMap[chiNhanhKey].forEach(d => {
            ws.getCell(`A${rowIdx}`).value = stt++;
            ws.getCell(`B${rowIdx}`).value = d.chiNhanh;
            ws.getCell(`C${rowIdx}`).value = d.ngayNhapKho;
            ws.getCell(`D${rowIdx}`).value = d.chotSoCoCay;
            ws.getCell(`E${rowIdx}`).value = d.soPhieu;
            ws.getCell(`F${rowIdx}`).value = d.bienSo;
            ws.getCell(`G${rowIdx}`).value = d.loaiNhienLieu;
            ws.getCell(`H${rowIdx}`).value = d.nhanHieu;
            ws.getCell(`I${rowIdx}`).value = d.donViSauQuyDoi;
            ws.getCell(`J${rowIdx}`).value = d.slNhapKhoCoThue;
            ws.getCell(`K${rowIdx}`).value = d.slNhapKhoKhongThue;
            ws.getCell(`L${rowIdx}`).value = d.tongSoLuong;
            ws.getCell(`M${rowIdx}`).value = d.soNhienLieuDaBom;
            ws.getCell(`N${rowIdx}`).value = d.soCoLyThuyet;
            ws.getCell(`O${rowIdx}`).value = d.soCoThucTe;
            ws.getCell(`P${rowIdx}`).value = { formula: `=N${rowIdx}-O${rowIdx}` };
            ws.getCell(`Q${rowIdx}`).value = { formula: `=L${rowIdx}-M${rowIdx}` };
            rowIdx++;
        });

        // Tổng cộng
        ws.getCell(`A${rowIdx}`).value = "Tổng cộng";
        ws.mergeCells(`A${rowIdx}:H${rowIdx}`);
        ["J", "K", "L", "P", "Q"].forEach(col => {
            ws.getCell(`${col}${rowIdx}`).value = { formula: `SUBTOTAL(9,${col}8:${col}${rowIdx - 1})` };
        });
        ws.getCell(`A${rowIdx}`).font = { bold: true };

        // Định dạng, border, số
        function applyBorder(ws, sr, er, sc, ec) {
            for (let r = sr; r <= er; r++) {
                for (let c = sc; c <= ec; c++) {
                    ws.getCell(r, c).border = {
                        top: { style: "thin" },
                        left: { style: "thin" },
                        bottom: { style: "thin" },
                        right: { style: "thin" }
                    };
                }
            }
        }
        applyBorder(ws, 6, rowIdx, 1, 17);

        // Định dạng số
        for (let r = 8; r <= rowIdx; r++) {
            ["J", "K", "L", "M", "N", "O", "P", "Q"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##0.00");
        }
        for (let i = 1; i <= 17; i++) ws.getColumn(i).width = 14;

        // Font toàn bộ sheet
        ws.eachRow(row => {
            row.eachCell(cell => {
                cell.font = { ...cell.font, name: "Times New Roman" };
            });
        });

        // Footer
        let footerStart = rowIdx + 2;
        ws.mergeCells(`D${footerStart}:G${footerStart}`);
        ws.getCell(`D${footerStart}`).value = "Thủ kho trạm trộn";
        ws.getCell(`D${footerStart}`).alignment = { horizontal: "center" };

        ws.mergeCells(`H${footerStart}:J${footerStart + 1}`);
        ws.getCell(`H${footerStart}`).value = "Giám đốc trạm trộn";
        ws.getCell(`H${footerStart}`).alignment = { horizontal: "center" };

        ws.mergeCells(`K${footerStart}:N${footerStart}`);
        ws.getCell(`K${footerStart}`).value = "Kiểm soát nội bộ";
        ws.getCell(`K${footerStart}`).alignment = { horizontal: "center" };

        ws.mergeCells(`D${footerStart + 1}:E${footerStart + 1}`);
        ws.getCell(`D${footerStart + 1}`).value = "Cầu cân";
        ws.getCell(`D${footerStart + 1}`).alignment = { horizontal: "center" };

        ws.mergeCells(`F${footerStart + 1}:G${footerStart + 1}`);
        ws.getCell(`F${footerStart + 1}`).value = "Thủ kho";
        ws.getCell(`F${footerStart + 1}`).alignment = { horizontal: "center" };

        ws.mergeCells(`K${footerStart + 1}:L${footerStart + 1}`);
        ws.getCell(`K${footerStart + 1}`).value = "Nhân viên";
        ws.getCell(`K${footerStart + 1}`).alignment = { horizontal: "center" };

        ws.mergeCells(`M${footerStart + 1}:N${footerStart + 1}`);
        ws.getCell(`M${footerStart + 1}`).value = "Trưởng phòng";
        ws.getCell(`M${footerStart + 1}`).alignment = { horizontal: "center" };

        // In đậm, căn giữa, wrap cho footer
        let footerRange = [`A${rowIdx - 1}:Q${footerStart + 1}`];
        footerRange.forEach(range => {
            ws.getCell(range.split(":")[0]).font = { bold: true, name: "Times New Roman", size: 11 };
        });
    }

    // Xuất file
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName.endsWith(".xlsx") ? fileName : fileName + ".xlsx");
}

//NKNL theo loại nhiên liệu(Tổng hợp)
async function exportExcelBCNKVLtheoLoaiVatLieuTH(tableId, fileName, chiNhanh) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const workbook = new ExcelJS.Workbook();
    const ws = workbook.addWorksheet("KetQua");

    // 1. Đọc dữ liệu từ bảng HTML
    const trs = Array.from(table.querySelectorAll("tbody tr"));
    const data = [];
    for (let i = 0; i < trs.length; i++) {
        const tds = Array.from(trs[i].querySelectorAll("td"));
        // Lấy dữ liệu theo đúng cột VBA
        const ngay = tds[3]?.innerText.trim(); // dd/MM/yyyy
        const loai = tds[13]?.innerText.trim();
        const dvt = tds[18]?.innerText.trim();
        const slNhapKhoCoThue = parseFloat(tds[19]?.innerText.replace(/,/g, "")) || 0;
        const slNhapKhoKhongThue = parseFloat(tds[20]?.innerText.replace(/,/g, "")) || 0;
        const tongSoLuong = parseFloat(tds[21]?.innerText.replace(/,/g, "")) || 0;
        const thanhTienCoThue = parseFloat(tds[32]?.innerText.replace(/,/g, "")) || 0;
        const thanhTienKhongThue = parseFloat(tds[33]?.innerText.replace(/,/g, "")) || 0;
        const tongTien = parseFloat(tds[34]?.innerText.replace(/,/g, "")) || 0;

        // Parse tháng-năm từ ngày (dd/MM/yyyy)
        let month = "", year = "";
        if (ngay) {
            const parts = ngay.split("/");
            if (parts.length === 3) {
                month = parseInt(parts[1], 10);
                year = parseInt(parts[2], 10);
            }
        }

        data.push({
            ngay,
            month,
            year,
            loai,
            dvt,
            slNhapKhoCoThue,
            slNhapKhoKhongThue,
            tongSoLuong,
            thanhTienCoThue,
            thanhTienKhongThue,
            tongTien
        });
    }

    // 2. Nhóm dữ liệu theo loại nhiên liệu + tháng-năm
    const groupMap = new Map();
    for (const row of data) {
        const key = `${row.loai}|${row.month}|${row.year}`;
        if (!groupMap.has(key)) {
            groupMap.set(key, {
                ngay: row.ngay,
                loai: row.loai,
                dvt: row.dvt,
                month: row.month,
                year: row.year,
                slNhapKhoCoThue: 0,
                slNhapKhoKhongThue: 0,
                tongSoLuong: 0,
                thanhTienCoThue: 0,
                thanhTienKhongThue: 0,
                tongTien: 0
            });
        }
        const g = groupMap.get(key);
        g.slNhapKhoCoThue += row.slNhapKhoCoThue;
        g.slNhapKhoKhongThue += row.slNhapKhoKhongThue;
        g.tongSoLuong += row.tongSoLuong;
        g.thanhTienCoThue += row.thanhTienCoThue;
        g.thanhTienKhongThue += row.thanhTienKhongThue;
        g.tongTien += row.tongTien;
        // Lấy ngày đầu tiên, dvt đầu tiên cho nhóm
        if (!g.ngay) g.ngay = row.ngay;
        if (!g.dvt) g.dvt = row.dvt;
    }
    // Sắp xếp theo loại, năm, tháng
    const grouped = Array.from(groupMap.values()).sort((a, b) => {
        if (a.loai !== b.loai) return a.loai.localeCompare(b.loai);
        if (a.year !== b.year) return a.year - b.year;
        if (a.month !== b.month) return a.month - b.month;
        return 0;
    });

    // 3. Header
    ws.mergeCells("A1:D1");
    ws.getCell("A1").value = "CÔNG TY CPXD ĐỨC ANH";
    ws.getCell("A1").font = { bold: true, size: 11 };
    ws.mergeCells("A2:D2");
    ws.getCell("A2").value = chiNhanh;
    ws.getCell("A2").font = { bold: true, size: 11 };
    ws.mergeCells("A3:L3");
    ws.getCell("A3").value = "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO LOẠI NHIÊN LIỆU NĂM 2020";
    ws.getCell("A3").font = { bold: true, size: 14 };

    // Header dòng 6-7
    ws.mergeCells("A6:A7"); ws.getCell("A6").value = "STT";
    ws.mergeCells("B6:B7"); ws.getCell("B6").value = "Ngày nhập kho";
    ws.mergeCells("C6:C7"); ws.getCell("C6").value = "Loại nhiên liệu";
    ws.mergeCells("D6:G6"); ws.getCell("D6").value = "Thông tin nhập kho sau qui đổi";
    ws.getCell("D7").value = "Đơn vị sau qui đổi";
    ws.getCell("E7").value = "Số lượng nhập kho có thuế";
    ws.getCell("F7").value = "Số lượng nhập kho không thuế";
    ws.getCell("G7").value = "Tổng số lượng nhập kho";
    ws.mergeCells("H6:J6"); ws.getCell("H6").value = "Tổng tiền hàng";
    ws.getCell("H7").value = "Có thuế";
    ws.getCell("I7").value = "Không thuế";
    ws.getCell("J7").value = "Tổng tiền";
    ws.mergeCells("K6:L6"); ws.getCell("K6").value = "Đơn giá trung bình";
    ws.getCell("K7").value = "Có thuế";
    ws.getCell("L7").value = "Không thuế";

    // Tô màu header
    function fillHeader(ws, r1, r2, c1, c2, color) {
        for (let r = r1; r <= r2; r++) {
            for (let c = c1; c <= c2; c++) {
                const cell = ws.getCell(r, c);
                cell.fill = {
                    type: "pattern",
                    pattern: "solid",
                    fgColor: { argb: color }
                };
                cell.alignment = { horizontal: "center", vertical: "middle", wrapText: true };
                cell.font = { bold: true, name: "Times New Roman", size: 11 };
            }
        }
    }
    fillHeader(ws, 6, 7, 1, 12, "AEEFC2");

    // 4. Ghi dữ liệu
    let rowIdx = 8, stt = 1;
    for (const d of grouped) {
        ws.getCell(`A${rowIdx}`).value = stt++;
        // Xuất ngày dạng mm-yyyy
        if (d.ngay) {
            const parts = d.ngay.split("/");
            if (parts.length === 3) {
                ws.getCell(`B${rowIdx}`).value = `${parts[1]}-${parts[2]}`;
            } else {
                ws.getCell(`B${rowIdx}`).value = d.ngay;
            }
        } else {
            ws.getCell(`B${rowIdx}`).value = "";
        }
        ws.getCell(`C${rowIdx}`).value = d.loai;
        ws.getCell(`D${rowIdx}`).value = d.dvt;
        ws.getCell(`E${rowIdx}`).value = d.slNhapKhoCoThue;
        ws.getCell(`F${rowIdx}`).value = d.slNhapKhoKhongThue;
        ws.getCell(`G${rowIdx}`).value = d.tongSoLuong;
        ws.getCell(`H${rowIdx}`).value = d.thanhTienCoThue;
        ws.getCell(`I${rowIdx}`).value = d.thanhTienKhongThue;
        ws.getCell(`J${rowIdx}`).value = d.tongTien;
        ws.getCell(`K${rowIdx}`).value = { formula: `IFERROR(H${rowIdx}/E${rowIdx},0)` };
        ws.getCell(`L${rowIdx}`).value = { formula: `IFERROR(I${rowIdx}/F${rowIdx},0)` };
        rowIdx++;
    }
    const lastRow = rowIdx - 1;

    // 5. Subtotal
    ws.getCell(`A${rowIdx}`).value = "Tổng cộng";
    ws.mergeCells(`A${rowIdx}:D${rowIdx}`);
    ["E", "F", "G", "H", "I", "J"].forEach(col => {
        ws.getCell(`${col}${rowIdx}`).value = { formula: `SUBTOTAL(9,${col}8:${col}${lastRow})` };
    });
    ws.getCell(`K${rowIdx}`).value = { formula: `IFERROR(H${rowIdx}/E${rowIdx},0)` };
    ws.getCell(`L${rowIdx}`).value = { formula: `IFERROR(I${rowIdx}/F${rowIdx},0)` };
    ws.getCell(`A${rowIdx}`).font = { bold: true };

    // 6. Định dạng, border, footer
    function applyBorder(ws, sr, er, sc, ec) {
        for (let r = sr; r <= er; r++) {
            for (let c = sc; c <= ec; c++) {
                ws.getCell(r, c).border = {
                    top: { style: "thin" },
                    left: { style: "thin" },
                    bottom: { style: "thin" },
                    right: { style: "thin" }
                };
            }
        }
    }
    applyBorder(ws, 6, rowIdx, 1, 12);

    // Footer
    let footerStart = rowIdx + 2;
    ws.mergeCells(`D${footerStart}:L${footerStart}`);
    ws.getCell(`D${footerStart}`).value = "Kiểm soát nội bộ";
    ws.getCell(`D${footerStart}`).alignment = { horizontal: "center" };
    ws.getCell(`D${footerStart}`).font = { bold: true, name: "Times New Roman", size: 11 };

    ws.mergeCells(`D${footerStart + 1}:H${footerStart + 1}`);
    ws.getCell(`D${footerStart + 1}`).value = "Nhân viên";
    ws.getCell(`D${footerStart + 1}`).alignment = { horizontal: "center" };

    ws.mergeCells(`I${footerStart + 1}:L${footerStart + 1}`);
    ws.getCell(`I${footerStart + 1}`).value = "Trưởng phòng";
    ws.getCell(`I${footerStart + 1}`).alignment = { horizontal: "center" };

    // Định dạng số
    for (let r = 8; r <= rowIdx; r++) {
        ["E", "F", "G"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##0.00");
        ["H", "I", "J", "K", "L"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##");
    }
    for (let i = 1; i <= 12; i++) ws.getColumn(i).width = 14;

    // Font toàn bộ sheet
    ws.eachRow(row => {
        row.eachCell(cell => {
            cell.font = { ...cell.font, name: "Times New Roman" };
        });
    });

    // Xuất file
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName.endsWith(".xlsx") ? fileName : fileName + ".xlsx");
}
//NKNL theo loại nhiên liệu(Chi tiết)
async function exportExcelBCNKVLtheoLoaiVatLieuCT(tableId, fileName, chiNhanh) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const workbook = new ExcelJS.Workbook();

    // 1. Đọc dữ liệu từ bảng HTML
    const trs = Array.from(table.querySelectorAll("tbody tr"));
    const data = [];
    for (let i = 0; i < trs.length; i++) {
        const tds = Array.from(trs[i].querySelectorAll("td"));
        // Lấy dữ liệu theo đúng cột VBA
        const ngay = tds[3]?.innerText.trim(); // dd/MM/yyyy
        const loai = tds[13]?.innerText.trim();
        const dvt = tds[18]?.innerText.trim();
        const slNhapKhoCoThue = parseFloat(tds[19]?.innerText.replace(/,/g, "")) || 0;
        const slNhapKhoKhongThue = parseFloat(tds[20]?.innerText.replace(/,/g, "")) || 0;
        const tongSoLuong = parseFloat(tds[21]?.innerText.replace(/,/g, "")) || 0;
        const thanhTienCoThue = parseFloat(tds[24]?.innerText.replace(/,/g, "")) || 0;
        const thanhTienKhongThue = parseFloat(tds[25]?.innerText.replace(/,/g, "")) || 0;
        const cuocCoThue = parseFloat(tds[30]?.innerText.replace(/,/g, "")) || 0;
        const cuocKhongThue = parseFloat(tds[31]?.innerText.replace(/,/g, "")) || 0;
        const tongTienHangCoThue = parseFloat(tds[32]?.innerText.replace(/,/g, "")) || 0;
        const tongTienHangKhongThue = parseFloat(tds[33]?.innerText.replace(/,/g, "")) || 0;
        const tongTienHang = parseFloat(tds[34]?.innerText.replace(/,/g, "")) || 0;

        data.push({
            ngay,
            loai,
            dvt,
            slNhapKhoCoThue,
            slNhapKhoKhongThue,
            tongSoLuong,
            thanhTienCoThue,
            thanhTienKhongThue,
            cuocCoThue,
            cuocKhongThue,
            tongTienHangCoThue,
            tongTienHangKhongThue,
            tongTienHang
        });
    }

    // 2. Nhóm dữ liệu theo loại nhiên liệu
    const loaiMap = {};
    for (const row of data) {
        if (!loaiMap[row.loai]) loaiMap[row.loai] = [];
        loaiMap[row.loai].push(row);
    }

    // 3. Tạo sheet cho từng loại nhiên liệu
    for (const loai in loaiMap) {
        // Tên sheet hợp lệ
        let sheetName = loai.replace(/[\\/?*\[\]]/g, "");
        if (sheetName.length > 31) sheetName = sheetName.substring(0, 26) + Math.floor(Math.random() * 1000);
        const ws = workbook.addWorksheet(sheetName);

        // Header
        ws.mergeCells("A1:F1");
        ws.getCell("A1").value = "CÔNG TY CPXD ĐỨC ANH";
        ws.getCell("A1").font = { bold: true, size: 11 };
        ws.mergeCells("A2:F2");
        ws.getCell("A2").value = chiNhanh;
        ws.getCell("A2").font = { bold: true, size: 11 };
        ws.mergeCells("A3:P3");
        ws.getCell("A3").value = "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO LOẠI NHIÊN LIỆU NĂM 2020";
        ws.getCell("A3").font = { bold: true, size: 14 };
        ws.mergeCells("A4:P4");
        ws.getCell("A4").value = "Loại nhiên liệu: " + loai;
        ws.getCell("A4").font = { bold: true, size: 12 };

        // Header dòng 6-7
        ws.mergeCells("A6:A7"); ws.getCell("A6").value = "STT";
        ws.mergeCells("B6:B7"); ws.getCell("B6").value = "Ngày nhập kho";
        ws.mergeCells("C6:C7"); ws.getCell("C6").value = "Loại nhiên liệu";
        ws.mergeCells("D6:G6"); ws.getCell("D6").value = "Thông tin nhập kho sau qui đổi";
        ws.getCell("D7").value = "Đơn vị sau qui đổi";
        ws.getCell("E7").value = "Số lượng nhập kho có thuế";
        ws.getCell("F7").value = "Số lượng nhập kho không thuế";
        ws.getCell("G7").value = "Tổng số lượng nhập kho";
        ws.mergeCells("H6:I6"); ws.getCell("H6").value = "Thành tiền nhiên liệu";
        ws.getCell("H7").value = "Có thuế";
        ws.getCell("I7").value = "Không thuế";
        ws.mergeCells("J6:K6"); ws.getCell("J6").value = "Thành tiền cước vận chuyển";
        ws.getCell("J7").value = "Có thuế";
        ws.getCell("K7").value = "Không thuế";
        ws.mergeCells("L6:N6"); ws.getCell("L6").value = "Tổng tiền hàng";
        ws.getCell("L7").value = "Có thuế";
        ws.getCell("M7").value = "Không thuế";
        ws.getCell("N7").value = "Tổng tiền";
        ws.mergeCells("O6:P6"); ws.getCell("O6").value = "Đơn giá trung bình";
        ws.getCell("O7").value = "Có thuế";
        ws.getCell("P7").value = "Không thuế";

        // Tô màu header
        function fillHeader(ws, r1, r2, c1, c2, color) {
            for (let r = r1; r <= r2; r++) {
                for (let c = c1; c <= c2; c++) {
                    const cell = ws.getCell(r, c);
                    cell.fill = {
                        type: "pattern",
                        pattern: "solid",
                        fgColor: { argb: color }
                    };
                    cell.alignment = { horizontal: "center", vertical: "middle", wrapText: true };
                    cell.font = { bold: true, name: "Times New Roman", size: 11 };
                }
            }
        }
        fillHeader(ws, 6, 7, 1, 16, "AEEFC2");

        // 4. Gộp dữ liệu theo ngày + dvt (giống VBA)
        const groupMap = new Map();
        for (const row of loaiMap[loai]) {
            // Parse ngày dd/MM/yyyy
            let ngayKey = row.ngay;
            let dvtKey = row.dvt;
            const key = `${ngayKey}|${dvtKey}`;
            if (!groupMap.has(key)) {
                groupMap.set(key, {
                    ngay: row.ngay,
                    loai: row.loai,
                    dvt: row.dvt,
                    slNhapKhoCoThue: 0,
                    slNhapKhoKhongThue: 0,
                    tongSoLuong: 0,
                    thanhTienCoThue: 0,
                    thanhTienKhongThue: 0,
                    cuocCoThue: 0,
                    cuocKhongThue: 0,
                    tongTienHangCoThue: 0,
                    tongTienHangKhongThue: 0,
                    tongTienHang: 0
                });
            }
            const g = groupMap.get(key);
            g.slNhapKhoCoThue += row.slNhapKhoCoThue;
            g.slNhapKhoKhongThue += row.slNhapKhoKhongThue;
            g.tongSoLuong += row.tongSoLuong;
            g.thanhTienCoThue += row.thanhTienCoThue;
            g.thanhTienKhongThue += row.thanhTienKhongThue;
            g.cuocCoThue += row.cuocCoThue;
            g.cuocKhongThue += row.cuocKhongThue;
            g.tongTienHangCoThue += row.tongTienHangCoThue;
            g.tongTienHangKhongThue += row.tongTienHangKhongThue;
            g.tongTienHang += row.tongTienHang;
        }
        // Sắp xếp theo ngày, dvt
        const grouped = Array.from(groupMap.values()).sort((a, b) => {
            // So sánh ngày dd/MM/yyyy
            const [da, ma, ya] = a.ngay.split("/").map(Number);
            const [db, mb, yb] = b.ngay.split("/").map(Number);
            const dateA = new Date(ya, ma - 1, da);
            const dateB = new Date(yb, mb - 1, db);
            if (dateA - dateB !== 0) return dateA - dateB;
            return a.dvt.localeCompare(b.dvt);
        });

        // 5. Ghi dữ liệu
        let rowIdx = 8, stt = 1;
        for (const d of grouped) {
            ws.getCell(`A${rowIdx}`).value = stt++;
            ws.getCell(`B${rowIdx}`).value = d.ngay;
            ws.getCell(`C${rowIdx}`).value = d.loai;
            ws.getCell(`D${rowIdx}`).value = d.dvt;
            ws.getCell(`E${rowIdx}`).value = d.slNhapKhoCoThue;
            ws.getCell(`F${rowIdx}`).value = d.slNhapKhoKhongThue;
            ws.getCell(`G${rowIdx}`).value = d.tongSoLuong;
            ws.getCell(`H${rowIdx}`).value = d.thanhTienCoThue;
            ws.getCell(`I${rowIdx}`).value = d.thanhTienKhongThue;
            ws.getCell(`J${rowIdx}`).value = d.cuocCoThue;
            ws.getCell(`K${rowIdx}`).value = d.cuocKhongThue;
            ws.getCell(`L${rowIdx}`).value = d.tongTienHangCoThue;
            ws.getCell(`M${rowIdx}`).value = d.tongTienHangKhongThue;
            ws.getCell(`N${rowIdx}`).value = d.tongTienHang;
            ws.getCell(`O${rowIdx}`).value = { formula: `IFERROR(L${rowIdx}/E${rowIdx},0)` };
            ws.getCell(`P${rowIdx}`).value = { formula: `IFERROR(M${rowIdx}/F${rowIdx},0)` };
            rowIdx++;
        }
        const lastRow = rowIdx - 1;

        // 6. Subtotal
        ws.getCell(`A${rowIdx}`).value = "Tổng cộng";
        ws.mergeCells(`A${rowIdx}:D${rowIdx}`);
        ["E", "F", "G", "H", "I", "J", "K", "L", "M", "N"].forEach(col => {
            ws.getCell(`${col}${rowIdx}`).value = { formula: `SUBTOTAL(9,${col}8:${col}${lastRow})` };
        });
        ws.getCell(`O${rowIdx}`).value = { formula: `IFERROR(L${rowIdx}/E${rowIdx},0)` };
        ws.getCell(`P${rowIdx}`).value = { formula: `IFERROR(M${rowIdx}/F${rowIdx},0)` };
        ws.getCell(`A${rowIdx}`).font = { bold: true };

        // 7. Định dạng, border, footer
        function applyBorder(ws, sr, er, sc, ec) {
            for (let r = sr; r <= er; r++) {
                for (let c = sc; c <= ec; c++) {
                    ws.getCell(r, c).border = {
                        top: { style: "thin" },
                        left: { style: "thin" },
                        bottom: { style: "thin" },
                        right: { style: "thin" }
                    };
                }
            }
        }
        applyBorder(ws, 6, rowIdx, 1, 16);

        // Footer
        let footerStart = rowIdx + 2;
        ws.mergeCells(`D${footerStart}:N${footerStart}`);
        ws.getCell(`D${footerStart}`).value = "Kiểm soát nội bộ";
        ws.getCell(`D${footerStart}`).alignment = { horizontal: "center" };
        ws.getCell(`D${footerStart}`).font = { bold: true, name: "Times New Roman", size: 11 };

        ws.mergeCells(`D${footerStart + 1}:H${footerStart + 1}`);
        ws.getCell(`D${footerStart + 1}`).value = "Nhân viên";
        ws.getCell(`D${footerStart + 1}`).alignment = { horizontal: "center" };

        ws.mergeCells(`I${footerStart + 1}:N${footerStart + 1}`);
        ws.getCell(`I${footerStart + 1}`).value = "Trưởng phòng";
        ws.getCell(`I${footerStart + 1}`).alignment = { horizontal: "center" };

        // Định dạng số
        for (let r = 8; r <= rowIdx; r++) {
            ["E", "F", "G"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##0.00");
            ["H", "I", "J", "K", "L", "M", "N", "O", "P"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##");
        }
        for (let i = 1; i <= 16; i++) ws.getColumn(i).width = 14;

        // Font toàn bộ sheet
        ws.eachRow(row => {
            row.eachCell(cell => {
                cell.font = { ...cell.font, name: "Times New Roman" };
            });
        });
    }

    // Xuất file
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName.endsWith(".xlsx") ? fileName : fileName + ".xlsx");
}
//NKNL theo NCC cước VC công ty(Tổng hợp)
async function exportExcelBCNKVLtheoNCCCuocCongTyTH(tableId, fileName, chiNhanh) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const workbook = new ExcelJS.Workbook();
    const ws = workbook.addWorksheet("KetQua");

    // 1. ĐỌC DATA GIỐNG VBA (dòng 13 trở đi, index 12)
    const trs = Array.from(table.querySelectorAll("tbody tr"));
    const startRow = 0;
    let endRow = startRow;
    let soDong = 0;

    // Xác định số dòng thực tế (dừng khi cột C, M, R đều rỗng)
    for (let i = startRow; i < trs.length; i++) {
        const tds = Array.from(trs[i].querySelectorAll("td"));
        if (
            !tds[2] || !tds[12] || !tds[17] ||
            !tds[2].innerText.trim() || !tds[12].innerText.trim() || !tds[17].innerText.trim()
        ) break;
        endRow = i;
        soDong++;
    }

    // Đọc dữ liệu từng dòng
    const data = [];
    for (let i = startRow; i <= endRow && i < trs.length; i++) {
        const tds = Array.from(trs[i].querySelectorAll("td"));
        // Lấy ngày, loại, dvt, biển số xe, số lượng, thành tiền
        const ngay = parseVNDate(tds[3]?.innerText.trim()) || null;
        const loai = tds[13]?.innerText.trim();
        const dvt = tds[18]?.innerText.trim();
        const slNhapKhoCoThue = parseFloat(tds[19]?.innerText.replace(/,/g, "")) || 0;
        const slNhapKhoKhongThue = parseFloat(tds[20]?.innerText.replace(/,/g, "")) || 0;
        const tongSoLuong = parseFloat(tds[21]?.innerText.replace(/,/g, "")) || 0;
        const bienSoXe = tds[27]?.innerText.trim();
        const thanhTienCoThue = parseFloat(tds[30]?.innerText.replace(/,/g, "")) || 0;
        const thanhTienKhongThue = parseFloat(tds[31]?.innerText.replace(/,/g, "")) || 0;

        // Bỏ qua nếu không có biển số xe
        if (!bienSoXe) continue;

        // Tính khối lượng có thuế/không thuế như VBA
        const klcothue = thanhTienCoThue > 0 ? tongSoLuong : 0;
        const klkhongthue = thanhTienKhongThue > 0 ? tongSoLuong : 0;

        // Lấy tháng-năm từ ngày
        let month = "", year = "";
        if (ngay) {
             month = ngay.getMonth();
             year = ngay.getFullYear();
        }

        data.push({
            ngay,
            month,
            year,
            loai,
            dvt,
            bienSoXe,
            klcothue,
            klkhongthue,
            tongSoLuong,
            thanhTienCoThue,
            thanhTienKhongThue
        });
    }

    // 2. GROUP DATA GIỐNG VBA
    // Key: bienSoXe|month|year|loai|dvt
    const groupMap = new Map();
    for (const row of data) {
        const key = [
            row.bienSoXe,
            row.month,
            row.year,
            row.loai,
            row.dvt
        ].join("|");
        if (!groupMap.has(key)) {
            groupMap.set(key, {
                ngay: row.ngay,
                month: row.month,
                year: row.year,
                loai: row.loai,
                dvt: row.dvt,
                bienSoXe: row.bienSoXe,
                klcothue: 0,
                klkhongthue: 0,
                tongSoLuong: 0,
                thanhTienCoThue: 0,
                thanhTienKhongThue: 0
            });
        }
        const g = groupMap.get(key);
        g.klcothue += row.klcothue;
        g.klkhongthue += row.klkhongthue;
        g.tongSoLuong += row.tongSoLuong;
        g.thanhTienCoThue += row.thanhTienCoThue;
        g.thanhTienKhongThue += row.thanhTienKhongThue;
    }
    // Chuyển về mảng và sort lại như VBA
    const grouped = Array.from(groupMap.values());
    grouped.sort((a, b) => {
        // Sắp xếp theo: bienSoXe DESC, tháng-năm(ngay) ASC, loai ASC, dvt ASC
        if (a.bienSoXe !== b.bienSoXe) return b.bienSoXe.localeCompare(a.bienSoXe);
        if (a.year !== b.year) return a.year - b.year;
        if (a.month !== b.month) return a.month - b.month;
        if (a.loai !== b.loai) return a.loai.localeCompare(b.loai);
        return a.dvt.localeCompare(b.dvt);
    });

    // 3. HEADER GIỐNG VBA
    const title = (range, text, size = 12) => {
        ws.mergeCells(range);
        const c = ws.getCell(range.split(":")[0]);
        c.value = text;
        c.font = { name: "Times New Roman", bold: true, size: size };
        c.alignment = { horizontal: "center" };
    };
    title("A1:M1", "CÔNG TY CPXD ĐỨC ANH", 11);
    title("A2:M2", chiNhanh, 11);
    title("A3:M4", "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO CƯỚC VẬN CHUYỂN CÔNG TY NĂM 2020", 14);
    title("O1:W1", "CÔNG TY CPXD ĐỨC ANH", 11);
    title("O2:W2", chiNhanh, 11);
    title("O3:W4", "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO CƯỚC VẬN CHUYỂN CÔNG TY NĂM 2020", 14);

    // Header bảng trái
    ws.mergeCells("A6:A7"); ws.getCell("A6").value = "STT";
    ws.mergeCells("B6:B7"); ws.getCell("B6").value = "Ngày nhập kho";
    ws.mergeCells("C6:G6"); ws.getCell("C6").value = "Thông tin nhiên liệu";
    ws.getCell("C7").value = "Loại nhiên liệu";
    ws.getCell("D7").value = "Đơn vị sau qui đổi";
    ws.getCell("E7").value = "Số lượng nhập kho có thuế";
    ws.getCell("F7").value = "Số lượng nhập kho không thuế";
    ws.getCell("G7").value = "Tổng số lượng nhập kho";
    ws.mergeCells("H6:H7"); ws.getCell("H6").value = "Biển số xe";
    ws.mergeCells("I6:K6"); ws.getCell("I6").value = "Thành tiền cước vận chuyển";
    ws.getCell("I7").value = "Thành tiền có thuế";
    ws.getCell("J7").value = "Thành tiền không thuế";
    ws.getCell("K7").value = "Tổng tiền";
    ws.mergeCells("L6:M6"); ws.getCell("L6").value = "Đơn giá trung bình";
    ws.getCell("L7").value = "Có thuế";
    ws.getCell("M7").value = "Không thuế";

    // Header bảng phải
    ws.mergeCells("O6:O7"); ws.getCell("O6").value = "Loại nhiên liệu";
    ws.mergeCells("P6:R6"); ws.getCell("P6").value = "Khối lượng nhiên liệu";
    ws.getCell("P7").value = "KL có thuế";
    ws.getCell("Q7").value = "KL không thuế";
    ws.getCell("R7").value = "Tổng KL";
    ws.mergeCells("S6:U6"); ws.getCell("S6").value = "Giá trị cước vận chuyển";
    ws.getCell("S7").value = "Thành tiền có thuế";
    ws.getCell("T7").value = "Thành tiền không thuế";
    ws.getCell("U7").value = "Tổng tiền";
    ws.mergeCells("V6:W6"); ws.getCell("V6").value = "Đơn giá trung bình";
    ws.getCell("V7").value = "Có thuế";
    ws.getCell("W7").value = "Không thuế";

    // Tô màu header
    function fillHeader(ws, r1, r2, c1, c2, color) {
        for (let r = r1; r <= r2; r++) {
            for (let c = c1; c <= c2; c++) {
                const cell = ws.getCell(r, c);
                cell.fill = {
                    type: "pattern",
                    pattern: "solid",
                    fgColor: { argb: color }
                };
                cell.alignment = { horizontal: "center", vertical: "middle", wrapText: true };
                cell.font = { bold: true, name: "Times New Roman", size: 11 };
            }
        }
    }
    fillHeader(ws, 6, 7, 1, 13, "AEEFC2");
    fillHeader(ws, 6, 7, 15, 23, "AEEFC2");

    // 4. GHI DATA BẢNG TRÁI
    let row = 8, stt = 1;
    grouped.forEach(d => {
        ws.getCell(`A${row}`).value = stt++;
        ws.getCell(`B${row}`).value = d.ngay;
        ws.getCell(`C${row}`).value = d.loai;
        ws.getCell(`D${row}`).value = d.dvt;
        ws.getCell(`E${row}`).value = d.klcothue;
        ws.getCell(`F${row}`).value = d.klkhongthue;
        ws.getCell(`G${row}`).value = d.tongSoLuong;
        ws.getCell(`H${row}`).value = d.bienSoXe;
        ws.getCell(`I${row}`).value = d.thanhTienCoThue;
        ws.getCell(`J${row}`).value = d.thanhTienKhongThue;
        ws.getCell(`K${row}`).value = { formula: `I${row}+J${row}` };
        ws.getCell(`L${row}`).value = { formula: `IFERROR(I${row}/E${row},0)` };
        ws.getCell(`M${row}`).value = { formula: `IFERROR(J${row}/F${row},0)` };
        row++;
    });
    const lastRow = row - 1;

    // 5. SUBTOTAL BẢNG TRÁI
    ws.getCell(`A${row}`).value = "Tổng cộng";
    ws.mergeCells(`A${row}:D${row}`);
    ["E", "F", "G", "I", "J", "K"].forEach(col => {
        ws.getCell(`${col}${row}`).value = { formula: `SUBTOTAL(9,${col}8:${col}${lastRow})` };
    });
    ws.getCell(`A${row}`).font = { bold: true };

    // 6. GHI DATA BẢNG PHẢI
    const loaiList = [...new Set(grouped.map(x => x.loai))];
    let r2 = 8;
    loaiList.forEach(loai => {
        ws.getCell(`O${r2}`).value = loai;
        ws.getCell(`P${r2}`).value = { formula: `SUMPRODUCT((C8:C${lastRow}=O${r2})*E8:E${lastRow})` };
        ws.getCell(`Q${r2}`).value = { formula: `SUMPRODUCT((C8:C${lastRow}=O${r2})*F8:F${lastRow})` };
        ws.getCell(`R${r2}`).value = { formula: `P${r2}+Q${r2}` };
        ws.getCell(`S${r2}`).value = { formula: `SUMPRODUCT((C8:C${lastRow}=O${r2})*I8:I${lastRow})` };
        ws.getCell(`T${r2}`).value = { formula: `SUMPRODUCT((C8:C${lastRow}=O${r2})*J8:J${lastRow})` };
        ws.getCell(`U${r2}`).value = { formula: `S${r2}+T${r2}` };
        ws.getCell(`V${r2}`).value = { formula: `IFERROR(S${r2}/P${r2},0)` };
        ws.getCell(`W${r2}`).value = { formula: `IFERROR(T${r2}/Q${r2},0)` };
        r2++;
    });
    ws.getCell(`O${r2}`).value = "Tổng cộng";
    ws.mergeCells(`O${r2}:R${r2}`);
    ["S", "T", "U"].forEach(col => {
        ws.getCell(`${col}${r2}`).value = { formula: `SUBTOTAL(9,${col}8:${col}${r2 - 1})` };
    });
    ws.getCell(`O${r2}`).font = { bold: true };

    // 7. ĐỊNH DẠNG, BORDER, FOOTER
    function applyBorder(ws, sr, er, sc, ec) {
        for (let r = sr; r <= er; r++) {
            for (let c = sc; c <= ec; c++) {
                ws.getCell(r, c).border = {
                    top: { style: "thin" },
                    left: { style: "thin" },
                    bottom: { style: "thin" },
                    right: { style: "thin" }
                };
            }
        }
    }
    applyBorder(ws, 6, row, 1, 13);
    applyBorder(ws, 6, r2, 15, 23);

    // Footer bảng phải
    let footerStart = Math.max(row, r2) + 3;
    ws.getCell("O" + footerStart).value = "Kiểm soát nội bộ";
    ws.mergeCells(`O${footerStart}:W${footerStart}`);
    ws.getCell("O" + (footerStart + 1)).value = "Nhân viên";
    ws.mergeCells(`O${footerStart + 1}:R${footerStart + 1}`);
    ws.getCell("S" + (footerStart + 1)).value = "Trưởng phòng";
    ws.mergeCells(`S${footerStart + 1}:W${footerStart + 1}`);
    fillHeader(ws, footerStart, footerStart + 1, 15, 23, "FFFFFF");

    // Footer bảng trái
    ws.getCell("A" + footerStart).value = "Kiểm soát nội bộ";
    ws.mergeCells(`A${footerStart}:M${footerStart}`);
    ws.getCell("A" + (footerStart + 1)).value = "Nhân viên";
    ws.mergeCells(`A${footerStart + 1}:G${footerStart + 1}`);
    ws.getCell("H" + (footerStart + 1)).value = "Trưởng phòng";
    ws.mergeCells(`H${footerStart + 1}:M${footerStart + 1}`);
    fillHeader(ws, footerStart, footerStart + 1, 1, 13, "FFFFFF");

    // Định dạng số
    for (let r = 8; r <= row; r++) {
        ["E", "F", "G"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##0.00");
        ["I", "J", "K", "L", "M"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##");
    }
    for (let r = 8; r <= r2; r++) {
        ["P", "Q", "R"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##0.00");
        ["S", "T", "U", "V", "W"].forEach(col => ws.getCell(`${col}${r}`).numFmt = "#,##");
    }

    // Set width cho các cột
    for (let i = 1; i <= 23; i++) ws.getColumn(i).width = 14;

    // Font toàn bộ sheet
    ws.eachRow(row => {
        row.eachCell(cell => {
            cell.font = { ...cell.font, name: "Times New Roman" };
        });
    });

    // Xuất file
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName.endsWith(".xlsx") ? fileName : fileName + ".xlsx");
}
//NKNL theo NCC cước VC công ty(Chi tiết)
async function BCNKVLtheoNCCCuocCongTyCT(idTable, fileName, chiNhanh = "") {

    const table = document.getElementById(idTable);
    const workbook = new ExcelJS.Workbook();

    const rows = [];

    const num = td => {
        if (!td) return 0;
        return parseFloat(td.innerText.replace(/,/g, "").trim()) || 0;
    };

    table.querySelectorAll("tbody tr").forEach(tr => {

        const tds = [...tr.querySelectorAll("td")];

        const tongkl = num(tds[21]);
        const ttcothue = num(tds[30]);
        const ttkhongthue = num(tds[31]);

        const biensoxe = (tds[27]?.innerText || "").trim();

        if (!biensoxe) return; // KHÔNG TẠO SHEET nếu rỗng

        rows.push({

            ngay: tds[3]?.innerText.trim(),
            loai: tds[13]?.innerText.trim(),
            dvt: tds[18]?.innerText.trim(),

            biensoxe: biensoxe,

            klcothue: ttcothue > 0 ? tongkl : 0,
            klkhongthue: ttkhongthue > 0 ? tongkl : 0,

            tongkl: tongkl,

            ttcothue: ttcothue,
            ttkhongthue: ttkhongthue

        });

    });

    if (!rows.length) return;

    rows.sort((a, b) => {

        if (a.biensoxe !== b.biensoxe)
            return b.biensoxe.localeCompare(a.biensoxe);

        if (a.ngay !== b.ngay)
            return new Date(a.ngay) - new Date(b.ngay);

        if (a.loai !== b.loai)
            return a.loai.localeCompare(b.loai);

        return a.dvt.localeCompare(b.dvt);

    });

    const grouped = [];
    let current = null;

    rows.forEach(r => {

        if (current &&
            current.biensoxe === r.biensoxe &&
            current.ngay === r.ngay &&
            current.loai === r.loai &&
            current.dvt === r.dvt) {

            current.klcothue += r.klcothue;
            current.klkhongthue += r.klkhongthue;
            current.tongkl += r.tongkl;
            current.ttcothue += r.ttcothue;
            current.ttkhongthue += r.ttkhongthue;

        } else {

            if (current) grouped.push(current);
            current = { ...r };

        }

    });

    if (current) grouped.push(current);

    const xeMap = {};

    grouped.forEach(r => {
        if (!xeMap[r.biensoxe]) xeMap[r.biensoxe] = [];
        xeMap[r.biensoxe].push(r);
    });

    // Hàm thêm border cho vùng dữ liệu
    function applyBorder(ws, startRow, endRow, startCol, endCol) {
        for (let r = startRow; r <= endRow; r++) {
            for (let c = startCol; c <= endCol; c++) {
                ws.getCell(r, c).border = {
                    top: { style: "thin" },
                    left: { style: "thin" },
                    bottom: { style: "thin" },
                    right: { style: "thin" }
                };
            }
        }
    }

    for (const biensoxe in xeMap) {

        const ws = workbook.addWorksheet(biensoxe.substring(0, 31));

        const title = (range, text) => {
            ws.mergeCells(range);
            const c = ws.getCell(range.split(":")[0]);
            c.value = text;
            c.font = { name: "Times New Roman", bold: true, size: 12 };
            c.alignment = { horizontal: "center" };
        };

        title("A1:M1", "CÔNG TY CPXD ĐỨC ANH");
        title("O1:W1", "CÔNG TY CPXD ĐỨC ANH");

        title("A2:M2", chiNhanh);
        title("O2:W2", chiNhanh);

        title("A3:M3", "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO CƯỚC VẬN CHUYỂN CÔNG TY NĂM");
        title("O3:W3", "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO CƯỚC VẬN CHUYỂN CÔNG TY NĂM");

        title("A4:M4", "Biển số xe: " + biensoxe);
        title("O4:W4", "Biển số xe: " + biensoxe);

        const header = (cell, text) => {
            const c = ws.getCell(cell);
            c.value = text;
            c.font = { bold: true };
            c.alignment = { horizontal: "center", vertical: "middle" };
        };

        ws.mergeCells("A6:A7"); header("A6", "STT");
        ws.mergeCells("B6:B7"); header("B6", "Ngày nhập kho");

        ws.mergeCells("C6:G6"); header("C6", "Thông tin nhiên liệu");

        header("C7", "Loại nhiên liệu");
        header("D7", "Ðơn vị sau qui đổi");
        header("E7", "Số lượng nhập kho có thuế");
        header("F7", "Số lượng nhập kho không thuế");
        header("G7", "Tổng số lượng nhập kho");

        ws.mergeCells("H6:H7"); header("H6", "Biển số xe");

        ws.mergeCells("I6:K6"); header("I6", "Thành tiền cước vận chuyển");

        header("I7", "Thành tiền có thuế");
        header("J7", "Thành tiền không thuế");
        header("K7", "Tổng tiền");

        ws.mergeCells("L6:M6"); header("L6", "Ðơn giá trung bình");

        header("L7", "Có thuế");
        header("M7", "Không thuế");

        ws.mergeCells("O6:O7"); header("O6", "Loại nhiên liệu");

        ws.mergeCells("P6:R6"); header("P6", "Khối lượng nhiên liệu");

        header("P7", "KL có thuế");
        header("Q7", "KL không thuế");
        header("R7", "Tổng KL");

        ws.mergeCells("S6:U6"); header("S6", "Giá trị cước vận chuyển");

        header("S7", "Thành tiền có thuế");
        header("T7", "Thành tiền không thuế");
        header("U7", "Tổng tiền");

        ws.mergeCells("V6:W6"); header("V6", "Ðơn giá trung bình");

        header("V7", "Có thuế");
        header("W7", "Không thuế");

        let row = 8;
        let stt = 1;

        xeMap[biensoxe].forEach(d => {

            ws.getCell(`A${row}`).value = stt++;
            ws.getCell(`B${row}`).value = d.ngay;

            ws.getCell(`C${row}`).value = d.loai;
            ws.getCell(`D${row}`).value = d.dvt;

            ws.getCell(`E${row}`).value = d.klcothue;
            ws.getCell(`F${row}`).value = d.klkhongthue;
            ws.getCell(`G${row}`).value = d.tongkl;

            ws.getCell(`H${row}`).value = d.biensoxe;

            ws.getCell(`I${row}`).value = d.ttcothue;
            ws.getCell(`J${row}`).value = d.ttkhongthue;

            ws.getCell(`K${row}`).value = { formula: `I${row}+J${row}` };

            ws.getCell(`L${row}`).value = { formula: `IFERROR(I${row}/E${row},0)` };
            ws.getCell(`M${row}`).value = { formula: `IFERROR(J${row}/F${row},0)` };

            row++;

        });

        const lastRow = row - 1;

        ws.getCell(`A${row}`).value = "Tổng cộng";
        ws.mergeCells(`A${row}:D${row}`);

        ws.getCell(`E${row}`).value = { formula: `SUBTOTAL(9,E8:E${lastRow})` };
        ws.getCell(`F${row}`).value = { formula: `SUBTOTAL(9,F8:F${lastRow})` };
        ws.getCell(`G${row}`).value = { formula: `SUBTOTAL(9,G8:G${lastRow})` };

        ws.getCell(`I${row}`).value = { formula: `SUBTOTAL(9,I8:I${lastRow})` };
        ws.getCell(`J${row}`).value = { formula: `SUBTOTAL(9,J8:J${lastRow})` };
        ws.getCell(`K${row}`).value = { formula: `SUBTOTAL(9,K8:K${lastRow})` };

        const loaiList = [...new Set(xeMap[biensoxe].map(x => x.loai))];

        let r2 = 8;

        loaiList.forEach(loai => {

            ws.getCell(`O${r2}`).value = loai;

            ws.getCell(`P${r2}`).value = {
                formula: `SUMPRODUCT((C8:C${lastRow}="${loai}")*E8:E${lastRow})`
            };

            ws.getCell(`Q${r2}`).value = {
                formula: `SUMPRODUCT((C8:C${lastRow}="${loai}")*F8:F${lastRow})`
            };

            ws.getCell(`R${r2}`).value = { formula: `P${r2}+Q${r2}` };

            ws.getCell(`S${r2}`).value = {
                formula: `SUMPRODUCT((C8:C${lastRow}="${loai}")*I8:I${lastRow})`
            };

            ws.getCell(`T${r2}`).value = {
                formula: `SUMPRODUCT((C8:C${lastRow}="${loai}")*J8:J${lastRow})`
            };

            ws.getCell(`U${r2}`).value = { formula: `S${r2}+T${r2}` };

            ws.getCell(`V${r2}`).value = { formula: `IFERROR(S${r2}/P${r2},0)` };
            ws.getCell(`W${r2}`).value = { formula: `IFERROR(T${r2}/Q${r2},0)` };

            r2++;

        });

        ws.getCell(`O${r2}`).value = "Tổng cộng";
        ws.mergeCells(`O${footerStart + 1}:R${footerStart + 1}`);
        ws.getCell(`O${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        ws.getCell(`S${r2}`).value = { formula: `SUM(S8:S${r2 - 1})` };
        ws.getCell(`T${r2}`).value = { formula: `SUM(T8:T${r2 - 1})` };
        ws.getCell(`U${r2}`).value = { formula: `SUM(U8:U${r2 - 1})` };

        const footerStart = Math.max(row, r2) + 3;

        // ===== FOOTER TRÁI =====
        ws.getCell("A" + footerStart).value = "Kiểm soát nội bộ";
        ws.mergeCells(`A${footerStart}:M${footerStart}`);
        ws.getCell(`A${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        ws.getCell("A" + (footerStart + 1)).value = "Nhân viên";
        ws.mergeCells(`A${footerStart + 1}:G${footerStart + 1}`);
        ws.getCell(`A${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        ws.getCell("H" + (footerStart + 1)).value = "Trưởng phòng";
        ws.mergeCells(`H${footerStart + 1}:M${footerStart + 1}`);
        ws.getCell(`H${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        // ===== FOOTER PHẢI =====
        ws.getCell("O" + footerStart).value = "Kiểm soát nội bộ";
        ws.mergeCells(`O${footerStart}:W${footerStart}`);
        ws.getCell(`O${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        ws.getCell("O" + (footerStart + 1)).value = "Nhân viên";
        ws.mergeCells(`O${footerStart + 1}:R${footerStart + 1}`);
        ws.getCell(`O${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        ws.getCell("S" + (footerStart + 1)).value = "Trưởng phòng";
        ws.mergeCells(`S${footerStart + 1}:W${footerStart + 1}`);
        ws.getCell(`S${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

        applyBorder(ws, 6, row, 1, 13);
        applyBorder(ws, 6, r2, 15, 23);

        ws.eachRow(r => {
            r.eachCell(c => {
                if (typeof c.value === "number") {
                    c.numFmt = "#,##0";
                }
            });
        });

        for (let i = 1; i <= 23; i++) ws.getColumn(i).width = 14;

    }

    const buffer = await workbook.xlsx.writeBuffer();

    saveAs(new Blob([buffer]), fileName);

}
//NKNL theo NCC(Tổng hợp)
async function exportExcelBCNKVLtheoNCCTH(idTable, fileName, chiNhanh) {

    const table = document.getElementById(idTable);
    if (!table) return;

    const workbook = new ExcelJS.Workbook();
    const ws = workbook.addWorksheet("KetQua");

    // ===============================
    // 1. ĐỌC DATA
    // ===============================

    let dataRows = [];

    table.querySelectorAll("tbody tr").forEach(tr => {
        const tds = tr.querySelectorAll("td");
        if (tds.length < 27) return;

        dataRows.push({
            ngay: parseVNDate(tds[3]?.innerText.trim()) || null,
            sohd: tds[7]?.innerText.trim(),
            ncc: tds[9]?.innerText.trim(),
            loai: tds[13]?.innerText.trim(),
            donvi: tds[18]?.innerText.trim(),
            klcothue: parseFloat(tds[19].innerText.replace(/,/g, "")) || 0,
            klkhongthue: parseFloat(tds[20].innerText.replace(/,/g, "")) || 0,
            tongkl: parseFloat(tds[21].innerText.replace(/,/g, "")) || 0,
            tiencothue: parseFloat(tds[24].innerText.replace(/,/g, "")) || 0,
            tienkhongthue: parseFloat(tds[25].innerText.replace(/,/g, "")) || 0,
            tongtien: parseFloat(tds[26].innerText.replace(/,/g, "")) || 0
        });
    });

    // ===============================
    // GROUP THEO THÁNG + HĐ + LOẠI + ĐVT (GIỐNG VBA)
    // ===============================

    const groupedMap = new Map();

    dataRows.forEach(r => {

        if (!r.ngay) return;

        const month = r.ngay.getMonth();
        const year = r.ngay.getFullYear();

        const key = [
            year,
            month,
            r.sohd,
            r.loai,
            r.donvi
        ].join("|");

        if (!groupedMap.has(key)) {
            groupedMap.set(key, {
                ngay: new Date(year, month, 1), // giống VBA format mm-yyyy
                sohd: r.sohd,
                ncc: r.ncc,
                loai: r.loai,
                donvi: r.donvi,
                klcothue: 0,
                klkhongthue: 0,
                tongkl: 0,
                tiencothue: 0,
                tienkhongthue: 0,
                tongtien: 0
            });
        }

        const g = groupedMap.get(key);

        g.klcothue += r.klcothue;
        g.klkhongthue += r.klkhongthue;
        g.tongkl += r.tongkl;
        g.tiencothue += r.tiencothue;
        g.tienkhongthue += r.tienkhongthue;
        g.tongtien += r.tongtien;
    });

    // thay dataRows bằng dữ liệu đã group
    dataRows = Array.from(groupedMap.values());

    // 1. HEADER
    // ===============================

    ws.mergeCells("A1:F1");
    ws.getCell("A1").value = "CÔNG TY CPXD ĐỨC ANH";

    ws.mergeCells("A2:F2");
    ws.getCell("A2").value = chiNhanh;

    ws.mergeCells("A3:N3");
    ws.getCell("A3").value = "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO NHÀ CUNG CẤP";

    ws.mergeCells("P1:S1");
    ws.getCell("P1").value = "CÔNG TY CPXD ĐỨC ANH";

    ws.mergeCells("P2:S2");
    ws.getCell("P2").value = chiNhanh;

    ws.mergeCells("P3:X3");
    ws.getCell("P3").value = "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO NHÀ CUNG CẤP";

    // ===============================
    // 2. HEADER (6-7)
    // ===============================

    ws.getCell("A6").value = "STT"; ws.mergeCells("A6:A7");
    ws.getCell("B6").value = "Tháng"; ws.mergeCells("B6:B7");

    ws.getCell("C6").value = "Thông tin nhà cung cấp"; ws.mergeCells("C6:D6");
    ws.getCell("C7").value = "Số HĐ";
    ws.getCell("D7").value = "Tên nhà cung cấp";

    ws.getCell("E6").value = "Loại nhiên liệu"; ws.mergeCells("E6:E7");

    ws.getCell("F6").value = "Thông tin nhập kho sau qui đổi"; ws.mergeCells("F6:I6");
    ws.getCell("F7").value = "Đơn vị sau qui đổi";
    ws.getCell("G7").value = "Số lượng nhập kho có thuế";
    ws.getCell("H7").value = "Số lượng nhập kho không thuế";
    ws.getCell("I7").value = "Tổng số lượng nhập kho";

    ws.getCell("J6").value = "Thành tiền nhiên liệu"; ws.mergeCells("J6:L6");
    ws.getCell("J7").value = "Thành tiền có thuế";
    ws.getCell("K7").value = "Thành tiền không thuế";
    ws.getCell("L7").value = "Tổng tiền";

    ws.getCell("M6").value = "Đơn giá trung bình"; ws.mergeCells("M6:N6");
    ws.getCell("M7").value = "Có thuế";
    ws.getCell("N7").value = "Không thuế";

    ws.getCell("P6").value = "Loại nhiên liệu"; ws.mergeCells("P6:P7");
    ws.getCell("Q6").value = "Khối lượng nhiên liệu"; ws.mergeCells("Q6:S6");
    ws.getCell("Q7").value = "KL có thuế";
    ws.getCell("R7").value = "KL không thuế";
    ws.getCell("S7").value = "Tổng KL";

    ws.getCell("T6").value = "Giá trị nhiên liệu"; ws.mergeCells("T6:V6");
    ws.getCell("T7").value = "Thành tiền có thuế";
    ws.getCell("U7").value = "Thành tiền không thuế";
    ws.getCell("V7").value = "Tổng tiền";

    ws.getCell("W6").value = "Đơn giá trung bình"; ws.mergeCells("W6:X6");
    ws.getCell("W7").value = "Có thuế";
    ws.getCell("X7").value = "Không thuế";

    // ===============================
    // FORMAT HEADER
    // ===============================

    // ================= HEADER FORMAT + BORDER =================

    // ===== BẢNG 1 (Cột 1 → 14) =====
    for (let r = 6; r <= 7; r++) {
        for (let c = 1; c <= 14; c++) {

            let cell = ws.getCell(r, c);

            cell.font = { name: "Times New Roman", size: 11, bold: true };
            cell.alignment = {
                vertical: "middle",
                horizontal: "center",
                wrapText: true
            };

            cell.border = {
                top: { style: "thin" },
                left: { style: "thin" },
                bottom: { style: "thin" },
                right: { style: "thin" }
            };
            cell.fill = {
                type: "pattern",
                pattern: "solid",
                fgColor: { argb: "FFAEEFC2" }
            };
        }
    }


    // ===== BẢNG 2 (Cột 16 → 24) =====
    for (let r = 6; r <= 7; r++) {
        for (let c = 16; c <= 24; c++) {

            let cell = ws.getCell(r, c);

            cell.font = { name: "Times New Roman", size: 11, bold: true };
            cell.alignment = {
                vertical: "middle",
                horizontal: "center",
                wrapText: true
            };

            cell.border = {
                top: { style: "thin" },
                left: { style: "thin" },
                bottom: { style: "thin" },
                right: { style: "thin" }
            };

            // Tô màu bảng phải
            cell.fill = {
                type: "pattern",
                pattern: "solid",
                fgColor: { argb: "FFAEEFC2" }
            };
        }
    }

    // ===============================
    // 3. GHI DATA
    // ===============================

    let rowIndex = 8;
    let stt = 1;

    dataRows.forEach(r => {

        ws.getCell(`A${rowIndex}`).value = stt++;
        ws.getCell(`B${rowIndex}`).value = r.ngay;
        ws.getCell(`C${rowIndex}`).value = r.sohd;
        ws.getCell(`D${rowIndex}`).value = r.ncc;
        ws.getCell(`E${rowIndex}`).value = r.loai;
        ws.getCell(`F${rowIndex}`).value = r.donvi;

        ws.getCell(`G${rowIndex}`).value = r.klcothue;
        ws.getCell(`H${rowIndex}`).value = r.klkhongthue;
        ws.getCell(`I${rowIndex}`).value = r.tongkl;

        ws.getCell(`J${rowIndex}`).value = r.tiencothue;
        ws.getCell(`K${rowIndex}`).value = r.tienkhongthue;
        ws.getCell(`L${rowIndex}`).value = r.tongtien;

        ws.getCell(`M${rowIndex}`).value = { formula: `IFERROR(J${rowIndex}/G${rowIndex},0)` };
        ws.getCell(`N${rowIndex}`).value = { formula: `IFERROR(K${rowIndex}/H${rowIndex},0)` };

        rowIndex++;
    });

    let lastRow = rowIndex - 1;

    // ===============================
    // 4. SUBTOTAL TRÁI
    // ===============================

    let totalRow = rowIndex;

    ws.getCell(`A${totalRow}`).value = "Tổng cộng";
    ws.mergeCells(`A${totalRow}:F${totalRow}`);

    ["G", "H", "I", "J", "K", "L"].forEach(col => {
        ws.getCell(`${col}${totalRow}`).value = {
            formula: `SUBTOTAL(9,${col}8:${col}${lastRow})`
        };
    });

    // ===============================
    // 5. BẢNG PHẢI
    // ===============================

    let uniqueLoai = [...new Set(dataRows.map(r => r.loai))];
    let rightStart = 8;

    uniqueLoai.forEach((loai, i) => {
        let r = rightStart + i;

        ws.getCell(`P${r}`).value = loai;

        ws.getCell(`Q${r}`).value = { formula: `SUMPRODUCT(($E$8:$E$${lastRow}=P${r})*($G$8:$G$${lastRow}))` };
        ws.getCell(`R${r}`).value = { formula: `SUMPRODUCT(($E$8:$E$${lastRow}=P${r})*($H$8:$H$${lastRow}))` };
        ws.getCell(`S${r}`).value = { formula: `Q${r}+R${r}` };

        ws.getCell(`T${r}`).value = { formula: `SUMPRODUCT(($E$8:$E$${lastRow}=P${r})*($J$8:$J$${lastRow}))` };
        ws.getCell(`U${r}`).value = { formula: `SUMPRODUCT(($E$8:$E$${lastRow}=P${r})*($K$8:$K$${lastRow}))` };
        ws.getCell(`V${r}`).value = { formula: `T${r}+U${r}` };

        ws.getCell(`W${r}`).value = { formula: `IFERROR(T${r}/Q${r},0)` };
        ws.getCell(`X${r}`).value = { formula: `IFERROR(U${r}/R${r},0)` };
    });

    let rightLast = rightStart + uniqueLoai.length;

    ws.getCell(`P${rightLast}`).value = "Tổng cộng";
    ws.mergeCells(`P${rightLast}:S${rightLast}`);

    ["T", "U", "V"].forEach(col => {
        ws.getCell(`${col}${rightLast}`).value = {
            formula: `SUBTOTAL(9,${col}8:${col}${rightLast - 1})`
        };
    });

    // ===============================
    // BORDER CẢ 2 BẢNG
    // ===============================

    function applyBorder(sr, er, sc, ec) {
        for (let r = sr; r <= er; r++) {
            for (let c = sc; c <= ec; c++) {
                ws.getCell(r, c).border = {
                    top: { style: "thin" },
                    left: { style: "thin" },
                    bottom: { style: "thin" },
                    right: { style: "thin" }
                };
            }
        }
    }

    applyBorder(6, totalRow, 1, 14);     // bảng trái
    applyBorder(6, rightLast, 16, 24);   // bảng phải

    // ===============================
    // FOOTER CHỮ KÝ
    // ===============================

    let footerStart = Math.max(totalRow, rightLast) + 3;

    ws.mergeCells(`A${footerStart}:F${footerStart}`);
    ws.getCell(`A${footerStart}`).value = "Phòng HCTC";
    ws.getCell(`A${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    ws.mergeCells(`G${footerStart}:L${footerStart}`);
    ws.getCell(`G${footerStart}`).value = "Kiểm soát nội bộ";
    ws.getCell(`G${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    ws.mergeCells(`P${footerStart}:X${footerStart}`);
    ws.getCell(`P${footerStart}`).value = "Kiểm soát nội bộ";
    ws.getCell(`P${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    footerStart++;

    ws.mergeCells(`A${footerStart}:C${footerStart}`);
    ws.getCell(`A${footerStart}`).value = "Nhân viên";
    ws.getCell(`A${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    ws.mergeCells(`D${footerStart}:F${footerStart}`);
    ws.getCell(`D${footerStart}`).value = "Trưởng phòng";
    ws.getCell(`D${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    ws.mergeCells(`G${footerStart}:I${footerStart}`);
    ws.getCell(`G${footerStart}`).value = "Nhân viên";

    ws.mergeCells(`J${footerStart}:L${footerStart}`);
    ws.getCell(`J${footerStart}`).value = "Trưởng phòng";
    ws.getCell(`J${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };


    ws.mergeCells(`P${footerStart}:S${footerStart}`);
    ws.getCell(`P${footerStart}`).value = "Nhân viên";
    ws.getCell(`J${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    ws.mergeCells(`T${footerStart}:X${footerStart}`);
    ws.getCell(`T${footerStart}`).value = "Trưởng phòng";
    ws.getCell(`J${footerStart}`).alignment = { horizontal: "center", vertical: "middle" };

    // ===============================
    // SET FONT TOÀN BỘ SHEET = TIMES NEW ROMAN
    // ===============================

    function applyTimesNewRoman(ws) {
        ws.eachRow((row) => {
            row.eachCell((cell) => {
                cell.font = {
                    ...cell.font,
                    name: "Times New Roman"
                };
            });
        });
    }

    applyTimesNewRoman(ws);

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName + ".xlsx");
}
//NKNL theo NCC(Chi tiết)
async function exportExcelBCNKVLtheoNCCCT(idTable, fileName) {

    const table = document.getElementById(idTable);
    if (!table) return;

    const workbook = new ExcelJS.Workbook();
    workbook.creator = "Ứng dụng";

    const rows = [];

    table.querySelectorAll("tbody tr").forEach(tr => {
        const tds = Array.from(tr.querySelectorAll("td"));

        rows.push({
            sheetName: tds[9]?.innerText.trim(),

            ngay: tds[3]?.innerText.trim(),
            sohd: tds[7]?.innerText.trim(),
            ncc: tds[9]?.innerText.trim(),
            loai: tds[13]?.innerText.trim(),
            dvt: tds[18]?.innerText.trim(),

            klcothue: num(tds[19]),
            klkhongthue: num(tds[20]),
            tongkl: num(tds[21]),

            ttcothue: num(tds[24]),
            ttkhongthue: num(tds[25]),
            tongtien: num(tds[26])
        });
    });

    // GROUP THEO NCC
    const nccMap = {};
    rows.forEach(r => {
        if (!nccMap[r.ncc]) nccMap[r.ncc] = [];
        nccMap[r.ncc].push(r);
    });

    for (const ncc in nccMap) {

        const ws = workbook.addWorksheet(
            ncc.replace(/[\\/?*\[\]]/g, "").substring(0, 31)
        );

        const data = nccMap[ncc];

        // ================= HEADER CÔNG TY (2 BÊN) =================
        addTitle(ws, "A1:F1", "CÔNG TY CPXD ĐỨC ANH");
        addTitle(ws, "P1:S1", "CÔNG TY CPXD ĐỨC ANH");

        addTitle(ws, "A2:F2", "TẤT CẢ CÁC TRẠM TRỘN");
        addTitle(ws, "P2:S2", "TẤT CẢ CÁC TRẠM TRỘN");

        addTitle(ws, "A3:N3",
            "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO NHÀ CUNG CẤP");
        addTitle(ws, "P3:X3",
            "BẢNG THEO DÕI CHI TIẾT NHẬP KHO NHIÊN LIỆU THEO NHÀ CUNG CẤP");

        addTitle(ws, "A4:N4", "Tên nhà cung cấp: " + ncc);
        addTitle(ws, "P4:X4", "Tên nhà cung cấp: " + ncc);

        // ================= HEADER 2 DÒNG =================
        const headerColor = "A9D08E";

        // DÒNG 6
        ws.mergeCells("A6:A7");
        ws.getCell("A6").value = "STT";
        ws.getCell("A6").alignment = {
            horizontal: "center",
            vertical: "middle"
        };
        ws.getCell("A6").font = { bold: true };

        ws.mergeCells("B6:B7");
        ws.getCell("B6").value = "Ngày nhập kho";
        ws.getCell("B6").alignment = {
            horizontal: "center",
            vertical: "middle"
        };
        ws.getCell("B6").font = { bold: true };


        ws.mergeCells("C6:D6");
        ws.getCell("C6").value = "Thông tin nhà cung cấp";
        ws.getCell("E6").value = "Loại nhiên liệu";

        ws.mergeCells("F6:I6");
        ws.getCell("F6").value = "Thông tin nhập kho sau qui đổi";

        ws.mergeCells("J6:L6");
        ws.getCell("J6").value = "Thành tiền nhiên liệu";

        ws.mergeCells("M6:N6");
        ws.getCell("M6").value = "Đơn giá trung bình";

        // DÒNG 7
        ws.getCell("C7").value = "Số HĐ";
        ws.getCell("D7").value = "Tên nhà cung cấp";

        ws.getCell("F7").value = "Đơn vị sau qui đổi";
        ws.getCell("G7").value = "Số lượng nhập kho có thuế";
        ws.getCell("H7").value = "Số lượng nhập kho không thuế";
        ws.getCell("I7").value = "Tổng số lượng nhập kho";

        ws.getCell("J7").value = "Thành tiền có thuế";
        ws.getCell("K7").value = "Thành tiền không thuế";
        ws.getCell("L7").value = "Tổng tiền";

        ws.getCell("M7").value = "Có thuế";
        ws.getCell("N7").value = "Không thuế";

        // HEADER PHẢI
        ws.mergeCells("P6:P7");
        ws.getCell("P6").value = "Loại nhiên liệu";
        ws.getCell("P6").alignment = {
            horizontal: "center",
            vertical: "middle"
        };
        ws.getCell("A6").font = { bold: true };

        ws.mergeCells("Q6:S6");
        ws.getCell("Q6").value = "Khối lượng nhiên liệu";

        ws.mergeCells("T6:V6");
        ws.getCell("T6").value = "Giá trị nhiên liệu";

        ws.mergeCells("W6:X6");
        ws.getCell("W6").value = "Đơn giá trung bình";

        ws.getCell("Q7").value = "KL có thuế";
        ws.getCell("R7").value = "KL không thuế";
        ws.getCell("S7").value = "Tổng KL";

        ws.getCell("T7").value = "Thành tiền có thuế";
        ws.getCell("U7").value = "Thành tiền không thuế";
        ws.getCell("V7").value = "Tổng tiền";

        ws.getCell("W7").value = "Có thuế";
        ws.getCell("X7").value = "Không thuế";

        // TÔ MÀU HEADER
        fillHeader(ws, 6, 7, 1, 14, headerColor);
        fillHeader(ws, 6, 7, 16, 24, headerColor);

        // ================= DATA =================
        let row = 8;
        let stt = 1;
        const loaiMap = {};
        let totalKL = 0, totalTien = 0;

        data.forEach(d => {
            ws.getCell("A" + row).value = stt++;
            ws.getCell("B" + row).value = d.ngay;
            ws.getCell("C" + row).value = d.sohd;
            ws.getCell("D" + row).value = d.ncc;
            ws.getCell("E" + row).value = d.loai;
            ws.getCell("F" + row).value = d.dvt;
            ws.getCell("G" + row).value = d.klcothue;
            ws.getCell("H" + row).value = d.klkhongthue;
            ws.getCell("I" + row).value = d.tongkl;
            ws.getCell("J" + row).value = d.ttcothue;
            ws.getCell("K" + row).value = d.ttkhongthue || "";
            ws.getCell("L" + row).value = d.tongtien;
            ws.getCell("M" + row).value = d.ttcothue / d.klcothue || "";
            ws.getCell("N" + row).value = d.ttkhongthue / d.klkhongthue || "";

            totalKL += d.tongkl;
            totalTien += d.tongtien;

            if (!loaiMap[d.loai]) {
                loaiMap[d.loai] = {
                    klcothue: 0,
                    klkhongthue: 0,
                    tongkl: 0,
                    ttcothue: 0,
                    ttkhongthue: 0,
                    tongtien: 0
                };
            }

            loaiMap[d.loai].klcothue += d.klcothue;
            loaiMap[d.loai].klkhongthue += d.klkhongthue;
            loaiMap[d.loai].tongkl += d.tongkl;

            loaiMap[d.loai].ttcothue += d.ttcothue;
            loaiMap[d.loai].ttkhongthue += d.ttkhongthue;
            loaiMap[d.loai].tongtien += d.tongtien;

            row++;
        });

        // TỔNG CỘNG
        ws.getCell("E" + row).value = "Tổng cộng";
        ws.getCell("E" + row).font = { bold: true };
        ws.getCell("I" + row).value = totalKL;
        ws.getCell("I" + row).font = { bold: true };
        ws.getCell("L" + row).value = totalTien;
        ws.getCell("L" + row).font = { bold: true };


        // PHẢI
        let rowRight = 8;
        for (const loai in loaiMap) {

            ws.getCell("P" + rowRight).value = loai;

            ws.getCell("Q" + rowRight).value = loaiMap[loai].klcothue;
            ws.getCell("R" + rowRight).value = loaiMap[loai].klkhongthue;
            ws.getCell("S" + rowRight).value = loaiMap[loai].tongkl;

            ws.getCell("T" + rowRight).value = loaiMap[loai].ttcothue;
            ws.getCell("U" + rowRight).value = loaiMap[loai].ttkhongthue;
            ws.getCell("V" + rowRight).value = loaiMap[loai].tongtien;

            ws.getCell("W" + rowRight).value =
                loaiMap[loai].klcothue
                    ? loaiMap[loai].ttcothue / loaiMap[loai].klcothue
                    : 0;

            ws.getCell("X" + rowRight).value =
                loaiMap[loai].klkhongthue
                    ? loaiMap[loai].ttkhongthue / loaiMap[loai].klkhongthue
                    : 0;

            rowRight++;
        }

        // BORDER
        applyBorder(ws, 6, row, 1, 14);
        applyBorder(ws, 6, rowRight - 1, 16, 24);

        // FOOTER
        const footerStart = Math.max(row, rowRight) + 3;

        // ===== BẢNG TRÁI =====
        ws.getCell("B" + footerStart).value = "Phòng HCTC";
        ws.getCell("J" + footerStart).value = "Kiểm soát nội bộ";

        ws.getCell("A" + (footerStart + 2)).value = "Nhân viên";
        ws.getCell("D" + (footerStart + 2)).value = "Trưởng phòng";
        ws.getCell("H" + (footerStart + 2)).value = "Nhân viên";
        ws.getCell("K" + (footerStart + 2)).value = "Trưởng phòng";


        // ===== BẢNG PHỤ (P → X) =====
        // Kiểm soát nội bộ
        ws.mergeCells(`P${footerStart}:X${footerStart}`);
        let c1 = ws.getCell(`P${footerStart}`);
        c1.value = "Kiểm soát nội bộ";
        c1.alignment = { horizontal: "center" };
        c1.font = { bold: true };

        // Nhân viên
        ws.mergeCells(`P${footerStart + 2}:S${footerStart + 2}`);
        let c2 = ws.getCell(`P${footerStart + 2}`);
        c2.value = "Nhân viên";
        c2.alignment = { horizontal: "center" };

        // Trưởng phòng
        ws.mergeCells(`T${footerStart + 2}:X${footerStart + 2}`);
        let c3 = ws.getCell(`T${footerStart + 2}`);
        c3.value = "Trưởng phòng";
        c3.alignment = { horizontal: "center" };


        // FORMAT SỐ
        ws.eachRow(r => {
            r.eachCell(c => {
                if (typeof c.value === "number") {
                    c.numFmt = "#,##0";
                }
            });
        });

        for (let i = 1; i <= 24; i++) ws.getColumn(i).width = 14;
    }

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), fileName);
}

function num(td) {
    if (!td) return 0;
    return parseFloat(td.innerText.replace(/,/g, "")) || 0;
}

function addTitle(ws, range, text) {
    ws.mergeCells(range);
    const cell = ws.getCell(range.split(":")[0]);
    cell.value = text;
    cell.font = { bold: true, size: 12 };
    cell.alignment = { horizontal: "center" };
}

function fillHeader(ws, r1, r2, c1, c2, color) {
    for (let r = r1; r <= r2; r++) {
        for (let c = c1; c <= c2; c++) {
            const cell = ws.getCell(r, c);
            cell.fill = {
                type: "pattern",
                pattern: "solid",
                fgColor: { argb: color }
            };
            cell.alignment = { horizontal: "center", vertical: "middle", wrapText: true };
            cell.font = { bold: true };
        }
    }
}

function applyBorder(ws, sr, er, sc, ec) {
    for (let r = sr; r <= er; r++) {
        for (let c = sc; c <= ec; c++) {
            ws.getCell(r, c).border = {
                top: { style: "thin" },
                left: { style: "thin" },
                bottom: { style: "thin" },
                right: { style: "thin" }
            };
        }
    }
}
