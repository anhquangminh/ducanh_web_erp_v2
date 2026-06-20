using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_47TKThepHGaRepository : IBaseRepository<PKKL_OngNhua_47TKThepHGa>
    {
        Task<List<PKKL_OngNhua_47TKThepHGaModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_47TKThepHGaModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_47TKThepHGaModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_47TKThepHGa input);
        Task<bool> CheckEdit(PKKL_OngNhua_47TKThepHGa input);
        Task<bool> CheckDelete(PKKL_OngNhua_47TKThepHGa input);
        Task Approval(PKKL_OngNhua_47TKThepHGa input, string userId);
        Task NoApproval(PKKL_OngNhua_47TKThepHGa input, string userId);
        Task<List<PKKL_OngNhua_47TKThepHGaModel>> GetAllByVM(PKKL_OngNhua_47TKThepHGaModel input, string groupId);
       
    }
}
