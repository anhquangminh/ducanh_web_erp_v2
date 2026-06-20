using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.DanhMuc;
namespace DucAnh2025.Repository.DanhMuc
{
    public interface IKho_DM_LoaiNhaCungCapRepository : IBaseRepository<Kho_DM_LoaiNhaCungCap>
    {
        Task<List<Kho_DM_LoaiNhaCungCapModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_DM_LoaiNhaCungCapModel> GetDetails(string id);
        Task<List<Kho_DM_LoaiNhaCungCapModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_DM_LoaiNhaCungCap input);
        Task<bool> CheckEdit(Kho_DM_LoaiNhaCungCap input);
        Task<bool> CheckDelete(Kho_DM_LoaiNhaCungCap input);
        Task Approval(Kho_DM_LoaiNhaCungCap input, string userId);
        Task NoApproval(Kho_DM_LoaiNhaCungCap input, string userId);
        Task<List<Kho_DM_LoaiNhaCungCapModel>> GetAllByVM(Kho_DM_LoaiNhaCungCapModel input, string groupId);
    }
}
