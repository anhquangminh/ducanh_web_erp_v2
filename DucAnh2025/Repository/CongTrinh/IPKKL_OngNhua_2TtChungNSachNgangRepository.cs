using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh
{
     public interface IPKKL_OngNhua_1TtChungNSachDocRepository : IBaseRepository<PKKL_OngNhua_1TtChungNSachDoc>
    {
        Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetHistoryIsValidEdit(string id);
        Task<PKKL_OngNhua_1TtChungNSachDocModel> GetDetails(string id);
        Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetHistory(string id);
        Task<bool> CheckSave(PKKL_OngNhua_1TtChungNSachDoc input);
        Task<bool> CheckEdit(PKKL_OngNhua_1TtChungNSachDoc input);
        Task<bool> CheckDelete(PKKL_OngNhua_1TtChungNSachDoc input);
        Task Approval(PKKL_OngNhua_1TtChungNSachDoc input, string userId);
        Task NoApproval(PKKL_OngNhua_1TtChungNSachDoc input, string userId);
        Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetAllByVM(PKKL_OngNhua_1TtChungNSachDocModel input, string groupId);
       
    }
}
