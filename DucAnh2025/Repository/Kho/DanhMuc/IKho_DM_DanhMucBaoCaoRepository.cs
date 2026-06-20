using DucAnh2025.Models;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_DanhMucBaoCaoRepository : IBaseRepository<Kho_DM_DanhMucBaoCao>
    {
        Task<List<Kho_DM_DanhMucBaoCaoModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_DanhMucBaoCaoModel> GetDetails(string id);
        Task<List<Kho_DM_DanhMucBaoCaoModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_DanhMucBaoCao input);
        Task<bool> CheckEdit(Kho_DM_DanhMucBaoCao input);
        Task<bool> CheckDelete(Kho_DM_DanhMucBaoCao input);
        Task Approval(Kho_DM_DanhMucBaoCao input, string userId);
        Task NoApproval(Kho_DM_DanhMucBaoCao input, string userId);
        Task<List<Kho_DM_DanhMucBaoCaoModel>> GetAllByVM(Kho_DM_DanhMucBaoCaoModel input, string groupId);
    }
}
