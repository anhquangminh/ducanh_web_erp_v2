using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface Ikho_DM_LoaiPhuTungRepository : IBaseRepository<kho_DM_LoaiPhuTung>
    {
        Task<List<kho_DM_LoaiPhuTungModel>> GetHistoryIsValidEdit(string id);
        Task<kho_DM_LoaiPhuTungModel> GetDetails(string id);
        Task<List<kho_DM_LoaiPhuTungModel>> GetHistory(string id);
        Task<bool> CheckSave(kho_DM_LoaiPhuTung input);
        Task<bool> CheckEdit(kho_DM_LoaiPhuTung input);
        Task<bool> CheckDelete(kho_DM_LoaiPhuTung input);
        Task Approval(kho_DM_LoaiPhuTung input, string userId);
        Task NoApproval(kho_DM_LoaiPhuTung input, string userId);
        Task<List<kho_DM_LoaiPhuTungModel>> GetAllByVM(kho_DM_LoaiPhuTungModel input, string groupId);
    }
}
