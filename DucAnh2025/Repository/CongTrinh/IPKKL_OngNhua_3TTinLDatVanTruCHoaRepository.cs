using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_3TTinLDatVanTruCHoaRepository : IBaseRepository<PKKL_OngNhua_3TTinLDatVanTruCHoa>
    {
        Task<List<PKKL_OngNhua_3TTinLDatVanTruCHoaModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_3TTinLDatVanTruCHoaModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_3TTinLDatVanTruCHoaModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_3TTinLDatVanTruCHoa input);
        Task<bool> CheckEdit(PKKL_OngNhua_3TTinLDatVanTruCHoa input);
        Task<bool> CheckDelete(PKKL_OngNhua_3TTinLDatVanTruCHoa input);
        Task Approval(PKKL_OngNhua_3TTinLDatVanTruCHoa input, string userId);
        Task NoApproval(PKKL_OngNhua_3TTinLDatVanTruCHoa input, string userId);
        Task<List<PKKL_OngNhua_3TTinLDatVanTruCHoaModel>> GetAllByVM(PKKL_OngNhua_3TTinLDatVanTruCHoaModel input, string groupId);
       
    }
}
