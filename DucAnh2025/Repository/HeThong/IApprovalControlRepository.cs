using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.Accounts;

namespace DucAnh2025.Repository
{
    public interface IApprovalControlRepository : IBaseRepository<ApprovalControl>
    {
        Task<List<ApprovalControlModel>> GetAllByVM(ApprovalControlModel dataModel, string groupId);
        Task<List<ApprovalControlModel>> GetHistoryIsValidEdit(string id);
        Task<ApprovalControlModel> GetDetails(string id);
        Task<List<ApprovalControlModel>> GetHistory(string id);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId);
        Task<List<MajorModel>>? GetMajorsForParentMajorId(string groupId);
        Task<List<MajorModel>>? GetMajorsForMajorId(string groupId);
        Task<List<ApplicationUserModel>>? GetApplicationUsersForUserId(string groupId);
        Task UpdateMulti(List<ApprovalControl> approvalControls, string PermissionId);
        Task<bool> CheckSave(ApprovalControl input);
        Task<bool> CheckEdit(ApprovalControl input);
        Task<bool> CheckDelete(ApprovalControl input);
    }
}