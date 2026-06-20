using DucAnh2025.Data;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_QuanLyNhanVienRepository : IQLNV_QuanLyNhanVienRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public QLNV_QuanLyNhanVienRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<QLNV_QuanLyNhanVienModel>> GetByVM(string groupId ,QLNV_QuanLyNhanVienModel input)
        {
            List<QLNV_QuanLyNhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.QLNV_QuanLyNhanViens
                            where p1.IsActive != 100 && p1.GroupId == groupId
                            select p1;

                var result = from p1 in query
                             join nv in context.QLNV_NhanViens on p1.Id_NhanVien equals nv.Id
                             join nhom in context.QLNV_NhomNhanViens on p1.Id_NhomNhanVien equals nhom.Id
                             join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                             where p1.IsActive != 100
                             orderby p1.CreateAt descending
                             select new QLNV_QuanLyNhanVienModel
                             {
                                 Id = p1.Id,

                                 Id_NhomNhanVien = p1.Id_NhomNhanVien,
                                 TenNhom = nhom.TenNhom,
                                 IconName = nhom.IconName,

                                 Id_NhanVien = p1.Id_NhanVien,
                                 TenNhanVien = nv.TenNhanVien,
                                 TaiKhoan = nv.TaiKhoan,
                                 CompanyId = nv.CompanyId,
                                 CompanyName = cn.TenChiNhanh,
                                 GroupId = p1.GroupId,
                                 CreateAt = p1.CreateAt,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
                                 IsStatus = p1.IsStatus
                             };
                if (!string.IsNullOrEmpty(input.CompanyId))
                {
                    result = result.Where(x => x.CompanyId == input.CompanyId);
                }
                if (!string.IsNullOrEmpty(input.GroupId))
                {
                    result = result.Where(x => x.GroupId == input.GroupId);
                }
                if (!string.IsNullOrEmpty(input.Id_NhanVien))
                {
                    result = result.Where(x => x.Id_NhanVien == input.Id_NhanVien);
                }
                if (!string.IsNullOrEmpty(input.Id_NhomNhanVien))
                {
                    result = result.Where(x => x.Id_NhomNhanVien == input.Id_NhomNhanVien);
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
        public async Task<List<QLNV_QuanLyNhanVien>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_QuanLyNhanViens.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<QLNV_QuanLyNhanVien> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_QuanLyNhanViens.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<List<QLNV_QuanLyNhanVien>> GetByIdNhom(string groupId, string idNhom)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_QuanLyNhanViens
                                .Where(x => x.Id_NhomNhanVien == idNhom && x.GroupId == groupId && x.IsActive != 100)
                                .ToListAsync();
        }
        public async Task<QLNV_QuanLyNhanVien> GetByNVbyIdNhom(string groupId, string idNhom,string idNhanVien)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_QuanLyNhanViens.Where(x => x.GroupId == groupId &&
            x.Id_NhomNhanVien == idNhom && x.Id_NhanVien == idNhanVien && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<List<QLNV_QuanLyNhanVienModel>> GetQuanLyNhanVienByNhomAsync(string Id_NhomNhanVien)
        {
            List<QLNV_QuanLyNhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();

                var result = from qlnv in context.QLNV_QuanLyNhanViens
                             join nhom in context.QLNV_NhomNhanViens on qlnv.Id_NhomNhanVien equals nhom.Id
                             join nv in context.QLNV_NhanViens on qlnv.Id_NhanVien equals nv.Id
                             where  qlnv.Id_NhomNhanVien == Id_NhomNhanVien && qlnv.IsActive !=100
                             select new QLNV_QuanLyNhanVienModel
                             {
                                 Id = qlnv.Id,
                                 Id_NhomNhanVien = qlnv.Id_NhomNhanVien,
                                 TenNhom = nhom.TenNhom,
                                 Id_NhanVien = qlnv.Id_NhanVien,
                                 TenNhanVien = nv.TenNhanVien,
                                 TaiKhoan = nv.TaiKhoan,
                                 CreateAt = qlnv.CreateAt,
                                 CreateBy = qlnv.CreateBy,
                                 IsActive = qlnv.IsActive
                             };

                data = await result.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return data;
            }
        }
        public async Task<bool> CheckExist(string id, QLNV_QuanLyNhanVien input)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_QuanLyNhanViens
                .AnyAsync(x => x.Id != id &&
                                x.GroupId == input.GroupId && x.IsActive != 100 &&
                               x.Id_NhomNhanVien == input.Id_NhomNhanVien &&
                               x.Id_NhanVien == input.Id_NhanVien 
                               );
        }
        public async Task<bool> IsIdInUse(string id)
        {
            using var context = _context.CreateDbContext();
            var item = await GetById(id);
            if(item.IsActive == 3) {
                bool isInUse = await context.QLNV_QuanLyNhanViens
                .Where(ql =>
                    ql.Id_NhanVien == item.Id_NhanVien &&
                    ql.IsActive != 100
                )
                .AnyAsync(ql =>
                    context.QLNV_NhanVienThucHiens
                        .Join(context.QLNV_CongViecs,
                              th => th.Id_CongViec,
                              cv => cv.Id,
                              (th, cv) => new { th, cv })
                        .Any(x =>
                            x.th.Id_NhanVien == ql.Id_NhanVien &&
                            x.cv.NhomCongViec == ql.Id_NhomNhanVien
                        )
                );

                return isInUse;
            }
            return false;
        }
        public async Task Insert(QLNV_QuanLyNhanVien entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_QuanLyNhanViens.Add(entity);
                var addLog = new QLNV_QuanLyNhanVien_Log()
                {
                    Id = entity.Id,
                    Id_NhanVien = entity.Id_NhanVien,
                    Id_NhomNhanVien = entity.Id_NhomNhanVien,

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
                context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(QLNV_QuanLyNhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy nhóm đã chọn");
            }
            context.QLNV_QuanLyNhanViens.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_QuanLyNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_QuanLyNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.QLNV_QuanLyNhanVien_Logs.Update(updateLog);
                }
            }
            var addLog = new QLNV_QuanLyNhanVien_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_NhanVien = entity.Id_NhanVien,
                Id_NhomNhanVien = entity.Id_NhomNhanVien,

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
            context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(QLNV_QuanLyNhanVien[] QLNV_QuanLyNhanViens)
        {
            using var context = _context.CreateDbContext();
            string[] ids = QLNV_QuanLyNhanViens.Select(x => x.Id).ToArray();
            var listEntities = await context.QLNV_QuanLyNhanViens.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.QLNV_QuanLyNhanViens.Update(entity);
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
                    var logdata = (from p in context.QLNV_QuanLyNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_NhanVien = logdata.Id_NhanVien;
                        data.Id_NhomNhanVien = logdata.Id_NhomNhanVien;
                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.QLNV_QuanLyNhanVien_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.QLNV_QuanLyNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_QuanLyNhanVien_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new QLNV_QuanLyNhanVien_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_NhanVien = data.Id_NhanVien,
                        Id_NhomNhanVien = data.Id_NhomNhanVien,

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
                    context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.QLNV_QuanLyNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_QuanLyNhanVien_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new QLNV_QuanLyNhanVien_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_NhanVien = data.Id_NhanVien,
                        Id_NhomNhanVien = data.Id_NhomNhanVien,

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
                    context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.QLNV_QuanLyNhanViens.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task DeleteByIdNhom(string groupId, string idNhom, string userId)
        {
            using var context = _context.CreateDbContext();
            var entities = await GetByIdNhom(groupId, idNhom);

            if (entities == null || !entities.Any())
            {
                throw new Exception($"Không tìm thấy Id nhóm !");
            }

            context.QLNV_QuanLyNhanViens.RemoveRange(entities);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.QLNV_QuanLyNhanViens.Where(p => p.Id == ids).FirstOrDefaultAsync();
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
    
        public async Task<bool> CheckExclusiveNVbyNhom(string groupId,string id_Nhom,string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetByNVbyIdNhom(groupId, id_Nhom, id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy nhân viên theo nhóm!");
                }
            }
            return true;
        }
        public async Task Approval(QLNV_QuanLyNhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy nhân viên đã chọn");
            }
            context.QLNV_QuanLyNhanViens.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_QuanLyNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_QuanLyNhanVien_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.QLNV_QuanLyNhanVien_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.QLNV_QuanLyNhanVien_Logs.Update(updateLog);
                }
            }

            var addLog = new QLNV_QuanLyNhanVien_Log
            {
                Id_NhanVien = data.Id_NhanVien,
                Id_NhomNhanVien = data.Id_NhomNhanVien,

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
            context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(QLNV_QuanLyNhanVien data, string userId)
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
                    var logdata = (from p in context.QLNV_QuanLyNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_NhanVien = logdata.Id_NhanVien;
                        entity.Id_NhomNhanVien = logdata.Id_NhomNhanVien;

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

                    var logupdate = (from p in context.QLNV_QuanLyNhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_QuanLyNhanVien_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.QLNV_QuanLyNhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_NhanVien = logdata.Id_NhanVien;
                        entity.Id_NhomNhanVien = logdata.Id_NhomNhanVien;
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
            var addLog = new QLNV_QuanLyNhanVien_Log()
            {
                Id = Guid.NewGuid().ToString(),
                Id_NhanVien = data.Id_NhanVien,
                Id_NhomNhanVien = data.Id_NhomNhanVien,
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
            context.QLNV_QuanLyNhanVien_Logs.Add(addLog);
            context.QLNV_QuanLyNhanViens.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
