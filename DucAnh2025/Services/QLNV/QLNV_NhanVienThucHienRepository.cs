using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using Microsoft.EntityFrameworkCore;
using DucAnh2025.Data;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_NhanVienThucHienRepository : IQLNV_NhanVienThucHienRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public QLNV_NhanVienThucHienRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<QLNV_NhanVienThucHien?> GetById(string id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.QLNV_NhanVienThucHiens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive != 100);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<QLNV_NhanVienThucHien>> GetAll(string groupId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.QLNV_NhanVienThucHiens
                    .AsNoTracking()
                    .Where(x => x.GroupId == groupId && x.IsActive != 100)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_NhanVienThucHien>();
            }
        }

        public async Task Insert(QLNV_NhanVienThucHien entity, string userName)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                if (entity == null)
                    throw new Exception("Thêm không thành công!");

                entity.Id = Guid.NewGuid().ToString();
                entity.CreateAt = DateTime.Now;
                entity.CreateBy = userName;
                entity.IsActive = 1;

                context.QLNV_NhanVienThucHiens.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task Update(QLNV_NhanVienThucHien entity, string userName)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var existing = await context.QLNV_NhanVienThucHiens
                    .FirstOrDefaultAsync(x => x.Id == entity.Id && x.IsActive != 100);

                if (existing == null)
                    throw new Exception("Không tìm thấy Id!");

                // Update fields
                existing.Id_CongViec = entity.Id_CongViec;
                existing.Id_NhanVien = entity.Id_NhanVien;
                existing.GroupId = entity.GroupId;
                existing.IsActive = entity.IsActive;

                context.QLNV_NhanVienThucHiens.Update(existing);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(string id, string userName)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var entity = await context.QLNV_NhanVienThucHiens
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive != 100);

                if (entity == null)
                    throw new Exception("Không tìm thấy Id!");

                context.QLNV_NhanVienThucHiens.Remove(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<List<QLNV_NhanVien>> GetNhanViensByCongViec(string id_CongViec, string groupId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var query = from nvth in context.QLNV_NhanVienThucHiens
                            join nv in context.QLNV_NhanViens on nvth.Id_NhanVien equals nv.Id
                            where nvth.Id_CongViec == id_CongViec
                                  && nvth.GroupId == groupId
                                  && nvth.IsActive != 100
                                  && nv.IsActive != 100
                            select nv;

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return new List<QLNV_NhanVien>();
            }
        }
    }
}