async function exportExcel() {
    await exportExcelCustom("table_data", [
        { text: "CÔNG TY CỔ PHẦN XÂY DỰNG ĐỨC ANH", cols: 5, align: "left", fontSize: 12, bold: true },
        { text: "KHU CÔNG NGHIỆP CẨM KHÊ", cols: 5, align: "left", fontSize: 12, bold: true },
        { text: "", cols: 10 },
        { text: "BẢNG THỐNG KÊ KHỐI LƯỢNG ĐÀO, ĐẮP, VẬN CHUYỂN ĐẤT THỪA ỐNG NƯỚC SẠCH DỌC ", align: "center", fontSize: 12, bold: true },
        { text: "Mẫu số 04", align: "center", fontSize: 12, bold: true },
        { text: "In theo từng tuyến", align: "center", fontSize: 11 }
    ], "1.4 KL đào, đắp ống nhựa dọc", "1.4 KL đào, đắp ống nhựa dọc.xlsx");
}
let sourceData = null;
$(function () {
    loadData();
});

function loadData() {
    showBlockLoading('.table-responsive');
    getJwtToken().then(async function (resUser) {
        await $.ajax({
            url: `/api/PKKLOngNhua1TtChungNSachDoc/GetByVM?groupId=${resUser.userInfor.groupId}`,
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
                    tenChiNhanh: item.tenChiNhanh || '',

                    tuyenDuong: item.tuyenDuong || '',
                    tuLyTrinh: item.tuLyTrinh || '',
                    denLyTrinh: item.denLyTrinh || '',
                    hangMucCongViec: item.hangMucCongViec || '',
                    loaiCauKien: item.loaiCauKien || '0',
                    hangMucKlDaoDat: item.hangMucKlDaoDat || '0',
                    loaiKlDaoDat: item.loaiKlDaoDat || '0',
                    klDao: item.klDao || '0',
                    hangMucKlDapCat: item.hangMucKlDapCat || '',
                    loaiKlDapCat: item.loaiKlDapCat || '',
                    klDapCatSauCCho: item.klDapCatSauCCho || '0',
                    hangMucKlDapDat: item.hangMucKlDapDat || '',
                    loaiKlDapDat: item.loaiKlDapDat || '',
                    klDapDatSauCCho: item.klDapDatSauCCho || '0',
                    hangMucKlDatThua: item.hangMucKlDatThua || '',
                    loaiKlDatThua: item.loaiKlDatThua || '',
                    klDatThua: item.klDatThua || '0',

                    isStatus: item.isStatus || '',
                    isActive: item.isActive || '',
                }));
            }

            if (sourceData) {
                sourceData = $('#table_data').DataTable({
                    data: dataSet,
                    columns: [
                        { data: 'stt', className: 'text-center align-middle', width: '1%' },
                        { data: 'isStatus', className: 'align-middle' },
                        { data: 'tenChiNhanh', className: 'align-middle' },

                        { data: 'tuyenDuong', className: 'text-center align-middle', width: '1%' },
                        { data: 'tuLyTrinh', className: 'text-center align-middle', width: '1%' },
                        { data: 'denLyTrinh', className: 'text-center align-middle', width: '1%' },
                        { data: 'hangMucCongViec', className: 'text-center align-middle', width: '1%' },
                        { data: 'loaiCauKien', className: 'text-center align-middle', width: '1%' },
                        { data: 'hangMucKlDaoDat', className: 'text-center align-middle', width: '1%' },
                        { data: 'loaiKlDaoDat', className: 'text-center align-middle', width: '1%' },
                        { data: 'klDao', className: 'text-center align-middle', width: '1%' },
                        { data: 'hangMucKlDapCat', className: 'text-center align-middle', width: '1%' },
                        { data: 'loaiKlDapCat', className: 'text-center align-middle', width: '1%' },
                        { data: 'klDapCatSauCCho', className: 'text-center align-middle', width: '1%' },
                        { data: 'hangMucKlDapDat', className: 'text-center align-middle', width: '1%' },
                        { data: 'loaiKlDapDat', className: 'text-center align-middle', width: '1%' },
                        { data: 'klDapDatSauCCho', className: 'text-center align-middle', width: '1%' },
                        { data: 'hangMucKlDatThua', className: 'text-center align-middle', width: '1%' },
                        { data: 'loaiKlDatThua', className: 'text-center align-middle', width: '1%' },
                        { data: 'klDatThua', className: 'text-center align-middle', width: '1%' },

                    ],
                    autoWidth: false,
                    scrollX: false,
                    ordering: true,
                    order: [[0, 'asc']],
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
                        this.api().columns().every(function (colIdx) {
                            var column = this;
                            var $footer = $(column.footer()).empty();
                            if (colIdx === 0 || colIdx === 1) {
                                $footer.append('<p></p>');
                            } else {
                                var select = $('<select><option value=""></option></select>')
                                    .appendTo($footer)
                                    .on('change', function () {
                                        var val = $.fn.dataTable.util.escapeRegex($(this).val());
                                        column
                                            .search(val ? '^' + val + '$' : '', true, false)
                                            .draw();
                                    });

                                column.data().unique().sort().each(function (d) {
                                    select.append('<option value="' + d + '">' + d + '</option>');
                                });
                            }

                        });
                    }

                });
            } else {
                sourceData.clear();
                sourceData.rows.add(dataSet);
                sourceData.draw(false);
            }
        }).fail(function (xhr) {
            var colCount = $('#table_data thead th').length;
            $('#table_data tbody').html('<tr><td colspan="' + colCount + '" class="text-left text-danger">Lỗi khi tải dữ liệu.</td></tr>');
        }).always(function () {
            hideBlockLoading('.table-responsive');
        });
    });
}


// Hiển thị lỗi
function showError(selector, message) {
    const $input = $(selector);
    $input.addClass('is-invalid');
    if ($input.next('.invalid-feedback').length === 0) {
        $input.after(`<div class="invalid-feedback">${message}</div>`);
    }
}
