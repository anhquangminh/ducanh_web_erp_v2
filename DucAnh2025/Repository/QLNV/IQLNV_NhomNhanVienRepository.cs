using DucAnh2025.ViewModel.QLNV;
using DucAnh2025.Models.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_NhomNhanVienRepository : IBaseRepository<QLNV_NhomNhanVien>
    {
        public Task<List<QLNV_NhomNhanVienModel>> GetByVM(string groupId, QLNV_NhomNhanVienModel input);
        Task<List<QLNV_NhomNhanVienModel>> GetNhomNhanVienByTaiKhoanAsync(string groupId, string companyId, string taiKhoan);
        Task<List<QLNV_NhomNhanVienModel>> GetNhomNhanVienByCVDGAsync(string groupId ,string taiKhoan);
        public Task<bool> CheckExist(string id, QLNV_NhomNhanVien input );
        public Task<bool> IsIdInUse(string id);
        public Task Approval(QLNV_NhomNhanVien data, string userId);
        public Task NoApproval(QLNV_NhomNhanVien data, string userId);
    }
}
