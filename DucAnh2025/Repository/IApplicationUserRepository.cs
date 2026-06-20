using DucAnh2025.Models.Accounts;
using DucAnh2025.ViewModels.Accounts;

namespace DucAnh2025.Repository
{
    public interface IApplicationUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<List<ApplicationUserModel>> GetAllByVM(ApplicationUserModel dataModel, string groupId);
        Task<List<ChiNhanhModel>> GetChiNhanhs(string groupId);
        Task<List<DepartmentModel>> GetDepartments(string groupId);
        ApplicationUser GetCurrentUser();
        ApplicationUser GetByUserName(string userName);
        bool IsExistByPhoneNumber(string phoneNumber);
        Task<List<string>> GetByIdUserName(List<string> userNames);
    }
}

//GetByUserName
//IsExistByPhoneNumber
//            Insert
