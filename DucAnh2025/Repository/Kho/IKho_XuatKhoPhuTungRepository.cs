using DucAnh2025.Models;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels;
namespace DucAnh2025.Repository
{
    public interface IKho_XuatKhoPhuTungRepository : IBaseRepository<Kho_XuatKhoPhuTung>
    {
        Task<List<Kho_XuatKhoPhuTungModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_XuatKhoPhuTungModel> GetDetails(string id);
        Task<List<Kho_XuatKhoPhuTungModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_XuatKhoPhuTung input);
        Task<bool> CheckEdit(Kho_XuatKhoPhuTung input);
        Task<bool> CheckDelete(Kho_XuatKhoPhuTung input);
        Task Approval(Kho_XuatKhoPhuTung input, string userId);
        Task NoApproval(Kho_XuatKhoPhuTung input, string userId);
        Task<List<Kho_XuatKhoPhuTungModel>> GetAllByVM(Kho_XuatKhoPhuTungModel input, string groupId);
    }
}
