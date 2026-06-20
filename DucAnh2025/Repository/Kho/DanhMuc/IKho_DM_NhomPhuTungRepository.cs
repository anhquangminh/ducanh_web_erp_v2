using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_NhomPhuTungRepository : IBaseRepository<Kho_DM_NhomPhuTung>
    {
        Task<List<Kho_DM_NhomPhuTungModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_NhomPhuTungModel> GetDetails(string id);
        Task<List<Kho_DM_NhomPhuTungModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_NhomPhuTung input);
        Task<bool> CheckEdit(Kho_DM_NhomPhuTung input);
        Task<bool> CheckDelete(Kho_DM_NhomPhuTung input);
        Task Approval(Kho_DM_NhomPhuTung input, string userId);
        Task NoApproval(Kho_DM_NhomPhuTung input, string userId);
        Task<List<Kho_DM_NhomPhuTungModel>> GetAllByVM(Kho_DM_NhomPhuTungModel input, string groupId);
    }
}

