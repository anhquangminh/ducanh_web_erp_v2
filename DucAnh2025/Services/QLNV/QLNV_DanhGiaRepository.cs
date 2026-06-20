using DucAnh2025.Data;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_DanhGiaRepository : IQLNV_DanhGiaRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public QLNV_DanhGiaRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<List<QLNV_DanhGiaModel>> GetByVM(string groupId, QLNV_DanhGiaModel input, string Id_NguoiGiaoViec)
        {
            var a = System.Text.Json.JsonSerializer.Serialize(input);

            List<QLNV_DanhGiaModel> data = new();
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p1 in context.QLNV_CongViecs
                            where p1.Id_NguoiGiaoViec == Id_NguoiGiaoViec && p1.IsActive != 100 && p1.GroupId == groupId
                            select p1;

                var result = from p1 in query
                             join d in context.QLNV_DanhGias on p1.Id equals d.Id_CongViec into gj
                             from d in gj.DefaultIfEmpty()
                             //join nv in context.QLNV_NhanViens on p1.Id_NguoiThucHien equals nv.Id
                            
                             where p1.IsActive != 100
                             orderby p1.CreateAt descending
                             select new QLNV_DanhGiaModel
                             {
                                 Id = d.Id,
                                 Id_CongViec = p1.Id,
                                 Id_NguoiGiaoViec = p1.Id_NguoiGiaoViec,
                                 NoiDungCongViec = p1.NoiDungCongViec,
                                 TenCongViec = p1.TenCongViec,
                                 NguoiThucHien = string.Join(", ",
                                    (from nvth in context.QLNV_NhanVienThucHiens
                                     join nv in context.QLNV_NhanViens on nvth.Id_NhanVien equals nv.Id
                                     where nvth.Id_CongViec == p1.Id
                                     select nv.TenNhanVien + " (" + nv.TaiKhoan + ")")),
                                 TienDo = p1.TienDo,
                                 DanhGia = (d.DanhGia == null)  ? -1 : d.DanhGia,
                                 GhiChu = d.GhiChu ?? "",
                                 GroupId = d.GroupId,
                                 CreateAt = p1.CreateAt,
                                 CreateBy = p1.CreateBy,
                                 IsActive = p1.IsActive,
                             };

                //if (!string.IsNullOrEmpty(input.GroupId))
                //{
                //    result = result.Where(x => x.GroupId == input.GroupId);
                //}
                if (!string.IsNullOrEmpty(input.Id_CongViec))
                {
                    result = result.Where(x => x.Id_CongViec == input.Id_CongViec);
                }
                if (!string.IsNullOrEmpty(input.NoiDungCongViec))
                {
                    result = result.Where(x => x.NoiDungCongViec == input.NoiDungCongViec);
                }
                if (!string.IsNullOrEmpty(input.TenCongViec))
                {
                    result = result.Where(x => x.TenCongViec == input.TenCongViec);
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
        public async Task<List<QLNV_DanhGia>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.QLNV_DanhGias.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<QLNV_DanhGia> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_DanhGias.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<QLNV_DanhGia> GetByIdCongViec(string Id_CongViec)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.QLNV_DanhGias.Where(x => x.Id_CongViec.Equals(Id_CongViec) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id");
            }
            return entity;
        }
        public async Task<bool> CheckExist(string id, QLNV_DanhGia input)
        {
            using var context = _context.CreateDbContext();
            return await context.QLNV_DanhGias
                .AnyAsync(x => x.Id != id &&
                                x.GroupId == input.GroupId &&
                               x.Id_CongViec == input.Id_CongViec
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
        public async Task Insert(QLNV_DanhGia entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Thêm không thành công!");
                }
                context.QLNV_DanhGias.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(QLNV_DanhGia data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy Id !");
            }
            context.QLNV_DanhGias.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(QLNV_DanhGia[] QLNV_DanhGias)
        {
            using var context = _context.CreateDbContext();
            string[] ids = QLNV_DanhGias.Select(x => x.Id).ToArray();
            var listEntities = await context.QLNV_DanhGias.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.QLNV_DanhGias.Update(entity);
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
            context.Set<QLNV_DanhGia>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.QLNV_DanhGias.Where(p => p.Id == ids).FirstOrDefaultAsync();
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
    }
}
