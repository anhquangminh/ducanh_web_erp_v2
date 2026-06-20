using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_WorkTypeRepository : IBaseRepository<NhanSu_DM_WorkType>
    {
        Task<List<NhanSu_DM_WorkTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_WorkTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_WorkTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_WorkType input);
        Task<bool> CheckEdit(NhanSu_DM_WorkType input);
        Task<bool> CheckDelete(NhanSu_DM_WorkType input);
        Task Approval(NhanSu_DM_WorkType input, string userId);
        Task NoApproval(NhanSu_DM_WorkType input, string userId);
        Task<List<NhanSu_DM_WorkTypeModel>> GetAllByVM(NhanSu_DM_WorkTypeModel input, string groupId);
    }
}
