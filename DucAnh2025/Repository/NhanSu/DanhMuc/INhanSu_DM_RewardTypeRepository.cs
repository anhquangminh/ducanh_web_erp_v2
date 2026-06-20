using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_RewardTypeRepository : IBaseRepository<NhanSu_DM_RewardType>
    {
        Task<List<NhanSu_DM_RewardTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_RewardTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_RewardTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_RewardType input);
        Task<bool> CheckEdit(NhanSu_DM_RewardType input);
        Task<bool> CheckDelete(NhanSu_DM_RewardType input);
        Task Approval(NhanSu_DM_RewardType input, string userId);
        Task NoApproval(NhanSu_DM_RewardType input, string userId);
        Task<List<NhanSu_DM_RewardTypeModel>> GetAllByVM(NhanSu_DM_RewardTypeModel input, string groupId);
    }
}
