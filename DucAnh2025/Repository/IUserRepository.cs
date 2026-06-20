using DucAnh2025.Models.Accounts;

namespace DucAnh2025.Repository
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        ApplicationUser GetByUserName(string userName);

        bool IsExistByPhoneNumber(string phoneNumber);

        Task<ApplicationUser> GetById(string id, int isActive);

    }
}
