using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public PermissionRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetExist(Permission input)
        {
            using var context = _context.CreateDbContext();

            if (input.PermissionType == 6)
            {
                var query = context.Permissions
                    .Where(permission =>
                        permission.MajorId == input.MajorId &&
                        permission.PermissionName == input.PermissionName)
                    .OrderByDescending(permission => permission.CreateAt);
                // Lấy kết quả dưới dạng danh sách
                var data = await query.ToListAsync();
                return data;
            }
            else
            {
                var query = context.Permissions
                    .Where(permission =>
                        permission.MajorId == input.MajorId &&
                        permission.PermissionType == input.PermissionType)
                    .OrderByDescending(permission => permission.CreateAt);

                // Lấy kết quả dưới dạng danh sách
                var data = await query.ToListAsync();
                return data;
            }
        }

        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"ID: {id} đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            var isExist = await context.MajorUserPermissions.Where(x => x.PermissionId.Equals(id) && x.IsActive != 100).ToListAsync();

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }
            if (isExist != null && isExist.Count() > 0)
            {
                throw new Exception($"Không thể xóa bản ghi đang được sử dụng : {id}");
            }
            context.Set<Permission>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<List<PermissionModel>> GetAllByVM(PermissionModel permissionModel)
        {
            using var context = _context.CreateDbContext();
            var query = from permission in context.Permissions
                        join major in context.Majors
                        on permission.MajorId equals major.Id into majorGroup
                        from major in majorGroup.DefaultIfEmpty()
                        orderby permission.CreateAt descending
                        select new PermissionModel
                        {
                            Id = permission.Id,
                            MajorId = permission.MajorId,
                            MajorName = major.MajorName,
                            PermissionType = permission.PermissionType,
                            PermissionName = permission.PermissionName,
                            CreateAt = permission.CreateAt,
                            CreateBy = permission.CreateBy,
                            IsActive = permission.IsActive
                        };

            if (!string.IsNullOrEmpty(permissionModel.MajorId))
            {
                query = query.Where(m => m.MajorId == permissionModel.MajorId);
            }

            if (!string.IsNullOrEmpty(permissionModel.PermissionName))
            {
                query = query.Where(m => m.PermissionName == permissionModel.PermissionName);
            }

            var data = await query.ToListAsync();
            return data;
        }
        public async Task<List<PermissionModel>> GetAllCorePermission(string screenId, string companyId)
        {
            using var context = _context.CreateDbContext();
            var query = from permission in context.Permissions
                        join major in context.Majors
                        on permission.MajorId equals major.Id into majorGroup
                        from major in majorGroup.DefaultIfEmpty()

                        select new PermissionModel
                        {
                            Id = permission.Id,
                            MajorId = permission.MajorId,
                            PermissionType = permission.PermissionType,
                            PermissionName = permission.PermissionName,
                        };

            return await query.OrderBy(p => p.PermissionType).ToListAsync();
        }
        public async Task<List<Permission>> GetAllMPermissions()
        {
            using var context = _context.CreateDbContext();
            var query = context.Permissions.Where(x => x.IsActive != 100);
            return await query.ToListAsync();
        }

        public async Task<List<Permission>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.Permissions.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw; // Optionally rethrow the exception
            }
        }
        public async Task<List<Permission>> LoadByMajor(string majorId)
        {
            using var context = _context.CreateDbContext();
            var entity = await (from p in context.Permissions
                                where p.MajorId == majorId
                                select p).OrderBy(p => p.PermissionType).Distinct().ToListAsync();
            return entity;
        }
        public List<Permission> LoadByMajor1(string majorId)
        {
            using var context = _context.CreateDbContext();
            var entity = (from p in context.Permissions
                          where p.MajorId == majorId
                          select p).OrderBy(p => p.PermissionType).Distinct().ToList();
            return entity;
        }
        public async Task<List<Permission>> LoadToApproval(string majorId)
        {
            using var context = _context.CreateDbContext();

            var list = await context.Permissions
                .Where(p => p.MajorId == majorId)
                .ToListAsync();

            var result = list
                .GroupBy(p => p.PermissionType)
                .Select(g => g
                    .OrderBy(x => x.Id)
                    .First())
                .OrderBy(p => p.PermissionType)
                .ToList();

            return result;
        }


        public async Task<Permission> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.Permissions.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }
            return entity;
        }
        public async Task Insert(Permission entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có bản ghi nào để thêm!");
                }
                context.Permissions.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(Permission mpermission, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(mpermission.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {mpermission.Id}");
            }
            context.Permissions.Update(mpermission);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(Permission[] MPermissions)
        {
            using var context = _context.CreateDbContext();
            string[] ids = MPermissions.Select(x => x.Id).ToArray();
            var listEntities = await context.Permissions.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.Permissions.Update(entity);
            }
            await context.SaveChangesAsync();
        }

        public Task<bool> CheckStatus(string ids, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Permission>> GetPermissionsByTable(string table)
        {
            using var context = _context.CreateDbContext();
            var result = await (from p in context.Permissions
                                join m in context.Majors on p.MajorId equals m.Id
                                where m.Table == table
                                select p).ToListAsync();
            return result;
        }
    }
}
