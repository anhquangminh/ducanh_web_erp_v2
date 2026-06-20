using DucAnh2025.Data;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.NhanSu
{
    public class DM_ChuyenMonRepository : IDM_ChuyenMonRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public DM_ChuyenMonRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public DM_ChuyenMonModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var query = (from p1 in context.DM_ChuyenMons
                         where p1.Id == id && p1.IsActive != 100
                         select new DM_ChuyenMonModel
                         {
                             Id = p1.Id,
                             ChuyenMon = p1.ChuyenMon,
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
        public async Task<List<DM_ChuyenMonModel>> GetAllByVM(DM_ChuyenMonModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.DM_ChuyenMons
                        where p1.GroupId == groupId && p1.IsActive != 100
                        select p1;
            var data = await (from p1 in query
                              where p1.GroupId == groupId
                              orderby p1.CreateAt ascending
                              select new DM_ChuyenMonModel
                              {
                                  Id = p1.Id,
                                  ChuyenMon = p1.ChuyenMon,
                                  IsActive = p1.IsActive,
                                  IsStatus = p1.IsStatus
                              }).ToListAsync();
            return data;
        }
        public async Task<List<DM_ChuyenMonModel>> GetHistoryIsValidEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.DM_ChuyenMon_Logs
                              where p1.IdChung == id && p1.IsValid == true
                              orderby p1.CreateAt
                              select new DM_ChuyenMonModel
                              {
                                  Id = p1.Id,
                                  ChuyenMon = p1.ChuyenMon,
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
        public async Task<DM_ChuyenMonModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.DM_ChuyenMons
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new DM_ChuyenMonModel
                              {
                                  Id = p1.Id,
                                  ChuyenMon = p1.ChuyenMon,
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
        public async Task<List<DM_ChuyenMonModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.DM_ChuyenMon_Logs
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new DM_ChuyenMonModel
                              {
                                  Id = p1.Id,
                                  ChuyenMon = p1.ChuyenMon,
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
        public async Task<List<DM_ChuyenMon>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.DM_ChuyenMons.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<DM_ChuyenMon> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.DM_ChuyenMons.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn.");
            }
            return entity;
        }
        public async Task Insert(DM_ChuyenMon entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có chuyên môn nào được thêm!");
                }
                context.DM_ChuyenMons.Add(entity);
                var addLog = new DM_ChuyenMon_Log()
                {
                    Id = entity.Id,
                    ChuyenMon = entity.ChuyenMon,
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
                    IsValid = true
                };
                context.DM_ChuyenMon_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(DM_ChuyenMon data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
            }
            context.DM_ChuyenMons.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.DM_ChuyenMon_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.DM_ChuyenMon_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.DM_ChuyenMon_Logs.Update(updateLog);
                }
            }
            var addLog = new DM_ChuyenMon_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ChuyenMon = data.ChuyenMon,
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
            context.DM_ChuyenMon_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(DM_ChuyenMon[] DM_ChuyenMons)
        {
            using var context = _context.CreateDbContext();
            string[] ids = DM_ChuyenMons.Select(x => x.Id).ToArray();
            var listEntities = await context.DM_ChuyenMons.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.DM_ChuyenMons.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(DM_ChuyenMon data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
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
                    var logdata = (from p in context.DM_ChuyenMon_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.ChuyenMon = logdata.ChuyenMon;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;
                        logdata.IsValid = true;
                        context.DM_ChuyenMon_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.DM_ChuyenMon_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.DM_ChuyenMon_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new DM_ChuyenMon_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChuyenMon = data.ChuyenMon,
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
                    context.DM_ChuyenMon_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.DM_ChuyenMon_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.DM_ChuyenMon_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new DM_ChuyenMon_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChuyenMon = data.ChuyenMon,
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
                    context.DM_ChuyenMon_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.DM_ChuyenMons.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(DM_ChuyenMon data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
            }
            context.DM_ChuyenMons.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.DM_ChuyenMon_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.DM_ChuyenMon_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.DM_ChuyenMon_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.DM_ChuyenMon_Logs.Update(updateLog);
                }
            }
            var addLog = new DM_ChuyenMon_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ChuyenMon = data.ChuyenMon,
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
            context.DM_ChuyenMon_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(DM_ChuyenMon data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
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
                    var logdata = (from p in context.DM_ChuyenMon_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.ChuyenMon = logdata.ChuyenMon;
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
                    var logupdate = (from p in context.DM_ChuyenMon_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.DM_ChuyenMon_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.DM_ChuyenMon_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.ChuyenMon = data.ChuyenMon;
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
                    entity.IsStatus = "Đã duyệt";
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại.");
                }
            }
            var addLog = new DM_ChuyenMon_Log()
            {
                Id = Guid.NewGuid().ToString(),
                ChuyenMon = data.ChuyenMon,
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
            context.DM_ChuyenMon_Logs.Add(addLog);
            context.DM_ChuyenMons.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
            }
            context.Set<DM_ChuyenMon>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.DM_ChuyenMons.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy chuyên môn đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Chuyên môn đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Chuyên môn đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Chuyên môn đang chờ duyệt xóa.");
            }
            //if (model != null && model.TenDM_ChuyenMon.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên chuyên môn đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(DM_ChuyenMon input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.DM_ChuyenMon_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.ChuyenMon == input.ChuyenMon
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
        public async Task<bool> CheckEdit(DM_ChuyenMon input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.DM_ChuyenMon_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.ChuyenMon == input.ChuyenMon
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
        public async Task<bool> CheckDelete(DM_ChuyenMon input)
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
                    throw new Exception($"Không tìm thấy chuyên môn đã chọn");
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
