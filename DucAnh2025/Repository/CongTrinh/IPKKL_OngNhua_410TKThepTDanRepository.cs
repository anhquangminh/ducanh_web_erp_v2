using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_410TKThepTDanRepository : IBaseRepository<PKKL_OngNhua_410TKThepTDan>
    {
        Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_410TKThepTDanModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_410TKThepTDan input);
        Task<bool> CheckEdit(PKKL_OngNhua_410TKThepTDan input);
        Task<bool> CheckDelete(PKKL_OngNhua_410TKThepTDan input);
        Task Approval(PKKL_OngNhua_410TKThepTDan input, string userId);
        Task NoApproval(PKKL_OngNhua_410TKThepTDan input, string userId);
        Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetAllByVM(PKKL_OngNhua_410TKThepTDanModel input, string groupId);
       
    }
}
