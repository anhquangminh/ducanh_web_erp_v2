using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_LeaveTypeRepository : IBaseRepository<NhanSu_DM_LeaveType>
    {
        Task<List<NhanSu_DM_LeaveTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_LeaveTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_LeaveTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_LeaveType input);
        Task<bool> CheckEdit(NhanSu_DM_LeaveType input);
        Task<bool> CheckDelete(NhanSu_DM_LeaveType input);
        Task Approval(NhanSu_DM_LeaveType input, string userId);
        Task NoApproval(NhanSu_DM_LeaveType input, string userId);
        Task<List<NhanSu_DM_LeaveTypeModel>> GetAllByVM(NhanSu_DM_LeaveTypeModel input, string groupId);
    }
}
