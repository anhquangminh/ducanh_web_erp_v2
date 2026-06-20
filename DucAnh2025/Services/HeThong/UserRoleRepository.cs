using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRole>> GetByUserIdAsync(string userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }
        public async Task<List<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }
        public async Task AddAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(UserRole userRole)
        {
            try
            {
                var entity = await _context.UserRoles
                    .FirstOrDefaultAsync(ur =>
                        ur.UserId == userRole.UserId &&
                        ur.RoleId == userRole.RoleId 
                    );

                if (entity == null)
                {
                    // Optionally, throw or log that the entity was not found
                    Console.WriteLine("UserRole not found for removal.");
                    return;
                }

                _context.UserRoles.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<List<UserRole>> GetByUserAsync(string userId)
        {
            return await _context.UserRoles
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
        public async Task<List<Role>> GetRolesByUserAsync(string userId)
        {
            return await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where ur.UserId == userId
                select r
            ).ToListAsync();
        }
    }
}