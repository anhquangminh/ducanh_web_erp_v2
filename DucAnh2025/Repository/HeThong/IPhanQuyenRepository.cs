using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.HeThong
{
    public interface IPhanQuyenRepository
    {
        Task<bool> CheckPermission(string groupId, string companyId, ApplicationUser user, string permissionId);
        Task<List<PermissionModel>> getAllPermissionByMajor(string groupId, string companyId, ApplicationUser user, string majorId);
        Task<bool> CheckApproval(string companyId, string deptId, ApplicationUser user, string approvalId);
        Task<ApprovalStepSetting> GetApprovalStepSettingById(string id);
        Task<ApprovalStepSetting> GetFirstApprovalStep(string companyId, string majorId, string permissionId);
        Task<ApprovalStepSetting> GetNextApprovalStep(string companyId, string majorId, string permissionId, string departmentId, int departmentOrder, int approvalOrder);
        Task<ApprovalStepSetting> GetLastApprovalStep(string companyId, string majorId, string permissionId);
        Task<List<ApprovalStaffSetting>> GetApprovalStaffByApprovalStepId(string approvalStepId);
        Task<List<PermissionModel>> GetAllPermissionByGroupId(string groupId, string userId, string parentMajorId, string majorId);
    }
}
