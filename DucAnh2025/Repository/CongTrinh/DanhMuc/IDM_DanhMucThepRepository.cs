using DucAnh2025.ViewModels.CongTrinh;

namespace DucAnh2025.Repository.CongTrinh.DanhMuc
{
     public interface IDM_DanhMucThepRepository : IBaseRepository<CT_DM_DanhMucThep>
    {
        Task<List<DM_DanhMucThepModel>> GetHistoryIsValidEdit(string id);
        Task<DM_DanhMucThepModel> GetDetails(string id);
        Task<List<DM_DanhMucThepModel>> GetHistory(string id);
        Task<bool> CheckSave(CT_DM_DanhMucThep input);
        Task<bool> CheckEdit(CT_DM_DanhMucThep input);
        Task<bool> CheckDelete(CT_DM_DanhMucThep input);
        Task Approval(CT_DM_DanhMucThep input, string userId);
        Task NoApproval(CT_DM_DanhMucThep input, string userId);
        Task<List<DM_DanhMucThepModel>> GetAllByVM(DM_DanhMucThepModel input, string groupId);
       
    }
}
