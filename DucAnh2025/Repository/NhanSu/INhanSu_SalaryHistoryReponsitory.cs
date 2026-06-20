using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_SalaryHistoryRepository : IBaseRepository<NhanSu_SalaryHistory>
    {
        Task<List<NhanSu_SalaryHistoryModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_SalaryHistoryModel> GetDetails(string id);
        Task<List<NhanSu_SalaryHistoryModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_SalaryHistory input);
        Task<bool> CheckEdit(NhanSu_SalaryHistory input);
        Task<bool> CheckDelete(NhanSu_SalaryHistory input);
        Task Approval(NhanSu_SalaryHistory input, string userId);
        Task NoApproval(NhanSu_SalaryHistory input, string userId);
        Task<List<NhanSu_SalaryHistoryModel>> GetAllByVM(NhanSu_SalaryHistoryModel input, string groupId);
    }
}

