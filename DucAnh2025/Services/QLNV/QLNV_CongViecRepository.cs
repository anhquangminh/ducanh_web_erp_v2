
using Microsoft.EntityFrameworkCore;
using DucAnh2025.Data;
using DucAnh2025.ViewModel.QLNV;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;


namespace DucAnh2025.Services.QLNV
{
    public class QLNV_CongViecRepository : IQLNV_CongViecRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public QLNV_CongViecRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<QLNV_CongViecModel>> GetByVM(string groupId, QLNV_CongViecModel input)
        {
            try
            {
                using var context = _context.CreateDbContext();

               
                var query = context.QLNV_CongViecs
                    .AsNoTracking()
                    .Where(p1 => p1.IsActive != 100 && p1.GroupId == groupId);

                if (!string.IsNullOrEmpty(input.Id))
                    query = query.Where(x => x.Id == input.Id);
                if (!string.IsNullOrEmpty(input.CompanyId))
                    query = query.Where(x => x.CompanyId == input.CompanyId);
                if (!string.IsNullOrEmpty(input.GroupId))
                    query = query.Where(x => x.GroupId == input.GroupId);
                if (!string.IsNullOrEmpty(input.Id_NguoiGiaoViec))
                    query = query.Where(x => x.Id_NguoiGiaoViec == input.Id_NguoiGiaoViec);
                if (!string.IsNullOrEmpty(input.NhomCongViec))
                    query = query.Where(x => x.NhomCongViec == input.NhomCongViec);
                if (!string.IsNullOrEmpty(input.MucDoUuTien))
                    query = query.Where(x => x.MucDoUuTien == input.MucDoUuTien);
                if (!string.IsNullOrEmpty(input.TenCongViec))
                    query = query.Where(x => x.TenCongViec == input.TenCongViec);
                if (!string.IsNullOrEmpty(input.NoiDungCongViec))
                    query = query.Where(x => x.NoiDungCongViec == input.NoiDungCongViec);

                var result = await (
                    from p1 in query
                    join nhom in context.QLNV_NhomNhanViens.AsNoTracking() on p1.NhomCongViec equals nhom.Id
                    join cn in context.ChiNhanhs.AsNoTracking() on p1.CompanyId equals cn.Id
                    join dg in context.QLNV_DanhGias.AsNoTracking() on p1.Id equals dg.Id_CongViec into danhgias
                    from danhgia in danhgias.DefaultIfEmpty()
                    select new QLNV_CongViecModel
                    {
                        Id = p1.Id,
                        Id_NguoiGiaoViec = p1.Id_NguoiGiaoViec,
                        NhomCongViec = p1.NhomCongViec,
                        TenNhom = nhom.TenNhom,
                        NgayBatDau = p1.NgayBatDau,
                        NgayKetThuc = p1.NgayKetThuc,
                        MucDoUuTien = p1.MucDoUuTien ?? "",
                        TienDo = p1.TienDo,
                        TuDanhGia = p1.TuDanhGia ?? "",
                        DuocDanhGia = danhgia != null ? danhgia.DanhGia : 0,
                        LapLai = p1.LapLai,
                        TenCongViec = p1.TenCongViec,
                        NoiDungCongViec = p1.NoiDungCongViec,
                        FileDinhKem = p1.FileDinhKem ?? "",
                        CompanyId = p1.CompanyId,
                        CompanyName = cn.TenChiNhanh,
                        GroupId = p1.GroupId,
                        CreateAt = p1.CreateAt,
                        CreateBy = p1.CreateBy,
                        IsActive = p1.IsActive,
                        
                        NguoiThucHien = string.Join(", ",
                            (from nvth in context.QLNV_NhanVienThucHiens.AsNoTracking()
                             join nv in context.QLNV_NhanViens.AsNoTracking() on nvth.Id_NhanVien equals nv.Id
                             where nvth.Id_CongViec == p1.Id
                             select nv.TenNhanVien + " (" + nv.TaiKhoan + ")"))
                    }
                )
                .OrderBy(x => x.NgayBatDau)
                .ToListAsync();

                // Lọc NguoiThucHien phía server nếu có
                if (!string.IsNullOrEmpty(input.NguoiThucHien))
                {
                    result = result
                        .Where(x => x.NguoiThucHien != null && x.NguoiThucHien.Contains(input.NguoiThucHien))
                        .ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<QLNV_CongViecModel>();
            }
        }


