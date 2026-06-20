using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels;
namespace DucAnh2025.Repository
{
    public interface IKho_NhapKhoPhuTungRepository : IBaseRepository<Kho_NhapKhoPhuTung>
    {
        Task<List<Kho_NhapKhoPhuTungModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_NhapKhoPhuTungModel> GetDetails(string id);
        Task<List<Kho_NhapKhoPhuTungModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_NhapKhoPhuTung input);
        Task<bool> CheckEdit(Kho_NhapKhoPhuTung input);
        Task<bool> CheckDelete(Kho_NhapKhoPhuTung input);
        Task Approval(Kho_NhapKhoPhuTung input, string userId);
        Task NoApproval(Kho_NhapKhoPhuTung input, string userId);
        Task<List<Kho_NhapKhoPhuTungModel>> GetAllByVM(Kho_NhapKhoPhuTungModel input, string groupId);
    }
}
