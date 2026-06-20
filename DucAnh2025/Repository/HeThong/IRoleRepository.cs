using DucAnh2025.Models.HeThong;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(string id);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(string id);
}