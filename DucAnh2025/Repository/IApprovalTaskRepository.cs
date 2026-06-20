using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;

namespace DucAnh2025.Repository
{
    public interface IApprovalTaskRepository : IBaseRepository<ApprovalTask>
    {
        Task Approval(ApprovalTask input, string userId);
        Task NoApproval(ApprovalTask input, string userId);
        Task<List<ApprovalTaskModel>> GetAllByVM(ApprovalTaskModel input, string groupId,int currentPage = 0, int pageSize = 20);
        Task<List<ApprovalTaskModel>> GetAwaitingApprovalTasks(string groupId, ApplicationUser user,int currentPage = 0, int pageSize = 20, string ParentMajorId ="", string MajorId="");
        Task<List<ApprovalTaskModel>> GetApprovalByUserIdTasks(string groupId, ApplicationUser user,int currentPage = 0, int pageSize = 20, string ParentMajorId ="", string MajorId="");
        Task<List<Major>> GetAllParentMajors(string groupId, string userId);
        Task<List<Major>> GetAllMajorByParentId(string groupId, string parentId);
        Task<ApprovalTask> GetByOriginalId(string originalId);
    }
}
