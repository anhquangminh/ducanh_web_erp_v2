using DucAnh2025.Models.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh.DanhMuc
{
     public interface IDM_TenCongTacRepository : IBaseRepository<CT_DM_TenCongTac>
    {
        Task<List<DM_TenCongTacModel>> GetHistoryIsValidEdit(string id);
        Task<DM_TenCongTacModel> GetDetails(string id);
        Task<List<DM_TenCongTacModel>> GetHistory(string id);
        Task<bool> CheckSave(CT_DM_TenCongTac input);
        Task<bool> CheckEdit(CT_DM_TenCongTac input);
        Task<bool> CheckDelete(CT_DM_TenCongTac input);
        Task Approval(CT_DM_TenCongTac input, string userId);
        Task NoApproval(CT_DM_TenCongTac input, string userId);
        Task<List<DM_TenCongTacModel>> GetAllByVM(DM_TenCongTacModel input, string groupId);
       
    }
}
