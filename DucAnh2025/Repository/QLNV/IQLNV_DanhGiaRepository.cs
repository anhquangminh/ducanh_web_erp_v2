using DucAnh2025.Models.QLNV;
using DucAnh2025.ViewModel.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_DanhGiaRepository : IBaseRepository<QLNV_DanhGia>
    {
        public Task<List<QLNV_DanhGiaModel>> GetByVM(string groupId, QLNV_DanhGiaModel input, string Id_NguoiGiaoViec);
        public Task<bool> CheckExist(string id, QLNV_DanhGia input );
        public Task<bool> IsIdInUse(string id);
        Task<QLNV_DanhGia> GetByIdCongViec(string Id_CongViec);
    }
}
