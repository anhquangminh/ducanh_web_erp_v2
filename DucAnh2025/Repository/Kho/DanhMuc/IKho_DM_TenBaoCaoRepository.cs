using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_TenBaoCaoRepository : IBaseRepository<Kho_DM_TenBaoCao>
    {
        Task<List<Kho_DM_TenBaoCaoModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_TenBaoCaoModel> GetDetails(string id);
        Task<List<Kho_DM_TenBaoCaoModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_TenBaoCao input);
        Task<bool> CheckEdit(Kho_DM_TenBaoCao input);
        Task<bool> CheckDelete(Kho_DM_TenBaoCao input);
        Task Approval(Kho_DM_TenBaoCao input, string userId);
        Task NoApproval(Kho_DM_TenBaoCao input, string userId);
        Task<List<Kho_DM_TenBaoCaoModel>> GetAllByVM(Kho_DM_TenBaoCaoModel input, string groupId);
    }
}
