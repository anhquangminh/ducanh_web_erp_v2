using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_DisciplineTypeRepository : IBaseRepository<NhanSu_DM_DisciplineType>
    {
        Task<List<NhanSu_DM_DisciplineTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_DisciplineTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_DisciplineTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_DisciplineType input);
        Task<bool> CheckEdit(NhanSu_DM_DisciplineType input);
        Task<bool> CheckDelete(NhanSu_DM_DisciplineType input);
        Task Approval(NhanSu_DM_DisciplineType input, string userId);
        Task NoApproval(NhanSu_DM_DisciplineType input, string userId);
        Task<List<NhanSu_DM_DisciplineTypeModel>> GetAllByVM(NhanSu_DM_DisciplineTypeModel input, string groupId);
    }
}
