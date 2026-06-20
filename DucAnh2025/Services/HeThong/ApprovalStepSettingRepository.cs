using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class ApprovalStepSettingRepository : IApprovalStepSettingRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public ApprovalStepSettingRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public ApprovalStepSettingModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.ApprovalStepSettings
                         join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                         join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                         join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                         join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                         join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                         where p1.Id == id && p1.IsActive != 100
                         select new ApprovalStepSettingModel
                         {
                             Id = p1.Id,
                             IdMain = p1.IdMain,
                             CompanyId = ChiNhanhs1.TenChiNhanh,
                             DeptId = Departments1.DeptName,
                             DeptOrder = p1.DeptOrder,
                             ParentMajorId = Majors1.MajorName,
                             MajorId = Majors2.MajorName,
                             PermissionId = Permissions1.PermissionName,
                             Content = p1.Content,
                             ApprovalStep = p1.ApprovalStep,
                             GroupId = p1.GroupId,
                             Ordinarily = p1.Ordinarily,
                             CreateAt = (DateTime)p1.CreateAt,
                             CreateBy = p1.CreateBy,
                             IsActive = p1.IsActive,
                             ApprovalUserId = p1.ApprovalUserId,
                             DateApproval = (DateTime)p1.DateApproval,
                             ApprovalDept = p1.ApprovalDept,
                             DepartmentId = p1.DepartmentId,
                             DepartmentOrder = p1.DepartmentOrder,
                             ApprovalOrder = p1.ApprovalOrder,
                             ApprovalId = p1.ApprovalId,
                             LastApprovalId = p1.LastApprovalId,
                             IsStatus = p1.IsStatus
                         }).FirstOrDefault();
            return query;
        }
        public async Task<List<ApprovalStepSettingModel>> GetAllByVM(ApprovalStepSettingModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.ApprovalStepSettings
                        where p1.GroupId == groupId && p1.IsActive != 100
                        select p1;
            if (!string.IsNullOrEmpty(dataModel.CompanyId))
            {
                query = query.Where(m => m.CompanyId == dataModel.CompanyId);
            }
            if (!string.IsNullOrEmpty(dataModel.DeptId))
            {
                query = query.Where(m => m.DeptId == dataModel.DeptId);
            }
            if (!string.IsNullOrEmpty(dataModel.ParentMajorId))
            {
                query = query.Where(m => m.ParentMajorId == dataModel.ParentMajorId);
            }
            if (!string.IsNullOrEmpty(dataModel.MajorId))
            {
                query = query.Where(m => m.MajorId == dataModel.MajorId);
            }
            if (!string.IsNullOrEmpty(dataModel.PermissionId))
            {
                query = query.Where(m => m.PermissionId == dataModel.PermissionId);
            }
            var data = await (from p1 in query
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join MMajors1 in context.Majors on p1.ParentMajorId equals MMajors1.Id
                              join MMajors2 in context.Majors on p1.MajorId equals MMajors2.Id
                              join MPermissions1 in context.Permissions on p1.PermissionId equals MPermissions1.Id
                              where p1.GroupId == groupId
                              select new ApprovalStepSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  DeptOrder = p1.DeptOrder,
                                  ParentMajorId = MMajors1.MajorName,
                                  MajorId = MMajors2.MajorName,
                                  PermissionId = MPermissions1.PermissionName,
                                  Content = p1.ApprovalStep + ". " + p1.Content,
                                  ApprovalStep = p1.ApprovalStep,
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
                              }).OrderBy(p => p.IdMain).OrderBy(p => p.ApprovalStep).ToListAsync();

            var result = data.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId, p.DeptId, p.DeptOrder, p.PermissionId,p.IsActive }).Select(g => new
            {
                g.Key.IdMain,
                g.Key.CompanyId,
                g.Key.ParentMajorId,
                g.Key.MajorId,
                g.Key.DeptId,
                g.Key.DeptOrder,
                g.Key.PermissionId,
                g.Key.IsActive,
                Content = string.Join(" => ", g.Select(i => i.Content))
            });

            var vlus = new List<ApprovalStepSettingModel>();
            foreach (var item in result)
            {
                var additem = new ApprovalStepSettingModel()
                {
                    IdMain = item.IdMain,
                    CompanyId = item.CompanyId,
                    ParentMajorId = item.ParentMajorId,
                    DeptOrder = item.DeptOrder,
                    MajorId = item.MajorId,
                    DeptId = item.DeptId,
                    PermissionId = item.PermissionId,
                    Content = item.Content,
                    IsActive = item.IsActive,
                };
                vlus.Add(additem);
            }
            return vlus;
        }
        public async Task<List<ApprovalStepSettingModel>> GetHistoryIsValidEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalStepSetting_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              where p1.IdChung == id && p1.IsValid == true
                              orderby p1.CreateAt
                              select new ApprovalStepSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  DeptOrder = p1.DeptOrder,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  PermissionId = Permissions1.PermissionName,
                                  Content = p1.Content,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  ApprovalDept = p1.ApprovalDept,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<ApprovalStepSettingModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalStepSettings
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new ApprovalStepSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  DeptOrder = p1.DeptOrder,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  PermissionId = Permissions1.PermissionName,
                                  Content = p1.Content,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  ApprovalDept = approvalDept.DeptName,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<ApprovalStepSettingModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalStepSetting_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join Permissions1 in context.Permissions on p1.PermissionId equals Permissions1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new ApprovalStepSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  DeptOrder = p1.DeptOrder,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  PermissionId = Permissions1.PermissionName,
                                  Content = p1.Content,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  ApprovalDept = approvalDept.DeptName,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<List<ApprovalStepSetting>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalStepSettings.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
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
                var entity = await (from p1 in context.ApprovalStepSettings
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
        public async Task<List<DepartmentModel>>? GetDepartmentsForDeptId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalStepSettings
                                    join p2 in context.Departments on p1.DeptId equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.DeptName
                                    select new DepartmentModel
                                    {
                                        Id = p2.Id,
                                        DeptName = p2.DeptName
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
                var entity = await (from p1 in context.ApprovalStepSettings
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
                var entity = await (from p1 in context.ApprovalStepSettings
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
        public async Task<List<PermissionModel>>? GetPermissionsForPermissionId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalStepSettings
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
        public async Task<ApprovalStepSetting> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ApprovalStepSettings.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn.");
            }
            return entity;
        }
        public async Task Insert(ApprovalStepSetting entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có cài đặt thứ tự duyệt nào được thêm!");
                }
                context.ApprovalStepSettings.Add(entity);
                var addLog = new ApprovalStepSetting_Log()
                {
                    Id = entity.Id,
                    IdMain = entity.IdMain,
                    CompanyId = entity.CompanyId,
                    DeptId = entity.DeptId,
                    DeptOrder = entity.DeptOrder,
                    ParentMajorId = entity.ParentMajorId,
                    MajorId = entity.MajorId,
                    PermissionId = entity.PermissionId,
                    Content = entity.Content,
                    ApprovalStep = entity.ApprovalStep,
                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = DateTime.Now,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = (DateTime?)entity.DateApproval,
                    ApprovalDept = entity.ApprovalDept,
                    DepartmentId = entity.DepartmentId,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,
                    IsValid = true
                };
                context.ApprovalStepSetting_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(ApprovalStepSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
            }
            context.ApprovalStepSettings.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalStepSetting_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalStepSetting_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ApprovalStepSetting_Logs.Update(updateLog);
                }
            }
            var addLog = new ApprovalStepSetting_Log()
            {
                Id = Guid.NewGuid().ToString(),
                IdMain = data.IdMain,
                CompanyId = data.CompanyId,
                DeptId = data.DeptId,
                DeptOrder = data.DeptOrder,
                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                PermissionId = data.PermissionId,
                Content = data.Content,
                ApprovalStep = data.ApprovalStep,
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
            context.ApprovalStepSetting_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(ApprovalStepSetting[] ApprovalStepSettings)
        {
            using var context = _context.CreateDbContext();
            string[] ids = ApprovalStepSettings.Select(x => x.Id).ToArray();
            var listEntities = await context.ApprovalStepSettings.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.ApprovalStepSettings.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(ApprovalStepSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
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
                    var logdata = (from p in context.ApprovalStepSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.IdMain = logdata.IdMain;
                        data.CompanyId = logdata.CompanyId;
                        data.DeptId = logdata.DeptId;
                        data.DeptOrder = logdata.DeptOrder;
                        data.ParentMajorId = logdata.ParentMajorId;
                        data.MajorId = logdata.MajorId;
                        data.PermissionId = logdata.PermissionId;
                        data.Content = logdata.Content;
                        data.ApprovalStep = logdata.ApprovalStep;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;
                        logdata.IsValid = true;
                        context.ApprovalStepSetting_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.ApprovalStepSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalStepSetting_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalStepSetting_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdMain = data.IdMain,
                        CompanyId = data.CompanyId,
                        DeptId = data.DeptId,
                        DeptOrder = data.DeptOrder,
                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        PermissionId = data.PermissionId,
                        Content = data.Content,
                        ApprovalStep = data.ApprovalStep,
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
                    context.ApprovalStepSetting_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.ApprovalStepSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalStepSetting_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalStepSetting_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdMain = data.IdMain,
                        CompanyId = data.CompanyId,
                        DeptId = data.DeptId,
                        DeptOrder = data.DeptOrder,
                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        PermissionId = data.PermissionId,
                        Content = data.Content,
                        ApprovalStep = data.ApprovalStep,
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
                    context.ApprovalStepSetting_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.ApprovalStepSettings.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(ApprovalStepSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
            }
            context.ApprovalStepSettings.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalStepSetting_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalStepSetting_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ApprovalStepSetting_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ApprovalStepSetting_Logs.Update(updateLog);
                }
            }
            var addLog = new ApprovalStepSetting_Log()
            {
                Id = Guid.NewGuid().ToString(),
                IdMain = data.IdMain,
                CompanyId = data.CompanyId,
                DeptId = data.DeptId,
                DeptOrder = data.DeptOrder,
                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                PermissionId = data.PermissionId,
                Content = data.Content,
                ApprovalStep = data.ApprovalStep,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = data.ApprovalUserId,
                DateApproval = (DateTime?)data.DateApproval,
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
            context.ApprovalStepSetting_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(ApprovalStepSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    entity.IsActive = 90;
                    entity.IsStatus = "Không duyệt";
                    entity.ApprovalUserId = data.ApprovalUserId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.ApprovalStepSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.IdMain = logdata.IdMain;
                        entity.CompanyId = logdata.CompanyId;
                        entity.DeptId = logdata.DeptId;
                        entity.DeptOrder = logdata.DeptOrder;
                        entity.ParentMajorId = logdata.ParentMajorId;
                        entity.MajorId = logdata.MajorId;
                        entity.PermissionId = logdata.PermissionId;
                        entity.Content = logdata.Content;
                        entity.ApprovalStep = logdata.ApprovalStep;
                        entity.GroupId = logdata.GroupId;
                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = (DateTime?)logdata.CreateAt;
                        entity.CreateBy = logdata.CreateBy;
                        entity.IsActive = logdata.IsActive;
                        entity.ApprovalUserId = logdata.ApprovalUserId;
                        entity.DateApproval = (DateTime?)logdata.DateApproval;
                        entity.ApprovalDept = logdata.ApprovalDept;
                        entity.DepartmentId = logdata.DepartmentId;
                        entity.DepartmentOrder = logdata.DepartmentOrder;
                        entity.ApprovalOrder = logdata.ApprovalOrder;
                        entity.ApprovalId = logdata.ApprovalId;
                        entity.LastApprovalId = logdata.LastApprovalId;
                        entity.IsStatus = logdata.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                    var logupdate = (from p in context.ApprovalStepSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalStepSetting_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.ApprovalStepSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.IdMain = data.IdMain;
                        entity.CompanyId = data.CompanyId;
                        entity.DeptId = data.DeptId;
                        entity.DeptOrder = data.DeptOrder;
                        entity.ParentMajorId = data.ParentMajorId;
                        entity.MajorId = data.MajorId;
                        entity.PermissionId = data.PermissionId;
                        entity.Content = data.Content;
                        entity.ApprovalStep = data.ApprovalStep;
                        entity.GroupId = data.GroupId;
                        entity.Ordinarily = data.Ordinarily;
                        entity.CreateAt = (DateTime?)data.CreateAt;
                        entity.CreateBy = data.CreateBy;
                        entity.IsActive = data.IsActive;
                        entity.ApprovalUserId = data.ApprovalUserId;
                        entity.DateApproval = (DateTime?)data.DateApproval;
                        entity.ApprovalDept = data.ApprovalDept;
                        entity.DepartmentId = data.DepartmentId;
                        entity.DepartmentOrder = data.DepartmentOrder;
                        entity.ApprovalOrder = data.ApprovalOrder;
                        entity.ApprovalId = data.ApprovalId;
                        entity.LastApprovalId = data.LastApprovalId;
                        entity.IsStatus = data.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại.");
                }
            }
            var addLog = new ApprovalStepSetting_Log()
            {
                Id = Guid.NewGuid().ToString(),
                IdMain = data.IdMain,
                CompanyId = data.CompanyId,
                DeptId = data.DeptId,
                DeptOrder = data.DeptOrder,
                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                PermissionId = data.PermissionId,
                Content = data.Content,
                ApprovalStep = data.ApprovalStep,
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
            context.ApprovalStepSetting_Logs.Add(addLog);
            context.ApprovalStepSettings.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
            }
            context.Set<ApprovalStepSetting>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.ApprovalStepSettings.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Cài đặt thứ tự duyệt đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Cài đặt thứ tự duyệt đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Cài đặt thứ tự duyệt đang chờ duyệt xóa.");
            }
            //if (model != null && model.TenApprovalStepSetting.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên cài đặt thứ tự duyệt đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(ApprovalStepSetting input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.ApprovalStepSetting_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.CompanyId == input.CompanyId
                                   && p.DeptId == input.DeptId
                                   && p.ParentMajorId == input.ParentMajorId
                                   && p.MajorId == input.MajorId
                                   && p.PermissionId == input.PermissionId
                                   //&& p.SoApprovalStepSetting == input.SoApprovalStepSetting
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
        public async Task<bool> CheckEdit(ApprovalStepSetting input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.ApprovalStepSetting_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.CompanyId == input.CompanyId
                                   && p.DeptId == input.DeptId
                                   && p.ParentMajorId == input.ParentMajorId
                                   && p.MajorId == input.MajorId
                                   && p.PermissionId == input.PermissionId
                                   //&& p.SoApprovalStepSetting == input.SoApprovalStepSetting
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
        public async Task<bool> CheckDelete(ApprovalStepSetting input)
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
                    throw new Exception($"Không tìm thấy cài đặt thứ tự duyệt đã chọn");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }

        public List<ApprovalStepSettingModel> ListStepSetting(string groupId, string majorId, string departmentId, string permissionId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = (from p in context.ApprovalStepSettings
                              join q in context.Permissions on p.PermissionId equals q.Id
                              where p.GroupId == groupId
                                  && p.DeptId == departmentId
                                  && p.MajorId == majorId
                                  && p.PermissionId == permissionId
                                  && p.IsActive != 100
                                  && q.IsActive != 100
                              select new ApprovalStepSettingModel
                              {
                                  Id = p.Id,
                                  IdMain = p.IdMain,
                                  CompanyId = p.CompanyId,
                                  DeptId = p.DeptId,
                                  DeptOrder = p.DeptOrder,
                                  ParentMajorId = p.ParentMajorId,
                                  MajorId = p.MajorId,
                                  PermissionId = p.PermissionId,
                                  Content = p.Content,
                                  ApprovalStep = p.ApprovalStep,
                                  GroupId = p.GroupId,
                                  Ordinarily = p.Ordinarily,
                                  CreateAt = p.CreateAt,
                                  CreateBy = p.CreateBy,
                                  IsActive = q.PermissionType,// Thứ tự quyền
                                  ApprovalUserId = p.ApprovalUserId,
                                  DateApproval = p.DateApproval,
                                  DepartmentId = p.DepartmentId,
                                  DepartmentOrder = p.DepartmentOrder,
                                  ApprovalOrder = p.ApprovalOrder,
                                  ApprovalId = p.ApprovalId,
                                  LastApprovalId = p.LastApprovalId,
                                  IsStatus = p.IsStatus
                              }).Distinct().OrderBy(p => p.ApprovalStep).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Permission>> LoadPermissionsByMajorUserApproval(MajorUserApproval input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalStepSettings
                                    join p2 in context.Permissions on p1.PermissionId equals p2.Id
                                    where p1.CompanyId == input.CompanyId && p1.ParentMajorId == input.ParentMajorId && p1.MajorId == input.MajorId && p1.DeptId == input.DeptId
                                    && p1.IsActive != 100
                                    orderby p2.PermissionName descending
                                    select new Permission
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
        public async Task<List<Major>> LoadParentMajors(string companyId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Majors on p1.ParentMajorId equals p2.Id
                                    where p1.CompanyId == companyId && p1.IsActive != 100
                                    orderby p2.Order descending
                                    select new Major
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
        public async Task<List<Major>> LoadMajors(string companyId, string parentMajorId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Majors on p1.MajorId equals p2.Id
                                    where p1.ParentMajorId == parentMajorId && p1.CompanyId == companyId && p1.IsActive != 100
                                    orderby p2.Order descending
                                    select new Major
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
        public async Task<List<Permission>> LoadPermissionsByApprovalControl(ApprovalControl input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalStepSettings
                                    join p2 in context.Permissions on p1.PermissionId equals p2.Id
                                    where p1.CompanyId == input.CompanyId && p1.ParentMajorId == input.ParentMajorId && p1.MajorId == input.MajorId && p1.DeptId == input.DeptId
                                    && p1.IsActive != 100
                                    orderby p2.PermissionName descending
                                    select new Permission
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
        public async Task<List<ApprovalStepSetting>> LoadStepByApprovalControl(ApprovalControl input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalStepSettings.Where(p => p.IsActive != 100
                && p.CompanyId == input.CompanyId
                && p.ParentMajorId == input.ParentMajorId
                && p.MajorId == input.MajorId
                && p.DeptId == input.DeptId
                && p.PermissionId == input.PermissionId
                ).OrderBy(p => p.ApprovalStep).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ApprovalStepSetting>> LoadStepByMajorUserApproval(MajorUserApproval input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalStepSettings.Where(p => p.IsActive != 100
                && p.CompanyId == input.CompanyId
                && p.ParentMajorId == input.ParentMajorId
                && p.MajorId == input.MajorId
                && p.DeptId == input.DeptId
                && p.PermissionId == input.PermissionId
                ).OrderBy(p => p.ApprovalStep).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Department>> LoadDepartments(string companyId, string parentMajorId, string majorId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Departments on p1.DeptId equals p2.Id
                                    where p1.MajorId == majorId && p1.ParentMajorId == parentMajorId && p1.CompanyId == companyId && p1.IsActive != 100
                                    orderby p2.Ordinarily descending
                                    select new Department
                                    {
                                        Id = p2.Id,
                                        DeptName = p2.DeptName
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public ApprovalDeptSetting GetIdApprodeptSetting(string companyId, string majorId, string deptId)
        {
            using var context = _context.CreateDbContext();
            var entity = (from p in context.ApprovalDeptSettings
                          where p.CompanyId.Equals(companyId)
                          && p.MajorId.Equals(majorId)
                          && p.DeptId.Equals(deptId)
                          && p.IsActive != 100
                          select p).FirstOrDefault();
            return entity;
        }
        public async Task<bool> CreateApprovalStepSetting(List<ApprovalStepWrapper> approvalRowWrappers, DateTime baseTime)
        {
            if (approvalRowWrappers.Count == 0)
            {
                return false;
            }
            using var context = _context.CreateDbContext();
            var mainId = approvalRowWrappers.First().ApprovalRow.IdMain;

            // Xóa cài đặt cũ
            var listToDelete = await GetByMainId(mainId);
            listToDelete.ForEach(item => item.IsActive = 100);
            context.ApprovalStepSettings.UpdateRange(listToDelete);
            await context.SaveChangesAsync();

            // Đăng ký cài đặt mới
            var listInsert = new List<ApprovalStaffSetting>();
            foreach (var row in approvalRowWrappers)
            {
                await Insert(row.ApprovalRow);
            }

            return true;
        }
        public async Task<bool> DeleteApprovalStepSetting(string mainId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var listToDelete = await GetByMainId(mainId);
                listToDelete.ForEach(item => item.IsActive = 100);
                context.ApprovalStepSettings.UpdateRange(listToDelete);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ApprovalStepSetting>> GetByMainId(string MainId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalStepSettings.Where(p => p.IsActive != 100 && p.IdMain == MainId).OrderBy(p => p.ApprovalStep).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task Insert(ApprovalStepSetting entity)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.ApprovalStepSettings.Add(entity);
            await context.SaveChangesAsync();
        }
        public async Task<List<ChiNhanh>> CheckChoDuyet(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                SqlParameter param1 = new SqlParameter("@id", id);
                string sql = "EXEC dbo.proc_ApprovalStepSettings_CheckDuyet @id";
                var entity = await context.ChiNhanhs.FromSqlRaw<ChiNhanh>(sql, param1).ToListAsync();
                return entity;

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<bool> CheckSave(string companyId, string majorId, string deptId, string permissionId, string mainId, bool loai)
        {
            using var context = _context.CreateDbContext();
            if (loai)
            {
                var query = await (from p in context.ApprovalStepSettings
                                   where p.CompanyId.Equals(companyId)
                                   && p.MajorId.Equals(majorId)
                                   && p.DeptId.Equals(deptId)
                                   && p.PermissionId.Equals(permissionId)
                                   && p.IdMain != mainId
                                   && p.IsActive != 100
                                   select p).CountAsync();
                if (query > 0)
                {
                    throw new Exception($"Nghiệp vụ này đã được cài đặt duyệt phòng ban theo chi nhánh đã chọn.");
                }
            }
            else
            {
                var query = await (from p in context.ApprovalStepSettings
                                   where p.CompanyId.Equals(companyId)
                                   && p.MajorId.Equals(majorId)
                                   && p.DeptId.Equals(deptId)
                                   && p.PermissionId.Equals(permissionId)
                                   && p.IsActive != 100
                                   select p).CountAsync();
                if (query > 0)
                {
                    throw new Exception($"Nghiệp vụ này đã được cài đặt duyệt phòng ban theo chi nhánh đã chọn.");
                }
            }
            return true;
        }



    }
}
