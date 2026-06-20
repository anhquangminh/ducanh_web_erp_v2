using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_DonViRepository : IBaseRepository<Kho_DM_DonVi>
    {
        Task<List<Kho_DM_DonViModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_DonViModel> GetDetails(string id);
        Task<List<Kho_DM_DonViModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_DonVi input);
        Task<bool> CheckEdit(Kho_DM_DonVi input);
        Task<bool> CheckDelete(Kho_DM_DonVi input);
        Task Approval(Kho_DM_DonVi input, string userId);
        Task NoApproval(Kho_DM_DonVi input, string userId);
        Task<List<Kho_DM_DonViModel>> GetAllByVM(Kho_DM_DonViModel input, string groupId);
    }
}
