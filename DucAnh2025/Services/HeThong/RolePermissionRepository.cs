using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DucAnh2025.Services.HeThong
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(string roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }
        public async Task<List<RolePermission>> GetAll()
        {
            return await _context.RolePermissions.ToListAsync();
        }
        public async Task<List<RolePermissionViewDto>> GetAllAsync()
        {
            var query =
         from rp in _context.RolePermissions
         join r in _context.Roles on rp.RoleId equals r.Id
         join p in _context.Permissions on rp.PermissionId equals p.Id
         join m in _context.Majors on p.MajorId equals m.Id
         select new RolePermissionViewDto
         {
             RoleId = r.Id,
             RoleName = r.Name,

             MajorId = m.Id,
             MajorName = m.MajorName,

             PermissionId = p.Id,
             PermissionName = p.PermissionName,
             PermissionType = p.PermissionType
         };

            var data = await query
                .OrderBy(x => x.RoleName)
                .ThenBy(x => x.MajorName)
                .ThenByDescending(x => x.PermissionType)
                .ToListAsync();
            return data;
        }

        public async Task<List<RolePermission>> GetByRoleAsync(string roleId)
        {
            return await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<List<RolePermission>> GetPermissionsByRolesAsync(IEnumerable<string> roleIds)
        {
            return await _context.RolePermissions
                .Where(x => roleIds.Contains(x.RoleId))
                .ToListAsync();
        }
        public async Task UpdateAsync(RolePermission rolePermission)
        {
            _context.RolePermissions.Update(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task AddByMajorAsync(string roleId,string majorId,List<string> permissionIds,string groupId,
string createBy)
        {
            // 1. L?y permission thu?c major
            var permissionInMajor = await _context.Permissions
                .Where(p => p.MajorId == majorId)
                .Select(p => p.Id)
                .ToListAsync();

            // 2. Xóa quy?n c? c?a role theo major
            var oldRolePermissions = await _context.RolePermissions
                .Where(rp =>
                    rp.RoleId == roleId &&
                    permissionInMajor.Contains(rp.PermissionId))
                .ToListAsync();

            _context.RolePermissions.RemoveRange(oldRolePermissions);

            // 3. Thêm quy?n m?i
            var newItems = permissionIds.Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });

            await _context.RolePermissions.AddRangeAsync(newItems);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(RolePermission rolePermission)
        {
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(RolePermission rolePermission)
        {
            try
            {
                var entity = await _context.RolePermissions
               .FirstOrDefaultAsync(rp => rp.RoleId == rolePermission.RoleId && rp.PermissionId == rolePermission.PermissionId);

                if (entity == null)
                    throw new Exception("RolePermission not found.");

                _context.RolePermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task RemoveByRoleAsync(string roleId)
        {
            var items = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetMajorIdsByUserAsync(string userId)
        {
            return await (
                from ur in _context.UserRoles
                join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                join p in _context.Permissions on rp.PermissionId equals p.Id
                where ur.UserId == userId
                select p.MajorId
            )
            .Distinct()
            .ToListAsync();
        }


    }
}