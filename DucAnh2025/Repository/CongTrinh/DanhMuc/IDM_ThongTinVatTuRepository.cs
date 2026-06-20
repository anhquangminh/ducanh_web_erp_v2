using DucAnh2025.Models.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh.DanhMuc
{
     public interface IDM_ThongTinVatTuRepository : IBaseRepository<CT_DM_ThongTinVatTu>
    {
        Task<List<CT_DM_ThongTinVatTu>> GetHistoryIsValidEdit(string id);
        Task<CT_DM_ThongTinVatTu> GetDetails(string id);
        Task<List<CT_DM_ThongTinVatTu>> GetHistory(string id);
        Task<bool> CheckSave(CT_DM_ThongTinVatTu input);
        Task<bool> CheckEdit(CT_DM_ThongTinVatTu input);
        Task<bool> CheckDelete(CT_DM_ThongTinVatTu input);
        Task Approval(CT_DM_ThongTinVatTu input, string userId);
        Task NoApproval(CT_DM_ThongTinVatTu input, string userId);
    }
}
