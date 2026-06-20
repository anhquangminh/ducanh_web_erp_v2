using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.Accounts;
using DucAnh2025.ViewModels.HeThong;

namespace DucAnh2025.Repository.HeThong
{
    public interface IPermissionControlRepository : IBaseRepository<PermissionControl>
    {
        Task<List<PermissionControlModel>> GetAllByVM(PermissionControlModel dataModel, string groupId);
        Task<List<PermissionControlModel>> GetHistoryIsValidEdit(string id);
        Task<PermissionControlModel> GetDetails(string id);
        Task<List<PermissionControlModel>> GetHistory(string id);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId);
        Task<List<MajorModel>>? GetMajorsForParentMajorId(string groupId);
        Task<List<MajorModel>>? GetMajorsForMajorId(string groupId);
        Task<List<ApplicationUserModel>>? GetApplicationUsersForUserId(string groupId);
        Task<bool> CheckSave(PermissionControl input);
        Task<bool> CheckEdit(PermissionControl input);
        Task<bool> CheckDelete(PermissionControl input);
    }
}
