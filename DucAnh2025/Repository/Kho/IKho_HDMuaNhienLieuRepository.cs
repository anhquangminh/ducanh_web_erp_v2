using DucAnh2025.Models.Kho;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.Kho;
namespace DucAnh2025.Repository.Kho
{
    public interface IKho_HDMuaNhienLieuRepository : IBaseRepository<Kho_HDMuaNhienLieu>
    {
        Task<List<Kho_HDMuaNhienLieuModel>> GetHistoryIsValidEdit(string id);
        Task<Kho_HDMuaNhienLieuModel> GetDetails(string id);
        Task<List<Kho_HDMuaNhienLieuModel>> GetHistory(string id);
        Task<bool> CheckSave(Kho_HDMuaNhienLieu input);
        Task<bool> CheckEdit(Kho_HDMuaNhienLieu input);
        Task<bool> CheckDelete(Kho_HDMuaNhienLieu input);
        Task Approval(Kho_HDMuaNhienLieu input, string userId);
        Task NoApproval(Kho_HDMuaNhienLieu input, string userId);
        Task<List<Kho_HDMuaNhienLieuModel>> GetAllByVM(Kho_HDMuaNhienLieuModel input, string groupId);
        Task<List<Kho_HDMuaNhienLieuModel>> GetAllModel(string groupId);
    }
}
