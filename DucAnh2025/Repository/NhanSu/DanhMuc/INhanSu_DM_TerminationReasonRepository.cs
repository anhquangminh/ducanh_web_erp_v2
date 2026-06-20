using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_TerminationReasonRepository : IBaseRepository<NhanSu_DM_TerminationReason>
    {
        Task<List<NhanSu_DM_TerminationReasonModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_TerminationReasonModel> GetDetails(string id);
        Task<List<NhanSu_DM_TerminationReasonModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_TerminationReason input);
        Task<bool> CheckEdit(NhanSu_DM_TerminationReason input);
        Task<bool> CheckDelete(NhanSu_DM_TerminationReason input);
        Task Approval(NhanSu_DM_TerminationReason input, string userId);
        Task NoApproval(NhanSu_DM_TerminationReason input, string userId);
        Task<List<NhanSu_DM_TerminationReasonModel>> GetAllByVM(NhanSu_DM_TerminationReasonModel input, string groupId);
    }
}
