using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.NhanSu;

namespace DucAnh2025.Repository.HeThong
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<List<PermissionModel>> GetAllCorePermission(string majorId, string companyId);
        Task<List<Permission>> GetAllMPermissions();
        Task<List<PermissionModel>> GetAllByVM(PermissionModel permissionModel);
        Task<List<Permission>> GetExist(Permission input);
        Task<List<Permission>> LoadByMajor(string majorId);
        List<Permission> LoadByMajor1(string majorId);
        Task<List<Permission>> LoadToApproval(string majorId);
        Task<List<Permission>> GetPermissionsByTable(string table);
    }
}
