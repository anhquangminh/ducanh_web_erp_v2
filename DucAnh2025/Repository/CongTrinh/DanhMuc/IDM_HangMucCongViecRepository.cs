using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh.DanhMuc
{
     public interface IDM_HangMucCongViecRepository : IBaseRepository<CT_DM_HangMucCongViec>
    {
        Task<List<DM_ChungModel>> GetHistoryIsValidEdit(string id);
        Task<DM_ChungModel> GetDetails(string id);
        Task<List<DM_ChungModel>> GetHistory(string id);
        Task<bool> CheckSave(CT_DM_HangMucCongViec input);
        Task<bool> CheckEdit(CT_DM_HangMucCongViec input);
        Task<bool> CheckDelete(CT_DM_HangMucCongViec input);
        Task Approval(CT_DM_HangMucCongViec input, string userId);
        Task NoApproval(CT_DM_HangMucCongViec input, string userId);
    }
}
