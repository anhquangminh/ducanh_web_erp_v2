using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_NhanHieuRepository : IBaseRepository<Kho_DM_NhanHieu>
    {
        Task<List<Kho_DM_NhanHieuModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_NhanHieuModel> GetDetails(string id);
        Task<List<Kho_DM_NhanHieuModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_NhanHieu input);
        Task<bool> CheckEdit(Kho_DM_NhanHieu input);
        Task<bool> CheckDelete(Kho_DM_NhanHieu input);
        Task Approval(Kho_DM_NhanHieu input, string userId);
        Task NoApproval(Kho_DM_NhanHieu input, string userId);
        Task<List<Kho_DM_NhanHieuModel>> GetAllByVM(Kho_DM_NhanHieuModel input, string groupId);
    }
}
