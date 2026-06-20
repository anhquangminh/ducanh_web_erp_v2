using DucAnh2025.ViewModel.QLNV;
using DucAnh2025.Models.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_CongViecRepository : IBaseRepository<QLNV_CongViec>
    {
        public Task<List<QLNV_CongViecModel>> GetByVM(string groupId, QLNV_CongViecModel input);
        public Task<bool> CheckExist(string id, QLNV_CongViec input );
        public Task<bool> IsIdInUse(string id);
        //cvc
        public Task InsertCVC(QLNV_CongViecCon entity, string userId);
        public Task<List<QLNV_CongViecCon>> GetAllCVC();
        public Task<QLNV_CongViecCon> GetByIdCVC(string id);
        public  Task<List<QLNV_CongViecCon>> GetByIdCongViecCVC(string id_task);
        public  Task DeleteByIdCVC(string id, string userId);
        public  Task DeleteByIdCongViecCVC(string Id_Task, string userId);
        public  Task UpdateCVC(QLNV_CongViecCon data, string userId);
        public Task<bool> CheckExistCVC(string id, QLNV_CongViecCon input);
        public Task<bool> CheckExclusiveCVC(string[] ids, DateTime baseTime);
        //NVTH
        public  Task InsertNVTH(QLNV_NhanVienThucHien entity, string userId);
        public  Task<List<QLNV_NhanVienThucHien>> GetByIdCongViecNVTH(string Id_CongViec);
        public Task DeleteByIdCongViecNVTH(string Id_CongViec, string userId);
        public Task UpdateNVTH(QLNV_NhanVienThucHien data, string userId);
        public Task<QLNV_NhanVienThucHien> GetByIdNVTH(string id);
        public Task<QLNV_NhanVienThucHien> GetIdNVTHByIdCongViec(string Id_CongViec, string userName);
        public Task<List<QLNV_NhanVienThucHien>> GetAllNVTH(string groupId, QLNV_NhanVienThucHien input);

        //Thêm ngày
        public Task<List<QLNV_CongViec>> GetCVByIdNhanVien(string groupId, string[] Id_NhanVien);
        public Task InsertThemNgay(QLNV_ThemNgay entity, string userId);
        public Task<List<QLNV_ThemNgay>> GetByIdThemNgay(string Id);
        public  Task<QLNV_ThemNgay> GetByIdCV(string Id);
        public Task DeleteByIdThemNgay(string Id, string userId);
        public Task DeleteByIdCVThemNgay(string Id, string userId);

        //Báo cáo công việc theo nguời giao việc
        public Task<CongViecStatusReport> GetStatusReport(string groupId, string id_NguoiGiaoViec);
        public Task<List<CongViecByNhomReport>> GetBaoCaoTheoNhom(string groupId, string id_NguoiGiaoViec);
        public Task<TienDoTrungBinhReport> GetTienDoTrungBinh(string groupId, string id_NguoiGiaoViec);
        public Task<List<SoLuongTheoUuTienReport>> GetSoLuongTheoUuTien(string groupId, string id_NguoiGiaoViec);
        public Task<List<SoLuongTheoThoiGianReport>> GetSoLuongTheoThoiGian(string groupId, string id_NguoiGiaoViec);

        //Báo cáo công việc theo nguời thực hiện
        public Task<CongViecStatusReport> GetStatusReportNVTH(string groupId, string idNhanVien);
        public Task<List<CongViecTheoUuTienReport>> GetBaoCaoTheoUuTienNVTH(string groupId, string idNhanVien);
        public Task<CongViecDanhGiaReport> GetBaoCaoDanhGiaNVTH(string groupId, string idNhanVien);
        public Task<CongViecThoiHanReport> GetBaoCaoThoiHanNVTH(string groupId, string idNhanVien, int soNgayCanhBao = 3);
        public Task<TienDoTrungBinhNVTHReport> GetTienDoTrungBinhNVTH(string groupId, string idNhanVien);
    }
}
