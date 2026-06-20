using DucAnh2025.Models.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_NhanVienThucHienRepository
    {
        Task<QLNV_NhanVienThucHien?> GetById(string id);
        Task<IEnumerable<QLNV_NhanVienThucHien>> GetAll(string groupId);
        Task Insert(QLNV_NhanVienThucHien entity, string userName);
        Task Update(QLNV_NhanVienThucHien entity, string userName);
        Task Delete(string id, string userName);
        Task<List<QLNV_NhanVien>> GetNhanViensByCongViec(string id_CongViec, string groupId);
    }
}