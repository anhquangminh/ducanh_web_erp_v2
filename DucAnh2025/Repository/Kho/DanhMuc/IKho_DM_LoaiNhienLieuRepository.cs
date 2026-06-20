using DucAnh2025.Models.Kho.DanhMuc;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_LoaiNhienLieuRepository : IBaseRepository<Kho_DM_LoaiNhienLieu>
    {
        Task<List<Kho_DM_LoaiNhienLieuModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_LoaiNhienLieuModel> GetDetails(string id);
        Task<List<Kho_DM_LoaiNhienLieuModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_LoaiNhienLieu input);
        Task<bool> CheckEdit(Kho_DM_LoaiNhienLieu input);
        Task<bool> CheckDelete(Kho_DM_LoaiNhienLieu input);
        Task Approval(Kho_DM_LoaiNhienLieu input, string userId);
        Task NoApproval(Kho_DM_LoaiNhienLieu input, string userId);
        Task<List<Kho_DM_LoaiNhienLieuModel>> GetAllByVM(Kho_DM_LoaiNhienLieuModel input, string groupId);
    }
}
