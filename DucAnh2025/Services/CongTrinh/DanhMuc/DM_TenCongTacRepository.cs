using DucAnh2025.Data;
using DucAnh2025.Models.CongTrinh;
using DucAnh2025.Repository.CongTrinh.DanhMuc;
using DucAnh2025.ViewModels.CongTrinh;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.CongTrinh.DanhMuc
{
    public class DM_TenCongTacRepository : IDM_TenCongTacRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public DM_TenCongTacRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<DM_TenCongTacModel>> GetAllByVM(DM_TenCongTacModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.CT_DM_TenCongTacs
                            where p1.GroupId == groupId && p1.IsActive != 100
                            select p1;
                if (!string.IsNullOrEmpty(dataModel.Id_HangMucCongViec))
                {
                    query = query.Where(m => m.Id == dataModel.Id_HangMucCongViec);
                }
                if (!string.IsNullOrEmpty(dataModel.Id_LoaiCauKien))
                {
                    query = query.Where(m => m.Id_LoaiCauKien == dataModel.Id_LoaiCauKien);
                }
                if (!string.IsNullOrEmpty(dataModel.Id_HangMucKhoiLuong))
                {
                    query = query.Where(m => m.Id_HangMucKhoiLuong == dataModel.Id_HangMucKhoiLuong);
                }
                if (!string.IsNullOrEmpty(dataModel.Id_LoaiKhoiLuong))
                {
                    query = query.Where(m => m.Id_LoaiKhoiLuong == dataModel.Id_LoaiKhoiLuong);
                }
                var data = await (from p1 in query
                                  join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                                  from hmcv in hmcvJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                                  from lck in ttvtJoin.DefaultIfEmpty()

                                  join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                                  from hmkl in hmklJoin.DefaultIfEmpty()

                                  join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                                  from lkl in lklJoin.DefaultIfEmpty()

                                  where p1.GroupId == groupId
                                  orderby p1.CreateAt descending
                                  select new DM_TenCongTacModel
                                  {
                                      Id = p1.Id,
                                      Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                      HangMucCongViec = hmcv.TenDanhMuc,
                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                      HangMucKhoiLuong = hmkl.TenDanhMuc,
                                      Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                      LoaiKhoiLuong = lkl.TenDanhMuc,
                                      TenCongTac = p1.TenCongTac,
                                      DonVi = p1.DonVi,

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
        public async Task<List<DM_TenCongTacModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.CT_DM_TenCongTac_Logs
                                  join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                                  from hmcv in hmcvJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                                  from lck in ttvtJoin.DefaultIfEmpty()

                                  join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                                  from hmkl in hmklJoin.DefaultIfEmpty()

                                  join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                                  from lkl in lklJoin.DefaultIfEmpty()
                                  where p1.IdChung == id && p1.IsValid == true
                                  orderby p1.CreateAt
                                  select new DM_TenCongTacModel
                                  {
                                      Id = p1.Id,
                                      Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                      HangMucCongViec = hmcv.TenDanhMuc,
                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                      HangMucKhoiLuong = hmkl.TenDanhMuc,
                                      Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                      LoaiKhoiLuong = lkl.TenDanhMuc,
                                      TenCongTac = p1.TenCongTac,
                                      DonVi = p1.DonVi,

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
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<DM_TenCongTacModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.CT_DM_TenCongTacs
                              join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                              from hmcv in hmcvJoin.DefaultIfEmpty()

                              join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                              from lck in ttvtJoin.DefaultIfEmpty()

                              join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                              from hmkl in hmklJoin.DefaultIfEmpty()

                              join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                              from lkl in lklJoin.DefaultIfEmpty()

                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new DM_TenCongTacModel
                              {
                                  Id = p1.Id,
                                  Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                  HangMucCongViec = hmcv.TenDanhMuc,
                                  Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                  LoaiCauKien = lck.TenDanhMuc,
                                  Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                  HangMucKhoiLuong = hmkl.TenDanhMuc,
                                  Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                  LoaiKhoiLuong = lkl.TenDanhMuc,
                                  TenCongTac = p1.TenCongTac,
                                  DonVi = p1.DonVi,

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
        public async Task<List<DM_TenCongTacModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.CT_DM_TenCongTac_Logs
                                  join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                                  from hmcv in hmcvJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                                  from lck in ttvtJoin.DefaultIfEmpty()

                                  join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                                  from hmkl in hmklJoin.DefaultIfEmpty()

                                  join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                                  from lkl in lklJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new DM_TenCongTacModel
                                  {
                                      Id = p1.Id,
                                      Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                      HangMucCongViec = hmcv.TenDanhMuc,
                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                      HangMucKhoiLuong = hmkl.TenDanhMuc,
                                      Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                      LoaiKhoiLuong = lkl.TenDanhMuc,
                                      TenCongTac = p1.TenCongTac,
                                      DonVi = p1.DonVi,

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
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<CT_DM_TenCongTac>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.CT_DM_TenCongTacs.Where(p => p.IsActive != 100).OrderByDescending(p =>p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<CT_DM_TenCongTac> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.CT_DM_TenCongTacs.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn.");
            }
            return entity;
        }
        public async Task Insert(CT_DM_TenCongTac entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu nào được thêm!");
                }
                context.CT_DM_TenCongTacs.Add(entity);
                var addLog = new CT_DM_TenCongTac_Log()
                {
                    Id = entity.Id,
                    Id_HangMucCongViec = entity.Id_HangMucCongViec,
                    Id_LoaiCauKien = entity.Id_LoaiCauKien,
                    Id_HangMucKhoiLuong = entity.Id_HangMucKhoiLuong,
                    Id_LoaiKhoiLuong = entity.Id_LoaiKhoiLuong,
                    TenCongTac = entity.TenCongTac,
                    DonVi = entity.DonVi,

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
                context.CT_DM_TenCongTac_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(CT_DM_TenCongTac data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
            }
            context.CT_DM_TenCongTacs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.CT_DM_TenCongTac_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.CT_DM_TenCongTac_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.CT_DM_TenCongTac_Logs.Update(updateLog);
                }
            }
            var addLog = new CT_DM_TenCongTac_Log
            {
                Id = Guid.NewGuid().ToString(),

                Id_HangMucCongViec = entity.Id_HangMucCongViec,
                Id_LoaiCauKien = entity.Id_LoaiCauKien,
                Id_HangMucKhoiLuong = entity.Id_HangMucKhoiLuong,
                Id_LoaiKhoiLuong = entity.Id_LoaiKhoiLuong,
                TenCongTac = entity.TenCongTac,
                DonVi = entity.DonVi,

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
            context.CT_DM_TenCongTac_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(CT_DM_TenCongTac[] CT_DM_TenCongTacs)
        {
            using var context = _context.CreateDbContext();
            string[] ids = CT_DM_TenCongTacs.Select(x => x.Id).ToArray();
            var listEntities = await context.CT_DM_TenCongTacs.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.CT_DM_TenCongTacs.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(CT_DM_TenCongTac data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
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
                    var logdata = (from p in context.CT_DM_TenCongTac_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_HangMucCongViec = logdata.Id_HangMucCongViec;
                        data.Id_LoaiCauKien = logdata.Id_LoaiCauKien;
                        data.Id_HangMucKhoiLuong = logdata.Id_HangMucKhoiLuong;
                        data.Id_LoaiKhoiLuong = logdata.Id_LoaiKhoiLuong;
                        data.TenCongTac = logdata.TenCongTac;
                        data.DonVi = logdata.DonVi;

                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.CT_DM_TenCongTac_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.CT_DM_TenCongTac_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.CT_DM_TenCongTac_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new CT_DM_TenCongTac_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_HangMucCongViec = data.Id_HangMucCongViec,
                        Id_LoaiCauKien = data.Id_LoaiCauKien,
                        Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                        Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,
                        TenCongTac = data.TenCongTac,
                        DonVi = data.DonVi,

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
                    context.CT_DM_TenCongTac_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.CT_DM_TenCongTac_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.CT_DM_TenCongTac_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new CT_DM_TenCongTac_Log()
                    {
                        Id = Guid.NewGuid().ToString(),

                        Id_HangMucCongViec = data.Id_HangMucCongViec,
                        Id_LoaiCauKien = data.Id_LoaiCauKien,
                        Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                        Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,
                        TenCongTac = data.TenCongTac,
                        DonVi = data.DonVi,

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
                    context.CT_DM_TenCongTac_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.CT_DM_TenCongTacs.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(CT_DM_TenCongTac data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
            }
            context.CT_DM_TenCongTacs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.CT_DM_TenCongTac_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.CT_DM_TenCongTac_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.CT_DM_TenCongTac_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.CT_DM_TenCongTac_Logs.Update(updateLog);
                }
            }
            var addLog = new CT_DM_TenCongTac_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_HangMucCongViec = data.Id_HangMucCongViec,
                Id_LoaiCauKien = data.Id_LoaiCauKien,
                Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,
                TenCongTac = data.TenCongTac,
                DonVi = data.DonVi,

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
            context.CT_DM_TenCongTac_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(CT_DM_TenCongTac data, string userId)
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
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.CT_DM_TenCongTac_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_HangMucCongViec = logdata.Id_HangMucCongViec;
                        entity.Id_LoaiCauKien = logdata.Id_LoaiCauKien;
                        entity.Id_HangMucKhoiLuong = logdata.Id_HangMucKhoiLuong;
                        entity.Id_LoaiKhoiLuong = logdata.Id_LoaiKhoiLuong;
                        entity.TenCongTac = logdata.TenCongTac;
                        entity.DonVi = logdata.DonVi;

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

                    var logupdate = (from p in context.CT_DM_TenCongTac_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.CT_DM_TenCongTac_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.CT_DM_TenCongTac_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_HangMucCongViec = logdata.Id_HangMucCongViec;
                        entity.Id_LoaiCauKien = logdata.Id_LoaiCauKien;
                        entity.Id_HangMucKhoiLuong = logdata.Id_HangMucKhoiLuong;
                        entity.Id_LoaiKhoiLuong = logdata.Id_LoaiKhoiLuong;
                        entity.TenCongTac = logdata.TenCongTac;
                        entity.DonVi = logdata.DonVi;

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
            var addLog = new CT_DM_TenCongTac_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_HangMucCongViec = data.Id_HangMucCongViec,
                Id_LoaiCauKien = data.Id_LoaiCauKien,
                Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,
                TenCongTac = data.TenCongTac,
                DonVi = data.DonVi,

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
            context.CT_DM_TenCongTac_Logs.Add(addLog);
            context.CT_DM_TenCongTacs.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
            }
            context.Set<CT_DM_TenCongTac>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.CT_DM_TenCongTacs.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn");
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
            //throw new Exception($"Tên dữ liệu đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(CT_DM_TenCongTac input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.CT_DM_TenCongTac_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.Id_HangMucCongViec == input.Id_HangMucCongViec && p.Id_LoaiCauKien == input.Id_LoaiCauKien
                                   && p.Id_HangMucKhoiLuong == input.Id_HangMucKhoiLuong && p.Id_LoaiKhoiLuong == input.Id_LoaiKhoiLuong
                                   && p.TenCongTac == input.TenCongTac && p.DonVi == input.DonVi
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
        public async Task<bool> CheckEdit(CT_DM_TenCongTac input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.CT_DM_TenCongTac_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.Id_HangMucCongViec == input.Id_HangMucCongViec && p.Id_LoaiCauKien == input.Id_LoaiCauKien
                                   && p.Id_HangMucKhoiLuong == input.Id_HangMucKhoiLuong && p.Id_LoaiKhoiLuong == input.Id_LoaiKhoiLuong
                                   && p.TenCongTac == input.TenCongTac && p.DonVi == input.DonVi
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
        public async Task<bool> CheckDelete(CT_DM_TenCongTac input)
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
                    throw new Exception($"Không tìm thấy dữ liệu đã chọn");
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
