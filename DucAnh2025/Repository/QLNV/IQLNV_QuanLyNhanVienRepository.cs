using DucAnh2025.ViewModel.QLNV;
using DucAnh2025.Models.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_QuanLyNhanVienRepository : IBaseRepository<QLNV_QuanLyNhanVien>
    {
        public Task<List<QLNV_QuanLyNhanVienModel>> GetByVM(string groupId, QLNV_QuanLyNhanVienModel input);
        Task<List<QLNV_QuanLyNhanVienModel>> GetQuanLyNhanVienByNhomAsync(string Id_NhomNhanVien);
        Task<List<QLNV_QuanLyNhanVien>> GetByIdNhom(string groupId, string idNhom);
        public Task<bool> CheckExist(string id, QLNV_QuanLyNhanVien input );
        public Task<bool> IsIdInUse(string id);
        Task DeleteByIdNhom(string groupId, string idNhom, string userId);
        public Task<bool> CheckExclusiveNVbyNhom(string groupId, string id_Nhom, string[] ids, DateTime baseTime);

        public Task Approval(QLNV_QuanLyNhanVien data, string userId);
        public Task NoApproval(QLNV_QuanLyNhanVien data, string userId);
    }
}