        public async Task<List<QLNV_CongViec>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_CongViecs.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<QLNV_CongViec> GetById(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_CongViecs.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> CheckExist(string id, QLNV_CongViec input)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_CongViecs
                .AnyAsync(x => x.Id != input.Id &&
                x.GroupId == input.GroupId &&
                               x.Id_NguoiGiaoViec == input.Id_NguoiGiaoViec &&
                               //x.Id_NguoiThucHien == input.Id_NguoiThucHien &&
                               x.NhomCongViec == input.NhomCongViec &&
                               x.NgayBatDau == input.NgayBatDau &&
                               x.NgayKetThuc == input.NgayKetThuc &&
                               x.MucDoUuTien == input.MucDoUuTien &&
                               x.TienDo == input.TienDo &&
                               x.TuDanhGia == input.TuDanhGia && 
                               x.LapLai == input.LapLai && 
                               x.TenCongViec == input.TenCongViec && 
                               x.NoiDungCongViec == input.NoiDungCongViec && 
                               x.FileDinhKem == input.FileDinhKem  
                               );
        }
        public async Task<bool> IsIdInUse(string id)
        {
            using var context = _context.CreateDbContext();
            // Kiểm tra xem Id có đang được sử dụng trong bảng khác hay không
            // Ví dụ: kiểm tra trong bảng `SomeOtherTable`
            //bool isInUse = await context.SomeOtherTable.AnyAsync(x => x.QDBoiThuongGocId == id);
            return false;
        }
        public async Task Insert(QLNV_CongViec entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_CongViecs.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(QLNV_CongViec data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.QLNV_CongViecs.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(QLNV_CongViec[] QLNV_CongViecs)
        {
            using var context = _context.CreateDbContext();
            string[] ids = QLNV_CongViecs.Select(x => x.Id).ToArray();
            var listEntities = await context.QLNV_CongViecs.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.QLNV_CongViecs.Update(entity);
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
            context.Set<QLNV_CongViec>().Remove(entity);
            //var entity_nvths = context.Set<QLNV_NhanVienThucHien>().FirstOrDefault(x => x.Id_CongViec == id);

            //if (entity != null)
            //{
            //    context.Set<QLNV_NhanVienThucHien>().Remove(entity_nvths);
            //    context.SaveChanges();
            //}

            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.QLNV_CongViecs.Where(p => p.Id == ids).FirstOrDefaultAsync();
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

        //Công việc con
        public async Task InsertCVC(QLNV_CongViecCon entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_CongViecCons.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<QLNV_CongViecCon>> GetAllCVC()
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_CongViecCons.Where(p => p.IsActive != 100).OrderBy(p =>p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_CongViecCon>();
            }
        }
        public async Task<QLNV_CongViecCon> GetByIdCVC(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_CongViecCons.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new QLNV_CongViecCon();
            }
        }
        public async Task<List<QLNV_CongViecCon>> GetByIdCongViecCVC(string Id_CongViec)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_CongViecCons.Where(x => x.Id_CongViec.Equals(Id_CongViec) && x.IsActive != 100).ToListAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id CongViec");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_CongViecCon>();
            }
        }
        public async Task DeleteByIdCVC(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetByIdCVC(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.Set<QLNV_CongViecCon>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteByIdCongViecCVC(string Id_Task, string userId)
        {
            using var context = _context.CreateDbContext();

            // Lấy tất cả các bản ghi có Id_Task giống nhau
            var entities = await GetByIdCongViecCVC(Id_Task);

            if (entities == null || entities.Count == 0)
            {
                return;
            }
            // Xóa tất cả các bản ghi đã tìm thấy
            context.Set<QLNV_CongViecCon>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }
        public async Task UpdateCVC(QLNV_CongViecCon data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetByIdCVC(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.QLNV_CongViecCons.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckExistCVC(string id, QLNV_CongViecCon input)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_CongViecs
                .AnyAsync(x => x.Id != input.Id &&
                               x.NoiDungCongViec == input.NoiDungCongViec &&
                               x.FileDinhKem == input.FileDinhKem
                               );
        }
        public async Task<bool> CheckExclusiveCVC(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetByIdCVC(id);
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

        //Nhân viên thực hiện
        public async Task InsertNVTH(QLNV_NhanVienThucHien entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_NhanVienThucHiens.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<QLNV_NhanVienThucHien>> GetByIdCongViecNVTH(string Id_CongViec)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_NhanVienThucHiens.Where(x => x.Id_CongViec.Equals(Id_CongViec) && x.IsActive != 100).ToListAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id_Task");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_NhanVienThucHien>();
            }
        }
        public async Task DeleteByIdCongViecNVTH(string Id_CongViec, string userId)
        {
            using var context = _context.CreateDbContext();

            // Lấy tất cả các bản ghi có Id_CongViec giống nhau
            var entities = await GetByIdCongViecNVTH(Id_CongViec);

            if (entities == null || entities.Count == 0)
            {
                return;
            }
            // Xóa tất cả các bản ghi đã tìm thấy
            context.Set<QLNV_NhanVienThucHien>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }
        public async Task<QLNV_NhanVienThucHien> GetByIdNVTH(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_NhanVienThucHiens.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new QLNV_NhanVienThucHien();
            }
        }
        public async Task<QLNV_NhanVienThucHien> GetIdNVTHByIdCongViec(string Id_CongViec, string userName)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var entity = await (from nvth in context.QLNV_NhanVienThucHiens
                                    join nv in context.QLNV_NhanViens on nvth.Id_NhanVien equals nv.Id
                                    where nv.TaiKhoan == userName
                                       && nvth.Id_CongViec == Id_CongViec
                                       && nvth.IsActive != 100
                                    select nvth)
                                   .FirstOrDefaultAsync();

                if (entity == null)
                {
                    throw new Exception("Không tìm thấy Id");
                }

                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new QLNV_NhanVienThucHien();
            }
        }
        public async Task UpdateNVTH(QLNV_NhanVienThucHien data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetByIdNVTH(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.QLNV_NhanVienThucHiens.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task<List<QLNV_NhanVienThucHien>> GetAllNVTH(string groupId, QLNV_NhanVienThucHien input)
        {
            List<QLNV_NhanVienThucHien> data = new();
            try
            {
                using var context = _context.CreateDbContext();
                var result = from p1 in context.QLNV_NhanVienThucHiens
                            where p1.IsActive != 100 && p1.GroupId == groupId
                            select p1;

                if (!string.IsNullOrEmpty(input.Id_NhanVien))
                {
                    result = result.Where(x => x.Id_NhanVien == input.Id_NhanVien);
                }
                if (!string.IsNullOrEmpty(input.Id_CongViec))
                {
                    result = result.Where(x => x.Id_CongViec == input.Id_CongViec);
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

        //Thêm ngày
        public async Task<List<QLNV_CongViec>> GetCVByIdNhanVien(string groupId, string[] Id_NhanVien)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var entity = await context.QLNV_NhanVienThucHiens
                    .Where(nvth => Id_NhanVien.Contains(nvth.Id_NhanVien))
                    .Select(nvth => nvth.Id_CongViec)  
                    .Distinct() 
                    .Join(context.QLNV_CongViecs,
                          idCongViec => idCongViec,
                          cv => cv.Id,
                          (idCongViec, cv) => cv)  
                    .Where(cv => cv.IsActive != 100) 
                    .ToListAsync();

                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_CongViec>();
            }
        }
        public async Task InsertThemNgay(QLNV_ThemNgay entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_ThemNgays.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<List<QLNV_ThemNgay>> GetByIdThemNgay(string Id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_ThemNgays.Where(x => x.Id.Equals(Id) && x.IsActive != 100).ToListAsync();
                if (entity == null)
                {
                    throw new Exception($"Không tìm thấy Id");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_ThemNgay>();
            }
        }
        public async Task<QLNV_ThemNgay> GetByIdCV(string Id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_ThemNgays
                    .FirstOrDefaultAsync(x => x.Id_CongViec == Id && x.IsActive != 100);

                return entity ?? throw new Exception("Không tìm thấy công việc với Id đã cho.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new QLNV_ThemNgay(); 
            }
        }

        public async Task DeleteByIdThemNgay(string Id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entities = await GetByIdThemNgay(Id);
            if (entities == null || entities.Count == 0)
            {
                return;
            }
            // Xóa tất cả các bản ghi đã tìm thấy
            context.Set<QLNV_ThemNgay>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }
        public async Task DeleteByIdCVThemNgay(string Id, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entities = await context.QLNV_ThemNgays.Where(x => x.Id_CongViecThemNgay.Equals(Id) && x.IsActive != 100).ToListAsync();
                if (entities == null || entities.Count == 0)
                {
                    return;
                }
                // Xóa tất cả các bản ghi đã tìm thấy
                context.Set<QLNV_ThemNgay>().RemoveRange(entities);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Không tìm thấy Id "+ex.Message);
            }
        }


        //báo cáo theo người giao việc
        public async Task<CongViecStatusReport> GetStatusReport(string groupId, string id_NguoiGiaoViec)
        {
            using var context = _context.CreateDbContext();
            var now = DateTime.Now;
            var query = context.QLNV_CongViecs
                .Where(x => x.IsActive != 100 && x.GroupId == groupId);

            if (!string.IsNullOrEmpty(id_NguoiGiaoViec))
                query = query.Where(x => x.Id_NguoiGiaoViec == id_NguoiGiaoViec);

            var list = await query.ToListAsync();

            return new CongViecStatusReport
            {
                TongSo = list.Count,
                DangThucHien = list.Count(x => x.TienDo > 0 && x.TienDo < 10 && (x.NgayKetThuc == null || x.NgayKetThuc >= now)),
                HoanThanh = list.Count(x => x.TienDo >= 10),
                Cho = list.Count(x => x.TienDo == 0 && x.NgayBatDau > now),
                ChuaLam = list.Count(x => x.TienDo == 0 && x.NgayBatDau <= now),
                QuaHan = list.Count(x => (x.TienDo < 10) && x.NgayKetThuc != null && x.NgayKetThuc < now)
            };
        }
        public async Task<List<CongViecByNhomReport>> GetBaoCaoTheoNhom(string groupId, string id_NguoiGiaoViec)
        {
            using var context = _context.CreateDbContext();

            var query = context.QLNV_CongViecs
                .Where(x => x.IsActive != 100 && x.GroupId == groupId);

            if (!string.IsNullOrEmpty(id_NguoiGiaoViec))
            {
                query = query.Where(x => x.Id_NguoiGiaoViec == id_NguoiGiaoViec);
            }

            var result = await (
                from cv in query
                join nhom in context.QLNV_NhomNhanViens
                    on cv.NhomCongViec equals nhom.Id into nhomJoin
                from nhom in nhomJoin.DefaultIfEmpty()
                group new { cv, nhom } by new
                {
                    cv.NhomCongViec,
                    TenNhom = nhom != null ? nhom.TenNhom : "",
                    IconName = nhom != null ? nhom.IconName : ""
                }
                into g
                select new CongViecByNhomReport
                {
                    NhomCongViec = g.Key.NhomCongViec,
                    TenNhom = g.Key.TenNhom,
                    IconName = g.Key.IconName,
                    SoLuong = g.Count()
                }
            ).ToListAsync();

            return result;
        }
        public async Task<TienDoTrungBinhReport> GetTienDoTrungBinh(string groupId, string id_NguoiGiaoViec)
        {
            using var context = _context.CreateDbContext();
            var query = context.QLNV_CongViecs
                .Where(x => x.IsActive != 100 && x.GroupId == groupId && x.Id_NguoiGiaoViec == id_NguoiGiaoViec);

            var avg = await query.AverageAsync(x => (double?)x.TienDo) ?? 0;
            return new TienDoTrungBinhReport { TienDoTrungBinh = avg };
        }
        public async Task<List<SoLuongTheoUuTienReport>> GetSoLuongTheoUuTien(string groupId, string id_NguoiGiaoViec)
        {
            using var context = _context.CreateDbContext();
            var query = context.QLNV_CongViecs
                .Where(x => x.IsActive != 100 && x.GroupId == groupId && x.Id_NguoiGiaoViec == id_NguoiGiaoViec);

            var result = await query
                .GroupBy(x => x.MucDoUuTien)
                .Select(g => new SoLuongTheoUuTienReport
                {
                    MucDoUuTien = g.Key,
                    SoLuong = g.Count()
                })
                .ToListAsync();

            return result;
        }
        public async Task<List<SoLuongTheoThoiGianReport>> GetSoLuongTheoThoiGian(string groupId, string id_NguoiGiaoViec)
        {
            using var context = _context.CreateDbContext();
            var query = context.QLNV_CongViecs
                .Where(x => x.IsActive != 100 && x.GroupId == groupId && x.Id_NguoiGiaoViec == id_NguoiGiaoViec);

            var result = await query
                .GroupBy(x => new { x.CreateAt.Year, x.CreateAt.Month })
                .Select(g => new SoLuongTheoThoiGianReport
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToListAsync();

            return result;
        }

        //báo cáo theo nhân viên thực hiện
        public async Task<CongViecStatusReport> GetStatusReportNVTH(string groupId,string idNhanVien)
        {
            using var context = _context.CreateDbContext();
            var now = DateTime.Now;

            var list = await (
                from cv in context.QLNV_CongViecs
                join nvth in context.QLNV_NhanVienThucHiens
                    on cv.Id equals nvth.Id_CongViec
                where cv.IsActive != 100
                      && cv.GroupId == groupId
                      && nvth.Id_NhanVien == idNhanVien
                select cv
            ).ToListAsync();
            if (!list.Any())
            {
                return new CongViecStatusReport();
            }
            return new CongViecStatusReport
            {
                TongSo = list.Count,
                DangThucHien = list.Count(x => x.TienDo > 0 && x.TienDo < 10 && (x.NgayKetThuc == null || x.NgayKetThuc >= now)),
                HoanThanh = list.Count(x => x.TienDo >= 10),
                Cho = list.Count(x => x.TienDo == 0 && x.NgayBatDau > now),
                ChuaLam = list.Count(x => x.TienDo == 0 && x.NgayBatDau <= now),
                QuaHan = list.Count(x => x.TienDo < 10 && x.NgayKetThuc != null && x.NgayKetThuc < now)
            };
        }
        public async Task<List<CongViecTheoUuTienReport>>GetBaoCaoTheoUuTienNVTH(string groupId, string idNhanVien)
        {
            using var context = _context.CreateDbContext();

            return await (
                from cv in context.QLNV_CongViecs
                join nvth in context.QLNV_NhanVienThucHiens
                    on cv.Id equals nvth.Id_CongViec
                where cv.IsActive != 100
                      && cv.GroupId == groupId
                      && nvth.Id_NhanVien == idNhanVien
                group cv by cv.MucDoUuTien into g
                select new CongViecTheoUuTienReport
                {
                    MucDoUuTien = g.Key,
                    SoLuong = g.Count()
                }
            ).ToListAsync();
        }
        public async Task<CongViecDanhGiaReport>GetBaoCaoDanhGiaNVTH(string groupId, string idNhanVien)
        {
            using var context = _context.CreateDbContext();

            var data = await (
                from cv in context.QLNV_CongViecs
                join nvth in context.QLNV_NhanVienThucHiens
                    on cv.Id equals nvth.Id_CongViec
                join dg in context.QLNV_DanhGias
                    on cv.Id equals dg.Id_CongViec into dgJoin
                from dg in dgJoin.DefaultIfEmpty()
                where cv.IsActive != 100
                      && cv.GroupId == groupId
                      && nvth.Id_NhanVien == idNhanVien
                select new { dg }
            ).ToListAsync();
            if (!data.Any())
            {
                return new CongViecDanhGiaReport(); // tất cả = 0
            }

            return new CongViecDanhGiaReport
            {
                DaDanhGia = data.Count(x => x.dg != null),
                ChuaDanhGia = data.Count(x => x.dg == null)
            };
        }
        public async Task<CongViecThoiHanReport>GetBaoCaoThoiHanNVTH(string groupId, string idNhanVien, int soNgayCanhBao = 3)
        {
            using var context = _context.CreateDbContext();
            var now = DateTime.Now;
            var warningDate = now.AddDays(soNgayCanhBao);

            var list = await (
                from cv in context.QLNV_CongViecs
                join nvth in context.QLNV_NhanVienThucHiens
                    on cv.Id equals nvth.Id_CongViec
                where cv.IsActive != 100
                      && cv.GroupId == groupId
                      && nvth.Id_NhanVien == idNhanVien
                      && cv.NgayKetThuc != null
                select cv
            ).ToListAsync();
            if (!list.Any())
            {
                return new CongViecThoiHanReport(); // tất cả = 0
            }
            return new CongViecThoiHanReport
            {
                DungHan = list.Count(x => x.TienDo == 10 && x.NgayKetThuc >= now),
                QuaHan = list.Count(x => x.TienDo < 10 && x.NgayKetThuc < now),
                SapQuaHan = list.Count(x =>
                    x.TienDo < 10 &&
                    x.NgayKetThuc >= now &&
                    x.NgayKetThuc <= warningDate)
            };
        }
        public async Task<TienDoTrungBinhNVTHReport> GetTienDoTrungBinhNVTH(string groupId, string idNhanVien)
        {
            using var context = _context.CreateDbContext();

            var avg = await (
                from cv in context.QLNV_CongViecs
                join nvth in context.QLNV_NhanVienThucHiens
                    on cv.Id equals nvth.Id_CongViec
                where cv.IsActive != 100
                      && cv.GroupId == groupId
                      && nvth.Id_NhanVien == idNhanVien
                select (double?)cv.TienDo   // ⭐ rất quan trọng
            ).AverageAsync();

            return new TienDoTrungBinhNVTHReport
            {
                TienDoTrungBinh = Math.Round(avg ?? 0, 2)
            };
        }



    }
}
