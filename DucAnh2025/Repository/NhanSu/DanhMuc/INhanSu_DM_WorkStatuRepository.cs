using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_WorkStatuRepository : IBaseRepository<NhanSu_DM_WorkStatu>
    {
        Task<List<NhanSu_DM_WorkStatuModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_WorkStatuModel> GetDetails(string id);
        Task<List<NhanSu_DM_WorkStatuModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_WorkStatu input);
        Task<bool> CheckEdit(NhanSu_DM_WorkStatu input);
        Task<bool> CheckDelete(NhanSu_DM_WorkStatu input);
        Task Approval(NhanSu_DM_WorkStatu input, string userId);
        Task NoApproval(NhanSu_DM_WorkStatu input, string userId);
        Task<List<NhanSu_DM_WorkStatuModel>> GetAllByVM(NhanSu_DM_WorkStatuModel input, string groupId);
    }
}

