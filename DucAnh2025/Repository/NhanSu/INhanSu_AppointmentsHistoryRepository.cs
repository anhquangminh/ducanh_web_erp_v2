using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_AppointmentsHistoryRepository : IBaseRepository<NhanSu_AppointmentsHistory>
    {
        Task<List<NhanSu_AppointmentsHistoryModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_AppointmentsHistoryModel> GetDetails(string id);
        Task<List<NhanSu_AppointmentsHistoryModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_AppointmentsHistory input);
        Task<bool> CheckEdit(NhanSu_AppointmentsHistory input);
        Task<bool> CheckDelete(NhanSu_AppointmentsHistory input);
        Task Approval(NhanSu_AppointmentsHistory input, string userId);
        Task NoApproval(NhanSu_AppointmentsHistory input, string userId);
        Task<List<NhanSu_AppointmentsHistoryModel>> GetAllByVM(NhanSu_AppointmentsHistoryModel input, string groupId);
    }
}
