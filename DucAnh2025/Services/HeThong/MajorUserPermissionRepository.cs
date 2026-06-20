using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.Accounts;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class MajorUserPermissionRepository : IMajorUserPermissionRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public MajorUserPermissionRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public MajorUserPermissionModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.MajorUserPermissions
                         join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                         join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                         join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                         join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                         join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                         where p1.IdMain == id && p1.IsActive != 100
                         select new MajorUserPermissionModel
                         {
                             IdMain = p1.IdMain,
                             CompanyId = ChiNhanhs1.TenChiNhanh,
                             ParentMajorId = Majors1.MajorName,
                             MajorId = Majors2.MajorName,
                             UserId = ApplicationUsers1.Email,
                             PermissionId = Permissions1.PermissionName,
                             PermissionOrder = Permissions1.PermissionType,
                             DayInWeek = p1.DayInWeek,
                             DayInWeekText = p1.DayInWeek == 0 ? "Chủ nhật" :
                                  p1.DayInWeek == 1 ? "Thứ 2" :
                                  p1.DayInWeek == 2 ? "Thứ 3" :
                                  p1.DayInWeek == 3 ? "Thứ 4" :
                                  p1.DayInWeek == 4 ? "Thứ 5" :
                                  p1.DayInWeek == 5 ? "Thứ 6" :
                                  p1.DayInWeek == 6 ? "Thứ 7" : ""
                         }).Distinct().OrderBy(p => p.IdMain).OrderBy(p => p.DayInWeek).OrderBy(p => p.PermissionOrder).FirstOrDefault();

            return query;
        }
        public async Task<List<MajorUserPermissionModel>> GetAllByVM(MajorUserPermissionModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.MajorUserPermissions
                        where p1.GroupId == groupId && p1.IsActive != 100
                        select p1;
            if (!string.IsNullOrEmpty(dataModel.CompanyId))
            {
                query = query.Where(m => m.CompanyId == dataModel.CompanyId);
            }
            if (!string.IsNullOrEmpty(dataModel.ParentMajorId))
            {
                query = query.Where(m => m.ParentMajorId == dataModel.ParentMajorId);
            }
            if (!string.IsNullOrEmpty(dataModel.MajorId))
            {
                query = query.Where(m => m.MajorId == dataModel.MajorId);
            }
            if (!string.IsNullOrEmpty(dataModel.UserId))
            {
                query = query.Where(m => m.UserId == dataModel.UserId);
            }
            if (!string.IsNullOrEmpty(dataModel.PermissionId))
            {
                query = query.Where(m => m.PermissionId == dataModel.PermissionId);
            }
            var data = await (from p1 in query
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              select new MajorUserPermissionModel
                              {
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  PermissionOrder = Permissions1.PermissionType,
                                  DayInWeek = p1.DayInWeek,
                                  DayInWeekText = p1.DayInWeek == 0 ? "Chủ nhật" :
                                  p1.DayInWeek == 1 ? "Thứ 2" :
                                  p1.DayInWeek == 2 ? "Thứ 3" :
                                  p1.DayInWeek == 3 ? "Thứ 4" :
                                  p1.DayInWeek == 4 ? "Thứ 5" :
                                  p1.DayInWeek == 5 ? "Thứ 6" :
                                  p1.DayInWeek == 6 ? "Thứ 7" : ""
                              }).Distinct().OrderBy(p => p.IdMain).OrderBy(p => p.DayInWeek).OrderBy(p => p.PermissionOrder).ToListAsync();

            var result = data.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId, p.UserId, p.PermissionId }).Select(g => new
            {
                g.Key.IdMain,
                g.Key.CompanyId,
                g.Key.ParentMajorId,
                g.Key.MajorId,
                g.Key.UserId,
                g.Key.PermissionId,
                DayInWeekText = string.Join(" => ", g.Select(i => i.DayInWeekText))
            }).Distinct();
            result = result.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId, p.UserId, p.DayInWeekText }).Select(g => new
            {
                g.Key.IdMain,
                g.Key.CompanyId,
                g.Key.ParentMajorId,
                g.Key.MajorId,
                g.Key.UserId,
                PermissionId = string.Join(" => ", g.Select(i => i.PermissionId)),
                g.Key.DayInWeekText
            }).Distinct();
            var vlus = new List<MajorUserPermissionModel>();
            foreach (var item in result)
            {
                var additem = new MajorUserPermissionModel()
                {
                    IdMain = item.IdMain,
                    CompanyId = item.CompanyId,
                    ParentMajorId = item.ParentMajorId,
                    MajorId = item.MajorId,
                    UserId = item.UserId,
                    DayInWeekText = item.DayInWeekText,
                    PermissionId = item.PermissionId
                };
                vlus.Add(additem);
            }
            return vlus;
        }
        public async Task<List<MajorUserPermissionModel>> GetHistoryIsValidEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.MajorUserPermission_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              where p1.IdChung == id && p1.IsValid == true
                              orderby p1.CreateAt
                              select new MajorUserPermissionModel
                              {
                                  Id = p1.Id,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  DayInWeek = p1.DayInWeek,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = p1.DateApproval,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<MajorUserPermissionModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.MajorUserPermissions
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.Id == id
                              select new MajorUserPermissionModel
                              {
                                  Id = p1.Id,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  DayInWeek = p1.DayInWeek,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = p1.DateApproval,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<MajorUserPermissionModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.MajorUserPermission_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new MajorUserPermissionModel
                              {
                                  Id = p1.Id,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  DayInWeek = p1.DayInWeek,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = p1.DateApproval,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<List<MajorUserPermission>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.MajorUserPermissions.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.MajorUserPermissions
                                    join p2 in context.ChiNhanhs on p1.CompanyId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.TenChiNhanh
                                    select new ChiNhanhModel
                                    {
                                        Id = p2.Id,
                                        TenChiNhanh = p2.TenChiNhanh
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<MajorModel>>? GetMajorsForParentMajorId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.MajorUserPermissions
                                    join p2 in context.Majors on p1.ParentMajorId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.MajorName
                                    select new MajorModel
                                    {
                                        Id = p2.Id,
                                        MajorName = p2.MajorName
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<MajorModel>>? GetMajorsForMajorId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.MajorUserPermissions
                                    join p2 in context.Majors on p1.MajorId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.MajorName
                                    select new MajorModel
                                    {
                                        Id = p2.Id,
                                        MajorName = p2.MajorName
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ApplicationUserModel>>? GetApplicationUsersForUserId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.MajorUserPermissions
                                    join p2 in context.ApplicationUsers on p1.UserId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.Email
                                    select new ApplicationUserModel
                                    {
                                        Id = p2.Id,
                                        Email = p2.Email
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<PermissionModel>>? GetPermissionsForPermissionId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.MajorUserPermissions
                                    join p2 in context.Permissions on p1.PermissionId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.PermissionName
                                    select new PermissionModel
                                    {
                                        Id = p2.Id,
                                        PermissionName = p2.PermissionName
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<MajorUserPermission>> GetByIdMain(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.MajorUserPermissions.Where(x => x.IdMain.Equals(id) && x.IsActive != 100).ToListAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn.");
            }
            return entity;
        }
        public async Task<MajorUserPermission> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.MajorUserPermissions.Where(x => x.IdMain.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn.");
            }
            return entity;
        }
        public async Task Insert(MajorUserPermission entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có quản lý quyền theo người dùng, nghiệp vụ nào được thêm!");
                }
                context.MajorUserPermissions.Add(entity);
                var addLog = new MajorUserPermission_Log()
                {
                    Id = entity.Id,
                    CompanyId = entity.CompanyId,
                    ParentMajorId = entity.ParentMajorId,
                    MajorId = entity.MajorId,
                    UserId = entity.UserId,
                    PermissionId = entity.PermissionId,
                    DayInWeek = entity.DayInWeek,
                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = DateTime.Now,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = entity.DateApproval,
                    DepartmentId = entity.DepartmentId,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,
                    IsValid = true
                };
                context.MajorUserPermission_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(MajorUserPermission data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.IdMain);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn");
            }
            //context.MajorUserPermissions.Update(data);
            var recordsToUpdate = context.MajorUserPermissions.Where(m => m.IdMain == data.IdMain).ToList();
            foreach (var record in recordsToUpdate)
            {
                record.IsActive = data.IsActive;
            }
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.MajorUserPermission_Logs
                                       where p.IdChung == entity.IdMain && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.MajorUserPermission_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.MajorUserPermission_Logs
                                       where p.IdChung == entity.IdMain && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.MajorUserPermission_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.MajorUserPermission_Logs
                                       where p.IdChung == entity.IdMain
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.MajorUserPermission_Logs.Update(updateLog);
                }
            }
            var addLog = new MajorUserPermission_Log()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyId = entity.CompanyId,
                ParentMajorId = entity.ParentMajorId,
                MajorId = entity.MajorId,
                UserId = entity.UserId,
                PermissionId = entity.PermissionId,
                DayInWeek = entity.DayInWeek,
                GroupId = entity.GroupId,
                Ordinarily = entity.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = entity.CreateBy,
                IsActive = entity.IsActive,
                ApprovalUserId = entity.ApprovalUserId,
                DateApproval = entity.DateApproval,
                DepartmentId = entity.DepartmentId,
                DepartmentOrder = entity.DepartmentOrder,
                ApprovalOrder = entity.ApprovalOrder,
                ApprovalId = entity.ApprovalId,
                LastApprovalId = entity.LastApprovalId,
                IsStatus = entity.IsStatus,
                IdChung = data.IdMain,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.MajorUserPermission_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task InsertMulti(List<MajorUserPermission> majorUserPermissions)
        {
            using var context = _context.CreateDbContext();
            await context.MajorUserPermissions.AddRangeAsync(majorUserPermissions);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(List<MajorUserPermission> majorUserPermissions, string idMain)
        {
            using var context = _context.CreateDbContext();

            var updateIdMain = from p in context.MajorUserPermissions
                               where p.IdMain == idMain
                               select p;
            if (updateIdMain != null)
            {
                await updateIdMain.ForEachAsync(p => p.IsActive = 100);
                context.MajorUserPermissions.UpdateRange(updateIdMain);
            }
            await context.MajorUserPermissions.AddRangeAsync(majorUserPermissions);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(MajorUserPermission[] MajorUserPermissions)
        {
            using var context = _context.CreateDbContext();
            string[] ids = MajorUserPermissions.Select(x => x.Id).ToArray();
            var listEntities = await context.MajorUserPermissions.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.MajorUserPermissions.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn");
            }
            context.Set<MajorUserPermission>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.MajorUserPermissions.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Quản lý quyền theo người dùng, nghiệp vụ đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Quản lý quyền theo người dùng, nghiệp vụ đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Quản lý quyền theo người dùng, nghiệp vụ đang chờ duyệt xóa.");
            }
            //if (model != null && model.TenMajorUserPermission.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên quản lý quyền theo người dùng, nghiệp vụ đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(MajorUserPermission input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.MajorUserPermission_Logs
                                   where p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.CompanyId == input.CompanyId
                                   && p.ParentMajorId == input.ParentMajorId
                                   && p.MajorId == input.MajorId
                                   && p.UserId == input.UserId
                                   && p.PermissionId == input.PermissionId
                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckEdit(MajorUserPermission input)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var checkexist = (from p in context.MajorUserPermissions
                                  where p.IdMain == input.IdMain
                                  && p.IsActive != 100
                                  select p).Count();
                if (checkexist == 0)
                {
                    var model = (from p in context.MajorUserPermissions
                                 where p.IsActive != 100
                                 && p.CompanyId == input.CompanyId
                                 && p.MajorId == input.MajorId
                                 && p.UserId == input.UserId
                                 select p).Count();
                    if (model > 0)
                    {
                        throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                    }
                }
                else
                {
                    var model = (from p in context.MajorUserPermissions
                                 where p.IsActive != 100 && p.IdMain != input.IdMain
                                 && p.CompanyId == input.CompanyId
                                 && p.MajorId == input.MajorId
                                 && p.UserId == input.UserId
                                 select p).Count();
                    if (model > 0)
                    {
                        throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckDelete(MajorUserPermission input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (1 == 0)
                {
                    throw new Exception($"");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }
        public async Task<List<string>> GetAccessibleMajorIds(string userId, string companyId)
        {
            using var context = _context.CreateDbContext();

            int today = (int)DateTime.Today.DayOfWeek;

            var entity = await (
                from mup in context.MajorUserPermissions
                join p in context.Permissions on mup.PermissionId equals p.Id
                where mup.UserId == userId
                      && mup.CompanyId == companyId
                      && p.PermissionType == 0 // TRUY CẬP
                      && mup.IsActive == 1
                      && mup.DayInWeek == today
                select mup.MajorId
            )
            .Distinct()
            .ToListAsync();

            return entity;
        }


    }
}
