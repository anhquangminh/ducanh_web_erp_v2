using DucAnh2025.Models.NhanSu;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.NhanSu
{
    public interface IDM_ChucVuRepository : IBaseRepository<DM_ChucVu>
    {
        DM_ChucVuModel GetToEdit(string id);
        Task<List<DM_ChucVuModel>> GetAllByVM(DM_ChucVuModel dataModel, string groupId);
        Task<List<DM_ChucVuModel>> GetHistoryIsValidEdit(string id);
        Task<DM_ChucVuModel> GetDetails(string id);
        Task<List<DM_ChucVuModel>> GetHistory(string id);
        Task<bool> CheckSave(DM_ChucVu input);
        Task<bool> CheckEdit(DM_ChucVu input);
        Task<bool> CheckDelete(DM_ChucVu input);
        Task Approval(DM_ChucVu input, string userId);
        Task NoApproval(DM_ChucVu input, string userId);
    }
}
