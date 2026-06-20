using DucAnh2025.Models.HeThong;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetByRoleIdAsync(string roleId);
    Task<List<RolePermission>> GetAll();
    Task<List<RolePermissionViewDto>> GetAllAsync();
    Task AddAsync(RolePermission rolePermission);
    Task RemoveAsync(RolePermission rolePermission);

    Task<List<RolePermission>> GetByRoleAsync(string roleId);
    Task<List<RolePermission>> GetPermissionsByRolesAsync(IEnumerable<string> roleIds);
    Task RemoveByRoleAsync(string roleId);
    Task AddByMajorAsync(string roleId,string majorId,List<string> permissionIds,string groupId,string createBy);
    Task<List<string>> GetMajorIdsByUserAsync(string userId);
}