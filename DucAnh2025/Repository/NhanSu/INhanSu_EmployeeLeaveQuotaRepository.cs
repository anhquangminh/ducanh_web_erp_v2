using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_EmployeeLeaveQuotaRepository : IBaseRepository<NhanSu_EmployeeLeaveQuota>
    {
        Task<List<NhanSu_EmployeeLeaveQuotaModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_EmployeeLeaveQuotaModel> GetDetails(string id);
        Task<List<NhanSu_EmployeeLeaveQuotaModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_EmployeeLeaveQuota input);
        Task<bool> CheckEdit(NhanSu_EmployeeLeaveQuota input);
        Task<bool> CheckDelete(NhanSu_EmployeeLeaveQuota input);
        Task Approval(NhanSu_EmployeeLeaveQuota input, string userId);
        Task NoApproval(NhanSu_EmployeeLeaveQuota input, string userId);
        Task<List<NhanSu_EmployeeLeaveQuotaModel>> GetAllByVM(NhanSu_EmployeeLeaveQuotaModel input, string groupId);
    }
}
