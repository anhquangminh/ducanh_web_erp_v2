using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.HeThong;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class ApprovalDeptSettingRepository : IApprovalDeptSettingRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public ApprovalDeptSettingRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public ApprovalDeptSettingModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.ApprovalDeptSettings
                         join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                         join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                         join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                         join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                         where p1.Id == id && p1.IsActive != 100
                         select new ApprovalDeptSettingModel
                         {
                             Id = p1.Id,
                             IdMain = p1.IdMain,
                             CompanyId = ChiNhanhs1.TenChiNhanh,
                             DeptId = Departments1.DeptName,
                             ParentMajorId = Majors1.MajorName,
                             MajorId = Majors2.MajorName,
                             ApprovalStep = p1.ApprovalStep,
                             GroupId = p1.GroupId,
                             Ordinarily = p1.Ordinarily,
                             CreateAt = (DateTime)p1.CreateAt,
                             CreateBy = p1.CreateBy,
                             IsActive = p1.IsActive,
                             ApprovalUserId = p1.ApprovalUserId,
                             DateApproval = (DateTime)p1.DateApproval,
                             DepartmentId = p1.DepartmentId,
                             DepartmentOrder = p1.DepartmentOrder,
                             ApprovalOrder = p1.ApprovalOrder,
                             ApprovalId = p1.ApprovalId,
                             LastApprovalId = p1.LastApprovalId,
                             IsStatus = p1.IsStatus
                         }).FirstOrDefault();
            return query;
        }

        public async Task<List<ApprovalDeptSettingModel>> GetHistoryIsValidEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalDeptSetting_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              where p1.IdChung == id && p1.IsValid == true
                              orderby p1.CreateAt
                              select new ApprovalDeptSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<ApprovalDeptSettingModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalDeptSettings
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.Id == id
                              select new ApprovalDeptSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<ApprovalDeptSettingModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ApprovalDeptSetting_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join Majors1 in context.Majors on p1.ParentMajorId equals Majors1.Id
                              join Majors2 in context.Majors on p1.MajorId equals Majors2.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new ApprovalDeptSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = Departments1.DeptName,
                                  ParentMajorId = Majors1.MajorName,
                                  MajorId = Majors2.MajorName,
                                  ApprovalStep = p1.ApprovalStep,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = approvalUserId.Email,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  DepartmentId = departmentId.DeptName,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<List<ChiNhanhModel>>? GetChiNhanhsForCompanyId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
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
                var entity = await (from p1 in context.ApprovalDeptSettings
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
                var entity = await (from p1 in context.ApprovalDeptSettings
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
                var entity = await (from p1 in context.ApprovalDeptSettings
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
        public async Task Delete(ApprovalDeptSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
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
                    var logdata = (from p in context.ApprovalDeptSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.IdMain = logdata.IdMain;
                        data.CompanyId = logdata.CompanyId;
                        data.DeptId = logdata.DeptId;
                        data.ParentMajorId = logdata.ParentMajorId;
                        data.MajorId = logdata.MajorId;
                        data.ApprovalStep = logdata.ApprovalStep;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;
                        logdata.IsValid = true;
                        context.ApprovalDeptSetting_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.ApprovalDeptSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalDeptSetting_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalDeptSetting_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdMain = data.IdMain,
                        CompanyId = data.CompanyId,
                        DeptId = data.DeptId,
                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        ApprovalStep = data.ApprovalStep,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
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
                    context.ApprovalDeptSetting_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.ApprovalDeptSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalDeptSetting_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ApprovalDeptSetting_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdMain = data.IdMain,
                        CompanyId = data.CompanyId,
                        DeptId = data.DeptId,
                        ParentMajorId = data.ParentMajorId,
                        MajorId = data.MajorId,
                        ApprovalStep = data.ApprovalStep,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
                        DepartmentId = data.DepartmentId,
                        DepartmentOrder = data.DepartmentOrder,
                        ApprovalOrder = data.ApprovalOrder,
                        ApprovalId = data.ApprovalId,
                        LastApprovalId = data.LastApprovalId,
                        IsStatus = data.IsStatus,
                        IdChung = data.Id,
                        IsValid = true
                    };
                    context.ApprovalDeptSetting_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.ApprovalDeptSettings.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(ApprovalDeptSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
            }
            context.ApprovalDeptSettings.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ApprovalDeptSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalDeptSetting_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ApprovalDeptSetting_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ApprovalDeptSetting_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ApprovalDeptSetting_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ApprovalDeptSetting_Logs.Update(updateLog);
                }
            }
            var addLog = new ApprovalDeptSetting_Log()
            {
                Id = Guid.NewGuid().ToString(),
                IdMain = data.IdMain,
                CompanyId = data.CompanyId,
                DeptId = data.DeptId,
                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                ApprovalStep = data.ApprovalStep,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = data.ApprovalUserId,
                DateApproval = (DateTime?)data.DateApproval,
                DepartmentId = data.DepartmentId,
                DepartmentOrder = data.DepartmentOrder,
                ApprovalOrder = data.ApprovalOrder,
                ApprovalId = data.ApprovalId,
                LastApprovalId = data.LastApprovalId,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.ApprovalDeptSetting_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(ApprovalDeptSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
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
                    var logdata = (from p in context.ApprovalDeptSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.IdMain = logdata.IdMain;
                        entity.CompanyId = logdata.CompanyId;
                        entity.DeptId = logdata.DeptId;
                        entity.ParentMajorId = logdata.ParentMajorId;
                        entity.MajorId = logdata.MajorId;
                        entity.ApprovalStep = logdata.ApprovalStep;
                        entity.GroupId = logdata.GroupId;
                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = (DateTime?)logdata.CreateAt;
                        entity.CreateBy = logdata.CreateBy;
                        entity.IsActive = logdata.IsActive;
                        entity.ApprovalUserId = logdata.ApprovalUserId;
                        entity.DateApproval = (DateTime?)logdata.DateApproval;
                        entity.DepartmentId = logdata.DepartmentId;
                        entity.DepartmentOrder = logdata.DepartmentOrder;
                        entity.ApprovalOrder = logdata.ApprovalOrder;
                        entity.ApprovalId = logdata.ApprovalId;
                        entity.LastApprovalId = logdata.LastApprovalId;
                        entity.IsStatus = logdata.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                    var logupdate = (from p in context.ApprovalDeptSetting_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ApprovalDeptSetting_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.ApprovalDeptSetting_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.IdMain = data.IdMain;
                        entity.CompanyId = data.CompanyId;
                        entity.DeptId = data.DeptId;
                        entity.ParentMajorId = data.ParentMajorId;
                        entity.MajorId = data.MajorId;
                        entity.ApprovalStep = data.ApprovalStep;
                        entity.GroupId = data.GroupId;
                        entity.Ordinarily = data.Ordinarily;
                        entity.CreateAt = (DateTime?)data.CreateAt;
                        entity.CreateBy = data.CreateBy;
                        entity.IsActive = data.IsActive;
                        entity.ApprovalUserId = data.ApprovalUserId;
                        entity.DateApproval = (DateTime?)data.DateApproval;
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
            var addLog = new ApprovalDeptSetting_Log()
            {
                Id = Guid.NewGuid().ToString(),
                IdMain = data.IdMain,
                CompanyId = data.CompanyId,
                DeptId = data.DeptId,
                ParentMajorId = data.ParentMajorId,
                MajorId = data.MajorId,
                ApprovalStep = data.ApprovalStep,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = userId,
                DateApproval = DateTime.Now,
                DepartmentId = null,
                DepartmentOrder = 0,
                ApprovalOrder = 0,
                ApprovalId = null,
                LastApprovalId = null,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = false
            };
            context.ApprovalDeptSetting_Logs.Add(addLog);
            context.ApprovalDeptSettings.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
            }
            context.Set<ApprovalDeptSetting>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckEdit(ApprovalDeptSetting input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.ApprovalDeptSetting_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.CompanyId == input.CompanyId
                                   && p.DeptId == input.DeptId
                                   && p.ParentMajorId == input.ParentMajorId
                                   && p.MajorId == input.MajorId
                                   //&& p.SoApprovalDeptSetting == input.SoApprovalDeptSetting
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
        public async Task<List<ChiNhanh>> CheckChoDuyet(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                SqlParameter param1 = new SqlParameter("@id", id);
                string sql = "EXEC dbo.proc_ApprovalDeptSettings_CheckDuyet @id";
                var entity = await context.ChiNhanhs.FromSqlRaw<ChiNhanh>(sql, param1).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ApprovalDeptSettingModel>> GetAllByVM(ApprovalDeptSettingModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.ApprovalDeptSettings
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
            var data = await (from p1 in query
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join MMajors1 in context.Majors on p1.ParentMajorId equals MMajors1.Id
                              join MMajors2 in context.Majors on p1.MajorId equals MMajors2.Id
                              where p1.GroupId == groupId
                              select new ApprovalDeptSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = p1.ApprovalStep + ". " + Departments1.DeptName,
                                  ParentMajorId = MMajors1.MajorName,
                                  MajorId = MMajors2.MajorName,
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

            var result = data.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId }).Select(g => new
            {
                g.Key.IdMain,
                g.Key.CompanyId,
                g.Key.ParentMajorId,
                g.Key.MajorId,
                DeptId = string.Join(" => ", g.Select(i => i.DeptId))
            });

            var vlus = new List<ApprovalDeptSettingModel>();
            foreach (var item in result)
            {
                var additem = new ApprovalDeptSettingModel()
                {
                    IdMain = item.IdMain,
                    CompanyId = item.CompanyId,
                    DeptId = item.DeptId,
                    ParentMajorId = item.ParentMajorId,
                    MajorId = item.MajorId
                };
                vlus.Add(additem);
            }
            return vlus;
        }
        public async Task<List<ApprovalDeptSetting>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalDeptSettings.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ApprovalDeptSetting>> GetByMainId(string MainId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApprovalDeptSettings.Where(p => p.IsActive != 100 && p.IdMain == MainId).OrderBy(p => p.ApprovalStep).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public List<Department> ListDept(string companyId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = (from p in context.ApprovalDeptSettings
                              join q in context.Departments on p.DeptId equals q.Id
                              where p.CompanyId == companyId && p.IsActive != 100 && q.IsActive != 100
                              orderby p.ApprovalStep
                              select q).Distinct().ToList();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ChiNhanhModel>> GetChiNhanhs(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.ChiNhanhs on p1.CompanyId equals p2.Id
                                    where p1.GroupId == groupId
                                    orderby p1.CreateAt descending
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
        public async Task<List<DepartmentModel>> GetDepartments(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Departments on p1.DeptId equals p2.Id
                                    where p1.GroupId == groupId
                                    orderby p1.CreateAt descending
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
        public async Task<List<MajorModel>> GetMajors(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Majors on p1.ParentMajorId equals p2.Id
                                    where p1.GroupId == groupId
                                    orderby p1.CreateAt descending
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
        public async Task<List<MajorModel>> GetMajorsByParentId(string parentId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApprovalDeptSettings
                                    join p2 in context.Majors on p1.MajorId equals p2.Id
                                    where p1.ParentMajorId == parentId
                                    orderby p1.CreateAt descending
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
        public async Task<ApprovalDeptSetting> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ApprovalDeptSettings.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn.");
            }
            return entity;
        }
        public async Task Insert(ApprovalDeptSetting entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có cài đặt duyệt phòng ban nào được thêm!");
                }
                context.ApprovalDeptSettings.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(ApprovalDeptSetting data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
            }
            context.ApprovalDeptSettings.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(ApprovalDeptSetting[] ApprovalDeptSettings)
        {
            using var context = _context.CreateDbContext();
            string[] ids = ApprovalDeptSettings.Select(x => x.Id).ToArray();
            var listEntities = await context.ApprovalDeptSettings.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.ApprovalDeptSettings.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.ApprovalDeptSettings.Where(p => p.Id == ids).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt xóa.");
            }
            return true;
        }
        public async Task<bool> CheckSave(string companyId, string majorId, string mainId, bool loai)
        {
            using var context = _context.CreateDbContext();
            if (loai)
            {
                var query = await (from p in context.ApprovalDeptSettings
                                   where p.CompanyId.Equals(companyId)
                                   && p.MajorId.Equals(majorId)
                                   && p.IdMain != mainId
                                   && p.IsActive != 100
                                   select p).CountAsync();
                if (query > 0)
                {
                    throw new Exception($"Nghiệp vụ này đã được cài đặt duyệt phòng ban theo chi nhánh đã chọn.");
                }
                var checkStep = await (from p in context.ApprovalStepSettings
                                       where p.CompanyId.Equals(companyId)
                                       && p.MajorId.Equals(majorId)
                                       && p.IsActive != 100
                                       select p).CountAsync();
                if (checkStep > 0)
                {
                    throw new Exception($"Nghiệp vụ này đã được cài đặt lượt duyệt phòng ban theo chi nhánh đã chọn.");
                }
            }
            else
            {
                var query = await (from p in context.ApprovalDeptSettings
                                   where p.CompanyId.Equals(companyId)
                                   && p.MajorId.Equals(majorId)
                                   && p.IsActive != 100
                                   select p).CountAsync();
                if (query > 0)
                {
                    throw new Exception($"Nghiệp vụ này đã được cài đặt duyệt phòng ban theo chi nhánh đã chọn.");
                }
            }
            return true;
        }
        public async Task<bool> CheckDelete(string companyId, string majorId)
        {
            using var context = _context.CreateDbContext();
            var query = await (from p in context.ApprovalDeptSettings
                               where p.CompanyId.Equals(companyId)
                               && p.MajorId.Equals(majorId)
                               && p.IsActive != 100
                               select p).CountAsync();
            if (query > 0)
            {
                throw new Exception($"Nghiệp vụ này đã được cài đặt duyệt phòng ban theo chi nhánh đã chọn.");
            }
            return true;
        }
        //public async Task<bool> CheckDelete(string ids, string name)
        //{
        //    using var context = _context.CreateDbContext();
        //    var model = await context.ApprovalDeptSettings.Where(p => p.Id == ids).FirstOrDefaultAsync();
        //    if (model == null)
        //    {
        //        throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
        //    }
        //    if (model != null && model.IsActive == 0)
        //    {
        //        throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt thêm mới.");
        //    }
        //    if (model != null && model.IsActive == 1)
        //    {
        //        throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt sửa.");
        //    }
        //    if (model != null && model.IsActive == 2)
        //    {
        //        throw new Exception($"Cài đặt duyệt phòng ban đang chờ duyệt xóa.");
        //    }
        //    return true;
        //}

        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy cài đặt duyệt phòng ban đã chọn");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }
        public async Task<bool> CreateApprovalDeptSetting(List<ApprovalDeptWrapper> approvalRowWrappers, DateTime baseTime)
        {
            if (approvalRowWrappers.Count == 0)
            {
                return false;
            }
            using var context = _context.CreateDbContext();
            var companyId = approvalRowWrappers.First().ApprovalRow.CompanyId;
            var parentId = approvalRowWrappers.First().ApprovalRow.ParentMajorId;
            var majorId = approvalRowWrappers.First().ApprovalRow.MajorId;
            var mainId = approvalRowWrappers.First().ApprovalRow.IdMain;

            // Xóa cài đặt cũ
            var listToDelete = await GetByMainId(mainId);
            listToDelete.ForEach(item => item.IsActive = 100);
            context.ApprovalDeptSettings.UpdateRange(listToDelete);
            await context.SaveChangesAsync();
            //var listId = listToDelete.Select(x => x.Id).ToArray();
            //await CheckExclusive(listToDelete.ToArray(), baseTime);

            //await DeleteById(item.Id, "");

            // Đăng ký cài đặt mới
            foreach (var row in approvalRowWrappers)
            {
                await Insert(row.ApprovalRow);
            }

            return true;
        }
        public async Task<List<ApprovalDeptSettingModel>> GetSetApprovalDept(string companyId, string parentId, string majorId)
        {
            using var context = _context.CreateDbContext();
            var query = from settings in context.ApprovalDeptSettings
                        join company in context.ChiNhanhs on settings.CompanyId equals company.Id into companyGroup
                        from company in companyGroup.DefaultIfEmpty()
                        where company.IsActive == 1
                        join major in context.Majors on settings.MajorId equals major.Id into majorGroup
                        from major in majorGroup.DefaultIfEmpty()
                        where major.IsActive == 1
                        join parent in context.Majors on settings.ParentMajorId equals parent.Id into parentGroup
                        from parent in parentGroup.DefaultIfEmpty()
                        where parent.IsActive == 1
                        join dept in context.Departments on settings.DeptId equals dept.Id into deptGroup
                        from dept in deptGroup.DefaultIfEmpty()
                        where dept.IsActive == 1 && settings.IsActive == 1
                        select new ApprovalDeptSettingModel
                        {
                            Id = settings.Id,
                            CompanyId = settings.CompanyId,
                            DeptId = settings.DeptId,
                            MajorId = settings.MajorId,
                            ParentMajorId = settings.ParentMajorId,
                            ApprovalStep = settings.ApprovalStep,
                            CreateAt = settings.CreateAt,
                            CreateBy = settings.CreateBy,
                            CompanyName = company.TenChiNhanh,
                            MajorName = major.MajorName,
                            ParentName = parent.MajorName,
                            DeptName = dept.DeptName
                        };
            if (companyId != "")
            {
                query = query.Where(p => p.CompanyId.Equals(companyId));
            }

            if (parentId != "")
            {
                query = query.Where(p => p.ParentMajorId.Equals(parentId));
            }

            if (majorId != "")
            {
                query = query.Where(p => p.MajorId.Equals(majorId));
            }

            var listSetting = await query.OrderBy(x => x.ApprovalStep).ToListAsync();
            return listSetting;
        }
        public async Task Insert(ApprovalDeptSetting entity)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.ApprovalDeptSettings.Add(entity);
            await context.SaveChangesAsync();
        }
        public async Task<List<ApprovalDeptSettingData>> GetData(string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.ApprovalDeptSettings
                        where p1.GroupId == groupId && p1.IsActive != 100
                        orderby p1.ApprovalStep
                        select p1;
            //if (!string.IsNullOrEmpty(dataModel.CompanyId))
            //{
            //    query = query.Where(m => m.CompanyId == dataModel.CompanyId);
            //}
            //if (!string.IsNullOrEmpty(dataModel.DeptId))
            //{
            //    query = query.Where(m => m.DeptId == dataModel.DeptId);
            //}
            //if (!string.IsNullOrEmpty(dataModel.ParentMajorId))
            //{
            //    query = query.Where(m => m.ParentMajorId == dataModel.ParentMajorId);
            //}
            //if (!string.IsNullOrEmpty(dataModel.MajorId))
            //{
            //    query = query.Where(m => m.MajorId == dataModel.MajorId);
            //}
            var data = await (from p1 in query
                              join ChiNhanhs1 in context.ChiNhanhs on p1.CompanyId equals ChiNhanhs1.Id
                              join Departments1 in context.Departments on p1.DeptId equals Departments1.Id
                              join MMajors1 in context.Majors on p1.ParentMajorId equals MMajors1.Id
                              join MMajors2 in context.Majors on p1.MajorId equals MMajors2.Id
                              where p1.GroupId == groupId
                              select new ApprovalDeptSettingModel
                              {
                                  Id = p1.Id,
                                  IdMain = p1.IdMain,
                                  CompanyId = ChiNhanhs1.TenChiNhanh,
                                  DeptId = p1.ApprovalStep + ". " + Departments1.DeptName,
                                  ParentMajorId = MMajors1.MajorName,
                                  MajorId = MMajors2.MajorName,
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

            var result = data.GroupBy(p => new { p.IdMain, p.CompanyId, p.ParentMajorId, p.MajorId }).Select(g => new
            {
                g.Key.IdMain,
                g.Key.CompanyId,
                g.Key.ParentMajorId,
                g.Key.MajorId,
                DeptId = string.Join(" => ", g.Select(i => i.DeptId))
            });

            var vlus = new List<ApprovalDeptSettingData>();
            foreach (var item in result)
            {
                var additem = new ApprovalDeptSettingData()
                {
                    IdMain = item.IdMain,
                    CompanyId = item.CompanyId,
                    DeptId = item.DeptId,
                    ParentMajorId = item.ParentMajorId,
                    MajorId = item.MajorId
                };
                vlus.Add(additem);
            }
            return vlus;
        }
        public async Task<bool> DeleteApprovalDeptSetting(string mainId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var listToDelete = await GetByMainId(mainId);
                listToDelete.ForEach(item => item.IsActive = 100);
                context.ApprovalDeptSettings.UpdateRange(listToDelete);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }

}
