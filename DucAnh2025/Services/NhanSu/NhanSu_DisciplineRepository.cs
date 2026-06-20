using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.EntityFrameworkCore;
namespace DucAnh2025.Services.NhanSu
{
    public class NhanSu_DisciplineRepository : INhanSu_DisciplineRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public NhanSu_DisciplineRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<NhanSu_DisciplineModel>> GetAllByVM(NhanSu_DisciplineModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.NhanSu_Disciplines
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                if (!string.IsNullOrEmpty(dataModel?.EmployeeId))
                {
                    baseQuery = baseQuery.Where(m => m.EmployeeId == dataModel.EmployeeId);
                }
                if (!string.IsNullOrEmpty(dataModel?.DisciplineTypeId))
                {
                    baseQuery = baseQuery.Where(m => m.DisciplineTypeId == dataModel.DisciplineTypeId);
                }
                if (!string.IsNullOrEmpty(dataModel?.SignerId))
                {
                    baseQuery = baseQuery.Where(m => m.SignerId == dataModel.SignerId);
                }

                var baseData = await baseQuery
                    .OrderByDescending(p => p.CreateAt)
                    .ToListAsync();

                var NhanSu_EmployeeProfilesDict = await context.NhanSu_EmployeeProfiles.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var NhanSu_DM_DisciplineTypesDict = await context.NhanSu_DM_DisciplineTypes.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new NhanSu_DisciplineModel
                {
                    Id = p1.Id,
                    EmployeeId = p1.EmployeeId,
                    MaNhanVien = NhanSu_EmployeeProfilesDict.TryGetValue(p1.EmployeeId, out var tmpNhanSu_EmployeeProfiles) ? tmpNhanSu_EmployeeProfiles.EmployeeId : "",
                    FullName = NhanSu_EmployeeProfilesDict.TryGetValue(p1.EmployeeId, out var tmpNhanSu_EmployeeProfiles1) ? tmpNhanSu_EmployeeProfiles1.FullName : "",
                    DisciplineTypeId = NhanSu_DM_DisciplineTypesDict.TryGetValue(p1.DisciplineTypeId, out var tmpNhanSu_DM_DisciplineTypes) ? tmpNhanSu_DM_DisciplineTypes.DisciplineTypeName : "",
                    DecisionCode = p1.DecisionCode,
                    IncidentName = p1.IncidentName,
                    IncidentDate = p1.IncidentDate,
                    DisciplineDate = p1.DisciplineDate,
                    DurationDays = p1.DurationDays,
                    FinancialImpact = p1.FinancialImpact,
                    ViolationDetail = p1.ViolationDetail,
                    SignerId = NhanSu_EmployeeProfilesDict.TryGetValue(p1.SignerId, out var tmpNhanSu_EmployeeProfiles2) ? tmpNhanSu_EmployeeProfiles2.FullName : "",
                    FilePath = p1.FilePath,
                    GroupId = p1.GroupId,
                    Ordinarily = p1.Ordinarily,
                    CreateAt = p1.CreateAt.HasValue ? (DateTime)p1.CreateAt : DateTime.MinValue,
                    CreateBy = p1.CreateBy,
                    IsActive = p1.IsActive,
                    ApprovalUserId = p1.ApprovalUserId,
                    DateApproval = p1.DateApproval.HasValue ? (DateTime)p1.DateApproval : DateTime.MinValue,
                    ApprovalDept = p1.ApprovalDept,
                    DepartmentId = p1.DepartmentId,
                    DepartmentOrder = p1.DepartmentOrder,
                    ApprovalOrder = p1.ApprovalOrder,
                    ApprovalId = p1.ApprovalId,
                    LastApprovalId = p1.LastApprovalId,
                    IsStatus = p1.IsStatus,
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("L?i khi l?y d? li?u: " + ex.Message, ex);
            }
        }
        public async Task<List<NhanSu_DisciplineModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = (from p1 in context.NhanSu_Discipline_Logs
                             join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.EmployeeId equals NhanSu_EmployeeProfiles1.Id
                             join NhanSu_DM_DisciplineTypes1 in context.NhanSu_DM_DisciplineTypes on p1.DisciplineTypeId equals NhanSu_DM_DisciplineTypes1.Id
                             join NhanSu_EmployeeProfiles2 in context.NhanSu_EmployeeProfiles on p1.SignerId equals NhanSu_EmployeeProfiles2.Id
                             join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                             join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                             where p1.IdChung == id && p1.IsValid == true
                             orderby p1.CreateAt
                             select new NhanSu_DisciplineModel
                             {
                                 Id = p1.Id,
                                 EmployeeId = p1.EmployeeId,
                                 MaNhanVien = NhanSu_EmployeeProfiles1.EmployeeId,
                                 FullName = NhanSu_EmployeeProfiles1.FullName,
                                 DisciplineTypeId = NhanSu_DM_DisciplineTypes1.DisciplineTypeName,
                                 DecisionCode = p1.DecisionCode,
                                 IncidentName = p1.IncidentName,
                                 IncidentDate = p1.IncidentDate,
                                 DisciplineDate = p1.DisciplineDate,
                                 DurationDays = p1.DurationDays,
                                 FinancialImpact = p1.FinancialImpact,
                                 ViolationDetail = p1.ViolationDetail,
                                 SignerId = NhanSu_EmployeeProfiles2.FullName,
                                 FilePath = p1.FilePath,
                                 GroupId = p1.GroupId,
                                 Ordinarily = p1.Ordinarily,
                                 CreateAt = p1.CreateAt ?? DateTime.MinValue,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
                                 ApprovalUserId = p1.ApprovalUserId,
                                 DateApproval = p1.DateApproval,
                                 ApprovalDept = p1.ApprovalDept,
                                 DepartmentId = p1.DepartmentId,
                                 DepartmentOrder = p1.DepartmentOrder,
                                 ApprovalOrder = p1.ApprovalOrder,
                                 ApprovalId = p1.ApprovalId,
                                 LastApprovalId = p1.LastApprovalId,
                                 IsStatus = p1.IsStatus
                             }).ToListAsync();
                return await query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<NhanSu_DisciplineModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.NhanSu_Disciplines
                              join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.EmployeeId equals NhanSu_EmployeeProfiles1.Id into NhanSu_EmployeeProfiles11
                              from NhanSu_EmployeeProfiles1 in NhanSu_EmployeeProfiles11.DefaultIfEmpty()
                              join NhanSu_DM_DisciplineTypes1 in context.NhanSu_DM_DisciplineTypes on p1.DisciplineTypeId equals NhanSu_DM_DisciplineTypes1.Id into NhanSu_DM_DisciplineTypes11
                              from NhanSu_DM_DisciplineTypes1 in NhanSu_DM_DisciplineTypes11.DefaultIfEmpty()
                              join NhanSu_EmployeeProfiles2 in context.NhanSu_EmployeeProfiles on p1.SignerId equals NhanSu_EmployeeProfiles2.Id into NhanSu_EmployeeProfiles21
                              from NhanSu_EmployeeProfiles2 in NhanSu_EmployeeProfiles21.DefaultIfEmpty()
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new NhanSu_DisciplineModel
                              {
                                  Id = p1.Id,
                                  EmployeeId = p1.EmployeeId,
                                  MaNhanVien = NhanSu_EmployeeProfiles1.EmployeeId,
                                  FullName = NhanSu_EmployeeProfiles1.FullName,
                                  DisciplineTypeId = NhanSu_DM_DisciplineTypes1.DisciplineTypeName,
                                  DecisionCode = p1.DecisionCode,
                                  IncidentName = p1.IncidentName,
                                  IncidentDate = p1.IncidentDate,
                                  DisciplineDate = p1.DisciplineDate,
                                  DurationDays = p1.DurationDays,
                                  FinancialImpact = p1.FinancialImpact,
                                  ViolationDetail = p1.ViolationDetail,
                                  SignerId = NhanSu_EmployeeProfiles2.FullName,
                                  FilePath = p1.FilePath,
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
                                  IsStatus = p1.IsStatus,
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<NhanSu_DisciplineModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.NhanSu_Discipline_Logs
                                  join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.EmployeeId equals NhanSu_EmployeeProfiles1.Id into NhanSu_EmployeeProfiles11
                                  from NhanSu_EmployeeProfiles1 in NhanSu_EmployeeProfiles11.DefaultIfEmpty()
                                  join NhanSu_DM_DisciplineTypes1 in context.NhanSu_DM_DisciplineTypes on p1.DisciplineTypeId equals NhanSu_DM_DisciplineTypes1.Id into NhanSu_DM_DisciplineTypes11
                                  from NhanSu_DM_DisciplineTypes1 in NhanSu_DM_DisciplineTypes11.DefaultIfEmpty()
                                  join NhanSu_EmployeeProfiles2 in context.NhanSu_EmployeeProfiles on p1.SignerId equals NhanSu_EmployeeProfiles2.Id into NhanSu_EmployeeProfiles21
                                  from NhanSu_EmployeeProfiles2 in NhanSu_EmployeeProfiles21.DefaultIfEmpty()
                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new NhanSu_DisciplineModel
                                  {
                                      Id = p1.Id,
                                      EmployeeId = p1.EmployeeId,
                                      MaNhanVien = NhanSu_EmployeeProfiles1.EmployeeId,
                                      FullName = NhanSu_EmployeeProfiles1.FullName,
                                      DisciplineTypeId = NhanSu_DM_DisciplineTypes1.DisciplineTypeName,
                                      DecisionCode = p1.DecisionCode,
                                      IncidentName = p1.IncidentName,
                                      IncidentDate = p1.IncidentDate,
                                      DisciplineDate = p1.DisciplineDate,
                                      DurationDays = p1.DurationDays,
                                      FinancialImpact = p1.FinancialImpact,
                                      ViolationDetail = p1.ViolationDetail,
                                      SignerId = NhanSu_EmployeeProfiles2.FullName,
                                      FilePath = p1.FilePath,
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
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<NhanSu_Discipline>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.NhanSu_Disciplines.Where(p => p.IsActive != 100).OrderByDescending(p => p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi khi lấy dữ liệu:  {ex.Message}");
                throw;
            }
        }
        public async Task<NhanSu_Discipline> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.NhanSu_Disciplines.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn : ");
            }
            return entity;
        }
        public async Task Insert(NhanSu_Discipline entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu dưọc thêm!");
                }
                context.NhanSu_Disciplines.Add(entity);
                var addLog = new NhanSu_Discipline_Log()
                {
                    EmployeeId = entity.EmployeeId,
                    DisciplineTypeId = entity.DisciplineTypeId,
                    DecisionCode = entity.DecisionCode,
                    IncidentName = entity.IncidentName,
                    IncidentDate = entity.IncidentDate,
                    DisciplineDate = entity.DisciplineDate,
                    DurationDays = entity.DurationDays,
                    FinancialImpact = entity.FinancialImpact,
                    ViolationDetail = entity.ViolationDetail,
                    SignerId = entity.SignerId,
                    FilePath = entity.FilePath,
                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = entity.CreateAt,
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
                context.NhanSu_Discipline_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(NhanSu_Discipline data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn !");
            }
            context.NhanSu_Disciplines.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_Discipline_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_Discipline_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.NhanSu_Discipline_Logs.Update(updateLog);
                }
            }
            var addLog = new NhanSu_Discipline_Log
            {
                EmployeeId = data.EmployeeId,
                DisciplineTypeId = data.DisciplineTypeId,
                DecisionCode = data.DecisionCode,
                IncidentName = data.IncidentName,
                IncidentDate = data.IncidentDate,
                DisciplineDate = data.DisciplineDate,
                DurationDays = data.DurationDays,
                FinancialImpact = data.FinancialImpact,
                ViolationDetail = data.ViolationDetail,
                SignerId = data.SignerId,
                FilePath = data.FilePath,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
            context.NhanSu_Discipline_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(NhanSu_Discipline[] NhanSu_Disciplines)
        {
            using var context = _context.CreateDbContext();
            string[] ids = NhanSu_Disciplines.Select(x => x.Id).ToArray();
            var listEntities = await context.NhanSu_Disciplines.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.NhanSu_Disciplines.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(NhanSu_Discipline data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Ðã xóa";
                    data.ApprovalUserId = userId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.NhanSu_Discipline_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.EmployeeId = logdata.EmployeeId;
                        data.DisciplineTypeId = logdata.DisciplineTypeId;
                        data.DecisionCode = logdata.DecisionCode;
                        data.IncidentName = logdata.IncidentName;
                        data.IncidentDate = logdata.IncidentDate;
                        data.DisciplineDate = logdata.DisciplineDate;
                        data.DurationDays = logdata.DurationDays;
                        data.FinancialImpact = logdata.FinancialImpact;
                        data.ViolationDetail = logdata.ViolationDetail;
                        data.SignerId = logdata.SignerId;
                        data.FilePath = logdata.FilePath;
                        data.GroupId = logdata.GroupId;
                        data.Ordinarily = logdata.Ordinarily;
                        data.CreateAt = logdata.CreateAt;
                        data.CreateBy = logdata.CreateBy;
                        data.IsActive = logdata.IsActive;
                        data.ApprovalUserId = logdata.ApprovalUserId;
                        data.DateApproval = logdata.DateApproval;
                        data.ApprovalDept = logdata.ApprovalDept;
                        data.DepartmentId = logdata.DepartmentId;
                        data.DepartmentOrder = logdata.DepartmentOrder;
                        data.ApprovalOrder = logdata.ApprovalOrder;
                        data.ApprovalId = logdata.ApprovalId;
                        data.LastApprovalId = logdata.LastApprovalId;
                        data.IsStatus = logdata.IsStatus;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;
                        logdata.IsValid = true;
                        context.NhanSu_Discipline_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.NhanSu_Discipline_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_Discipline_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new NhanSu_Discipline_Log()
                    {
                        EmployeeId = data.EmployeeId,
                        DisciplineTypeId = data.DisciplineTypeId,
                        DecisionCode = data.DecisionCode,
                        IncidentName = data.IncidentName,
                        IncidentDate = data.IncidentDate,
                        DisciplineDate = data.DisciplineDate,
                        DurationDays = data.DurationDays,
                        FinancialImpact = data.FinancialImpact,
                        ViolationDetail = data.ViolationDetail,
                        SignerId = data.SignerId,
                        FilePath = data.FilePath,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = data.CreateAt,
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
                        IsValid = true,
                        IdChung = data.Id
                    };
                    addLog.Ordinarily = logdata != null ? logdata.Ordinarily : addLog.Ordinarily;
                    context.NhanSu_Discipline_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin đang chờ duyệt xóa!");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.NhanSu_Discipline_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_Discipline_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new NhanSu_Discipline_Log()
                    {
                        EmployeeId = data.EmployeeId,
                        DisciplineTypeId = data.DisciplineTypeId,
                        DecisionCode = data.DecisionCode,
                        IncidentName = data.IncidentName,
                        IncidentDate = data.IncidentDate,
                        DisciplineDate = data.DisciplineDate,
                        DurationDays = data.DurationDays,
                        FinancialImpact = data.FinancialImpact,
                        ViolationDetail = data.ViolationDetail,
                        SignerId = data.SignerId,
                        FilePath = data.FilePath,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = data.CreateAt,
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
                        IsValid = true
                    };
                    context.NhanSu_Discipline_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt !";
                }
            }
            context.NhanSu_Disciplines.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(NhanSu_Discipline data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.NhanSu_Disciplines.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_Discipline_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_Discipline_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.NhanSu_Discipline_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.NhanSu_Discipline_Logs.Update(updateLog);
                }
            }
            var addLog = new NhanSu_Discipline_Log()
            {
                EmployeeId = data.EmployeeId,
                DisciplineTypeId = data.DisciplineTypeId,
                DecisionCode = data.DecisionCode,
                IncidentName = data.IncidentName,
                IncidentDate = data.IncidentDate,
                DisciplineDate = data.DisciplineDate,
                DurationDays = data.DurationDays,
                FinancialImpact = data.FinancialImpact,
                ViolationDetail = data.ViolationDetail,
                SignerId = data.SignerId,
                FilePath = data.FilePath,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
            context.NhanSu_Discipline_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(NhanSu_Discipline data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    entity.IsActive = 90;
                    entity.IsStatus = "Không duyệt!";
                    entity.ApprovalUserId = data.ApprovalUserId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.NhanSu_Discipline_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.EmployeeId = logdata.EmployeeId;
                        entity.DisciplineTypeId = logdata.DisciplineTypeId;
                        entity.DecisionCode = logdata.DecisionCode;
                        entity.IncidentName = logdata.IncidentName;
                        entity.IncidentDate = logdata.IncidentDate;
                        entity.DisciplineDate = logdata.DisciplineDate;
                        entity.DurationDays = logdata.DurationDays;
                        entity.FinancialImpact = logdata.FinancialImpact;
                        entity.ViolationDetail = logdata.ViolationDetail;
                        entity.SignerId = logdata.SignerId;
                        entity.FilePath = logdata.FilePath;
                        entity.GroupId = logdata.GroupId;
                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = logdata.CreateAt;
                        entity.CreateBy = logdata.CreateBy;
                        entity.IsActive = logdata.IsActive;
                        entity.ApprovalUserId = logdata.ApprovalUserId;
                        entity.DateApproval = logdata.DateApproval;
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
                    var logupdate = (from p in context.NhanSu_Discipline_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_Discipline_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.NhanSu_Discipline_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.EmployeeId = logdata.EmployeeId;
                        entity.DisciplineTypeId = logdata.DisciplineTypeId;
                        entity.DecisionCode = logdata.DecisionCode;
                        entity.IncidentName = logdata.IncidentName;
                        entity.IncidentDate = logdata.IncidentDate;
                        entity.DisciplineDate = logdata.DisciplineDate;
                        entity.DurationDays = logdata.DurationDays;
                        entity.FinancialImpact = logdata.FinancialImpact;
                        entity.ViolationDetail = logdata.ViolationDetail;
                        entity.SignerId = logdata.SignerId;
                        entity.FilePath = logdata.FilePath;
                        entity.GroupId = data.GroupId;
                        entity.Ordinarily = data.Ordinarily;
                        entity.CreateAt = data.CreateAt;
                        entity.CreateBy = data.CreateBy;
                        entity.IsActive = data.IsActive;
                        entity.ApprovalUserId = data.ApprovalUserId;
                        entity.DateApproval = data.DateApproval;
                        entity.ApprovalDept = data.ApprovalDept;
                        entity.DepartmentId = data.DepartmentId;
                        entity.DepartmentOrder = data.DepartmentOrder;
                        entity.ApprovalOrder = data.ApprovalOrder;
                        entity.ApprovalId = data.ApprovalId;
                        entity.LastApprovalId = data.LastApprovalId;
                        entity.IsStatus = data.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt!";
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại !");
                }
            }
            var addLog = new NhanSu_Discipline_Log()
            {
                EmployeeId = data.EmployeeId,
                DisciplineTypeId = data.DisciplineTypeId,
                DecisionCode = data.DecisionCode,
                IncidentName = data.IncidentName,
                IncidentDate = data.IncidentDate,
                DisciplineDate = data.DisciplineDate,
                DurationDays = data.DurationDays,
                FinancialImpact = data.FinancialImpact,
                ViolationDetail = data.ViolationDetail,
                SignerId = data.SignerId,
                FilePath = data.FilePath,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
                IsValid = false
            };
            context.NhanSu_Discipline_Logs.Add(addLog);
            context.NhanSu_Disciplines.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.Set<NhanSu_Discipline>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.NhanSu_Disciplines.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Đang chờ duyệt thêm mới !");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Đang chờ duyệt sửa !");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Đang chờ duyệt xóa !");
            }
            return true;
        }
        public async Task<bool> CheckSave(NhanSu_Discipline input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.NhanSu_Discipline_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                            && p.EmployeeId == input.EmployeeId
                            && p.DisciplineTypeId == input.DisciplineTypeId
                            && p.DecisionCode == input.DecisionCode
                            && p.IncidentName == input.IncidentName
                            && p.IncidentDate == input.IncidentDate
                            && p.DisciplineDate == input.DisciplineDate
                            && p.DurationDays == input.DurationDays
                            && p.FinancialImpact == input.FinancialImpact
                            && p.ViolationDetail == input.ViolationDetail
                            && p.SignerId == input.SignerId
                            && p.FilePath == input.FilePath

                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception("Thông tin bạn nhập đã tồn tại !");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckEdit(NhanSu_Discipline input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.NhanSu_Discipline_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                            && p.EmployeeId == input.EmployeeId
                            && p.DisciplineTypeId == input.DisciplineTypeId
                            && p.DecisionCode == input.DecisionCode
                            && p.IncidentName == input.IncidentName
                            && p.IncidentDate == input.IncidentDate
                            && p.DisciplineDate == input.DisciplineDate
                            && p.DurationDays == input.DurationDays
                            && p.FinancialImpact == input.FinancialImpact
                            && p.ViolationDetail == input.ViolationDetail
                            && p.SignerId == input.SignerId
                            && p.FilePath == input.FilePath
                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception("Thông tin bạn nhập đã tồn tại !");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckDelete(NhanSu_Discipline input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (1 == 0)
                {
                    throw new Exception("");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã đuợc thay đổi bởi người khác . Vui lòng tải lại trang !");
                }
            }
            return true;
        }
    }
}

