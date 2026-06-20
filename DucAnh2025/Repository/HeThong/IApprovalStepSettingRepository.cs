using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.ViewModels.HeThong;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.HeThong
{
    public interface IApprovalStepSettingRepository : IBaseRepository<ApprovalStepSetting>
    {
        ApprovalStepSettingModel GetToEdit(string id);
        Task<List<ApprovalStepSettingModel>> GetAllByVM(ApprovalStepSettingModel dataModel, string groupId);
        Task<List<ApprovalStepSettingModel>> GetHistoryIsValidEdit(string id);
        Task<ApprovalStepSettingModel> GetDetails(string id);
        Task<List<ApprovalStepSettingModel>> GetHistory(string id);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId);
        Task<List<DepartmentModel>>? GetDepartmentsForDeptId(string groupId);
        Task<List<MajorModel>>? GetMajorsForParentMajorId(string groupId);
        Task<List<MajorModel>>? GetMajorsForMajorId(string groupId);
        Task<List<PermissionModel>>? GetPermissionsForPermissionId(string groupId);
        Task<bool> CheckSave(ApprovalStepSetting input);
        Task<bool> CheckEdit(ApprovalStepSetting input);
        Task<bool> CheckDelete(ApprovalStepSetting input);
        Task Approval(ApprovalStepSetting input, string userId);
        Task NoApproval(ApprovalStepSetting input, string userId);


        List<ApprovalStepSettingModel> ListStepSetting(string groupId, string majorId, string departmentId, string permissionId);
        Task<List<Permission>> LoadPermissionsByMajorUserApproval(MajorUserApproval input);
        Task<List<Major>> LoadParentMajors(string companyId);
        Task<List<Major>> LoadMajors(string companyId, string parentMajorId);
        Task<List<Permission>> LoadPermissionsByApprovalControl(ApprovalControl input);
        Task<List<ApprovalStepSetting>> LoadStepByApprovalControl(ApprovalControl input);
        Task<List<ApprovalStepSetting>> LoadStepByMajorUserApproval(MajorUserApproval input);
        Task<List<Department>> LoadDepartments(string companyId, string parentMajorId, string majorId);
        ApprovalDeptSetting GetIdApprodeptSetting(string companyId, string majorId, string deptId);
        Task<bool> CreateApprovalStepSetting(List<ApprovalStepWrapper> approvalRowWrappers, DateTime baseTime);
        Task<bool> DeleteApprovalStepSetting(string mainId);
        Task<List<ApprovalStepSetting>> GetByMainId(string mainId);
        Task<List<ChiNhanh>> CheckChoDuyet(string id);
        Task<bool> CheckSave(string companyId, string majorId, string deptId, string permissionId, string mainId, bool loai);
    }
}
