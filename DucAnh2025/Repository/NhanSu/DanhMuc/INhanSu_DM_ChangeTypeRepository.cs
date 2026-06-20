using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_DM_ChangeTypeRepository : IBaseRepository<NhanSu_DM_ChangeType>
    {
        Task<List<NhanSu_DM_ChangeTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_ChangeTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_ChangeTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_ChangeType input);
        Task<bool> CheckEdit(NhanSu_DM_ChangeType input);
        Task<bool> CheckDelete(NhanSu_DM_ChangeType input);
        Task Approval(NhanSu_DM_ChangeType input, string userId);
        Task NoApproval(NhanSu_DM_ChangeType input, string userId);
        Task<List<NhanSu_DM_ChangeTypeModel>> GetAllByVM(NhanSu_DM_ChangeTypeModel input, string groupId);
    }
}
