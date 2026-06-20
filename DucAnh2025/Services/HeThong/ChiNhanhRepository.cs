using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class ChiNhanhRepository : IChiNhanhRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public ChiNhanhRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public ChiNhanhModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.ChiNhanhs
                         join ChiNhanhs1 in context.ChiNhanhs on p1.ParentId equals ChiNhanhs1.Id
                         join CompanyTypes1 in context.CompanyTypes on p1.CompanyType equals CompanyTypes1.Id
                         where p1.Id == id && p1.IsActive != 100
                         select new ChiNhanhModel
                         {
                             Id = p1.Id,
                             ParentId = ChiNhanhs1.TenChiNhanh,
                             TenChiNhanh = p1.TenChiNhanh,
                             CompanyType = CompanyTypes1.TenLoaiChiNhanh,
                             Phone = p1.Phone,
                             Email = p1.Email,
                             Address = p1.Address,
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
        public async Task<List<ChiNhanhModel>> GetAllByVM(ChiNhanhModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.ChiNhanhs
                        where p1.GroupId == groupId && p1.IsActive != 100
                        select p1;
            if (!string.IsNullOrEmpty(dataModel.ParentId))
            {
                query = query.Where(m => m.ParentId == dataModel.ParentId);
            }
            if (!string.IsNullOrEmpty(dataModel.CompanyType))
            {
                query = query.Where(m => m.CompanyType == dataModel.CompanyType);
            }
            var data = await (from p1 in query
                              join ChiNhanhs1 in context.ChiNhanhs on p1.ParentId equals ChiNhanhs1.Id
                              join CompanyTypes1 in context.CompanyTypes on p1.CompanyType equals CompanyTypes1.Id
                              where p1.GroupId == groupId
                              orderby p1.CreateAt descending
                              select new ChiNhanhModel
                              {
                                  Id = p1.Id,
                                  ParentId = ChiNhanhs1.TenChiNhanh,
                                  TenChiNhanh = p1.TenChiNhanh,
                                  CompanyType = CompanyTypes1.TenLoaiChiNhanh,
                                  Phone = p1.Phone,
                                  Email = p1.Email,
                                  Address = p1.Address,
                                  IsActive = p1.IsActive,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<List<ChiNhanhModel>> GetHistoryIsValidEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ChiNhanh_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.ParentId equals ChiNhanhs1.Id
                              join CompanyTypes1 in context.CompanyTypes on p1.CompanyType equals CompanyTypes1.Id
                              where p1.IdChung == id && p1.IsValid == true
                              orderby p1.CreateAt
                              select new ChiNhanhModel
                              {
                                  Id = p1.Id,
                                  ParentId = ChiNhanhs1.TenChiNhanh,
                                  TenChiNhanh = p1.TenChiNhanh,
                                  CompanyType = CompanyTypes1.TenLoaiChiNhanh,
                                  Phone = p1.Phone,
                                  Email = p1.Email,
                                  Address = p1.Address,
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
        public async Task<ChiNhanhModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ChiNhanhs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.ParentId equals ChiNhanhs1.Id
                              join CompanyTypes1 in context.CompanyTypes on p1.CompanyType equals CompanyTypes1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new ChiNhanhModel
                              {
                                  Id = p1.Id,
                                  ParentId = ChiNhanhs1.TenChiNhanh,
                                  TenChiNhanh = p1.TenChiNhanh,
                                  CompanyType = CompanyTypes1.TenLoaiChiNhanh,
                                  Phone = p1.Phone,
                                  Email = p1.Email,
                                  Address = p1.Address,
                                  
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
        public async Task<List<ChiNhanhModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.ChiNhanh_Logs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.ParentId equals ChiNhanhs1.Id
                              join CompanyTypes1 in context.CompanyTypes on p1.CompanyType equals CompanyTypes1.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new ChiNhanhModel
                              {
                                  Id = p1.Id,
                                  ParentId = ChiNhanhs1.TenChiNhanh,
                                  TenChiNhanh = p1.TenChiNhanh,
                                  CompanyType = CompanyTypes1.TenLoaiChiNhanh,
                                  Phone = p1.Phone,
                                  Email = p1.Email,
                                  Address = p1.Address,
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
        public async Task<List<ChiNhanh>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ChiNhanhs.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ChiNhanhModel>>? GetChiNhanhsForParentId(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ChiNhanhs
                                    join p2 in context.ChiNhanhs on p1.ParentId equals p2.Id
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
        public async Task<List<CompanyTypeModel>>? GetCompanyTypesForCompanyType(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ChiNhanhs
                                    join p2 in context.CompanyTypes on p1.CompanyType equals p2.Id
                                    where p1.GroupId == groupId && p1.IsActive != 100
                                    orderby p2.TenLoaiChiNhanh
                                    select new CompanyTypeModel
                                    {
                                        Id = p2.Id,
                                        TenLoaiChiNhanh = p2.TenLoaiChiNhanh
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<ChiNhanh> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ChiNhanhs.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn.");
            }
            return entity;
        }
        public async Task Insert(ChiNhanh entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có chi nhánh nào được thêm!");
                }
                context.ChiNhanhs.Add(entity);
                var addLog = new ChiNhanh_Log()
                {
                    Id = entity.Id,
                    ParentId = entity.ParentId,
                    TenChiNhanh = entity.TenChiNhanh,
                    CompanyType = entity.CompanyType,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    Address = entity.Address,
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
                   
                };
                context.ChiNhanh_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(ChiNhanh data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
            }
            context.ChiNhanhs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ChiNhanh_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ChiNhanh_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ChiNhanh_Logs.Update(updateLog);
                }
            }
            var addLog = new ChiNhanh_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ParentId = data.ParentId,
                TenChiNhanh = data.TenChiNhanh,
                CompanyType = data.CompanyType,
                Phone = data.Phone,
                Email = data.Email,
                Address = data.Address,
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
            context.ChiNhanh_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(ChiNhanh[] ChiNhanhs)
        {
            using var context = _context.CreateDbContext();
            string[] ids = ChiNhanhs.Select(x => x.Id).ToArray();
            var listEntities = await context.ChiNhanhs.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.ChiNhanhs.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(ChiNhanh data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
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
                    var logdata = (from p in context.ChiNhanh_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.ParentId = logdata.ParentId;
                        data.TenChiNhanh = logdata.TenChiNhanh;
                        data.CompanyType = logdata.CompanyType;
                        data.Phone = logdata.Phone;
                        data.Email = logdata.Email;
                        data.Address = logdata.Address;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.ChiNhanh_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.ChiNhanh_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ChiNhanh_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ChiNhanh_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ParentId = data.ParentId,
                        TenChiNhanh = data.TenChiNhanh,
                        CompanyType = data.CompanyType,
                        Phone = data.Phone,
                        Email = data.Email,
                        Address = data.Address,
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
                    context.ChiNhanh_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.ChiNhanh_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ChiNhanh_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new ChiNhanh_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ParentId = data.ParentId,
                        TenChiNhanh = data.TenChiNhanh,
                        CompanyType = data.CompanyType,
                        Phone = data.Phone,
                        Email = data.Email,
                        Address = data.Address,
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
                    context.ChiNhanh_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.ChiNhanhs.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(ChiNhanh data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
            }
            context.ChiNhanhs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ChiNhanh_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.ChiNhanh_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.ChiNhanh_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.ChiNhanh_Logs.Update(updateLog);
                }
            }
            var addLog = new ChiNhanh_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ParentId = data.ParentId,
                TenChiNhanh = data.TenChiNhanh,
                CompanyType = data.CompanyType,
                Phone = data.Phone,
                Email = data.Email,
                Address = data.Address,
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
            context.ChiNhanh_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(ChiNhanh data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
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
                    var logdata = (from p in context.ChiNhanh_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.ParentId = logdata.ParentId;
                        entity.TenChiNhanh = logdata.TenChiNhanh;
                        entity.CompanyType = logdata.CompanyType;
                        entity.Phone = logdata.Phone;
                        entity.Email = logdata.Email;
                        entity.Address = logdata.Address;
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

                    var logupdate = (from p in context.ChiNhanh_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.ChiNhanh_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.ChiNhanh_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.ParentId = data.ParentId;
                        entity.TenChiNhanh = data.TenChiNhanh;
                        entity.CompanyType = data.CompanyType;
                        entity.Phone = data.Phone;
                        entity.Email = data.Email;
                        entity.Address = data.Address;
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
            var addLog = new ChiNhanh_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ParentId = data.ParentId,
                TenChiNhanh = data.TenChiNhanh,
                CompanyType = data.CompanyType,
                Phone = data.Phone,
                Email = data.Email,
                Address = data.Address,
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
            context.ChiNhanh_Logs.Add(addLog);
            context.ChiNhanhs.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
            }
            context.Set<ChiNhanh>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.ChiNhanhs.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy chi nhánh đã chọn");
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
            //throw new Exception($"Tên chi nhánh đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(ChiNhanh input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.ChiNhanh_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.ParentId == input.ParentId
                                   && p.CompanyType == input.CompanyType
                                   && p.TenChiNhanh == input.TenChiNhanh
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
        public async Task<bool> CheckEdit(ChiNhanh input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.ChiNhanh_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.ParentId == input.ParentId
                                   && p.CompanyType == input.CompanyType
                                   && p.TenChiNhanh == input.TenChiNhanh
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
        public async Task<bool> CheckDelete(ChiNhanh input)
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
                    throw new Exception($"Không tìm thấy chi nhánh đã chọn");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }
        public async Task<List<ChiNhanh>> GetChiNhanhByPermission(string groupId, string majorId, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p in context.ChiNhanhs
                                    join q in context.MajorUserPermissions on p.GroupId equals q.GroupId
                                    where q.MajorId == majorId
                                    && q.UserId == userId
                                    select p).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
