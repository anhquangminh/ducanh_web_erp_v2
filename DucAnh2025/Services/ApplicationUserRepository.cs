using DucAnh2025.Data;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Repository;
using DucAnh2025.ViewModels.Accounts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DucAnh2025.Services
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationUserRepository(IDbContextFactory<ApplicationDbContext> context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationUser GetCurrentUser()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
                return null;

            return GetByUserName(userName);
        }
        public ApplicationUser GetByUserName(string userName)
        {
            using var context = _context.CreateDbContext();
            var query = (from user in context.ApplicationUsers
                         where user.UserName.Equals(userName) && user.IsActive != 100
                         select user);
            var entity = query.FirstOrDefault();
            return entity;
        }
        public bool IsExistByPhoneNumber(string phoneNumber)
        {
            using var context = _context.CreateDbContext();
            var entity = (from user in context.ApplicationUsers
                          where user.PhoneNumber.Equals(phoneNumber) && user.IsActive != 100
                          select user).FirstOrDefault();
            return entity != null;
        }
        public async Task<List<ApplicationUserModel>> GetAllByVM(ApplicationUserModel dataModel, string groupId)
        {
            using var context = _context.CreateDbContext();
            var query = from p1 in context.ApplicationUsers
                        where p1.GroupId == groupId
                        select p1;
            if (!string.IsNullOrEmpty(dataModel.CompanyId))
            {
                query = query.Where(m => m.CompanyId == dataModel.CompanyId);
            }
            if (!string.IsNullOrEmpty(dataModel.DeptId))
            {
                query = query.Where(m => m.DeptId == dataModel.DeptId);
            }
            var data = await (from p1 in query
                              join p2 in context.ChiNhanhs on p1.CompanyId equals p2.Id
                              join p3 in context.Departments on p1.DeptId equals p3.Id
                              where p1.GroupId == groupId
                              orderby p1.CreateAt descending
                              select new ApplicationUserModel
                              {
                                  Id = p1.Id,
                                  UserName = p1.UserName,
                                  FirstName = p1.FirstName,
                                  LastName = p1.LastName,
                                  Dob = p1.Dob,
                                  PhoneNumber = p1.PhoneNumber,
                                  Email = p1.Email,
                                  Address = p1.Address,
                                  CompanyId = p2.TenChiNhanh,
                                  DeptId = p3.DeptName,
                                  IsFirstLogin = p1.IsFirstLogin,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = p1.DateApproval,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus,
                              }).ToListAsync();
            return data;
        }
        public async Task<List<ApplicationUser>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.ApplicationUsers.Where(p => p.IsActive != 100 && p.GroupId == groupId).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<ChiNhanhModel>> GetChiNhanhs(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApplicationUsers
                                    join p2 in context.ChiNhanhs on p1.CompanyId equals p2.Id
                                    where p1.GroupId == groupId
                                    orderby p1.CreateAt descending
                                    select new ChiNhanhModel
                                    {
                                        Id = p2.Id,
                                        TenChiNhanh = p2.TenChiNhanh
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<List<DepartmentModel>> GetDepartments(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await (from p1 in context.ApplicationUsers
                                    join p2 in context.Departments on p1.DeptId equals p2.Id
                                    where p1.GroupId == groupId
                                    orderby p1.CreateAt descending
                                    select new DepartmentModel
                                    {
                                        Id = p2.Id,
                                        DeptName = p2.DeptName
                                    }).Distinct().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<ApplicationUser> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ApplicationUsers.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý người dùng đã chọn.");
            }
            return entity;
        }
        public async Task<List<string>> GetByIdUserName(List<string> userNames)
        {
            var realUserIds = new List<string>();
            using var context = _context.CreateDbContext();

            if (userNames.Count <= 0)
            {
                return realUserIds;
            }
            foreach (var userId in userNames)
            {
                if (Regex.IsMatch(userId, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                {
                    var user = GetByUserName(userId);
                    if (user != null)
                    {
                        realUserIds.Add(user.Id);
                    }
                }
                else
                {
                    realUserIds.Add(userId);
                }
            }
            return realUserIds;
        }
        public async Task Insert(ApplicationUser entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có quản lý người dùng nào được thêm!");
                }
                context.ApplicationUsers.Add(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(ApplicationUser data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý người dùng đã chọn");
            }
            context.ApplicationUsers.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(ApplicationUser[] ApplicationUsers)
        {
            using var context = _context.CreateDbContext();
            string[] ids = ApplicationUsers.Select(x => x.Id).ToArray();
            var listEntities = await context.ApplicationUsers.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.ApplicationUsers.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy quản lý người dùng đã chọn");
            }
            context.Set<ApplicationUser>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.ApplicationUsers.Where(p => p.Id == ids).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy quản lý người dùng đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Quản lý người dùng đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Quản lý người dùng đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Quản lý người dùng đang chờ duyệt xóa.");
            }
            if (model != null && model.UserName.ToUpper() != name.ToUpper() && name != "")
            {
                throw new Exception($"Tên quản lý người dùng đã bị thay đổi.");
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
                    throw new Exception($"Không tìm thấy quản lý người dùng đã chọn");
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
