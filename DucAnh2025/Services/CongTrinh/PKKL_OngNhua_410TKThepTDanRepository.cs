using DucAnh2025.Data;
using DucAnh2025.Models.CongTrinh;
using DucAnh2025.Repository.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.CongTrinh
{
    public class PKKL_OngNhua_410TKThepTDanRepository : IPKKL_OngNhua_410TKThepTDanRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public PKKL_OngNhua_410TKThepTDanRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetAllByVM(PKKL_OngNhua_410TKThepTDanModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.PKKL_OngNhua_410TKThepTDans
                            where p1.IsActive != 100
                            where p1.GroupId == groupId && p1.IsActive != 100
                            select p1;

                var danhMucWithTenLoai = from dmt in context.CT_DM_DanhMucTheps
                                         join tlt in context.CT_DM_TenLoaiTheps on dmt.Id_TenLoaiThep equals tlt.Id
                                         select new
                                         {
                                             dmt,
                                             TenLoaiThep = tlt.TenDanhMuc
                                         };

                var data = await (from p1 in query
                                  join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                                  from cn in cnJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into lckJoin
                                  from lck in lckJoin.DefaultIfEmpty()
                                  join bb in danhMucWithTenLoai on p1.Id_TenLoaiThep equals bb.dmt.Id into bbJoin
                                  from bb in bbJoin.DefaultIfEmpty()

                                  where p1.GroupId == groupId
                                  orderby p1.CreateAt descending
                                  select new PKKL_OngNhua_410TKThepTDanModel
                                  {
                                      Id = p1.Id,
                                      Id_ChiNhanh = p1.Id_ChiNhanh,
                                      TenChiNhanh = cn.TenChiNhanh,

                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      TenCongTacTheoKL = p1.TenCongTacTheoKL,
                                      TenCongTacTheoDK = p1.TenCongTacTheoDK,
                                      ViTriLayKL = p1.ViTriLayKL,
                                      Id_TenLoaiThep = p1.Id_TenLoaiThep,
                                      TenLoaiThep = bb.TenLoaiThep,
                                      TenThanhThepTheoCK = p1.TenThanhThepTheoCK,
                                      TenThanhThepTheoCD = p1.TenThanhThepTheoCD,
                                      SoHieuThep = p1.SoHieuThep,
                                      DKCDay = p1.DKCDay,
                                      SoThanhCK = p1.SoThanhCK,
                                      SoCK = p1.SoCK,
                                      TongSoThanh = p1.TongSoThanh,
                                      CDai1Thanh = p1.CDai1Thanh,
                                      TongCDai = p1.TongCDai,
                                      TrongLuong = p1.TrongLuong,
                                      TongTrongLuong = p1.TongTrongLuong,

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
        public async Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var danhMucWithTenLoai = from dmt in context.CT_DM_DanhMucTheps
                                         join tlt in context.CT_DM_TenLoaiTheps on dmt.Id_TenLoaiThep equals tlt.Id
                                         select new
                                         {
                                             dmt,
                                             TenLoaiThep = tlt.TenDanhMuc
                                         };
                var data = await (from p1 in context.PKKL_OngNhua_410TKThepTDan_Logs
                                  join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                                  from cn in cnJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into lckJoin
                                  from lck in lckJoin.DefaultIfEmpty()
                                  join bb in danhMucWithTenLoai on p1.Id_TenLoaiThep equals bb.dmt.Id into bbJoin
                                  from bb in bbJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1

                                  where p1.IdChung == id && p1.IsValid == true
                                  orderby p1.CreateAt
                                  select new PKKL_OngNhua_410TKThepTDanModel
                                  {
                                      Id = p1.Id,
                                      Id_ChiNhanh = p1.Id_ChiNhanh,
                                      TenChiNhanh = cn.TenChiNhanh,

                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      TenCongTacTheoKL = p1.TenCongTacTheoKL,
                                      TenCongTacTheoDK = p1.TenCongTacTheoDK,
                                      ViTriLayKL = p1.ViTriLayKL,
                                      Id_TenLoaiThep = p1.Id_TenLoaiThep,
                                      TenLoaiThep = bb.TenLoaiThep,
                                      TenThanhThepTheoCK = p1.TenThanhThepTheoCK,
                                      TenThanhThepTheoCD = p1.TenThanhThepTheoCD,
                                      SoHieuThep = p1.SoHieuThep,
                                      DKCDay = p1.DKCDay,
                                      SoThanhCK = p1.SoThanhCK,
                                      SoCK = p1.SoCK,
                                      TongSoThanh = p1.TongSoThanh,
                                      CDai1Thanh = p1.CDai1Thanh,
                                      TongCDai = p1.TongCDai,
                                      TrongLuong = p1.TrongLuong,
                                      TongTrongLuong = p1.TongTrongLuong,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = p1.CreateAt ?? DateTime.MinValue,
                                      CreateBy = createBy.Email,
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
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<PKKL_OngNhua_410TKThepTDanModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var danhMucWithTenLoai = from dmt in context.CT_DM_DanhMucTheps
                                     join tlt in context.CT_DM_TenLoaiTheps on dmt.Id_TenLoaiThep equals tlt.Id
                                     select new
                                     {
                                         dmt,
                                         TenLoaiThep = tlt.TenDanhMuc
                                     };
            var data = await (from p1 in context.PKKL_OngNhua_410TKThepTDans
                              join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                              from cn in cnJoin.DefaultIfEmpty()

                              join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into lckJoin
                              from lck in lckJoin.DefaultIfEmpty()
                              join bb in danhMucWithTenLoai on p1.Id_TenLoaiThep equals bb.dmt.Id into bbJoin
                              from bb in bbJoin.DefaultIfEmpty()

                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new PKKL_OngNhua_410TKThepTDanModel
                              {
                                  Id = p1.Id,
                                  Id_ChiNhanh = p1.Id_ChiNhanh,
                                  TenChiNhanh = cn.TenChiNhanh,

                                  Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                  LoaiCauKien = lck.TenDanhMuc,
                                  TenCongTacTheoKL = p1.TenCongTacTheoKL,
                                  TenCongTacTheoDK = p1.TenCongTacTheoDK,
                                  ViTriLayKL = p1.ViTriLayKL,
                                  Id_TenLoaiThep = p1.Id_TenLoaiThep,
                                  TenLoaiThep = bb.TenLoaiThep,
                                  TenThanhThepTheoCK = p1.TenThanhThepTheoCK,
                                  TenThanhThepTheoCD = p1.TenThanhThepTheoCD,
                                  SoHieuThep = p1.SoHieuThep,
                                  DKCDay = p1.DKCDay,
                                  SoThanhCK = p1.SoThanhCK,
                                  SoCK = p1.SoCK,
                                  TongSoThanh = p1.TongSoThanh,
                                  CDai1Thanh = p1.CDai1Thanh,
                                  TongCDai = p1.TongCDai,
                                  TrongLuong = p1.TrongLuong,
                                  TongTrongLuong = p1.TongTrongLuong,

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
        public async Task<List<PKKL_OngNhua_410TKThepTDanModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var danhMucWithTenLoai = from dmt in context.CT_DM_DanhMucTheps
                                         join tlt in context.CT_DM_TenLoaiTheps on dmt.Id_TenLoaiThep equals tlt.Id
                                         select new
                                         {
                                             dmt,
                                             TenLoaiThep = tlt.TenDanhMuc
                                         };
                var data = await (from p1 in context.PKKL_OngNhua_410TKThepTDan_Logs
                                  join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                                  from cn in cnJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into lckJoin
                                  from lck in lckJoin.DefaultIfEmpty()
                                  join bb in danhMucWithTenLoai on p1.Id_TenLoaiThep equals bb.dmt.Id into bbJoin
                                  from bb in bbJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new PKKL_OngNhua_410TKThepTDanModel
                                  {
                                      Id = p1.Id,
                                      Id_ChiNhanh = p1.Id_ChiNhanh,
                                      TenChiNhanh = cn.TenChiNhanh,

                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      TenCongTacTheoKL = p1.TenCongTacTheoKL,
                                      TenCongTacTheoDK = p1.TenCongTacTheoDK,
                                      ViTriLayKL = p1.ViTriLayKL,
                                      Id_TenLoaiThep = p1.Id_TenLoaiThep,
                                      TenLoaiThep = bb.TenLoaiThep,
                                      TenThanhThepTheoCK = p1.TenThanhThepTheoCK,
                                      TenThanhThepTheoCD = p1.TenThanhThepTheoCD,
                                      SoHieuThep = p1.SoHieuThep,
                                      DKCDay = p1.DKCDay,
                                      SoThanhCK = p1.SoThanhCK,
                                      SoCK = p1.SoCK,
                                      TongSoThanh = p1.TongSoThanh,
                                      CDai1Thanh = p1.CDai1Thanh,
                                      TongCDai = p1.TongCDai,
                                      TrongLuong = p1.TrongLuong,
                                      TongTrongLuong = p1.TongTrongLuong,

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
        public async Task<List<PKKL_OngNhua_410TKThepTDan>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.PKKL_OngNhua_410TKThepTDans.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<PKKL_OngNhua_410TKThepTDan> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.PKKL_OngNhua_410TKThepTDans.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn.");
            }
            return entity;
        }
        public async Task Insert(PKKL_OngNhua_410TKThepTDan entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có thông tin nào được thêm!");
                }
                context.PKKL_OngNhua_410TKThepTDans.Add(entity);
                var addLog = new PKKL_OngNhua_410TKThepTDan_Log()
                {
                    Id = entity.Id,
                    Id_ChiNhanh = entity.Id_ChiNhanh,
                    Id_LoaiCauKien = entity.Id_LoaiCauKien,
                    TenCongTacTheoKL = entity.TenCongTacTheoKL,
                    TenCongTacTheoDK = entity.TenCongTacTheoDK,
                    ViTriLayKL = entity.ViTriLayKL,
                    Id_TenLoaiThep = entity.Id_TenLoaiThep,
                    TenThanhThepTheoCK = entity.TenThanhThepTheoCK,
                    TenThanhThepTheoCD = entity.TenThanhThepTheoCD,
                    SoHieuThep = entity.SoHieuThep,
                    DKCDay = entity.DKCDay,
                    SoThanhCK = entity.SoThanhCK,
                    SoCK = entity.SoCK,
                    TongSoThanh = entity.TongSoThanh,
                    CDai1Thanh = entity.CDai1Thanh,
                    TongCDai = entity.TongCDai,
                    TrongLuong = entity.TrongLuong,
                    TongTrongLuong = entity.TongTrongLuong,

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
                context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(PKKL_OngNhua_410TKThepTDan data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.PKKL_OngNhua_410TKThepTDans.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.PKKL_OngNhua_410TKThepTDan_Logs.Update(updateLog);
                }
            }
            var addLog = new PKKL_OngNhua_410TKThepTDan_Log
            {
                Id = Guid.NewGuid().ToString(),
                Id_ChiNhanh = entity.Id_ChiNhanh,
                Id_LoaiCauKien = entity.Id_LoaiCauKien,
                TenCongTacTheoKL = entity.TenCongTacTheoKL,
                TenCongTacTheoDK = entity.TenCongTacTheoDK,
                ViTriLayKL = entity.ViTriLayKL,
                Id_TenLoaiThep = entity.Id_TenLoaiThep,
                TenThanhThepTheoCK = entity.TenThanhThepTheoCK,
                TenThanhThepTheoCD = entity.TenThanhThepTheoCD,
                SoHieuThep = entity.SoHieuThep,
                DKCDay = entity.DKCDay,
                SoThanhCK = entity.SoThanhCK,
                SoCK = entity.SoCK,
                TongSoThanh = entity.TongSoThanh,
                CDai1Thanh = entity.CDai1Thanh,
                TongCDai = entity.TongCDai,
                TrongLuong = entity.TrongLuong,
                TongTrongLuong = entity.TongTrongLuong,

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
            context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(PKKL_OngNhua_410TKThepTDan[] PKKL_OngNhua_410TKThepTDans)
        {
            using var context = _context.CreateDbContext();
            string[] ids = PKKL_OngNhua_410TKThepTDans.Select(x => x.Id).ToArray();
            var listEntities = await context.PKKL_OngNhua_410TKThepTDans.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.PKKL_OngNhua_410TKThepTDans.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(PKKL_OngNhua_410TKThepTDan data, string userId)
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
                    var logdata = (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_LoaiCauKien = logdata.Id_LoaiCauKien;
                        data.TenCongTacTheoKL = logdata.TenCongTacTheoKL;
                        data.TenCongTacTheoDK = logdata.TenCongTacTheoDK;
                        data.ViTriLayKL = logdata.ViTriLayKL;
                        data.Id_TenLoaiThep = logdata.Id_TenLoaiThep;
                        data.TenThanhThepTheoCK = logdata.TenThanhThepTheoCK;
                        data.TenThanhThepTheoCD = logdata.TenThanhThepTheoCD;
                        data.SoHieuThep = logdata.SoHieuThep;
                        data.DKCDay = logdata.DKCDay;
                        data.SoThanhCK = logdata.SoThanhCK;
                        data.SoCK = logdata.SoCK;
                        data.TongSoThanh = logdata.TongSoThanh;
                        data.CDai1Thanh = logdata.CDai1Thanh;
                        data.TongCDai = logdata.TongCDai;
                        data.TrongLuong = logdata.TrongLuong;
                        data.TongTrongLuong = logdata.TongTrongLuong;

                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.PKKL_OngNhua_410TKThepTDan_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new PKKL_OngNhua_410TKThepTDan_Log()
                    {
                        Id = data.Id,
                        Id_ChiNhanh = data.Id_ChiNhanh,
                        Id_LoaiCauKien = entity.Id_LoaiCauKien,
                        TenCongTacTheoKL = entity.TenCongTacTheoKL,
                        TenCongTacTheoDK = entity.TenCongTacTheoDK,
                        ViTriLayKL = entity.ViTriLayKL,
                        Id_TenLoaiThep = entity.Id_TenLoaiThep,
                        TenThanhThepTheoCK = entity.TenThanhThepTheoCK,
                        TenThanhThepTheoCD = entity.TenThanhThepTheoCD,
                        SoHieuThep = entity.SoHieuThep,
                        DKCDay = entity.DKCDay,
                        SoThanhCK = entity.SoThanhCK,
                        SoCK = entity.SoCK,
                        TongSoThanh = entity.TongSoThanh,
                        CDai1Thanh = entity.CDai1Thanh,
                        TongCDai = entity.TongCDai,
                        TrongLuong = entity.TrongLuong,
                        TongTrongLuong = entity.TongTrongLuong,

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
                    context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new PKKL_OngNhua_410TKThepTDan_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_ChiNhanh = data.Id_ChiNhanh,
                        Id_LoaiCauKien = entity.Id_LoaiCauKien,
                        TenCongTacTheoKL = entity.TenCongTacTheoKL,
                        TenCongTacTheoDK = entity.TenCongTacTheoDK,
                        ViTriLayKL = entity.ViTriLayKL,
                        Id_TenLoaiThep = entity.Id_TenLoaiThep,
                        TenThanhThepTheoCK = entity.TenThanhThepTheoCK,
                        TenThanhThepTheoCD = entity.TenThanhThepTheoCD,
                        SoHieuThep = entity.SoHieuThep,
                        DKCDay = entity.DKCDay,
                        SoThanhCK = entity.SoThanhCK,
                        SoCK = entity.SoCK,
                        TongSoThanh = entity.TongSoThanh,
                        CDai1Thanh = entity.CDai1Thanh,
                        TongCDai = entity.TongCDai,
                        TrongLuong = entity.TrongLuong,
                        TongTrongLuong = entity.TongTrongLuong,

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
                    context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.PKKL_OngNhua_410TKThepTDans.Update(data);
            await context.SaveChangesAsync();
        }
       
        public async Task Approval(PKKL_OngNhua_410TKThepTDan data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
                throw new Exception($"Không tìm thấy thông tin đã chọn");

            // Cập nhật entity từ data
            MapFromDataToEntity(data, entity);
            context.PKKL_OngNhua_410TKThepTDans.Update(entity);

            if (data.IsActive == 3 || data.IsActive == 100)
            {
                var updateLogs = await context.PKKL_OngNhua_410TKThepTDan_Logs
                    .Where(p => p.IdChung == entity.Id && p.IsValid == true)
                    .ToListAsync();

                updateLogs.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(updateLogs);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await context.PKKL_OngNhua_410TKThepTDan_Logs
                    .Where(p => p.IdChung == entity.Id)
                    .OrderByDescending(p => p.CreateAt)
                    .FirstOrDefaultAsync();

                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.PKKL_OngNhua_410TKThepTDan_Logs.Update(updateLog);
                }
            }

            var addLog = CreateLogFromData(data, userId);
            addLog.IsValid = data.IsActive == 100 ? false : true;

            context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(PKKL_OngNhua_410TKThepTDan data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
                throw new Exception("Không tìm thấy thông tin đã chọn");

            if (entity.IsActive == 0)
            {
                entity.IsActive = 90;
                entity.IsStatus = "Không duyệt";
                entity.ApprovalUserId = data.ApprovalUserId;
            }
            else if (entity.IsActive == 1 || entity.IsActive == 2)
            {
                var logdata = context.PKKL_OngNhua_410TKThepTDan_Logs
                    .Where(p => p.IdChung == entity.Id && p.IsValid == true)
                    .OrderBy(p => p.CreateAt)
                    .FirstOrDefault();

                if (logdata != null)
                {
                    MapFromLogToData(logdata, data);

                    if (entity.IsActive == 1)
                    {
                        MapFromLogToEntity(logdata, entity);

                        var logupdate = context.PKKL_OngNhua_410TKThepTDan_Logs
                            .Where(p => p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt)
                            .ToList();

                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_410TKThepTDan_Logs.UpdateRange(logupdate);
                    }
                    else
                    {
                        MapFromDataToEntity(data, entity);
                    }

                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                }
            }
            else if (entity.IsActive == 3)
            {
                throw new Exception("Thông tin hủy duyệt không tồn tại.");
            }

            var addLog = CreateLogFromData(data, userId);
            context.PKKL_OngNhua_410TKThepTDan_Logs.Add(addLog);
            context.PKKL_OngNhua_410TKThepTDans.Update(entity);
            await context.SaveChangesAsync();
        }
        private void MapFromLogToData(PKKL_OngNhua_410TKThepTDan_Log log, PKKL_OngNhua_410TKThepTDan data)
        {
            foreach (var prop in typeof(PKKL_OngNhua_410TKThepTDan_Log).GetProperties())
            {
                var targetProp = typeof(PKKL_OngNhua_410TKThepTDan).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    targetProp.SetValue(data, prop.GetValue(log));
                }
            }
        }
        private void MapFromDataToEntity(PKKL_OngNhua_410TKThepTDan data, PKKL_OngNhua_410TKThepTDan entity)
        {
            foreach (var prop in typeof(PKKL_OngNhua_410TKThepTDan).GetProperties())
            {
                if (prop.Name == "Id") continue; // Không ghi đè ID chính
                var entityProp = typeof(PKKL_OngNhua_410TKThepTDan).GetProperty(prop.Name);
                if (entityProp != null && entityProp.CanWrite)
                {
                    entityProp.SetValue(entity, prop.GetValue(data));
                }
            }
        }
        private void MapFromLogToEntity(PKKL_OngNhua_410TKThepTDan_Log log, PKKL_OngNhua_410TKThepTDan entity)
        {
            foreach (var prop in typeof(PKKL_OngNhua_410TKThepTDan_Log).GetProperties())
            {
                if (prop.Name == "Id" || prop.Name == "IsValid") continue;
                var targetProp = typeof(PKKL_OngNhua_410TKThepTDan).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    targetProp.SetValue(entity, prop.GetValue(log));
                }
            }
        }
        private PKKL_OngNhua_410TKThepTDan_Log CreateLogFromData(PKKL_OngNhua_410TKThepTDan data, string userId)
        {
            var log = new PKKL_OngNhua_410TKThepTDan_Log();
            foreach (var prop in typeof(PKKL_OngNhua_410TKThepTDan_Log).GetProperties())
            {
                if (prop.Name == "Id")
                {
                    prop.SetValue(log, Guid.NewGuid().ToString());
                }
                else if (prop.Name == "CreateAt")
                {
                    prop.SetValue(log, DateTime.Now);
                }
                else if (prop.Name == "ApprovalUserId")
                {
                    prop.SetValue(log, userId);
                }
                else if (prop.Name == "DateApproval")
                {
                    prop.SetValue(log, DateTime.Now);
                }
                else if (prop.Name == "IsValid")
                {
                    prop.SetValue(log, false);
                }
                else
                {
                    var dataProp = typeof(PKKL_OngNhua_410TKThepTDan).GetProperty(prop.Name);
                    if (dataProp != null)
                    {
                        prop.SetValue(log, dataProp.GetValue(data));
                    }
                }
            }
            log.IdChung = data.Id;
            log.DepartmentId = null;
            log.DepartmentOrder = 0;
            log.ApprovalOrder = 0;
            log.ApprovalId = null;
            log.LastApprovalId = null;
            return log;
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.Set<PKKL_OngNhua_410TKThepTDan>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.PKKL_OngNhua_410TKThepTDans.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
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
        public async Task<bool> CheckSave(PKKL_OngNhua_410TKThepTDan input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                    && p.Id_ChiNhanh != input.Id_ChiNhanh
                                    && p.Id_LoaiCauKien != input.Id_LoaiCauKien
                                    && p.TenCongTacTheoKL != input.TenCongTacTheoKL
                                    && p.TenCongTacTheoDK != input.TenCongTacTheoDK
                                    && p.ViTriLayKL != input.ViTriLayKL
                                    && p.Id_TenLoaiThep != input.Id_TenLoaiThep
                                    && p.TenThanhThepTheoCK != input.TenThanhThepTheoCK
                                    && p.TenThanhThepTheoCD != input.TenThanhThepTheoCD
                                    && p.SoHieuThep != input.SoHieuThep
                                    && p.DKCDay != input.DKCDay
                                    && p.SoThanhCK != input.SoThanhCK
                                    && p.SoCK != input.SoCK
                                    && p.TongSoThanh != input.TongSoThanh
                                    && p.CDai1Thanh != input.CDai1Thanh
                                    && p.TongCDai != input.TongCDai
                                    && p.TrongLuong != input.TrongLuong
                                    && p.TongTrongLuong != input.TongTrongLuong
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
        public async Task<bool> CheckEdit(PKKL_OngNhua_410TKThepTDan input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.PKKL_OngNhua_410TKThepTDan_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                    && p.Id_ChiNhanh != input.Id_ChiNhanh
                                    && p.Id_LoaiCauKien != input.Id_LoaiCauKien
                                    && p.TenCongTacTheoKL != input.TenCongTacTheoKL
                                    && p.TenCongTacTheoDK != input.TenCongTacTheoDK
                                    && p.ViTriLayKL != input.ViTriLayKL
                                    && p.Id_TenLoaiThep != input.Id_TenLoaiThep
                                    && p.TenThanhThepTheoCK != input.TenThanhThepTheoCK
                                    && p.TenThanhThepTheoCD != input.TenThanhThepTheoCD
                                    && p.SoHieuThep != input.SoHieuThep
                                    && p.DKCDay != input.DKCDay
                                    && p.SoThanhCK != input.SoThanhCK
                                    && p.SoCK != input.SoCK
                                    && p.TongSoThanh != input.TongSoThanh
                                    && p.CDai1Thanh != input.CDai1Thanh
                                    && p.TongCDai != input.TongCDai
                                    && p.TrongLuong != input.TrongLuong
                                    && p.TongTrongLuong != input.TongTrongLuong
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
        public async Task<bool> CheckDelete(PKKL_OngNhua_410TKThepTDan input)
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
