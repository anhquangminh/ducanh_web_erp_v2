using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.HeThong
{
    public interface IChiNhanhRepository : IBaseRepository<ChiNhanh>
    {
        ChiNhanhModel GetToEdit(string id);
        Task<List<ChiNhanhModel>> GetAllByVM(ChiNhanhModel dataModel, string groupId);
        Task<List<ChiNhanhModel>> GetHistoryIsValidEdit(string id);
        Task<ChiNhanhModel> GetDetails(string id);
        Task<List<ChiNhanhModel>> GetHistory(string id);
        Task<List<ChiNhanhModel>>? GetChiNhanhsForParentId(string groupId);
        Task<List<CompanyTypeModel>>? GetCompanyTypesForCompanyType(string groupId);
        Task<List<ChiNhanh>> GetChiNhanhByPermission(string groupId, string majorId, string userId);
        Task<bool> CheckSave(ChiNhanh input);
        Task<bool> CheckEdit(ChiNhanh input);
        Task<bool> CheckDelete(ChiNhanh input);
        Task Approval(ChiNhanh input, string userId);
        Task NoApproval(ChiNhanh input, string userId);
    }
}
