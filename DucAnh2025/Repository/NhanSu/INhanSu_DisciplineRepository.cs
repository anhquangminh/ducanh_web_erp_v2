using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
namespace DucAnh2025.Repository.NhanSu
{
    public interface INhanSu_DisciplineRepository : IBaseRepository<NhanSu_Discipline>
    {
        Task<List<NhanSu_DisciplineModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DisciplineModel> GetDetails(string id);
        Task<List<NhanSu_DisciplineModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_Discipline input);
        Task<bool> CheckEdit(NhanSu_Discipline input);
        Task<bool> CheckDelete(NhanSu_Discipline input);
        Task Approval(NhanSu_Discipline input, string userId);
        Task NoApproval(NhanSu_Discipline input, string userId);
        Task<List<NhanSu_DisciplineModel>> GetAllByVM(NhanSu_DisciplineModel input, string groupId);
    }
}
