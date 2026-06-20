using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_RequestTypeRepository : IBaseRepository<NhanSu_DM_RequestType>
    {
        Task<List<NhanSu_DM_RequestTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_RequestTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_RequestTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_RequestType input);
        Task<bool> CheckEdit(NhanSu_DM_RequestType input);
        Task<bool> CheckDelete(NhanSu_DM_RequestType input);
        Task Approval(NhanSu_DM_RequestType input, string userId);
        Task NoApproval(NhanSu_DM_RequestType input, string userId);
        Task<List<NhanSu_DM_RequestTypeModel>> GetAllByVM(NhanSu_DM_RequestTypeModel input, string groupId);
    }
}
