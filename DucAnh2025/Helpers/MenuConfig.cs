public static class MenuConfig
{
    public static readonly Dictionary<string, List<MenuItem>> Menus = new()
    {
            ["QLNV"] = new List<MenuItem>
            {
                new MenuItem { Heading = "Giao việc" },
                new MenuItem
                {
                    Title = "Quản lý nhân viên",
                    Icon = "pe-7s-way",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        //new MenuItem { Title = "Nhân viên", Controller = "QLNV", Action = "NhanVien", IsPublic = false },
                        new MenuItem { Title = "Quản lý nhóm", Controller = "QLNV", Action = "QuanLyNhom", IsPublic = true },
                        new MenuItem { Title = "Đánh giá", Controller = "QLNV", Action = "DanhGia", IsPublic = true },
                        new MenuItem { Title = "Quản lý công việc", Controller = "QLNV", Action = "QuanLyCongViec", IsPublic = true },
                        new MenuItem { Title = "Công việc được giao", Controller = "QLNV", Action = "CongViecDuocGiao", IsPublic = true }
                    }
                }
            },
            ["HeThong"] = new List<MenuItem>
            {
                new MenuItem { Heading = "Hệ thống" },
                new MenuItem
                {
                    Title = "Cài đặt duyệt",
                    Icon = "pe-7s-settings",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Cài đặt phòng ban duyệt", Controller = "HeThong", Action = "CaiDatPhongBanDuyet", IsPublic = true },
                        new MenuItem { Title = "Cài đặt số lượt duyệt", Controller = "HeThong", Action = "CaiDatSoLuotDuyet", IsPublic = true },
                        new MenuItem { Title = "Phân quyền cài đặt duyệt", Controller = "HeThong", Action = "PhanQuyenCaiDatDuyet", IsPublic = true },
                        new MenuItem { Title = "Phân quyền cài đặt thao tac", Controller = "HeThong", Action = "PhanQuyenCaiDatThaoTac", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Phân quyền",
                    Icon = "pe-7s-rocket",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Phân quyền duyệt", Controller = "HeThong", Action = "PhanQuyenDuyet", IsPublic = true },
                        new MenuItem { Title = "Phân quyền tao tác", Controller = "HeThong", Action = "PhanQuyenThaoTac", IsPublic = true },
                        new MenuItem { Title = "Phân quyền theo role", Controller = "HeThong", Action = "UserRoleManager", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Danh mục",
                    Icon = "pe-7s-key",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Loại chi nhánh", Controller = "HeThong", Action = "LoaiChiNhanh", IsPublic = true },
                        new MenuItem { Title = "Chi nhánh", Controller = "HeThong", Action = "ChiNhanh", IsPublic = true },
                        new MenuItem { Title = "Nghiệp vụ", Controller = "HeThong", Action = "NghiepVu", IsPublic = true },
                    }
                },
                 new MenuItem
                {
                    Title = "Loại quyền",
                    Icon = "pe-7s-lock",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Danh sách quyền", Controller = "HeThong", Action = "DanhSachQuyen", IsPublic = true },
                    }
                },
            },
            ["NhanSu"] = new List<MenuItem>
            {

                new MenuItem { Heading = "Cấu hình & Danh mục" },
                new MenuItem
                {
                    Title = "Quản lý danh mục chung",
                    Icon = "bi bi-collection",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Loại hợp đồng", Controller = "NhanSu", Action = "DMContractType",MajorId ="3e7259bf-a79e-4620-bbd9-a50e6be58c1d" , IsPublic = false },
                        new MenuItem { Title = "Tình trạng làm việc", Controller = "NhanSu", Action = "DMWorkStatus",MajorId="f5841802-1f87-44f5-8a92-e6c55ded102d", IsPublic = false },
                        new MenuItem { Title = "Loại biến động", Controller = "NhanSu", Action = "DMChangeType", MajorId="2d08a66a-d243-4214-b09c-8b9f2831db11", IsPublic = false },
                        new MenuItem { Title = "Lý do nghỉ việc", Controller = "NhanSu", Action = "DMTerminationReason", MajorId="76af41b0-52aa-48a8-914f-28e32f00a777", IsPublic = false},
                        new MenuItem { Title = "Loại hình khen thưởng", Controller = "NhanSu", Action = "DMRewardType", MajorId="691db4be-6d3a-454d-9f8f-a42c99fb4e10", IsPublic = false},
                        new MenuItem { Title = "Hình thức kỷ luật", Controller = "NhanSu", Action = "DMDisciplineType", MajorId="9a619430-e69d-4e26-8e33-bd6e7d219188", IsPublic = false },
                    }
                },
                new MenuItem
                {
                    Title = "Cấu hình loại công & lương",
                    Icon = "bi bi-cash-stack",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Loại công", Controller = "NhanSu", Action = "DMWorkType", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Cấu hình loại đơn & phép",
                    Icon = "bi bi-journal-check",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Loại hình nghỉ phép", Controller = "NhanSu", Action = "DMLeaveType", IsPublic = true },
                        new MenuItem { Title = "Loại đơn từ", Controller = "NhanSu", Action = "DMRequestType", IsPublic = true },
                    }
                },
                
                new MenuItem
                {
                    Title = "Vị trí & Cấu trúc",
                    Icon = "bi bi-diagram-3",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Phòng ban", Controller = "NhanSu", Action = "PhongBan", IsPublic = true },
                        new MenuItem { Title = "Chức vụ", Controller = "NhanSu", Action = "ChucVu", IsPublic = true },
                        new MenuItem { Title = "Chuyên môn", Controller = "NhanSu", Action = "ChuyenMon", IsPublic = true },
                    }
                },
                new MenuItem { Heading = "Quản lý hồ sơ" },
                new MenuItem
                {
                    Title = "Hồ sơ nhân viên",
                    Icon = "bi bi-people-fill",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Tổng quan", Controller = "NhanSu", Action = "NhanSuDashboard", IsPublic = true },
                        new MenuItem { Title = "Thông tin hồ sơ", Controller = "NhanSu", Action = "NhanSuEmployeeProfile", IsPublic = true },

                        new MenuItem { Title = "Hợp đồng", Controller = "NhanSu", Action = "NhanSuContract", IsPublic = true },
                        new MenuItem { Title = "Lịch sử lương", Controller = "NhanSu", Action = "NhanSuSalaryHistory", IsPublic = true },
                        new MenuItem { Title = "Lịch sử bổ nhiểm", Controller = "NhanSu", Action = "NhanSuAppointmentsHistorys", IsPublic = true },
                        new MenuItem { Title = "Nghỉ việc", Controller = "NhanSu", Action = "NhanSuTermination", IsPublic = true },
                        new MenuItem { Title = "Khen thưởng", Controller = "NhanSu", Action = "NhanSuRewards", IsPublic = true },
                        new MenuItem { Title = "Kỷ luật", Controller = "NhanSu", Action = "NhanSuDiscipline", IsPublic = true },
                        new MenuItem { Title = "Đơn từ", Controller = "NhanSu", Action = "NhanSuRequest", IsPublic = true },
                        new MenuItem { Title = "Chấm công", Controller = "NhanSu", Action = "NhanSuTimeSheet", IsPublic = true },
                        new MenuItem { Title = "Hạn mức phép", Controller = "NhanSu", Action = "NhanSuEmployeeLeaveQuota", IsPublic = true },
                    }
                },

                new MenuItem { Heading = "Quản lý nhân viên" },
                new MenuItem
                {
                    Title = "Quản lý nhân viên",
                    Icon = "bi bi-people-fill",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Nhân viên", Controller = "NhanSu", Action = "NhanVien", IsPublic = false },
                    }
                },
            },
            ["CongTrinh"] = new List<MenuItem>
            {
                new MenuItem { Heading = "Danh Mục" },

                new MenuItem
                {
                    Title = "Danh mục chung",
                    Icon = "pe-7s-settings", 
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Trạng thái thi công", Controller = "CongTrinh", Action = "DmTrangThaiThiCong", IsPublic = true },
                        new MenuItem { Title = "Hình thức đậy hố ga", Controller = "CongTrinh", Action = "DmHinhThucDayHoGa", IsPublic = true },
                        new MenuItem { Title = "Loại đấu nối", Controller = "CongTrinh", Action = "DmLoaiDauNoi", IsPublic = true },
                        new MenuItem { Title = "Hình thức đắp trả", Controller = "CongTrinh", Action = "DmHinhThucDapTra", IsPublic = true }
                    }
                },

                new MenuItem
                {
                    Title = "Danh mục thép",
                    Icon = "pe-7s-vector",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Tên loại thép", Controller = "CongTrinh", Action = "DmTenLoaiThep", IsPublic = true },
                        new MenuItem { Title = "Danh mục thép", Controller = "CongTrinh", Action = "DmDanhMucThep", IsPublic = true }
                    }
                },

                new MenuItem
                {
                    Title = "Danh mục công việc",
                    Icon = "pe-7s-note2",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Hạng mục công việc", Controller = "CongTrinh", Action = "DmHangMucCongViec", IsPublic = true },
                        new MenuItem { Title = "Hạng mục khối lượng", Controller = "CongTrinh", Action = "DmHangMucKhoiLuong", IsPublic = true },
                        new MenuItem { Title = "Loại cấu kiện", Controller = "CongTrinh", Action = "DmLoaiCauKien", IsPublic = true },
                        new MenuItem { Title = "Loại khối lượng", Controller = "CongTrinh", Action = "DmLoaiKhoiLuong", IsPublic = true }
                    }
                },

                new MenuItem
                {
                    Title = "Danh mục tên công tác",
                    Icon = "pe-7s-id",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Danh mục tên công tác tctt", Controller = "CongTrinh", Action = "DmTenCongTac", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Danh mục vật tư",
                    Icon = "pe-7s-box1",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Thông tin vật tư", Controller = "CongTrinh", Action = "DmThongTinVatTu", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Danh mục tuyến đường",
                    Icon = "pe-7s-id",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Tuyến đường", Controller = "CongTrinh", Action = "DmTuyenDuong", IsPublic = true },
                        new MenuItem { Title = "Danh mục lý trình", Controller = "CongTrinh", Action = "DmLyTrinh", IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "PKKL",
                    Icon = "pe-7s-plugin",
                    StateIcon = "pe-7s-angle-down caret-left",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "1.TT chung N.Sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1TtChungNSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.1 L.Trình N.Sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_1LTrinhNSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.2 C.Độ đào N.Sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_2CDoDapNSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.3 C.Độ đắp N.Sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_3CDoDapNSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.4 KL đào, đắp ống nhựa dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_4KLDaoDapOngNhuaDoc", IsPublic = true },
                        new MenuItem { Title = "1.5. THKL X.Dựng nước sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_5THKLXDungNuocSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.6. THKL đào N.Sạch dọc", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_6THKLDaoNSachDoc", IsPublic = true },
                        new MenuItem { Title = "1.7 THKL đào,đắp theo loại KL", Controller = "CongTrinh", Action = "PKKL_OngNhua_1_7THKLDaoDapTheoLoaiKL", IsPublic = true },

                        new MenuItem { Title = "2.TT chung N.Sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2TtChungNSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.1 L.Trình N.Sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_1LTrinhNSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.2 C.Độ đào N.Sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_2CDoDaoNSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.3 C.Độ đắp N.Sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_3CDoDapNSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.4 Đắp trả ống nhựa ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_4DapTraOngNhuaNgang", IsPublic = true },
                        new MenuItem { Title = "2.5. THKL nước sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_5THKLNuocSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.6. THKL đào N.Sạch ngang", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_6THKLDaoNSachNgang", IsPublic = true },
                        new MenuItem { Title = "2.7 THKL đào,đắp theo loại KL", Controller = "CongTrinh", Action = "PKKL_OngNhua_2_7THKLDaoDapTheoLoaiKL", IsPublic = true },

                        new MenuItem { Title = "3.T.Tin L.Đặt van +Trụ C.Hỏa", Controller = "CongTrinh", Action = "PKKL_OngNhua_3TTinLDatVanTruCHoa", IsPublic = true },
                        new MenuItem { Title = "3.1 THKL Van+Trụ C.Hỏa", Controller = "CongTrinh", Action = "PKKL_OngNhua_3_1THKLVanTruCHoa", IsPublic = true },
                        new MenuItem { Title = "4.Thông tin chung H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4ThongTinChungHGa", IsPublic = true },
                        new MenuItem { Title = "4.1 Cao độ K.Cấu H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_1CaoDoKCauHGa", IsPublic = true },
                        new MenuItem { Title = "4.1a Cao độ đắp H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_1aCaoDoDapHGa", IsPublic = true },
                        new MenuItem { Title = "4.2 KL đào đắp H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_2KLdaoDapHGa", IsPublic = true },
                        new MenuItem { Title = "4.3 THKL đào, đắp T.uyến", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_3THKLDaoDapTTuyen", IsPublic = true },
                        new MenuItem { Title = "4.4 KTHH HG", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_4KTHHHG", IsPublic = true },
                        new MenuItem { Title = "4.5 Hình thức đấu nối", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_5HinhThucDauNoi", IsPublic = true },
                        new MenuItem { Title = "4.6 K.Hợp T.Đan - H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_6KHopTDanHGa", IsPublic = true },
                        new MenuItem { Title = "4.7 KL H.ga theo tên H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_7KLHGaTheoTenHGa", IsPublic = true },
                        new MenuItem { Title = "4.8. KTHH T.Đan", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_8KTHHTDan", IsPublic = true },
                        new MenuItem { Title = "4.9 KL TĐHG theo L.Trình", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_9KLTDHGTheoLTrinh", IsPublic = true },
                        new MenuItem { Title = "4.9a THKL TĐHG theo tên C.Tác", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_9aTHKLTDHGTheoTenCTac", IsPublic = true },
                        new MenuItem { Title = "4.9b THKL TĐHV T.Tuyến", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_9bTHKLTDHVTTuyen", IsPublic = true },
                        new MenuItem { Title = "4.9c THKL TĐHG theo loại KL", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_9cTHKLTDHGTheoLoaiKL", IsPublic = true },
                        new MenuItem { Title = "4.7a KL H.Ga theo tên C.Tác", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_7aKLHGaTheoTenCTac", IsPublic = true },
                        new MenuItem { Title = "4.7b THKL H.Ga T.Tuyến", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_7bTHKLHGaTTuyen", IsPublic = true },
                        new MenuItem { Title = "4.7c THKL H.Ga theo loại KL", Controller = "CongTrinh", Action = "PKKL_OngNhua_4_7cTHKLHGaTheoLoaiKL", IsPublic = true },

                        new MenuItem { Title = "4.7 TK thép H.Ga", Controller = "CongTrinh", Action = "PKKL_OngNhua_47TKThepHGa", IsPublic = true },
                        new MenuItem { Title = "4.7a TH thép H.Ga theo C.Tác", Controller = "CongTrinh", Action = "PKKL_OngNhua_47_aTHThepHGaTheoCTac", IsPublic = true },
                        new MenuItem { Title = "4.7b TH thép H.Ga theo L.Thép", Controller = "CongTrinh", Action = "PKKL_OngNhua_47_bTHThepHGaTheoLThep", IsPublic = true },
                        new MenuItem { Title = "4.7c TH thép H.Ga theo C.Dài", Controller = "CongTrinh", Action = "PKKL_OngNhua_47_cTHThepHGaTheoCDai", IsPublic = true },
                        new MenuItem { Title = "4.10 TK thép T.Đan", Controller = "CongTrinh", Action = "PKKL_OngNhua_410TKThepTDan", IsPublic = true },
                        new MenuItem { Title = "4.10a TH thép theo loại TĐHG", Controller = "CongTrinh", Action = "PKKL_OngNhua_410_aTHThepTheoLoaiTDHG", IsPublic = true },
                        new MenuItem { Title = "4.10b TH thép TĐHV-Tên C.Tác", Controller = "CongTrinh", Action = "PKKL_OngNhua_410_bTHThepTDHVTenCTac", IsPublic = true },
                        new MenuItem { Title = "4.10c TH Đ.Kính thép TĐHV", Controller = "CongTrinh", Action = "PKKL_OngNhua_410_cTHDKinhThepTDHV", IsPublic = true },
                        new MenuItem { Title = "4.10d TH thép TĐHV-C.Dài", Controller = "CongTrinh", Action = "PKKL_OngNhua_410_dTHThepTDHVCDai", IsPublic = true },
                        new MenuItem { Title = "4.10e TH C.Dài thanh TĐHV", Controller = "CongTrinh", Action = "PKKL_OngNhua_410_eTHCDaiThanhTDHV", IsPublic = true },

                        new MenuItem { Title = "5.THKL ống + P.Kiện", Controller = "CongTrinh", Action = "PKKL_OngNhua_5THKLongPKien", IsPublic = true },
                        new MenuItem { Title = "5.1 THKL đào, đắp", Controller = "CongTrinh", Action = "PKKL_OngNhua_5_1THKLVanTruCHoa", IsPublic = true },
                        new MenuItem { Title = "5.2 THKL Van+Trụ C.Hỏa ", Controller = "CongTrinh", Action = "PKKL_OngNhua_5_2THKLVanTruCHoa", IsPublic = true },
                        new MenuItem { Title = "5.3 THKL xây dựng", Controller = "CongTrinh", Action = "PKKL_OngNhua_5_3THKLXayDung", IsPublic = true },
                        new MenuItem { Title = "5.4 THKL thép", Controller = "CongTrinh", Action = "PKKL_OngNhua_5_4THKLThep", IsPublic = true },
                    }
                }
            },
            ["Kho"] = new List<MenuItem>
            {

                new MenuItem { Heading = "Cấu hình & Danh mục" },
                new MenuItem
                {
                    Title = "Danh mục chung",
                    Icon = "bi bi-collection",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Danh mục báo cáo", Controller = "Kho", Action = "DMDanhMucBaoCao",IsPublic = true  },
                        new MenuItem { Title = "Tên báo cáo", Controller = "Kho", Action = "DMTenBaoCao",IsPublic = true },
                        new MenuItem { Title = "Nhóm nhiên liệu", Controller = "Kho", Action = "DMNhomNhienLieu",IsPublic = true },
                        new MenuItem { Title = "Loại nhiên liệu", Controller = "Kho", Action = "DMLoaiNhienLieu",IsPublic = true },
                        new MenuItem { Title = "Nhãn hiệu", Controller = "Kho", Action = "DMNhanHieu",IsPublic = true },
                        new MenuItem { Title = "Đơn vị", Controller = "Kho", Action = "DMDonVi",IsPublic = true },
                        new MenuItem { Title = "Loại nhà cung cấp", Controller = "Kho", Action = "DMLoaiNhaCungCap",IsPublic = true },
                        new MenuItem { Title = "Nhà cung cấp", Controller = "Kho", Action = "DMNhaCungCap",IsPublic = true },
                        new MenuItem { Title = "Nhóm phụ tùng", Controller = "Kho", Action = "DMNhomPhuTung",IsPublic = true },
                        new MenuItem { Title = "Loại phụ tùng", Controller = "Kho", Action = "DMLoaiPhuTung",IsPublic = true },
                    }
                },
                new MenuItem
                {
                    Title = "Nghiệp vụ",
                    Icon = "bi bi-cash-stack",
                    StateIcon = "bi bi-chevron-down",
                    Children = new List<MenuItem>
                    {
                        new MenuItem { Title = "Hợp đồng", Controller = "Kho", Action = "KhoHDMuaNhienLieu", IsPublic = true },
                        new MenuItem { Title = "Nhập kho nhiên liệu", Controller = "Kho", Action = "NhapkhoNhienLieu", IsPublic = true },
                        new MenuItem { Title = "Xuất kho nhiên liệu", Controller = "Kho", Action = "XuatkhoNhienLieu", IsPublic = true },
                        new MenuItem { Title = "Nhập kho phụ tùng", Controller = "Kho", Action = "NhapKhoPhuTung", IsPublic = true },
                        new MenuItem { Title = "Xuất kho phụ tùng", Controller = "Kho", Action = "XuatKhoPhuTung", IsPublic = true },
                    }
                },
                
            },
    };
}