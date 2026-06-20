using DucAnh2025.Models.NhanSu;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_TimeSheetRepository : IBaseRepository<NhanSu_TimeSheet>
    {
        Task<List<NhanSu_TimeSheetModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_TimeSheetModel> GetDetails(string id);
        Task<List<NhanSu_TimeSheetModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_TimeSheet input);
        Task<bool> CheckEdit(NhanSu_TimeSheet input);
        Task<bool> CheckDelete(NhanSu_TimeSheet input);
        Task Approval(NhanSu_TimeSheet input, string userId);
        Task NoApproval(NhanSu_TimeSheet input, string userId);
        Task<List<NhanSu_TimeSheetModel>> GetAllByVM(NhanSu_TimeSheetModel input, string groupId);
    }
}

