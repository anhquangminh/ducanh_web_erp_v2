using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_RequestRepository : IBaseRepository<NhanSu_Request>
    {
        Task<List<NhanSu_RequestModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_RequestModel> GetDetails(string id);
        Task<List<NhanSu_RequestModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_Request input);
        Task<bool> CheckEdit(NhanSu_Request input);
        Task<bool> CheckDelete(NhanSu_Request input);
        Task Approval(NhanSu_Request input, string userId);
        Task NoApproval(NhanSu_Request input, string userId);
        Task<List<NhanSu_RequestModel>> GetAllByVM(NhanSu_RequestModel input, string groupId);
    }
}
