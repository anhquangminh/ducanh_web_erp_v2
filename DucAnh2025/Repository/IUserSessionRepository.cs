using DucAnh2025.Models;

namespace DucAnh2025.Repository
{
    public interface IUserSessionRepository : IBaseRepository<UserSession>
    {
        Task<UserSession> GetByUserName(string userName);
    }
}
