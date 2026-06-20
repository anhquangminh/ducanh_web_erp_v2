using DucAnh2025.Data;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_NhanVienRepository : IQLNV_NhanVienRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public QLNV_NhanVienRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<QLNV_NhanVienModel>> GetByVM(string groupId, QLNV_NhanVienModel input)
        {
            List<QLNV_NhanVienModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.QLNV_NhanViens
                            where p1.IsActive != 100 && p1.GroupId == groupId
                            select p1;

                var result = from p1 in query
                             join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                             join dept in context.Departments on p1.DepartmentId equals dept.Id
                             join cv in context.DM_ChucVus on p1.ChucVuId equals cv.Id
                             join cm in context.DM_ChuyenMons on p1.ChuyenMonId equals cm.Id
                             orderby p1.CreateAt descending
                             select new QLNV_NhanVienModel
                             {
                                 Id = p1.Id,
                                 TenNhanVien = p1.TenNhanVien,
                                 TaiKhoan = p1.TaiKhoan,
                                 GroupId = p1.GroupId,
                                 CompanyId = p1.CompanyId,
                                 CompanyName = cn.TenChiNhanh,
                                 DepartmentId = p1.DepartmentId,
                                 DepartmentName = dept.DeptName,
                                 ChucVuId = p1.ChucVuId,
                                 ChucVu = cv.ChucVu,
                                 ChuyenMonId = p1.ChuyenMonId,
                                 ChuyenMon = cm.ChuyenMon,
                                 CreateAt = p1.CreateAt,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
                                 IsStatus =p1.IsStatus
                             };

                if (!string.IsNullOrEmpty(input.GroupId))
                {
                    result = result.Where(x => x.GroupId == input.GroupId);
                }
                if (!string.IsNullOrEmpty(input.CompanyId))
                {
                    result = result.Where(x => x.CompanyId == input.CompanyId);
                }
                if (!string.IsNullOrEmpty(input.DepartmentId))
                {
                    result = result.Where(x => x.DepartmentId == input.DepartmentId);
                }
                if (!string.IsNullOrEmpty(input.ChucVuId))
                {
                    result = result.Where(x => x.ChucVuId == input.ChucVuId);
                }
                if (!string.IsNullOrEmpty(input.ChuyenMonId))
                {
                    result = result.Where(x => x.ChuyenMonId == input.ChuyenMonId);
                }

                if (!string.IsNullOrEmpty(input.CompanyId))
                {
                    result = result.Where(x => x.CompanyId == input.CompanyId);
                }
                if (!string.IsNullOrEmpty(input.TenNhanVien))
                {
                    var ten = input.TenNhanVien.ToUpper().Trim();
                    result = result.Where(x => x.TenNhanVien.ToUpper().Contains(ten));
                }

                if (!string.IsNullOrEmpty(input.TaiKhoan))
                {
                    result = result.Where(x => x.TaiKhoan == input.TaiKhoan);
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
        public async Task<List<QLNV_NhanVien>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_NhanViens.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<QLNV_NhanVien>> GetNhanVienIsQuanLy(string groupId, bool isQuanLy)
        {
            List<QLNV_NhanVien> data = new();
            try
            {
                using var context = _context.CreateDbContext();

                var result = from p1 in context.QLNV_NhanViens
                             join nv in context.QLNV_NhomNhanViens on p1.Id equals nv.Id_QuanLy into gj
                             from subnv in gj.DefaultIfEmpty()
                             where p1.IsActive != 100 &&
                                   ((isQuanLy == true && subnv != null) || (isQuanLy == false && subnv == null))
                                   && p1.GroupId == groupId 
                             orderby p1.CreateAt descending
                             select new QLNV_NhanVien
                             {
                                 Id = p1.Id,
                                 TenNhanVien = p1.TenNhanVien,
                                 TaiKhoan = p1.TaiKhoan,
                                 CreateAt = p1.CreateAt,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
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
        public async Task<List<QLNV_NhanVien>> GetNhanVienByNhom(string groupId,string companyId, string Id_NhomNhanVien)
        {
            List<QLNV_NhanVien> data = new();
            try
            {
                using var context = _context.CreateDbContext();

                var result = from a in context.QLNV_QuanLyNhanViens
                             join b in context.QLNV_NhanViens on a.Id_NhanVien equals b.Id
                             join n in context.QLNV_NhomNhanViens on a.Id_NhomNhanVien equals n.Id
                             where a.Id_NhomNhanVien == Id_NhomNhanVien && a.GroupId == groupId && a.CompanyId ==companyId
                             && b.IsActive != 100 && a.IsActive !=100
                             select new QLNV_NhanVien
                             {
                                 Id = a.Id_NhanVien,
                                 TenNhanVien = b.TenNhanVien,
                                 TaiKhoan = b.TaiKhoan
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
        public async Task<List<QLNV_NhanVien>> GetNhanVienNotQL(string groupId, string companyId, string Id_NhomNhanVien)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var result = await context.QLNV_NhanViens
                    .Where(nv => nv.GroupId == groupId && nv.CompanyId == companyId)  // Lọc theo groupId
                    .Where(nv => !context.QLNV_NhomNhanViens
                        .Where(nnv => nnv.Id == Id_NhomNhanVien)
                        .Select(nnv => nnv.Id_QuanLy)
                        .Contains(nv.Id))
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_NhanVien>();
            }
        }
        public async Task<QLNV_NhanVien> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_NhanViens.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<List<string>> GetByIdApplicationUser(string[] ids)
        {
            using var context = _context.CreateDbContext();

            var result = await (from a in context.QLNV_NhanViens
                                join b in context.ApplicationUsers on a.TaiKhoan equals b.UserName
                                where ids.Contains(a.Id) && a.IsActive != 100
                                select b.Id).ToListAsync();

            return result;
        }
        public async Task<QLNV_NhanVien> GetNhanVienByTaiKhoan(string taiKhoan)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_NhanViens.Where(x => x.TaiKhoan.Equals(taiKhoan) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy tài khoản ");
            }
            return entity;
        }
        public async Task<bool> CheckExist(string id, QLNV_NhanVien input)
        {
            using var context = _context.CreateDbContext();

            if (string.IsNullOrEmpty(id))
            {
                bool a = await context.QLNV_NhanViens
                    .AnyAsync(x => x.TaiKhoan.ToLower() == input.TaiKhoan.ToLower() && x.GroupId == input.GroupId && x.IsActive != 100 && x.CompanyId == input.CompanyId);
                return a;
            }
            return await context.QLNV_NhanViens
                .AnyAsync(x => x.Id != id &&
                x.GroupId == input.GroupId && x.IsActive != 100 && x.CompanyId == input.CompanyId &&
                               x.TaiKhoan.ToLower() == input.TaiKhoan.ToLower());
        }
        public async Task<bool> IsIdInUse(string id)
        {
            using var context = _context.CreateDbContext();

            bool isInUse = await context.QLNV_NhanVienThucHiens.AnyAsync(x => x.Id_NhanVien == id && x.IsActive != 100) ||
                           await context.QLNV_NhomNhanViens.AnyAsync(x => x.Id_QuanLy == id && x.IsActive != 100);

            return isInUse;
        }
        public async Task Insert(QLNV_NhanVien entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_NhanViens.Add(entity);
                var addLog = new QLNV_NhanVien_Log
                {
                    TenNhanVien = entity.TenNhanVien,
                    TaiKhoan = entity.TaiKhoan,
                    CompanyId = entity.CompanyId,
                    GroupId = entity.GroupId,
                    DepartmentId = entity.DepartmentId,
                    ChucVuId = entity.ChucVuId,
                    ChuyenMonId = entity.ChuyenMonId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = entity.CreateAt,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = entity.DateApproval,
                    ApprovalDept = entity.ApprovalDept,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,
                    IsValid = true
                };

                context.QLNV_NhanVien_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(QLNV_NhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.QLNV_NhanViens.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(QLNV_NhanVien[] QLNV_NhanViens)
        {
            using var context = _context.CreateDbContext();
            string[] ids = QLNV_NhanViens.Select(x => x.Id).ToArray();
            var listEntities = await context.QLNV_NhanViens.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.QLNV_NhanViens.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.Set<QLNV_NhanVien>().Remove(entity);
            await context.SaveChangesAsync();

        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.QLNV_NhanViens.Where(p => p.Id == ids).FirstOrDefaultAsync();
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

        // new
        public async Task<QLNV_NhanVienModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.QLNV_NhanViens
                              join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                              join dept in context.Departments on p1.DepartmentId equals dept.Id
                              join cv in context.DM_ChucVus on p1.ChucVuId equals cv.Id
                              join cm in context.DM_ChuyenMons on p1.ChuyenMonId equals cm.Id
                              where p1.Id == id
                              select new QLNV_NhanVienModel
                              {
                                  Id = p1.Id,
                                  TenNhanVien = p1.TenNhanVien,
                                  TaiKhoan = p1.TaiKhoan,
                                  GroupId = p1.GroupId,
                                  CompanyId = p1.CompanyId,
                                  CompanyName = cn.TenChiNhanh,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentName = dept.DeptName,
                                  ChucVuId = p1.ChucVuId,
                                  ChucVu = cv.ChucVu,
                                  ChuyenMonId = p1.ChuyenMonId,
                                  ChuyenMon = cm.ChuyenMon,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  IsStatus = p1.IsStatus
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<QLNV_NhanVienModel>> GetHistory(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.QLNV_NhanVien_Logs
                              join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                              join dept in context.Departments on p1.DepartmentId equals dept.Id
                              join cv in context.DM_ChucVus on p1.ChucVuId equals cv.Id
                              join cm in context.DM_ChuyenMons on p1.ChuyenMonId equals cm.Id
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id into cb1
                              from createdBy in cb1.DefaultIfEmpty()
                              join approvalUser in context.ApplicationUsers on p1.ApprovalUserId equals approvalUser.Id into au1
                              from approvalUserId in au1.DefaultIfEmpty()
                              join approvalDept in context.Departments on p1.ApprovalDept equals approvalDept.Id into ad1
                              from approvalDeptObj in ad1.DefaultIfEmpty()
                              where p1.IdChung == id
                              orderby p1.CreateAt
                              select new QLNV_NhanVienModel
                              {
                                  Id = p1.Id,
                                  TenNhanVien = p1.TenNhanVien,
                                  TaiKhoan = p1.TaiKhoan,
                                  GroupId = p1.GroupId,
                                  CompanyId = p1.CompanyId,
                                  CompanyName = cn.TenChiNhanh,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentName = dept.DeptName,
                                  ChucVuId = p1.ChucVuId,
                                  ChucVu = cv.ChucVu,
                                  ChuyenMonId = p1.ChuyenMonId,
                                  ChuyenMon = cm.ChuyenMon,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = createdBy != null ? createdBy.Email : "",
                                  IsActive = p1.IsActive,
                                  IsStatus = p1.IsStatus,
                                  DateApproval = p1.DateApproval,
                                  ApprovalUserId = approvalUserId != null ? approvalUserId.Email : "",
                                  ApprovalDept = approvalDeptObj != null ? approvalDeptObj.DeptName : "",
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId??"",
                                  LastApprovalId = p1.LastApprovalId ?? ""
                              }).ToListAsync();
            return data;
        }
        public QLNV_NhanVienModel GetToEdit(string id)
        {
            using var context = _context.CreateDbContext();
            var data = (from p1 in context.QLNV_NhanViens
                        join cn in context.ChiNhanhs on p1.CompanyId equals cn.Id
                        join dept in context.Departments on p1.DepartmentId equals dept.Id
                        join cv in context.DM_ChucVus on p1.ChucVuId equals cv.Id
                        join cm in context.DM_ChuyenMons on p1.ChuyenMonId equals cm.Id
                        where p1.Id == id && p1.IsActive != 100
                        select new QLNV_NhanVienModel
                        {
                            Id = p1.Id,
                            TenNhanVien = p1.TenNhanVien,
                            TaiKhoan = p1.TaiKhoan,
                            GroupId = p1.GroupId,
                            CompanyId = p1.CompanyId,
                            CompanyName = cn.TenChiNhanh,
                            DepartmentId = p1.DepartmentId,
                            DepartmentName = dept.DeptName,
                            ChucVuId = p1.ChucVuId,
                            ChucVu = cv.ChucVu,
                            ChuyenMonId = p1.ChuyenMonId,
                            ChuyenMon = cm.ChuyenMon,
                            CreateAt = p1.CreateAt,
                            CreateBy = p1.CreateBy,
                            IsActive = p1.IsActive,
                            IsStatus = p1.IsStatus
                        }).FirstOrDefault();
            return data;
        }
        public async Task<bool> CheckSave(QLNV_NhanVien input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.QLNV_NhanVien_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                   && p.TaiKhoan == input.TaiKhoan
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
        public async Task<bool> CheckEdit(QLNV_NhanVien input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.QLNV_NhanVien_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                   && p.TaiKhoan == input.TaiKhoan
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
        public async Task<bool> CheckDelete(QLNV_NhanVien input)
        {
           return true;

        }
        public async Task Approval(QLNV_NhanVien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy nhân viên đã chọn");
            }
            context.QLNV_NhanViens.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.QLNV_NhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhanVien_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.QLNV_NhanVien_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.QLNV_NhanVien_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.QLNV_NhanVien_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.QLNV_NhanVien_Logs.Update(updateLog);
                }
            }
           
            var addLog = new QLNV_NhanVien_Log
            {
                TenNhanVien = data.TenNhanVien,
                TaiKhoan = data.TaiKhoan,
                CompanyId = data.CompanyId,
                GroupId = data.GroupId,
                DepartmentId = data.DepartmentId,
                ChucVuId = data.ChucVuId,
                ChuyenMonId = data.ChuyenMonId,
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
            context.QLNV_NhanVien_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(QLNV_NhanVien data, string userId)
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
                    var logdata = (from p in context.QLNV_NhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt descending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.TenNhanVien = logdata.TenNhanVien;
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

                    var logupdate = (from p in context.QLNV_NhanVien_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt > logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.QLNV_NhanVien_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.QLNV_NhanVien_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.TenNhanVien = logdata.TenNhanVien;
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
            var addLog = new QLNV_NhanVien_Log()
            {
                Id = Guid.NewGuid().ToString(),
                TenNhanVien = data.TenNhanVien,
                TaiKhoan = data.TaiKhoan,
                CompanyId = data.CompanyId,
                GroupId = data.GroupId,
                DepartmentId = data.DepartmentId,
                ChucVuId = data.ChucVuId,
                ChuyenMonId = data.ChuyenMonId,
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
            context.QLNV_NhanVien_Logs.Add(addLog);
            context.QLNV_NhanViens.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
