using DucAnh2025.Models.NhanSu;

namespace DucAnh2025.Repository.NhanSu
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        DepartmentModel GetToEdit(string id);
        Task<List<DepartmentModel>> GetAllByVM(DepartmentModel dataModel, string groupId);
        Task<List<DepartmentModel>> GetHistoryIsValidEdit(string id);
        Task<DepartmentModel> GetDetails(string id);
        Task<List<DepartmentModel>> GetHistory(string id);
        Task<List<Department>>? GetByChiNhanhs(string companyId);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId);
        Task<bool> CheckSave(Department input);
        Task<bool> CheckEdit(Department input);
        Task<bool> CheckDelete(Department input);
        Task Approval(Department input, string userId);
        Task NoApproval(Department input, string userId);
    }
}
