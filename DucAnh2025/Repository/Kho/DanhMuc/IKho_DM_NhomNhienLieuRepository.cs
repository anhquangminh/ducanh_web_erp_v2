using DucAnh2025.Models;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_NhomNhienLieuRepository : IBaseRepository<Kho_DM_NhomNhienLieu>
    {
        Task<List<Kho_DM_NhomNhienLieuModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_NhomNhienLieuModel> GetDetails(string id);
        Task<List<Kho_DM_NhomNhienLieuModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_NhomNhienLieu input);
        Task<bool> CheckEdit(Kho_DM_NhomNhienLieu input);
        Task<bool> CheckDelete(Kho_DM_NhomNhienLieu input);
        Task Approval(Kho_DM_NhomNhienLieu input, string userId);
        Task NoApproval(Kho_DM_NhomNhienLieu input, string userId);
        Task<List<Kho_DM_NhomNhienLieuModel>> GetAllByVM(Kho_DM_NhomNhienLieuModel input, string groupId);
    }
}
