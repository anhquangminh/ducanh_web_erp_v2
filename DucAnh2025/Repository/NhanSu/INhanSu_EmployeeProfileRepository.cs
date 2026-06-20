using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_EmployeeProfileRepository : IBaseRepository<NhanSu_EmployeeProfile>
    {
        Task<List<NhanSu_EmployeeProfileModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_EmployeeProfileModel> GetDetails(string id);
        Task<List<NhanSu_EmployeeProfileModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_EmployeeProfile input);
        Task<bool> CheckEdit(NhanSu_EmployeeProfile input);
        Task<bool> CheckDelete(NhanSu_EmployeeProfile input);
        Task Approval(NhanSu_EmployeeProfile input, string userId);
        Task NoApproval(NhanSu_EmployeeProfile input, string userId);
        Task<List<NhanSu_EmployeeProfileModel>> GetAllByVM(NhanSu_EmployeeProfileModel input, string groupId);
    }
}

