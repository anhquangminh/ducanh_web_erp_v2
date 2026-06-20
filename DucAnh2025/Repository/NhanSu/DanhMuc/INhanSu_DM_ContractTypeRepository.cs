using DucAnh2025.Models;
using DucAnh2025.ViewModels.NhanSu.DanhMuc;
namespace DucAnh2025.Repository.NhanSu.DanhMuc
{
    public interface INhanSu_DM_ContractTypeRepository : IBaseRepository<NhanSu_DM_ContractType>
    {
        Task<List<NhanSu_DM_ContractTypeModel>> GetHistoryIsValidEdit(string id);
        Task<NhanSu_DM_ContractTypeModel> GetDetails(string id);
        Task<List<NhanSu_DM_ContractTypeModel>> GetHistory(string id);
        Task<bool> CheckSave(NhanSu_DM_ContractType input);
        Task<bool> CheckEdit(NhanSu_DM_ContractType input);
        Task<bool> CheckDelete(NhanSu_DM_ContractType input);
        Task Approval(NhanSu_DM_ContractType input, string userId);
        Task NoApproval(NhanSu_DM_ContractType input, string userId);
        Task<List<NhanSu_DM_ContractTypeModel>> GetAllByVM(NhanSu_DM_ContractTypeModel input, string groupId);
    }
}
