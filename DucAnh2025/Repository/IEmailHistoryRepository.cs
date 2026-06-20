using DucAnh2025.Models;

namespace DucAnh2025.Repository
{
    public interface IEmailHistoryRepository : IBaseRepository<EmailHistory>
    {
        Task SendEmail(EmailHistory emailHistory);
        Task<int> GetUnreadNotiByUser(string userId);
        Task<List<NotificationModel>> GetAllNotiByUser(string userId);
        Task<List<(string, string, int)>> GetAllCategoriesByUser(string userId);
        Task InsertMulti(List<EmailHistory> entity);
        Task<List<EmailUserPermissionModel>> GetUserPermission(string companyId, string approvalStepId);
        Task ReadNotifi(EmailHistory emailHistory, string userId);

        Task<List<NotificationModel>> GetAllNotiByUser(string userId, int skip = 0, int take = 10);
    }
}
