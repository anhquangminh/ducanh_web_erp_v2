using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_ContractRepository : IBaseRepository<NhanSu_Contract>
    {
        Task<List<NhanSu_ContractModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_ContractModel> GetDetails(string id);
        Task<List<NhanSu_ContractModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_Contract input);
        Task<bool> CheckEdit(NhanSu_Contract input);
        Task<bool> CheckDelete(NhanSu_Contract input);
        Task Approval(NhanSu_Contract input, string userId);
        Task NoApproval(NhanSu_Contract input, string userId);
        Task<List<NhanSu_ContractModel>> GetAllByVM(NhanSu_ContractModel input, string groupId);
    }
}
