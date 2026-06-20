using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;
    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllAsync() => await _context.Roles.ToListAsync();
    public async Task<Role?> GetByIdAsync(string id)
    {
        var rol = await _context.Roles.FindAsync(id);
        return rol;
    }

    public async Task AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(string id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}