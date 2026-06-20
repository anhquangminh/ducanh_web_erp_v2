using DucAnh2025.Models.QLNV;
using DucAnh2025.ViewModel.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_NhanVienRepository : IBaseRepository<QLNV_NhanVien>
    {
        public Task<List<QLNV_NhanVienModel>> GetByVM(string groupId, QLNV_NhanVienModel input);
        public Task<List<QLNV_NhanVien>> GetNhanVienIsQuanLy(string groupId, bool isQuanLy);
        public Task<List<QLNV_NhanVien>> GetNhanVienByNhom(string groupId, string companyId, string Id_NhomNhanVien);
        public Task<List<QLNV_NhanVien>> GetNhanVienNotQL(string groupId,string companyId, string Id_NhomNhanVien);
        public Task<List<string>> GetByIdApplicationUser(string[] ids);
        public Task<QLNV_NhanVien> GetNhanVienByTaiKhoan(string taiKhoan);
        public Task<bool> CheckExist(string id, QLNV_NhanVien input );
        public Task<bool> IsIdInUse(string id);
        Task Approval(QLNV_NhanVien input, string userId);
        Task NoApproval(QLNV_NhanVien input, string userId);
    }
}
