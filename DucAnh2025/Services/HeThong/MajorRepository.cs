using DucAnh2025.Data;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class MajorRepository : IMajorRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public MajorRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<Major> AddMajor(Major mMajor)
        {
            using var context = _context.CreateDbContext();
            if (mMajor == null) return null;

            mMajor.Id = Guid.NewGuid().ToString();
            mMajor.CreateBy = "system@gmail.com";
            mMajor.CreateAt = DateTime.Now;
            mMajor.IsActive = 1;

            var newMajor = context.Majors.Add(mMajor).Entity;
            await context.SaveChangesAsync();

            return newMajor;

        }

        public async Task<List<MajorModel>> GetAllByVM(MajorModel majorModel)
        {
            using var context = _context.CreateDbContext();
            var query = from major in context.Majors
                        join parent in context.Majors.Where(p => p.IsActive != 100) // Lọc dữ liệu của parent trước khi ghép nối
                        on major.ParentId equals parent.Id into parentGroup
                        from parent in parentGroup.DefaultIfEmpty()
                        where major.IsActive != 100
                        orderby major.Order ascending
                        select new MajorModel
                        {
                            Id = major.Id,
                            ParentId = major.ParentId,
                            ParentName = parent.MajorName != null ? parent.MajorName : "Nghiệp vụ cha",
                            MajorName = major.MajorName,
                            Table = major.Table,
                            Order = major.Order,
                            CreateAt = major.CreateAt,
                            CreateBy = major.CreateBy,
                            IsActive = major.IsActive
                        };

            if (!string.IsNullOrEmpty(majorModel.ParentId))
            {
                query = query.Where(m => m.ParentId == majorModel.ParentId);
            }

            if (!string.IsNullOrEmpty(majorModel.MajorName))
            {
                query = query.Where(m => m.MajorName.Contains(majorModel.MajorName));
            }

            if (!string.IsNullOrEmpty(majorModel.Table))
            {
                query = query.Where(m => m.Table == majorModel.Table);
            }

            var data = await query.ToListAsync();
            return data;
        }

        public async Task<List<Major>> GetAllParentMajor()
        {
            using var context = _context.CreateDbContext();
            var query = context.Majors
                .Select(selectedMajor => new Major()
                {
                    Id = selectedMajor.Id,
                    ParentId = selectedMajor.ParentId,
                    MajorName = selectedMajor.MajorName,
                    Order = selectedMajor.Order,
                    Table = selectedMajor.Table
                })
                .Where(selectedMajor => selectedMajor.ParentId == null || selectedMajor.ParentId == "");

            var data = await query.ToListAsync();
            return data;
        }

        public async Task<Major> GetMajorByName(string majorName)
        {
            using var context = _context.CreateDbContext();
            var major = await context.Majors.Where(x => x.MajorName == majorName && x.IsActive == 1).FirstOrDefaultAsync();
            if (major == null) return null;

            return major;
        }

        public async Task<Major> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.Majors.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }

            return entity;
        }

        public async Task<List<Major>> GetMajorByParentId(string id)
        {
            using var context = _context.CreateDbContext();
            var majors = await context.Majors.Where(x => x.ParentId == id && x.IsActive != 100).OrderBy(x => x.Order).ToListAsync();
            return majors;
        }

        public List<Major> GetMajorByParentId1(string id)
        {
            using var context = _context.CreateDbContext();
            var majors = context.Majors.Where(x => x.ParentId == id && x.IsActive != 100).OrderBy(x => x.Order).ToList();
            return majors;
        }

        public async Task<List<Major>> GetParentMajor()
        {
            using var context = _context.CreateDbContext();
            var majors = await context.Majors.Where(x => x.ParentId == null && x.IsActive != 100).OrderBy(x => x.Order).ToListAsync();
            return majors;
        }

        public async Task<List<Major>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                List<Major> list = new();
                list = await context.Majors.ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw; // Optionally rethrow the exception
            }
        }

        public async Task Update(Major major, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(major.Id);

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {major.Id}");
            }

            context.Majors.Update(major);
            await context.SaveChangesAsync();
        }

        public async Task UpdateMulti(Major[] entities)
        {
            using var context = _context.CreateDbContext();
            string[] ids = entities.Select(x => x.Id).ToArray();
            var listEntities = await context.Majors.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.Majors.Update(entity);
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            var isExist = await context.MajorUserPermissions.Where(x => x.MajorId == id && x.IsActive == 1).ToListAsync();
            var isExist1 = await context.Permissions.Where(x => x.MajorId == id && x.IsActive == 1).ToListAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }
            if (isExist != null && isExist.Count() > 0)
            {
                throw new Exception($"Nghiệp vụ :{entity.MajorName} đang được sử dụng !");
            }
            if (isExist != null && isExist1.Count() > 0)
            {
                throw new Exception($"Nghiệp vụ :{entity.MajorName} đang được sử dụng !");
            }

            context.Set<Major>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy bản ghi theo ID: {id} vui lòng tải lại trang!");
                }

                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"ID: {id} đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }

            }
            return true;
        }

        public async Task Insert(Major entity, string userId)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.Majors.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.Majors.Where(p => p.Id == ids).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy nghiệp vụ đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Nghiệp vụ đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Nghiệp vụ đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Nghiệp vụ đang chờ duyệt xóa.");
            }
            if (model != null && model.MajorName.ToUpper() != name.ToUpper() && name != "")
            {
                throw new Exception($"Tên nghiệp vụ đã bị thay đổi.");
            }
            return true;
        }
    }
}
