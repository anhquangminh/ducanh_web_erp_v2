using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_TerminationRepository : IBaseRepository<NhanSu_Termination>
    {
        Task<List<NhanSu_TerminationModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_TerminationModel> GetDetails(string id);
        Task<List<NhanSu_TerminationModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_Termination input);
        Task<bool> CheckEdit(NhanSu_Termination input);
        Task<bool> CheckDelete(NhanSu_Termination input);
        Task Approval(NhanSu_Termination input, string userId);
        Task NoApproval(NhanSu_Termination input, string userId);
        Task<List<NhanSu_TerminationModel>> GetAllByVM(NhanSu_TerminationModel input, string groupId);
    }
}
