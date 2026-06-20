using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DucAnh2025.Services
{
    public class ApprovalTaskRepository : IApprovalTaskRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public ApprovalTaskRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<List<ApprovalTaskModel>> GetAllByVM(ApprovalTaskModel dataModel, string groupId, int skip = 0, int take = 0)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.ApprovalTasks
                            where p1.GroupId == groupId && p1.IsActive != 100
                            select p1;
                if (!string.IsNullOrEmpty(dataModel.ParentMajorId))
                {
                    query = query.Where(m => m.ParentMajorId == dataModel.ParentMajorId);
                }
                if (!string.IsNullOrEmpty(dataModel.MajorId))
                {
                    query = query.Where(m => m.MajorId == dataModel.MajorId);
                }
                if (!string.IsNullOrEmpty(dataModel.ApprovalUserId))
                {
                    query = query.Where(m => m.ApprovalUserId == dataModel.ApprovalUserId);
                }
                if (dataModel.IsActive < 100)
                {
                    query = query.Where(m => m.IsActive == dataModel.IsActive);
                }
                

                var data = await (from p1 in query

                                  join pm in context.Majors on p1.ParentMajorId equals pm.Id into pmJoin
                                  from pm in pmJoin.DefaultIfEmpty()
                                  join m in context.Majors on p1.MajorId equals m.Id into mJoin
                                  from m in mJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()

                                  where p1.GroupId == groupId
                                  orderby p1.CreateAt descending
                                  select new ApprovalTaskModel
                                  {
                                      Id = p1.Id,
                                      Title = p1.Title,
                                      Content = p1.Content,
                                      OriginalId = p1.OriginalId,
                                      RelatedTable = p1.RelatedTable,
                                      ParentMajorId = p1.ParentMajorId,
                                      ParentName = pm.MajorName,
                                      MajorId = p1.MajorId,
                                      MajorName = m.MajorName,
                                      CompanyId = p1.CompanyId,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = (DateTime)p1.CreateAt,
                                      CreateBy = createBy.Email ?? "",
                                      IsActive = p1.IsActive,
                                      ApprovalUserId = approvalUserId.Email ?? "",
                                      DateApproval = (DateTime)p1.DateApproval,
                                      ApprovalDept = approvalDept.DeptName ?? "",
                                      DepartmentId = departmentId != null ? departmentId.Id ?? "" : (p1.DepartmentId ?? ""),
                                      DepartmentOrder = p1.DepartmentOrder ?? 0,
                                      ApprovalOrder = p1.ApprovalOrder ?? 0,
                                      ApprovalId = p1.ApprovalId != null ? p1.ApprovalId : "",
                                      LastApprovalId = p1.LastApprovalId != null ? p1.LastApprovalId : "",
                                      IsStatus = p1.IsStatus
                                  }).Skip(skip)
                                  .Take(take)
                                  .ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<ApprovalTaskModel>> GetAwaitingApprovalTasks(string groupId, ApplicationUser user, int skip = 0, int take = 10, string ParentMajorId ="",string MajorId="")
        {
            try
            {
                int currentDayOfWeek = ((int)DateTime.Now.DayOfWeek + 6) % 7;
                using var context = _context.CreateDbContext();

                IQueryable<ApprovalTask> query;
                if (user.CreateBy == "symtem")
                {
                     query = from a in context.ApprovalTasks
                             where   (a.IsActive == 0 || a.IsActive == 1 || a.IsActive == 2)
                                   && a.GroupId == groupId
                                orderby a.CreateAt descending
                                select a;
                }
                else
                {
                     query = from a in context.ApprovalTasks
                                join mua in context.MajorUserApprovals on a.ParentMajorId equals mua.ParentMajorId
                                join p in context.Permissions on mua.PermissionId equals p.Id
                                where (a.IsActive == 0 && p.PermissionType == 3
                                       || a.IsActive == 1 && p.PermissionType == 4
                                       || a.IsActive == 2 && p.PermissionType == 5)
                                      && mua.IsActive != 100
                                      && mua.IsActive != 100
                                      && mua.DayinWeek == currentDayOfWeek.ToString()
                                      && mua.UserId == user.Id
                                      && a.GroupId == groupId
                                orderby a.CreateAt descending
                                select a;
                }
                    

                if (!string.IsNullOrEmpty(ParentMajorId))
                {
                    query = query.Where(x => x.ParentMajorId == ParentMajorId);
                }

                if (!string.IsNullOrEmpty(MajorId))
                {
                    query = query.Where(x => x.MajorId == MajorId);
                }

                var data = await (from p1 in query

                                  join pm in context.Majors on p1.ParentMajorId equals pm.Id into pmJoin
                                  from pm in pmJoin.DefaultIfEmpty()
                                  join m in context.Majors on p1.MajorId equals m.Id into mJoin
                                  from m in mJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()

                                  where p1.GroupId == groupId
                                  orderby p1.CreateAt descending
                                  select new ApprovalTaskModel
                                  {
                                      Id = p1.Id,
                                      Title = p1.Title,
                                      Content = p1.Content,
                                      OriginalId = p1.OriginalId,
                                      RelatedTable = p1.RelatedTable,
                                      ParentMajorId = p1.ParentMajorId,
                                      ParentName = pm.MajorName,
                                      MajorId = p1.MajorId,
                                      MajorName = m.MajorName,
                                      CompanyId = p1.CompanyId,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = (DateTime)p1.CreateAt,
                                      CreateBy = createBy.Email??"",
                                      IsActive = p1.IsActive,
                                      ApprovalUserId = approvalUserId.Email ?? "",
                                      DateApproval = (DateTime)p1.DateApproval,
                                      ApprovalDept = approvalDept.DeptName ?? "",
                                      DepartmentId = departmentId != null ? departmentId.Id ?? "" : (p1.DepartmentId ?? ""),
                                      DepartmentOrder = p1.DepartmentOrder ?? 0,
                                      ApprovalOrder = p1.ApprovalOrder ?? 0,
                                      ApprovalId = p1.ApprovalId != null ? p1.ApprovalId : "",
                                      LastApprovalId = p1.LastApprovalId != null ? p1.LastApprovalId : "",
                                      IsStatus = p1.IsStatus
                                  }).Distinct()
                                  .Skip(skip)
                                  .Take(take)
                                   .ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<ApprovalTaskModel>> GetApprovalByUserIdTasks(string groupId, ApplicationUser user, int skip = 0, int take = 10, string ParentMajorId = "", string MajorId = "")
        {
            try
            {
                using var context = _context.CreateDbContext();
                var activeStatuses = new[] { 3, 100};
                var query = from p1 in context.ApprovalTasks
                            where p1.GroupId == groupId && activeStatuses.Contains(p1.IsActive) && p1.ApprovalUserId == user.Id
                            select p1;

                if (!string.IsNullOrEmpty(ParentMajorId))
                {
                    query = query.Where(x => x.ParentMajorId == ParentMajorId);
                }

                if (!string.IsNullOrEmpty(MajorId))
                {
                    query = query.Where(x => x.MajorId == MajorId);
                }
                var data = await (from p1 in query

                                  join pm in context.Majors on p1.ParentMajorId equals pm.Id into pmJoin
                                  from pm in pmJoin.DefaultIfEmpty()
                                  join m in context.Majors on p1.MajorId equals m.Id into mJoin
                                  from m in mJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  select new ApprovalTaskModel
                                  {
                                      Id = p1.Id,
                                      Title = p1.Title,
                                      Content = p1.Content,
                                      OriginalId = p1.OriginalId,
                                      RelatedTable = p1.RelatedTable,
                                      ParentMajorId = p1.ParentMajorId,
                                      ParentName = pm.MajorName,
                                      MajorId = p1.MajorId,
                                      MajorName = m.MajorName,
                                      CompanyId = p1.CompanyId,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = p1.CreateAt,
                                      CreateBy = createBy.Email ?? "",
                                      IsActive = p1.IsActive,
                                      ApprovalUserId = approvalUserId.Email ?? "",
                                      DateApproval = p1.DateApproval,
                                      ApprovalDept = approvalDept.DeptName ?? "",
                                      DepartmentId = departmentId != null ? departmentId.Id ?? "" : (p1.DepartmentId ?? ""),
                                      DepartmentOrder = p1.DepartmentOrder ?? 0,
                                      ApprovalOrder = p1.ApprovalOrder ?? 0,
                                      ApprovalId = p1.ApprovalId != null ? p1.ApprovalId : "",
                                      LastApprovalId = p1.LastApprovalId != null ? p1.LastApprovalId : "",
                                      IsStatus = p1.IsStatus
                                  }).OrderByDescending(x => x.DateApproval.HasValue)
                                    .ThenByDescending(x => x.DateApproval).
                                    Skip(skip).Take(take).ToListAsync();

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<Major>> GetAllParentMajors(string groupId, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                // Chuyển đổi sang kiểu int giống DATEPART(WEEKDAY, GETDATE()) - 1
                int currentDayOfWeek = ((int)DateTime.Now.DayOfWeek + 6) % 7;

                var result = await (
                    from at in context.ApprovalTasks
                    join m in context.Majors on at.ParentMajorId equals m.Id
                    join a in context.MajorUserApprovals on m.Id equals a.ParentMajorId
                    where at.GroupId == groupId
                          && (at.IsActive == 0 || at.IsActive == 1 || at.IsActive == 2)
                          && a.UserId == userId
                          && a.GroupId == groupId
                          && a.DayinWeek == currentDayOfWeek.ToString()
                          && a.IsActive != 100
                    select m
                ).Distinct().ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Major>> GetAllMajorByParentId(string groupId ,string parentId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var result = await (
                    from at in context.ApprovalTasks
                    join m in context.Majors on at.MajorId equals m.Id
                    where at.GroupId == groupId && at.IsActive != 100 && at.ParentMajorId == parentId
                    && (at.IsActive == 0 || at.IsActive == 1 || at.IsActive == 2)
                    select m
                ).Distinct().ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ApprovalTask>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalTasks.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<ApprovalTask> GetById(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalTasks.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy id đã chọn.");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<ApprovalTask> GetByOriginalId(string originalId)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ApprovalTasks
            .Where(x => x.OriginalId.Trim() == originalId.Trim() && x.IsActive != 100)
            .FirstOrDefaultAsync();

            if (entity == null)
            {
                return null;
            }
            return entity;
        }
        public async Task Insert(ApprovalTask entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có thông tin nào được thêm!");
                }
                context.ApprovalTasks.Add(entity);
                var addLog = new ApprovalTask_Log()
                {
                    Id = entity.Id,

                    Title = entity.Title,
                    Content = entity.Content,
                    OriginalId = entity.OriginalId,
                    RelatedTable = entity.RelatedTable,

                    ParentMajorId = entity.ParentMajorId,
                    MajorId = entity.MajorId,
                    CompanyId = entity.CompanyId,

                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = DateTime.Now,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = entity.DateApproval,
                    ApprovalDept = entity.ApprovalDept,
                    DepartmentId = entity.DepartmentId,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,

                };
                context.ApprovalTask_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(ApprovalTask data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.ApprovalTasks.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ApprovalTask_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalTask_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ApprovalTask_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalTask_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ApprovalTask_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ApprovalTask_Logs.Update(updateLog);
                }
            }
            var addLog = new ApprovalTask_Log
            {
                Id = Guid.NewGuid().ToString(),
                Title = data.Title,
                Content = data.Content,
                OriginalId = data.OriginalId,
                RelatedTable = data.RelatedTable,

                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                CompanyId = data.CompanyId,

                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = data.ApprovalUserId,
                DateApproval = data.DateApproval,
                ApprovalDept = data.ApprovalDept,
                DepartmentId = data.DepartmentId,
                DepartmentOrder = data.DepartmentOrder,
                ApprovalOrder = data.ApprovalOrder,
                ApprovalId = data.ApprovalId,
                LastApprovalId = data.LastApprovalId,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.ApprovalTask_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(ApprovalTask[] ApprovalTasks)
        {
            using var context = _context.CreateDbContext();
            string[] ids = ApprovalTasks.Select(x => x.Id).ToArray();
            var listEntities = await context.ApprovalTasks.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.ApprovalTasks.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(ApprovalTask data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa";
                    data.ApprovalUserId = userId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.ApprovalTask_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Title = logdata.Title;
                        data.Content = logdata.Content;
                        data.OriginalId = logdata.OriginalId;
                        data.RelatedTable = logdata.RelatedTable;

                        data.ParentMajorId = logdata.ParentMajorId;
                        data.MajorId = logdata.MajorId;
                        data.CompanyId = logdata.CompanyId;

                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.ApprovalTask_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.ApprovalTask_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalTask_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalTask_Log()
                    {
                        Id = Guid.NewGuid().ToString(),

                        Title = data.Title,
                        Content = data.Content,
                        OriginalId = data.OriginalId,
                        RelatedTable = data.RelatedTable,

                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        CompanyId = data.CompanyId,

                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
                        ApprovalDept = data.ApprovalDept,
                        DepartmentId = data.DepartmentId,
                        DepartmentOrder = data.DepartmentOrder,
                        ApprovalOrder = data.ApprovalOrder,
                        ApprovalId = data.ApprovalId,
                        LastApprovalId = data.LastApprovalId,
                        IsStatus = data.IsStatus,
                        IsValid = true,
                        IdChung = data.Id
                    };
                    addLog.Ordinarily = logdata.Ordinarily;
                    context.ApprovalTask_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.ApprovalTask_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalTask_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalTask_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = data.Title,
                        Content = data.Content,
                        OriginalId = data.OriginalId,
                        RelatedTable = data.RelatedTable,

                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        CompanyId = data.CompanyId,

                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
                        ApprovalDept = data.ApprovalDept,
                        DepartmentId = data.DepartmentId,
                        DepartmentOrder = data.DepartmentOrder,
                        ApprovalOrder = data.ApprovalOrder,
                        ApprovalId = data.ApprovalId,
                        LastApprovalId = data.LastApprovalId,
                        IsStatus = data.IsStatus,
                        IdChung = data.Id,
                        IsValid = true
                    };
                    context.ApprovalTask_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.ApprovalTasks.Update(data);
            await context.SaveChangesAsync();
        }
        
        public async Task Approval(ApprovalTask data, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await GetById(data.Id);
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy chi nhánh đã chọn");
                }

                // --- BẮT ĐẦU: Cập nhật bảng liên quan trước khi duyệt ---
                if (!string.IsNullOrEmpty(data.RelatedTable) && data.OriginalId != null)
                {
                    // Lấy DbSet động theo tên bảng
                    var dbSetProp = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(data.RelatedTable, StringComparison.OrdinalIgnoreCase));
                    if (dbSetProp != null)
                    {
                        var dbSet = dbSetProp.GetValue(context);
                        var findAsync = dbSet?.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
                        if (findAsync != null)
                        {
                            var valueTask = findAsync.Invoke(dbSet, new object[] { new object[] { data.OriginalId } });
                            // Handle ValueTask<TEntity>
                            var valueTaskType = valueTask.GetType();
                            var resultProperty = valueTaskType.GetProperty("Result");
                            object relatedEntity = null;
                            if (resultProperty != null)
                            {
                                // If already completed, get Result
                                relatedEntity = resultProperty.GetValue(valueTask);
                            }
                            else
                            {
                                // Await ValueTask dynamically
                                var asTaskMethod = valueTaskType.GetMethod("AsTask");
                                if (asTaskMethod != null)
                                {
                                    var task = (Task)asTaskMethod.Invoke(valueTask, null);
                                    await task.ConfigureAwait(false);
                                    var taskResultProperty = task.GetType().GetProperty("Result");
                                    relatedEntity = taskResultProperty?.GetValue(task);
                                }
                            }

                            if (relatedEntity != null)
                            {
                                // Copy các trường giống nhau từ data sang relatedEntity (trừ Id)
                                var dataProps = data.GetType().GetProperties();
                                var relatedProps = relatedEntity.GetType().GetProperties();
                                foreach (var prop in dataProps)
                                {
                                    if (prop.Name == "Id" || prop.Name == "OriginalId" || prop.Name == "RelatedTable") continue;
                                    var targetProp = relatedProps.FirstOrDefault(p => p.Name == prop.Name && p.CanWrite);
                                    if (targetProp != null)
                                    {
                                        targetProp.SetValue(relatedEntity, prop.GetValue(data));
                                    }
                                }
                                // Đánh dấu entity đã thay đổi
                                context.Entry(relatedEntity).State = EntityState.Modified;
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception($"Không tìm thấy bảng thông tin");
                }
                // --- KẾT THÚC: Cập nhật bảng liên quan ---

                context.ApprovalTasks.Update(data);

                if (data.IsActive == 3)
                {
                    var updateLog = await (from p in context.ApprovalTask_Logs
                                           where p.IdChung == entity.Id && p.IsValid == true
                                           select p).ToListAsync();
                    updateLog.ForEach(p => p.IsValid = false);
                    context.ApprovalTask_Logs.UpdateRange(updateLog);
                }
                else if (data.IsActive == 100)
                {
                    var updateLog = await (from p in context.ApprovalTask_Logs
                                           where p.IdChung == entity.Id && p.IsValid == true
                                           select p).ToListAsync();
                    updateLog.ForEach(p => p.IsValid = false);
                    context.ApprovalTask_Logs.UpdateRange(updateLog);
                }
                else if (entity.IsActive != 3)
                {
                    var updateLog = await (from p in context.ApprovalTask_Logs
                                           where p.IdChung == entity.Id
                                           select p).OrderByDescending(p => p.CreateAt)
                    .FirstOrDefaultAsync();
                    if (updateLog != null)
                    {
                        updateLog.IsValid = false;
                        context.ApprovalTask_Logs.Update(updateLog);
                    }
                }
                // --- BẮT ĐẦU: Cập nhật bảng log động theo RelatedTable ---
                if (!string.IsNullOrEmpty(data.RelatedTable))
                {
                    var tableName = entity.RelatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        ? entity.RelatedTable.Substring(0, entity.RelatedTable.Length - 1)
                        : entity.RelatedTable;
                    var logTableName = tableName + "_Logs";
                    var logDbSetProp = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(logTableName, StringComparison.OrdinalIgnoreCase));
                    if (logDbSetProp != null)
                    {
                        var logDbSet = logDbSetProp.GetValue(context);
                        var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
                        if (logEntityType != null)
                        {
                            var queryable = logDbSet as IQueryable<object>;
                            var idChungProp = logEntityType.GetProperty("IdChung");
                            var isValidProp = logEntityType.GetProperty("IsValid");
                            var createAtProp = logEntityType.GetProperty("CreateAt");
                            var isActiveProp = logEntityType.GetProperty("IsActive");

                            var logs = queryable.AsEnumerable().Where(x => idChungProp.GetValue(x).ToString() == entity.OriginalId).ToList();
                            var validLogs = logs.Where(x => (isValidProp.GetValue(x) as bool?) == true).ToList();

                            if (data.IsActive == 3 || data.IsActive == 100)
                            {
                                foreach (var log in logs)
                                {
                                    isValidProp.SetValue(log, false);
                                }
                                var castMethod = typeof(Enumerable)
                                .GetMethod("Cast")!
                                .MakeGenericMethod(logEntityType);

                                var castedLogs = castMethod.Invoke(null, new object[] { logs });

                                var updateRangeMethod = logDbSet.GetType().GetMethod(
                                    "UpdateRange",
                                    new[] { typeof(IEnumerable<>).MakeGenericType(logEntityType) }
                                );

                                updateRangeMethod?.Invoke(logDbSet, new object[] { castedLogs });

                            }
                            else if (entity.IsActive != 3)
                            {
                                var latestLog = logs.OrderByDescending(x => createAtProp.GetValue(x)).FirstOrDefault();
                                if (latestLog != null)
                                {
                                    isValidProp.SetValue(latestLog, false);
                                    var updateMethod = logDbSet.GetType().GetMethod("Update");
                                    updateMethod?.Invoke(logDbSet, new object[] { latestLog });
                                }
                            }
                        }
                    }
                }
                await AddRelatedTableAsync(context, entity, data.IsActive, data.IsStatus, userId);
                // --- KẾT THÚC: Cập nhật bảng log động ---

                var addLog = new ApprovalTask_Log()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = data.Title,
                    Content = data.Content,
                    OriginalId = data.OriginalId,
                    RelatedTable = data.RelatedTable,

                    ParentMajorId = data.ParentMajorId,
                    MajorId = data.MajorId,
                    CompanyId = data.CompanyId,

                    GroupId = data.GroupId,
                    Ordinarily = data.Ordinarily,
                    CreateAt = DateTime.Now,
                    CreateBy = data.CreateBy,
                    IsActive = data.IsActive,
                    ApprovalUserId = data.ApprovalUserId,
                    DateApproval = data.DateApproval,
                    ApprovalDept = data.ApprovalDept,
                    DepartmentId = data.DepartmentId,
                    DepartmentOrder = data.DepartmentOrder,
                    ApprovalOrder = data.ApprovalOrder,
                    ApprovalId = data.ApprovalId,
                    LastApprovalId = data.LastApprovalId,
                    IsStatus = data.IsStatus,
                    IdChung = data.Id,
                    IsValid = data.IsActive == 100 ? false : true
                };
                context.ApprovalTask_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        private async Task AddRelatedTableAsync(ApplicationDbContext context, ApprovalTask entity, int isActive, string isStatus, string approvalUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.RelatedTable) || string.IsNullOrEmpty(entity.OriginalId))
                    return;

                // Chuẩn hóa tên bảng chính và bảng log
                var tableName = entity.RelatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                    ? entity.RelatedTable.Substring(0, entity.RelatedTable.Length - 1)
                    : entity.RelatedTable;
                var logTableName = tableName + "_Logs";

                // Lấy DbSet bảng chính theo tên property của DbContext
                var dbSetProp = context.GetType().GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(entity.RelatedTable, StringComparison.OrdinalIgnoreCase));
                if (dbSetProp == null) return;

                var dbSet = dbSetProp.GetValue(context);
                if (dbSet == null) return;

                // Gọi DbSet<TEntity>.FindAsync(params object[] keyValues) bằng reflection
                var findAsync = dbSet.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
                if (findAsync == null) return;

                var valueTask = findAsync.Invoke(dbSet, new object[] { new object[] { entity.OriginalId } });
                if (valueTask == null) return;

                var vtType = valueTask.GetType();
                var asTaskMethod = vtType.GetMethod("AsTask", Type.EmptyTypes);
                if (asTaskMethod == null) return;

                var taskObj = (Task)asTaskMethod.Invoke(valueTask, null);
                await taskObj.ConfigureAwait(false);

                var taskResultProp = taskObj.GetType().GetProperty("Result");
                if (taskResultProp == null) return;

                object mainEntity = taskResultProp.GetValue(taskObj);
                if (mainEntity == null) return;

                // Lấy DbSet bảng log
                var logDbSetProp = context.GetType().GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(logTableName, StringComparison.OrdinalIgnoreCase));
                if (logDbSetProp == null) return;

                var logDbSet = logDbSetProp.GetValue(context);
                if (logDbSet == null) return;

                var logEntityType = logDbSet.GetType().GenericTypeArguments.FirstOrDefault();
                if (logEntityType == null) return;

                // Tạo thực thể log
                var logEntity = Activator.CreateInstance(logEntityType);
                if (logEntity == null) return;

                // Lấy danh sách property kiểu tĩnh
                var logProps = logEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var mainProps = mainEntity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Dựng map để tra cứu nhanh theo tên, tránh dùng lambda trên 'dynamic'
                var mainPropMap = mainProps.ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);

                foreach (var prop in logProps)
                {
                    switch (prop.Name)
                    {
                        case "Id":
                            prop.SetValue(logEntity, Guid.NewGuid().ToString());
                            break;
                        case "IdChung":
                            prop.SetValue(logEntity, entity.OriginalId);
                            break;
                        case "IsActive":
                            prop.SetValue(logEntity, isActive);
                            break;
                        case "IsStatus":
                            prop.SetValue(logEntity, isStatus);
                            break;
                        case "ApprovalUserId":
                            prop.SetValue(logEntity, approvalUserId);
                            break;
                        case "CreateAt":
                            prop.SetValue(logEntity, DateTime.Now);
                            break;
                        case "IsValid":
                            var valid = isActive == 100 ? false : true;
                            prop.SetValue(logEntity, valid);
                            break;
                        default:
                            if (mainPropMap.TryGetValue(prop.Name, out var mainProp)
                                && mainProp.CanRead && prop.CanWrite)
                            {
                                var val = mainProp.GetValue(mainEntity);
                                prop.SetValue(logEntity, val);
                            }
                            break;
                    }
                }

                // Thêm vào DbSet log và lưu
                var addMethod = logDbSet.GetType().GetMethod("Add", new[] { logEntityType });
                if (addMethod == null) addMethod = logDbSet.GetType().GetMethod("Add"); // dự phòng
                addMethod?.Invoke(logDbSet, new[] { logEntity });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task NoApproval(ApprovalTask data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    entity.IsActive = 90;
                    entity.IsStatus = "Không duyệt";
                    entity.ApprovalUserId = data.ApprovalUserId;
                    await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 90, "Không duyệt", data.ApprovalUserId);
                    // Thêm log vào bảng log động
                    await AddRelatedTableLogAsync(context, entity, 90, "Không duyệt", data.ApprovalUserId);
                }
                else if (entity.IsActive == 1)
                {
                    // Xác định bảng log từ RelatedTable
                    var tableName = entity.RelatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        ? entity.RelatedTable.Substring(0, entity.RelatedTable.Length - 1)
                        : entity.RelatedTable;
                    var logTableName = tableName + "_Logs";

                    // Lấy DbSet của bảng log
                    var logDbSetProp = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(logTableName, StringComparison.OrdinalIgnoreCase));
                    if (logDbSetProp == null) return;

                    var logDbSet = logDbSetProp.GetValue(context);
                    var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
                    if (logEntityType == null) return;

                    // Tìm bản ghi log hợp lệ (IsValid == true) sớm nhất theo CreateAt
                    var logQueryable = logDbSet as IQueryable<object>;
                    var idChungProp = logEntityType.GetProperty("IdChung");
                    var isValidProp = logEntityType.GetProperty("IsValid");
                    var createAtProp = logEntityType.GetProperty("CreateAt");

                    var validLogs = logQueryable.ToList()
                        .Where(x => (idChungProp.GetValue(x)?.ToString() ?? "") == entity.OriginalId
                                 && (bool?)isValidProp.GetValue(x) == true)
                        .OrderBy(x => (DateTime)createAtProp.GetValue(x))
                        .ToList();

                    var logData = validLogs.FirstOrDefault();
                    if (logData == null) return;

                    // Lấy DbSet của bảng gốc
                    var dbSetProp = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(entity.RelatedTable, StringComparison.OrdinalIgnoreCase));
                    if (dbSetProp == null) return;

                    var dbSet = dbSetProp.GetValue(context);
                    var entityType = dbSetProp.PropertyType.GetGenericArguments()[0];
                    var keyProp = entityType.GetProperty("Id");

                    // Tìm entity gốc theo OriginalId
                    var findMethod = dbSet.GetType().GetMethod("Find", new[] { typeof(object[]) });
                    var dbEntity = findMethod?.Invoke(dbSet, new object[] { new object[] { entity.OriginalId } });
                    if (dbEntity == null) return;

                    // Copy dữ liệu từ log về entity gốc (trừ Id, IdChung, RelatedTable)
                    var logProps = logEntityType.GetProperties();
                    foreach (var prop in logProps)
                    {
                        if (prop.Name == "Id" || prop.Name == "IdChung" || prop.Name == "RelatedTable") continue;

                        var dbProp = entityType.GetProperty(prop.Name);
                        if (dbProp != null && dbProp.CanWrite)
                        {
                            var val = prop.GetValue(logData);
                            dbProp.SetValue(dbEntity, val);
                        }
                    }

                    // Đánh dấu entity gốc đã bị sửa đổi
                    context.Entry(dbEntity).State = EntityState.Modified;

                    // Cập nhật lại trạng thái cho log vừa dùng
                    var isActiveProp = logEntityType.GetProperty("IsActive");
                    isActiveProp?.SetValue(logData, 3);

                    var isStatusProp = logEntityType.GetProperty("IsStatus");
                    isStatusProp?.SetValue(logData, "Đã duyệt");

                    // Lưu thay đổi
                    await context.SaveChangesAsync();

                    // Cập nhật trạng thái cho ApprovalTask
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";

                    // Đồng bộ lại bảng liên quan
                    await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 3, "Đã duyệt", userId);

                    // Thêm log mới
                    await AddRelatedTableLogAsync(context, entity, 3, "Đã duyệt", userId);
                }
                else if (entity.IsActive == 2)
                {
                    // Xác định bảng log động từ RelatedTable
                    var tableName = entity.RelatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        ? entity.RelatedTable.Substring(0, entity.RelatedTable.Length - 1)
                        : entity.RelatedTable;
                    var logTableName = tableName + "_Logs";

                    // Lấy DbSet của bảng log động
                    var logDbSetProp = context.GetType().GetProperties()
                        .FirstOrDefault(p => p.Name.Equals(logTableName, StringComparison.OrdinalIgnoreCase));
                    if (logDbSetProp != null)
                    {
                        var logDbSet = logDbSetProp.GetValue(context);
                        var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
                        if (logEntityType != null)
                        {
                            // Lấy bản ghi log hợp lệ đầu tiên (IsValid == true, sắp xếp tăng dần CreateAt)
                            var logQueryable = logDbSet as IQueryable<object>;
                            var idChungProp = logEntityType.GetProperty("IdChung");
                            var isValidProp = logEntityType.GetProperty("IsValid");
                            var createAtProp = logEntityType.GetProperty("CreateAt");

                            var allLogs = logQueryable.ToList();
                            var logs = allLogs
                                .Where(x => (idChungProp.GetValue(x)?.ToString() ?? "") == entity.OriginalId && (bool?)isValidProp.GetValue(x) == true)
                                .OrderBy(x => (DateTime)createAtProp.GetValue(x))
                                .ToList();

                            var logdata = logs.FirstOrDefault();
                            if (logdata != null)
                            {
                                // Copy các trường từ dữ liệu hiện tại (data) về entity (trừ Id, OriginalId, RelatedTable)
                                var logProps = logEntityType.GetProperties();
                                var entityProps = entity.GetType().GetProperties();
                                foreach (var prop in logProps)
                                {
                                    if (prop.Name == "Id" || prop.Name == "OriginalId" || prop.Name == "RelatedTable") continue;
                                    var entityProp = entityProps.FirstOrDefault(p => p.Name == prop.Name && p.CanWrite);
                                    if (entityProp != null)
                                    {
                                        var dataProp = data.GetType().GetProperty(prop.Name);
                                        if (dataProp != null)
                                        {
                                            entityProp.SetValue(entity, dataProp.GetValue(data));
                                        }
                                    }
                                }
                            }

                            entity.IsActive = 3;
                            entity.IsStatus = "Đã duyệt";

                            // Cập nhật bảng liên quan
                            await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 3, "Đã duyệt", userId);

                            // Thêm log vào bảng log động
                            await AddRelatedTableLogAsync(context, entity, 3, "Đã duyệt", userId);
                        }
                    }
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại.");
                }
            }
            var addLog = new ApprovalTask_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Title = entity.Title,
                Content = entity.Content,
                OriginalId = entity.OriginalId,
                RelatedTable = entity.RelatedTable,

                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = userId,
                DateApproval = DateTime.Now,
                ApprovalDept = data.DepartmentId,
                DepartmentId = null,
                DepartmentOrder = 0,
                ApprovalOrder = 0,
                ApprovalId = null,
                LastApprovalId = null,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = false
            };
            context.ApprovalTask_Logs.Add(addLog);
            context.ApprovalTasks.Update(entity);
            await context.SaveChangesAsync();
        }

        // Helper method to update IsActive/IsStatus/ApprovalUserId in a dynamic table
        private async Task UpdateRelatedTableAsync(ApplicationDbContext context, string tableName, string originalId, int isActive, string isStatus, string approvalUserId)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(originalId))
                return;

            // Find the DbSet property by name (case-insensitive)
            var dbSetProp = context.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            if (dbSetProp == null)
                return;

            var dbSet = dbSetProp.GetValue(context);
            var findAsync = dbSet?.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
            if (findAsync == null)
                return;

            var valueTask = findAsync.Invoke(dbSet, new object[] { new object[] { originalId } });
            var valueTaskType = valueTask.GetType();
            var resultProperty = valueTaskType.GetProperty("Result");
            object relatedEntity = null;
            if (resultProperty != null)
            {
                relatedEntity = resultProperty.GetValue(valueTask);
            }
            else
            {
                var asTaskMethod = valueTaskType.GetMethod("AsTask");
                if (asTaskMethod != null)
                {
                    var task = (Task)asTaskMethod.Invoke(valueTask, null);
                    await task.ConfigureAwait(false);
                    var taskResultProperty = task.GetType().GetProperty("Result");
                    relatedEntity = taskResultProperty?.GetValue(task);
                }
            }

            if (relatedEntity != null)
            {
                var type = relatedEntity.GetType();
                var isActiveProp = type.GetProperty("IsActive");
                var isStatusProp = type.GetProperty("IsStatus");
                var approvalUserIdProp = type.GetProperty("ApprovalUserId");

                isActiveProp?.SetValue(relatedEntity, isActive);
                isStatusProp?.SetValue(relatedEntity, isStatus);
                approvalUserIdProp?.SetValue(relatedEntity, approvalUserId);

                context.Entry(relatedEntity).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
        private async Task AddRelatedTableLogAsync(ApplicationDbContext context, ApprovalTask entity, int isActive, string isStatus, string approvalUserId)
        {
            if (string.IsNullOrEmpty(entity.RelatedTable) || string.IsNullOrEmpty(entity.OriginalId))
                return;

            var tableName = entity.RelatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                ? entity.RelatedTable.Substring(0, entity.RelatedTable.Length - 1)
                : entity.RelatedTable;
            var logTableName = tableName + "_Logs";

            // Lấy DbSet của bảng chính
            var dbSetProp = context.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            if (dbSetProp == null)
                return;

            var dbSet = dbSetProp.GetValue(context);
            var findAsync = dbSet?.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
            if (findAsync == null)
                return;

            var valueTask = findAsync.Invoke(dbSet, new object[] { new object[] { entity.OriginalId } });
            object mainEntity = null;
            var valueTaskType = valueTask.GetType();
            var resultProperty = valueTaskType.GetProperty("Result");
            if (resultProperty != null)
            {
                mainEntity = resultProperty.GetValue(valueTask);
            }
            else
            {
                var asTaskMethod = valueTaskType.GetMethod("AsTask");
                if (asTaskMethod != null)
                {
                    var task = (Task)asTaskMethod.Invoke(valueTask, null);
                    await task.ConfigureAwait(false);
                    var taskResultProperty = task.GetType().GetProperty("Result");
                    mainEntity = taskResultProperty?.GetValue(task);
                }
            }
            if (mainEntity == null)
                return;

            // Lấy DbSet của bảng log
            var logDbSetProp = context.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.Equals(logTableName, StringComparison.OrdinalIgnoreCase));
            if (logDbSetProp == null)
                return;

            var logDbSet = logDbSetProp.GetValue(context);
            var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
            if (logEntityType == null)
                return;

            // Tạo instance log động
            var logEntity = Activator.CreateInstance(logEntityType);
            if (logEntity == null)
                return;

            // Gán các trường đặc biệt, còn lại lấy từ mainEntity
            var props = logEntityType.GetProperties();
            var mainProps = mainEntity.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == "Id")
                    prop.SetValue(logEntity, Guid.NewGuid().ToString());
                else if (prop.Name == "IdChung")
                    prop.SetValue(logEntity, entity.OriginalId);
                else if (prop.Name == "IsActive")
                    prop.SetValue(logEntity, isActive);
                else if (prop.Name == "IsStatus")
                    prop.SetValue(logEntity, isStatus);
                else if (prop.Name == "ApprovalUserId")
                    prop.SetValue(logEntity, approvalUserId);
                else if (prop.Name == "CreateAt")
                    prop.SetValue(logEntity, DateTime.Now);
                else if (prop.Name == "IsValid")
                    prop.SetValue(logEntity, false);
                else
                {
                    // Lấy giá trị từ entity gốc
                    var mainProp = mainProps.FirstOrDefault(p => p.Name == prop.Name && p.CanRead && prop.CanWrite);
                    if (mainProp != null)
                    {
                        prop.SetValue(logEntity, mainProp.GetValue(mainEntity));
                    }
                }
            }

            // Thêm vào DbSet
            var addMethod = logDbSet.GetType().GetMethod("Add");
            addMethod?.Invoke(logDbSet, new[] { logEntity });
            await context.SaveChangesAsync();
        }


        //public async Task Approval(ApprovalTask data, string userId)
        //{
        //    try
        //    {
        //        using var context = _context.CreateDbContext();
        //        var entity = await GetById(data.Id);
        //        if (entity == null)
        //            throw new Exception($"Không tìm thấy thông tin đã chọn");

        //        // --- BẮT ĐẦU: Cập nhật bảng liên quan trước khi duyệt ---
        //        if (!string.IsNullOrEmpty(data.RelatedTable) && data.OriginalId != null)
        //        {
        //            var dbSet = GetDbSet(context, data.RelatedTable);
        //            var relatedEntity = await FindEntityAsync(dbSet, data.OriginalId);
        //            if (relatedEntity != null)
        //            {
        //                CopyProperties(data, relatedEntity, "Id", "OriginalId", "RelatedTable");
        //                context.Entry(relatedEntity).State = EntityState.Modified;
        //                await context.SaveChangesAsync();
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Không tìm thấy bảng thông tin");
        //        }
        //        // --- KẾT THÚC: Cập nhật bảng liên quan ---

        //        context.ApprovalTasks.Update(data);

        //        // --- Cập nhật log cũ ---
        //        if (data.IsActive == 3 || data.IsActive == 100)
        //        {
        //            var updateLogs = await context.ApprovalTask_Logs
        //                .Where(p => p.IdChung == entity.Id && p.IsValid)
        //                .ToListAsync();
        //            updateLogs.ForEach(p => p.IsValid = false);
        //            context.ApprovalTask_Logs.UpdateRange(updateLogs);
        //        }
        //        else if (entity.IsActive != 3)
        //        {
        //            var updateLog = await context.ApprovalTask_Logs
        //                .Where(p => p.IdChung == entity.Id)
        //                .OrderByDescending(p => p.CreateAt)
        //                .FirstOrDefaultAsync();
        //            if (updateLog != null)
        //            {
        //                updateLog.IsValid = false;
        //                context.ApprovalTask_Logs.Update(updateLog);
        //            }
        //        }

        //        // --- BẮT ĐẦU: Cập nhật bảng log động theo RelatedTable ---
        //        if (!string.IsNullOrEmpty(data.RelatedTable))
        //        {
        //            var logTableName = GetLogTableName(data.RelatedTable);
        //            var logDbSet = GetDbSet(context, logTableName);
        //            var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
        //            if (logEntityType != null)
        //            {
        //                var queryable = logDbSet as IQueryable<object>;
        //                var idChungProp = logEntityType.GetProperty("IdChung");
        //                var isValidProp = logEntityType.GetProperty("IsValid");
        //                var createAtProp = logEntityType.GetProperty("CreateAt");

        //                var logs = queryable.ToList()
        //                .Where(x => idChungProp.GetValue(x) != null && idChungProp.GetValue(x).ToString() == entity.Id)
        //                .ToList();


        //                if (data.IsActive == 3 || data.IsActive == 100)
        //                {
        //                    foreach (var log in logs) isValidProp.SetValue(log, false);
        //                    logDbSet.GetType().GetMethod("UpdateRange")?.Invoke(logDbSet, new object[] { logs });
        //                }
        //                else if (entity.IsActive != 3)
        //                {
        //                    var latestLog = logs.OrderByDescending(x => createAtProp.GetValue(x)).FirstOrDefault();
        //                    if (latestLog != null)
        //                    {
        //                        isValidProp.SetValue(latestLog, false);
        //                        logDbSet.GetType().GetMethod("Update")?.Invoke(logDbSet, new object[] { latestLog });
        //                    }
        //                }
        //            }
        //        }
        //        // --- KẾT THÚC: Cập nhật bảng log động ---

        //        // --- Thêm log mới ---
        //        var addLog = new ApprovalTask_Log()
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Title = data.Title,
        //            Content = data.Content,
        //            OriginalId = data.OriginalId,
        //            RelatedTable = data.RelatedTable,
        //            ParentMajorId = data.ParentMajorId,
        //            MajorId = data.MajorId,
        //            CompanyId = data.CompanyId,
        //            GroupId = data.GroupId,
        //            Ordinarily = data.Ordinarily,
        //            CreateAt = DateTime.Now,
        //            CreateBy = data.CreateBy,
        //            IsActive = data.IsActive,
        //            ApprovalUserId = data.ApprovalUserId,
        //            DateApproval = data.DateApproval,
        //            ApprovalDept = data.ApprovalDept,
        //            DepartmentId = data.DepartmentId,
        //            DepartmentOrder = data.DepartmentOrder,
        //            ApprovalOrder = data.ApprovalOrder,
        //            ApprovalId = data.ApprovalId,
        //            LastApprovalId = data.LastApprovalId,
        //            IsStatus = data.IsStatus,
        //            IdChung = data.Id,
        //            IsValid = data.IsActive == 100 ? false : true
        //        };
        //        context.ApprovalTask_Logs.Add(addLog);
        //        await context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

        //public async Task NoApproval(ApprovalTask data, string userId)
        //{
        //    using var context = _context.CreateDbContext();
        //    var entity = await GetById(data.Id);
        //    if (entity == null || entity.IsActive != data.IsActive)
        //        throw new Exception($"Không tìm thấy dữ liệu đã chọn");

        //    if (entity.IsActive == 0)
        //    {
        //        entity.IsActive = 90;
        //        entity.IsStatus = "Không duyệt";
        //        entity.ApprovalUserId = data.ApprovalUserId;
        //        await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 90, "Không duyệt", data.ApprovalUserId);
        //        await AddRelatedTableLogAsync(context, entity, 90, "Không duyệt", data.ApprovalUserId);
        //    }
        //    else if (entity.IsActive == 1 || entity.IsActive == 2)
        //    {
        //        var logTableName = GetLogTableName(entity.RelatedTable);
        //        var logDbSet = GetDbSet(context, logTableName);
        //        var logEntityType = logDbSet?.GetType().GenericTypeArguments.FirstOrDefault();
        //        if (logEntityType != null)
        //        {
        //            var logQueryable = logDbSet as IQueryable<object>;
        //            var idChungProp = logEntityType.GetProperty("IdChung");
        //            var isValidProp = logEntityType.GetProperty("IsValid");
        //            var createAtProp = logEntityType.GetProperty("CreateAt");

        //            var logs = logQueryable.ToList()
        //                .Where(x => idChungProp.GetValue(x)?.ToString() == entity.OriginalId && (bool?)isValidProp.GetValue(x) == true)
        //                .OrderBy(x => (DateTime)createAtProp.GetValue(x))
        //                .ToList();

        //            var logdata = logs.FirstOrDefault();
        //            if (logdata != null)
        //            {
        //                if (entity.IsActive == 1)
        //                {
        //                    CopyProperties(logdata, entity, "Id", "OriginalId", "RelatedTable");
        //                    entity.IsActive = 90;
        //                    entity.IsStatus = "Không duyệt";
        //                    entity.ApprovalUserId = userId;
        //                    await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 90, "Không duyệt", userId);
        //                    await AddRelatedTableLogAsync(context, entity, 90, "Không duyệt", userId);
        //                }
        //                else if (entity.IsActive == 2)
        //                {
        //                    CopyProperties(data, entity, "Id", "OriginalId", "RelatedTable");
        //                    entity.IsActive = 3;
        //                    entity.IsStatus = "Đã duyệt";
        //                    await UpdateRelatedTableAsync(context, entity.RelatedTable, entity.OriginalId, 3, "Đã duyệt", userId);
        //                    await AddRelatedTableLogAsync(context, entity, 3, "Đã duyệt", userId);
        //                }
        //            }
        //        }
        //    }
        //    else if (entity.IsActive == 3)
        //    {
        //        throw new Exception($"Thông tin hủy duyệt không tồn tại.");
        //    }

        //    // Thêm log vào bảng ApprovalTask_Log
        //    var addLog = new ApprovalTask_Log()
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        Title = entity.Title,
        //        Content = entity.Content,
        //        OriginalId = entity.OriginalId,
        //        RelatedTable = entity.RelatedTable,
        //        GroupId = data.GroupId,
        //        Ordinarily = data.Ordinarily,
        //        CreateAt = DateTime.Now,
        //        CreateBy = data.CreateBy,
        //        IsActive = data.IsActive,
        //        ApprovalUserId = userId,
        //        DateApproval = DateTime.Now,
        //        ApprovalDept = data.DepartmentId,
        //        DepartmentId = null,
        //        DepartmentOrder = 0,
        //        ApprovalOrder = 0,
        //        ApprovalId = null,
        //        LastApprovalId = null,
        //        IsStatus = data.IsStatus,
        //        IdChung = data.Id,
        //        IsValid = false
        //    };
        //    context.ApprovalTask_Logs.Add(addLog);
        //    context.ApprovalTasks.Update(entity);
        //    await context.SaveChangesAsync();
        //}

        //#region Helpers
        //private static object? GetDbSet(ApplicationDbContext context, string tableName) =>
        //    context.GetType().GetProperties()
        //        .FirstOrDefault(p => p.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase))
        //        ?.GetValue(context);
        //private static string GetLogTableName(string relatedTable)
        //{
        //    var tableName = relatedTable.EndsWith("s", StringComparison.OrdinalIgnoreCase)
        //        ? relatedTable.Substring(0, relatedTable.Length - 1)
        //        : relatedTable;
        //    return tableName + "_Logs";
        //}
        //private static async Task<object?> FindEntityAsync(object dbSet, string id)
        //{
        //    var findAsync = dbSet?.GetType().GetMethod("FindAsync", new[] { typeof(object[]) });
        //    if (findAsync == null) return null;

        //    var valueTask = findAsync.Invoke(dbSet, new object[] { new object[] { id } });
        //    if (valueTask == null) return null;

        //    var valueTaskType = valueTask.GetType();
        //    var resultProperty = valueTaskType.GetProperty("Result");
        //    if (resultProperty != null)
        //        return resultProperty.GetValue(valueTask);

        //    var asTaskMethod = valueTaskType.GetMethod("AsTask");
        //    if (asTaskMethod != null)
        //    {
        //        var task = (Task)asTaskMethod.Invoke(valueTask, null);
        //        await task.ConfigureAwait(false);
        //        var taskResultProperty = task.GetType().GetProperty("Result");
        //        return taskResultProperty?.GetValue(task);
        //    }
        //    return null;
        //}
        //private static void CopyProperties(object source, object target, params string[] exclude)
        //{
        //    var srcProps = source.GetType().GetProperties();
        //    var tgtProps = target.GetType().GetProperties();
        //    foreach (var prop in srcProps)
        //    {
        //        if (exclude.Contains(prop.Name)) continue;
        //        var targetProp = tgtProps.FirstOrDefault(p => p.Name == prop.Name && p.CanWrite);
        //        if (targetProp != null)
        //            targetProp.SetValue(target, prop.GetValue(source));
        //    }
        //}
        //#endregion

        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.Set<ApprovalTask>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.ApprovalTasks.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt xóa.");
            }
            //if (model != null && model.TenChiNhanh.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên thông tin đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckDelete(ApprovalTask input)
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
                    throw new Exception($"Không tìm thấy thông tin đã chọn");
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
