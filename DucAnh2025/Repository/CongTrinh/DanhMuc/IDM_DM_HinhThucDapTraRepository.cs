using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh.DanhMuc
{
     public interface IDM_HinhThucDapTraRepository : IBaseRepository<CT_DM_HinhThucDapTra>
    {
        Task<List<DM_ChungModel>> GetHistoryIsValidEdit(string id);
        Task<DM_ChungModel> GetDetails(string id);
        Task<List<DM_ChungModel>> GetHistory(string id);
        Task<bool> CheckSave(CT_DM_HinhThucDapTra input);
        Task<bool> CheckEdit(CT_DM_HinhThucDapTra input);
        Task<bool> CheckDelete(CT_DM_HinhThucDapTra input);
        Task Approval(CT_DM_HinhThucDapTra input, string userId);
        Task NoApproval(CT_DM_HinhThucDapTra input, string userId);
    }
}
