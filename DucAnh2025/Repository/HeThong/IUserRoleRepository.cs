using DucAnh2025.Models.HeThong;

public interface IUserRoleRepository
{
    Task<List<UserRole>> GetByUserIdAsync(string userId);
    Task<List<UserRole>> GetAllAsync();
    Task AddAsync(UserRole userRole);
    Task RemoveAsync(UserRole userRole);
    Task<List<UserRole>> GetByUserAsync(string userId);
    Task<List<Role>> GetRolesByUserAsync(string userId);

}