using DucAnh2025.Data;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class MajorUserApprovalReponsitory : IMajorUserApprovalReponsitory
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public MajorUserApprovalReponsitory(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<MajorUserApproval>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.MajorUserApprovals.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception e)
            {

                throw new Exception("Lỗi " + e.Message);
            }
        }
        public async Task<List<MajorUserApprovalModel>> GetAllByVM(MajorUserApprovalModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.MajorUserApprovals
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
                if (!string.IsNullOrEmpty(dataModel.DeptId))
                {
                    query = query.Where(m => m.DeptId == dataModel.DeptId);
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
                                  join Department1 in context.Departments on p1.DeptId equals Department1.Id
                                  join AproSetting in context.ApprovalStepSettings on p1.ApprovalStepId equals AproSetting.Id
                                  select new MajorUserApprovalModel
                                  {
                                      IdMain = p1.IdMain,
                                      CompanyId = ChiNhanhs1.TenChiNhanh,
                                      ParentMajorId = Majors1.MajorName,
                                      MajorId = Majors2.MajorName,
                                      UserId = ApplicationUsers1.Email,
                                      DeptId = Department1.DeptName,
                                      PermissionId = Permissions1.PermissionName,
                                      ApprovalStepId = AproSetting.Content,
                                      ApprovalOrder = AproSetting.ApprovalStep,
                                      DayinWeek = p1.DayinWeek,
                                      DayInWeekText = int.Parse(p1.DayinWeek) == 0 ? "Chủ nhật" :
                                      int.Parse(p1.DayinWeek) == 1 ? "Thứ 2" :
                                      int.Parse(p1.DayinWeek) == 2 ? "Thứ 3" :
                                      int.Parse(p1.DayinWeek) == 3 ? "Thứ 4" :
                                      int.Parse(p1.DayinWeek) == 4 ? "Thứ 5" :
                                      int.Parse(p1.DayinWeek) == 5 ? "Thứ 6" :
                                      int.Parse(p1.DayinWeek) == 6 ? "Thứ 7" : ""
                                  }).Distinct().OrderBy(p => p.IdMain).OrderBy(p => p.DayinWeek).ToListAsync();

                var result = data.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId, p.DeptId, p.UserId, p.PermissionId, p.ApprovalStepId }).Select(g => new
                {
                    g.Key.IdMain,
                    g.Key.CompanyId,
                    g.Key.ParentMajorId,
                    g.Key.MajorId,
                    g.Key.DeptId,
                    g.Key.UserId,
                    g.Key.PermissionId,
                    g.Key.ApprovalStepId,
                    DayInWeekText = string.Join(" => ", g.Select(i => i.DayInWeekText))
                }).Distinct();
                result = result.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId, p.DeptId, p.UserId, p.DayInWeekText, p.PermissionId }).Select(g => new
                {
                    g.Key.IdMain,
                    g.Key.CompanyId,
                    g.Key.ParentMajorId,
                    g.Key.MajorId,
                    g.Key.DeptId,
                    g.Key.UserId,
                    g.Key.PermissionId,
                    ApprovalStepId = string.Join(" => ", g.Select(i => i.ApprovalStepId)),
                    g.Key.DayInWeekText
                }).Distinct();
                var vlus = new List<MajorUserApprovalModel>();
                foreach (var item in result)
                {
                    var additem = new MajorUserApprovalModel()
                    {
                        IdMain = item.IdMain,
                        CompanyId = item.CompanyId,
                        ParentMajorId = item.ParentMajorId,
                        MajorId = item.MajorId,
                        DeptId = item.DeptId,
                        UserId = item.UserId,
                        DayInWeekText = item.DayInWeekText,
                        PermissionId = item.PermissionId,
                        ApprovalStepId = item.ApprovalStepId
                    };
                    vlus.Add(additem);
                }
                return vlus;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi :" + ex.Message);
            }
        }
        public async Task<List<MajorUserApproval>> GetByIdMain(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.MajorUserApprovals.Where(x => x.IdMain.Equals(id) && x.IsActive != 100).ToListAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn.");
            }
            return entity;
        }
        public MajorUserApprovalModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.MajorUserApprovals
                         join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                         join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                         join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                         join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                         join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                         join Department1 in context.Departments on p1.DeptId equals Department1.Id
                         where p1.IdMain == id && p1.IsActive != 100
                         select new MajorUserApprovalModel
                         {
                             IdMain = p1.IdMain,
                             CompanyId = ChiNhanhs1.TenChiNhanh,
                             ParentMajorId = Majors1.MajorName,
                             MajorId = Majors2.MajorName,
                             DeptId = Department1.DeptName,
                             UserId = ApplicationUsers1.Email,
                             PermissionId = Permissions1.PermissionName,
                             DayinWeek = p1.DayinWeek,
                             DayInWeekText = int.Parse(p1.DayinWeek) == 0 ? "Chủ nhật" :
                                  int.Parse(p1.DayinWeek) == 1 ? "Thứ 2" :
                                  int.Parse(p1.DayinWeek) == 2 ? "Thứ 3" :
                                  int.Parse(p1.DayinWeek) == 3 ? "Thứ 4" :
                                  int.Parse(p1.DayinWeek) == 4 ? "Thứ 5" :
                                  int.Parse(p1.DayinWeek) == 5 ? "Thứ 6" :
                                  int.Parse(p1.DayinWeek) == 6 ? "Thứ 7" : ""
                         }).Distinct().OrderBy(p => p.IdMain).OrderBy(p => p.DayinWeek).OrderBy(p => p.PermissionId).FirstOrDefault();

            return query;
        }
        public async Task<MajorUserApproval> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.MajorUserApprovals.Where(x => x.IdMain.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn.");
            }
            return entity;
        }
        public async Task<MajorUserApprovalModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.MajorUserApprovals
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.PermissionId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DeptId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.Id == id
                              select new MajorUserApprovalModel
                              {
                                  Id = p1.Id,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  DeptId = departmentId.DeptName,
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  DayinWeek = p1.DayinWeek,
                                  GroupId = p1.GroupId,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<MajorUserApprovalModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.MajorUserApproval_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.UserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DeptId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new MajorUserApprovalModel
                              {
                                  Id = p1.Id,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  DeptId = departmentId != null ? departmentId.DeptName : "",
                                  UserId = ApplicationUsers1.Email,
                                  PermissionId = Permissions1.PermissionName,
                                  DayinWeek = p1.DayinWeek,
                                  GroupId = p1.GroupId,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive
                              }).ToListAsync();

            return data;
        }
        public async Task<List<MajorUserApprovalModel>> GetMajorUserApprovalToDay(ApplicationUser user)
        {
            using var context = _context.CreateDbContext();
            var today = (int)DateTime.Now.DayOfWeek;

            var data = await (from p1 in context.MajorUserApprovals
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join ApplicationUsers1 in context.ApplicationUsers on p1.UserId equals ApplicationUsers1.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.PermissionId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DeptId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.CompanyId == user.CompanyId &&
                                p1.UserId == user.Id &&
                                p1.DayinWeek == today.ToString() &&
                                p1.IsActive == 1
                              select new MajorUserApprovalModel
                              {
                                  Id = p1.Id,
                                  CompanyId = p1.CompanyId,
                                  CompanyName = ChiNhanhs1.TenChiNhanh,
                                  ParentMajorId = p1.ParentMajorId,
                                  ParentMajorName = Majors1.MajorName,
                                  MajorId = p1.MajorId,
                                  MajorName = Majors2.MajorName,
                                  DeptId = p1.DeptId,
                                  DeptName = departmentId.DeptName,
                                  UserId = p1.UserId,
                                  UserName = ApplicationUsers1.Email,
                                  PermissionId = p1.PermissionId,
                                  ApprovalName = Permissions1.PermissionName,
                                  DayinWeek = p1.DayinWeek,
                                  DayInWeekText = int.Parse(p1.DayinWeek) == 0 ? "Chủ nhật" :
                                      int.Parse(p1.DayinWeek) == 1 ? "Thứ 2" :
                                      int.Parse(p1.DayinWeek) == 2 ? "Thứ 3" :
                                      int.Parse(p1.DayinWeek) == 3 ? "Thứ 4" :
                                      int.Parse(p1.DayinWeek) == 4 ? "Thứ 5" :
                                      int.Parse(p1.DayinWeek) == 5 ? "Thứ 6" :
                                      int.Parse(p1.DayinWeek) == 6 ? "Thứ 7" : "",
                                  GroupId = p1.GroupId,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                              }).ToListAsync();
            return data;
        }
        public async Task<bool> CheckPermission(string groupId, string companyId, ApplicationUser user, string PermissionId)
        {
            using var context = _context.CreateDbContext();
            try
            {
                if (user.CreateBy == "symtem" && user.GroupId == groupId)
                {
                    return true;
                }
                else
                {
                    var result = await (from a in context.MajorUserApprovals
                                        where a.UserId.Equals(user.Id)
                                              && a.GroupId.Equals(groupId)
                                              && a.CompanyId.Equals(companyId)
                                              && a.PermissionId.Equals(PermissionId)
                                              && a.DayinWeek.Equals(DateTime.Now.DayOfWeek)
                                        select a.Id).CountAsync();
                    return result > 0;
                }
            }
            catch (Exception)
            {
                throw new Exception($"Bạn không có quyền để thực hiện hành động này");
            }
        }
        public async Task Insert(MajorUserApproval entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có quản lý quyền theo người dùng, nghiệp vụ nào được thêm!");
                }
                context.MajorUserApprovals.Add(entity);
                var addLog = new MajorUserApproval_Log()
                {
                    Id = entity.Id,
                    CompanyId = entity.CompanyId,
                    ParentMajorId = entity.ParentMajorId,
                    MajorId = entity.MajorId,
                    DeptId = entity.DeptId,
                    UserId = entity.UserId,
                    PermissionId = entity.PermissionId,
                    DayinWeek = entity.DayinWeek,
                    GroupId = entity.GroupId,
                    CreateAt = DateTime.Now,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    IdChung = entity.Id,
                    IsValid = true
                };
                context.MajorUserApproval_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(MajorUserApproval data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.IdMain);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý quyền theo người dùng, nghiệp vụ đã chọn");
            }

            var recordsToUpdate = context.MajorUserApprovals.Where(m => m.IdMain == data.IdMain).ToList();
            foreach (var record in recordsToUpdate)
            {
                record.IsActive = data.IsActive;
            }

            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.MajorUserApproval_Logs
                                       where p.IdChung == entity.IdMain && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.MajorUserApproval_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.MajorUserApproval_Logs
                                       where p.IdChung == entity.IdMain && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.MajorUserApproval_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.MajorUserApproval_Logs
                                       where p.IdChung == entity.IdMain
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.MajorUserApproval_Logs.Update(updateLog);
                }
            }
            var addLog = new MajorUserApproval_Log()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyId = entity.CompanyId,
                ParentMajorId = entity.ParentMajorId,
                MajorId = entity.MajorId,
                DeptId = entity.DeptId,
                UserId = entity.UserId,
                PermissionId = entity.PermissionId,
                DayinWeek = entity.DayinWeek,
                GroupId = entity.GroupId,
                CreateAt = DateTime.Now,
                CreateBy = entity.CreateBy,
                IsActive = entity.IsActive,
                IdChung = data.IdMain,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.MajorUserApproval_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task InsertMulti(List<MajorUserApproval> majorUserApprovals)
        {
            using var context = _context.CreateDbContext();
            await context.MajorUserApprovals.AddRangeAsync(majorUserApprovals);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(List<MajorUserApproval> majorUserApprovals, string idMain)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var updateIdMain = from p in context.MajorUserApprovals
                                   where p.IdMain == idMain
                                   select p;
                if (updateIdMain != null)
                {
                    await updateIdMain.ForEachAsync(p => p.IsActive = 100);
                    context.MajorUserApprovals.UpdateRange(updateIdMain);
                }
                await context.MajorUserApprovals.AddRangeAsync(majorUserApprovals);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Lỗi cập nhật dữ liệu."); ;
            }
        }
        public async Task UpdateMulti(MajorUserApproval[] MajorUserApprovals)
        {
            using var context = _context.CreateDbContext();
            string[] ids = MajorUserApprovals.Select(x => x.Id).ToArray();
            var listEntities = await context.MajorUserApprovals.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.MajorUserApprovals.Update(entity);
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
            context.Set<MajorUserApproval>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.MajorUserApprovals.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
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
            //if (model != null && model.TenMajorUserApproval.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên quản lý quyền theo người dùng, nghiệp vụ đã bị thay đổi.");
            //}
            return true;
        }

        public async Task<bool> CheckSave(MajorUserApproval input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.MajorUserApproval_Logs
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
        public async Task<bool> CheckEdit(MajorUserApproval input)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var checkexist = (from p in context.MajorUserApprovals
                                  where p.IdMain == input.IdMain
                                  && p.IsActive != 100
                                  select p).Count();
                if (checkexist == 0)
                {
                    var model = (from p in context.MajorUserApprovals
                                 where p.IsActive != 100
                                 && p.CompanyId == input.CompanyId
                                 && p.MajorId == input.MajorId
                                 && p.PermissionId == input.PermissionId
                                 && p.UserId == input.UserId
                                 select p).Count();
                    if (model > 0)
                    {
                        throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                    }
                }
                else
                {
                    var model = (from p in context.MajorUserApprovals
                                 where p.IsActive != 100 && p.IdMain != input.IdMain
                                 && p.CompanyId == input.CompanyId
                                 && p.MajorId == input.MajorId
                                 && p.PermissionId == input.PermissionId
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
        public async Task<bool> CheckDelete(MajorUserApproval input)
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
    }
}
