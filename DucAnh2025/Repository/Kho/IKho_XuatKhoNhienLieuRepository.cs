using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels;
namespace DucAnh2025.Repository
{
    public interface IKho_XuatKhoNhienLieuRepository : IBaseRepository<Kho_XuatKhoNhienLieu>
    {
        Task<List<Kho_XuatKhoNhienLieuModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_XuatKhoNhienLieuModel> GetDetails(string id);
        Task<List<Kho_XuatKhoNhienLieuModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_XuatKhoNhienLieu input);
        Task<bool> CheckEdit(Kho_XuatKhoNhienLieu input);
        Task<bool> CheckDelete(Kho_XuatKhoNhienLieu input);
        Task Approval(Kho_XuatKhoNhienLieu input, string userId);
        Task NoApproval(Kho_XuatKhoNhienLieu input, string userId);
        Task<List<Kho_XuatKhoNhienLieuModel>> GetAllByVM(Kho_XuatKhoNhienLieuModel input, string groupId);
    }
}
