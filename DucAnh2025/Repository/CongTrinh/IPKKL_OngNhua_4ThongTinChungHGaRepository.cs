using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_4ThongTinChungHGaRepository : IBaseRepository<PKKL_OngNhua_4ThongTinChungHGa>
    {
        Task<List<PKKL_OngNhua_4ThongTinChungHGaModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_4ThongTinChungHGaModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_4ThongTinChungHGaModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_4ThongTinChungHGa input);
        Task<bool> CheckEdit(PKKL_OngNhua_4ThongTinChungHGa input);
        Task<bool> CheckDelete(PKKL_OngNhua_4ThongTinChungHGa input);
        Task Approval(PKKL_OngNhua_4ThongTinChungHGa input, string userId);
        Task NoApproval(PKKL_OngNhua_4ThongTinChungHGa input, string userId);
        Task<List<PKKL_OngNhua_4ThongTinChungHGaModel>> GetAllByVM(PKKL_OngNhua_4ThongTinChungHGaModel input, string groupId);
        Task<List<LoaiHoVan>> GetTenHoVan(string groupId,string id_ChiNhanh);
    }
}
