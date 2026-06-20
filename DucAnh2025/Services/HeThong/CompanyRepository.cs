using DucAnh2025.Data;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels.HeThong;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.HeThong
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public CompanyRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public string CheckCondition(MCompany mcompanie, int InputSave)
        {
            string s = "";
            return s;
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
                }

                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"ID: {id} đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }

            }
            return true;
        }

        public async Task<List<MCompanyModel>> GetAllByVM()
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = from p in context.MCompanies
                            select new MCompanyModel
                            {
                                Id = p.Id,
                                ParentId = p.ParentId,
                                CompanyName = p.CompanyName,
                                CompanyType = p.CompanyType,
                                Phone = p.Phone,
                                Email = p.Email,
                                Address = p.Address,
                                GroupId = p.GroupId,
                                CreateAt = p.CreateAt,
                                CreateBy = p.CreateBy,
                                IsActive = p.IsActive,
                                UserId = p.UserId,
                                DateApproval = p.DateApproval,
                                IdDepartment = p.IdDepartment,
                                ApprovalOrder = p.ApprovalOrder
                            };
                var data = await query
                .ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }
            entity.IsActive = 3;
            context.MCompanies.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<MCompany>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.MCompanies.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw; // Optionally rethrow the exception
            }
        }

        public async Task<List<MCompany>> GetAllCompanies()
        {
            using var context = _context.CreateDbContext();
            var query = context.MCompanies.Where(x => x.IsActive != 100);
            return await query.ToListAsync();
        }

        public async Task<MCompany> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.MCompanies.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }

            return entity;
        }

        public async Task Insert(MCompany entity, string userId)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.MCompanies.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task Update(MCompany company, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(company.Id);

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {company.Id}");
            }

            context.MCompanies.Update(company);
            await context.SaveChangesAsync();
        }

        public async Task UpdateMulti(MCompany[] companies)
        {
            using var context = _context.CreateDbContext();
            string[] ids = companies.Select(x => x.Id).ToArray();
            var listEntities = await context.MCompanies.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.MCompanies.Update(entity);
            }
            await context.SaveChangesAsync();
        }

        public Task<bool> CheckStatus(string ids, string name)
        {
            throw new NotImplementedException();
        }
    }
}
