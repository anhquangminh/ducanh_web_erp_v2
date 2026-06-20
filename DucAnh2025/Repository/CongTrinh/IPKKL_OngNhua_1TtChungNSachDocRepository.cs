using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_2TtChungNSachNgangRepository : IBaseRepository<PKKL_OngNhua_2TtChungNSachNgang>
    {
        Task<List<PKKL_OngNhua_2TtChungNSachNgangModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_2TtChungNSachNgangModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_2TtChungNSachNgangModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_2TtChungNSachNgang input);
        Task<bool> CheckEdit(PKKL_OngNhua_2TtChungNSachNgang input);
        Task<bool> CheckDelete(PKKL_OngNhua_2TtChungNSachNgang input);
        Task Approval(PKKL_OngNhua_2TtChungNSachNgang input, string userId);
        Task NoApproval(PKKL_OngNhua_2TtChungNSachNgang input, string userId);
        Task<List<PKKL_OngNhua_2TtChungNSachNgangModel>> GetAllByVM(PKKL_OngNhua_2TtChungNSachNgangModel input, string groupId);
       
    }
}
