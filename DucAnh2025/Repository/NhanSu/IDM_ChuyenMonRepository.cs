using DucAnh2025.Models.NhanSu;
using DucAnh2025.ViewModels;

namespace DucAnh2025.Repository.NhanSu
{
    public interface IDM_ChuyenMonRepository : IBaseRepository<DM_ChuyenMon>
    {
        DM_ChuyenMonModel GetToEdit(string id);
        Task<List<DM_ChuyenMonModel>> GetAllByVM(DM_ChuyenMonModel dataModel, string groupId);
        Task<List<DM_ChuyenMonModel>> GetHistoryIsValidEdit(string id);
        Task<DM_ChuyenMonModel> GetDetails(string id);
        Task<List<DM_ChuyenMonModel>> GetHistory(string id);
        Task<bool> CheckSave(DM_ChuyenMon input);
        Task<bool> CheckEdit(DM_ChuyenMon input);
        Task<bool> CheckDelete(DM_ChuyenMon input);
        Task Approval(DM_ChuyenMon input, string userId);
        Task NoApproval(DM_ChuyenMon input, string userId);
    }
}
