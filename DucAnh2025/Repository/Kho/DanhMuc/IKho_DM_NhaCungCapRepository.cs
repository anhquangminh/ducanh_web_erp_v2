using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_NhaCungCapRepository : IBaseRepository<Kho_DM_NhaCungCap>
    {
        Task<List<Kho_DM_NhaCungCapModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_NhaCungCapModel> GetDetails(string id);
        Task<List<Kho_DM_NhaCungCapModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_NhaCungCap input);
        Task<bool> CheckEdit(Kho_DM_NhaCungCap input);
        Task<bool> CheckDelete(Kho_DM_NhaCungCap input);
        Task Approval(Kho_DM_NhaCungCap input, string userId);
        Task NoApproval(Kho_DM_NhaCungCap input, string userId);
        Task<List<Kho_DM_NhaCungCapModel>> GetAllByVM(Kho_DM_NhaCungCapModel input, string groupId);
    }
}
