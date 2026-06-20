using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_RewardRepository : IBaseRepository<NhanSu_Reward>
    {
        Task<List<NhanSu_RewardModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_RewardModel> GetDetails(string id);
        Task<List<NhanSu_RewardModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_Reward input);
        Task<bool> CheckEdit(NhanSu_Reward input);
        Task<bool> CheckDelete(NhanSu_Reward input);
        Task Approval(NhanSu_Reward input, string userId);
        Task NoApproval(NhanSu_Reward input, string userId);
        Task<List<NhanSu_RewardModel>> GetAllByVM(NhanSu_RewardModel input, string groupId);
    }
}
