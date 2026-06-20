using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;

namespace DucAnh2025.Repository.HeThong
{
    public interface IApprovalDeptSettingRepository : IBaseRepository<ApprovalDeptSetting>
    {
        ApprovalDeptSettingModel GetToEdit(string id);
        Task<List<ApprovalDeptSettingModel>> GetAllByVM(ApprovalDeptSettingModel dataModel, string groupId);
        Task<List<ApprovalDeptSettingModel>> GetHistoryIsValidEdit(string id);
        Task<ApprovalDeptSettingModel> GetDetails(string id);
        Task<List<ApprovalDeptSettingModel>> GetHistory(string id);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId);
        Task<List<DepartmentModel>>? GetDepartmentsForDeptId(string groupId);
        Task<List<MajorModel>>? GetMajorsForParentMajorId(string groupId);
        Task<List<MajorModel>>? GetMajorsForMajorId(string groupId);
        Task<bool> CheckEdit(ApprovalDeptSetting input);
        Task Approval(ApprovalDeptSetting input, string userId);
        Task NoApproval(ApprovalDeptSetting input, string userId);
        Task<List<ChiNhanhModel>> GetChiNhanhs(string groupId);
        Task<List<DepartmentModel>> GetDepartments(string groupId);
        Task<List<MajorModel>> GetMajors(string groupId);
        Task<List<MajorModel>> GetMajorsByParentId(string parentId);
        Task<bool> CreateApprovalDeptSetting(List<ApprovalDeptWrapper> approvalRowWrappers, DateTime baseTime);
        Task<List<ApprovalDeptSettingModel>> GetSetApprovalDept(string companyId, string majorId, string screenId);
        Task<List<ApprovalDeptSettingData>> GetData(string groupId);
        Task<List<ApprovalDeptSetting>> GetByMainId(string mainId);
        List<Department> ListDept(string companyId);
        Task<bool> DeleteApprovalDeptSetting(string mainId);
        Task<bool> CheckSave(string companyId, string majorId, string mainId, bool loai);
        Task<bool> CheckDelete(string companyId, string majorId);
        Task<List<ChiNhanh>> CheckChoDuyet(string id);
    }

}
