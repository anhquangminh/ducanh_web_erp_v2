using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.NhanSu
{
    public interface ICompanyTypeRepository : IBaseRepository<CompanyType>
    {
        CompanyTypeModel GetToEdit(string id);
        Task<List<CompanyTypeModel>> GetAllByVM(CompanyTypeModel dataModel, string groupId);
        Task<List<CompanyTypeModel>> GetHistoryIsValidEdit(string id);
        Task<CompanyTypeModel> GetDetails(string id);
        Task<List<CompanyTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(CompanyType input);
        Task<bool> CheckEdit(CompanyType input);
        Task<bool> CheckDelete(CompanyType input);
        Task Approval(CompanyType input, string userId);
        Task NoApproval(CompanyType input, string userId);
    }
}
