using DucAnh2025.Data;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_NhomNhanVienRepository : IQLNV_NhomNhanVienRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public QLNV_NhomNhanVienRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<QLNV_NhomNhanVienModel>> GetByVM(string groupId, QLNV_NhomNhanVienModel input)
        {
            List<QLNV_NhomNhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.QLNV_NhomNhanViens
                            where p1.IsActive != 100 && p1.GroupId == groupId
                            select p1;

                var result = from p1 in query
                             join nv in context.QLNV_NhanViens on p1.Id_QuanLy equals nv.Id
                             join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                             where p1.IsActive != 100
                             orderby p1.CreateAt descending
                             select new QLNV_NhomNhanVienModel
                             {
                                 Id = p1.Id,
                                 Id_QuanLy = p1.Id_QuanLy,
                                 TenNhanVien = nv.TenNhanVien,
                                 TaiKhoan = nv.TaiKhoan,
                                 TenNhom = p1.TenNhom,
                                 IconName = p1.IconName,
                                 CompanyId = p1.CompanyId,
                                 CompanyName = cn.TenChiNhanh,
                                 GroupId = p1.GroupId,
                                 CreateAt = p1.CreateAt,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
                                 IsStatus = p1.IsStatus
                             };

                if (!string.IsNullOrEmpty(input.GroupId))
                {
                    result = result.Where(x => x.GroupId == input.GroupId);
                }
                if (!string.IsNullOrEmpty(input.CompanyId))
                {
                    result = result.Where(x => x.CompanyId == input.CompanyId);
                }
                if (!string.IsNullOrEmpty(input.TenNhanVien))
                {
                    result = result.Where(x => x.TenNhanVien == input.TenNhanVien);
                }
                if (!string.IsNullOrEmpty(input.TaiKhoan))
                {
                    result = result.Where(x => x.TaiKhoan == input.TaiKhoan);
                }
                if (!string.IsNullOrEmpty(input.TenNhom))
                {
                    var ten = input.TenNhom.ToUpper().Trim();
                    result = result.Where(x => x.TenNhom.ToUpper().Contains(ten));
                }

                data = await result.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return data;
            }
        }
        public async Task<List<QLNV_NhomNhanVien>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_NhomNhanViens.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<QLNV_NhomNhanVien> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_NhomNhanViens.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<List<QLNV_NhomNhanVienModel>> GetNhomNhanVienByTaiKhoanAsync(string groupId, string CompanyId, string taiKhoan)
        {
            List<QLNV_NhomNhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();

                var result = from nnv in context.QLNV_NhomNhanViens
                             join a in context.QLNV_NhanViens on nnv.Id_QuanLy equals a.Id
                             join cn in context.ChiNhanhs on nnv.CompanyId equals cn.Id
                             where a.TaiKhoan.ToUpper().Trim() == taiKhoan.ToUpper().Trim()
                                   && a.IsActive != 100
                                   && nnv.IsActive != 100
                                   && nnv.GroupId == groupId
                             orderby nnv.CreateAt ascending
                             select new QLNV_NhomNhanVienModel
                             {
                                 Id = nnv.Id,
                                 Id_QuanLy = nnv.Id_QuanLy,
                                 GroupId = nnv.GroupId,
                                 CompanyId = nnv.CompanyId,
                                 CompanyName = cn.TenChiNhanh,
                                 TenNhanVien = a.TenNhanVien,
                                 TaiKhoan = a.TaiKhoan,
                                 TenNhom = nnv.TenNhom,
                                 IconName = nnv.IconName,
                                 Total = context.QLNV_CongViecs
                                     .Where(cv => cv.NhomCongViec == nnv.Id)
                                     .Select(cv => cv.Id)
                                     .Distinct()
                                     .Count()
                             };

                if (!string.IsNullOrEmpty(CompanyId))
                {
                    result = result.Where(x => x.CompanyId == CompanyId);
                }
                data = await result.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return data;
            }
        }
        public async Task<List<QLNV_NhomNhanVienModel>> GetNhomNhanVienByCVDGAsync(string groupId, string taiKhoan)
        {
            List<QLNV_NhomNhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();

                var result = await (from nv in context.QLNV_NhanViens
                                    join qlnv in context.QLNV_QuanLyNhanViens on nv.Id equals qlnv.Id_NhanVien
                                    join nnv in context.QLNV_NhomNhanViens on qlnv.Id_NhomNhanVien equals nnv.Id
                                    where nv.TaiKhoan == taiKhoan && nv.GroupId == groupId && nnv.GroupId == groupId && nnv.IsActive != 100
                                    select new QLNV_NhomNhanVienModel
                                    {
                                        Id = nnv.Id,
                                        Id_QuanLy = qlnv.Id_NhanVien,
                                        TenNhom = nnv.TenNhom,
                                        IconName = nnv.IconName,
                                        CreateAt = nnv.CreateAt,
                                        Total = context.QLNV_CongViecs.Join(context.QLNV_NhanVienThucHiens, cv => cv.Id, nvth => nvth.Id_CongViec, (cv, nvth) => new { cv, nvth })
                                            .Join(context.QLNV_NhanViens, combined => combined.nvth.Id_NhanVien, nv => nv.Id, (combined, nv) => new { combined.cv, nv })
                                            .Where(x => x.nv.TaiKhoan == taiKhoan && x.cv.NhomCongViec == nnv.Id)
                                            .Select(x => x.cv.Id).Count()

                                    })
                                    .OrderBy(x => x.CreateAt)
                                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return data;
            }
        }
        public async Task<bool> CheckExist(string id, QLNV_NhomNhanVien input)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_NhomNhanViens
                .AnyAsync(x => x.Id != id &&
                x.GroupId == input.GroupId && x.IsActive != 100 &&
                               x.TenNhom == input.TenNhom
                               );
        }
        public async Task<bool> IsIdInUse(string id)
        {
            using var context = _context.CreateDbContext();

            bool isInUse = await context.QLNV_QuanLyNhanViens.AnyAsync(x => x.Id_NhomNhanVien == id && x.IsActive !=100) ||
                           await context.QLNV_CongViecs.AnyAsync(x => x.NhomCongViec == id && x.IsActive != 100);

            return isInUse;
        }
        public async Task Insert(QLNV_NhomNhanVien entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_NhomNhanViens.Add(entity);
                var addLog = new QLNV_NhomNhanVien_Log()
                {
                    Id = entity.Id,
                    Id_QuanLy =entity.Id_QuanLy,
                    TenNhom =entity.TenNhom,
                    IconName = entity.IconName,
                    GroupId = entity.GroupId,
                    CompanyId = entity.CompanyId,

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
                context.QLNV_NhomNhanVien_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(QLNV_NhomNhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy nhóm đã chọn");
            }
            context.QLNV_NhomNhanViens.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhomNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhomNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.QLNV_NhomNhanVien_Logs.Update(updateLog);
                }
            }
            var addLog = new QLNV_NhomNhanVien_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_QuanLy = entity.Id_QuanLy,
                TenNhom = entity.TenNhom,
                IconName = entity.IconName,

                GroupId = data.GroupId,
                CompanyId = data.CompanyId,
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
            context.QLNV_NhomNhanVien_Logs.Add(addLog);
            await context.SaveChangesAsync();

        }
        public async Task UpdateMulti(QLNV_NhomNhanVien[] QLNV_NhomNhanViens)
        {
            using var context = _context.CreateDbContext();
            string[] ids = QLNV_NhomNhanViens.Select(x => x.Id).ToArray();
            var listEntities = await context.QLNV_NhomNhanViens.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.QLNV_NhomNhanViens.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            var data = entity;
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy loại chi nhánh đã chọn");
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
                    var logdata = (from p in context.QLNV_NhomNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_QuanLy = logdata.Id_QuanLy;
                        data.TenNhom = logdata.TenNhom;
                        data.IconName = logdata.IconName;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.QLNV_NhomNhanVien_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.QLNV_NhomNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_NhomNhanVien_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new QLNV_NhomNhanVien_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_QuanLy = data.Id_QuanLy,
                        TenNhom = data.TenNhom,
                        IconName = data.IconName,

                        GroupId = data.GroupId,
                        CompanyId = data.CompanyId,
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
                    context.QLNV_NhomNhanVien_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.QLNV_NhomNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_NhomNhanVien_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new QLNV_NhomNhanVien_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_QuanLy = data.Id_QuanLy,
                        TenNhom = data.TenNhom,
                        IconName = data.IconName,

                        GroupId = data.GroupId,
                        CompanyId = data.CompanyId,
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
                    context.QLNV_NhomNhanVien_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.QLNV_NhomNhanViens.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.QLNV_NhomNhanViens.Where(p => p.Id == ids).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Đang chờ duyệt xóa.");
            }

            return true;
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy Id !");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }

        public async Task Approval(QLNV_NhomNhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy nhân viên đã chọn");
            }
            context.QLNV_NhomNhanViens.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhomNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhomNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.QLNV_NhomNhanVien_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.QLNV_NhomNhanVien_Logs.Update(updateLog);
                }
            }

            var addLog = new QLNV_NhomNhanVien_Log
            {
                Id_QuanLy = data.Id_QuanLy,
                TenNhom = data.TenNhom,
                IconName = data.IconName,
                CompanyId = data.CompanyId,
                GroupId = data.GroupId,
                
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = data.ApprovalUserId,
                DateApproval = data.DateApproval,
                ApprovalDept = data.ApprovalDept,
                DepartmentOrder = data.DepartmentOrder,
                ApprovalOrder = data.ApprovalOrder,
                ApprovalId = data.ApprovalId,
                LastApprovalId = data.LastApprovalId,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.QLNV_NhomNhanVien_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(QLNV_NhomNhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy nhân viên đã chọn");
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
                    var logdata = (from p in context.QLNV_NhomNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_QuanLy = logdata.Id_QuanLy;
                        entity.TenNhom = logdata.TenNhom;
                        entity.IconName = logdata.IconName;
                        entity.CompanyId = logdata.CompanyId;
                        entity.GroupId = logdata.GroupId;

                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = (DateTime)(DateTime?)logdata.CreateAt;
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

                    var logupdate = (from p in context.QLNV_NhomNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_NhomNhanVien_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.QLNV_NhomNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_QuanLy = logdata.Id_QuanLy;
                        entity.TenNhom = logdata.TenNhom;
                        entity.IconName = logdata.IconName;
                        entity.CompanyId = logdata.CompanyId;
                        entity.GroupId = logdata.GroupId;

                        entity.GroupId = logdata.GroupId;
                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = (DateTime)(DateTime?)data.CreateAt;
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
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại.");
                }
            }
            var addLog = new QLNV_NhomNhanVien_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_QuanLy = data.Id_QuanLy,
                TenNhom = data.TenNhom,
                IconName = data.IconName,
                CompanyId = data.CompanyId,
                GroupId = data.GroupId,
               
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = userId,
                DateApproval = data.DateApproval,
                ApprovalDept = data.ApprovalDept,
                DepartmentOrder = 0,
                ApprovalOrder = 0,
                ApprovalId = null,
                LastApprovalId = null,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = false
            };
            context.QLNV_NhomNhanVien_Logs.Add(addLog);
            context.QLNV_NhomNhanViens.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
