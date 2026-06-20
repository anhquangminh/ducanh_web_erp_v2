using DucAnh2025.Data;
using DucAnh2025.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using DucAnh2025.Models.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using DucAnh2025.Repository.HeThong;

namespace DucAnh2025.Services.HeThong
{
    public class PhanQuyenRepository : IPhanQuyenRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public PhanQuyenRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<ApprovalStepSetting> GetApprovalStepSettingById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.ApprovalStepSettings.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi.");
            }
            return entity;
        }

        //Lưu phòng ban, thứ tự duyệt, fist id, last id

        //checked quyền
        //get it tiep theo
        //gán id tiếp theo vào bảng
        //trường hợp đã là cuối cùng thì update trạng thái
        //trường hợp thêm mới hoặc sửa thì gét ID đầu tiên

        //hàm getID first
        //hàm getID tiếp theo
        //thêm cột trạng thái text
        //thêm cột id duyệt
        //thêm cột id duyệt cuối
        //Thêm cột thứ tự phòng ban duyệt

        //khi ID duyệt bằng ID cuối chỉ cần check quyền

        //thiết kế bảng phòng ban theo chi nhánh
        //phân quyền phòng ban
        //duyệt phòng ban


        //không duyệt
        //check quyền duyệt
        //update dữ liệu: lấy trạng thái bằng 3 mới nhất tại bảng log
        //viết tại service của mỗi bảng

        public async Task<bool> CheckApproval(string companyId, string deptId, ApplicationUser user, string approvalId)
        {
            using var context = _context.CreateDbContext();
            try
            {
                if (user.CreateBy == "symtem")
                {
                    return true;
                }
                else
                {
                    var result = await (from a in context.MajorUserApprovals
                                        where a.UserId.Equals(user.Id)
                                              && a.CompanyId.Equals(companyId)
                                              && a.ApprovalId.Equals(approvalId)
                                              && a.DayinWeek.Equals(DateTime.Now.DayOfWeek)
                                        select a.Id).CountAsync();
                    return result > 0;
                }
            }
            catch (Exception)
            {
                var result = await (from p in context.ApprovalStepSettings
                                    where p.Id.Equals(approvalId)
                                    select p).FirstOrDefaultAsync();
                throw new Exception($"Bạn không có quyền " + result.Content + " trong thời gian này.");
            }
        }
        public async Task<bool> CheckPermission(string groupId, string companyId, ApplicationUser user, string permissionId)
        {
            using var context = _context.CreateDbContext();
            try
            {

                if (user.CreateBy == "symtem" && user.GroupId == groupId)
                {
                    return true;
                }

                int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
                bool hasPermission = await context.MajorUserPermissions.AnyAsync(a =>
                    a.UserId == user.Id &&
                    a.GroupId == groupId &&
                    a.CompanyId == user.CompanyId &&
                    a.PermissionId == permissionId &&
                    a.DayInWeek == currentDayOfWeek
                );

                if (hasPermission)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Bạn không có quyền thực hiện hành động này");
                }
            }
            catch (Exception ex)
            {
                // Ghi log exception nếu cần, sau đó ném ra ngoại lệ
                throw new Exception("Bạn không có quyền thực hiện hành động này", ex);
            }
        }

        public async Task<List<PermissionModel>> getAllPermissionByMajor(string groupId, string companyId, ApplicationUser user, string majorId)
        {
            List<PermissionModel> listPer = new List<PermissionModel>();
            using var context = _context.CreateDbContext();

            try
            {
                int currentDay = (int)DateTime.Now.DayOfWeek;

                var result = from mu in context.MajorUserPermissions
                             join p in context.Permissions on mu.PermissionId equals p.Id
                             join pm in context.Majors on mu.ParentMajorId equals pm.Id
                             join m in context.Majors on mu.MajorId equals m.Id
                             where mu.IsActive != 100
                                   && mu.DayInWeek == currentDay
                                   && mu.GroupId == groupId
                                   && mu.CompanyId == user.CompanyId
                                   && mu.MajorId == majorId
                             orderby p.PermissionType ascending
                             select new PermissionModel
                             {
                                 Id = p.Id,
                                 ParentMajorId = pm.Id,
                                 ParentMajorName = pm.MajorName,
                                 MajorId = m.Id,
                                 MajorName = m.MajorName,
                                 PermissionType = p.PermissionType,
                                 PermissionName = p.PermissionName,
                                 IsActive = p.IsActive
                             };
                listPer = await result.ToListAsync();
                return listPer;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi hàm getAllPermissionByMajor " + ex.Message);
                return listPer;
            }
        }

        //lấy tất cả quyên theo từng parentMajorId, majorId
        public async Task<List<PermissionModel>> GetAllPermissionByGroupId(string groupId, string userId, string parentMajorId, string majorId)
        {
            List<PermissionModel> listPer = new List<PermissionModel>();
            using var context = _context.CreateDbContext();

            try
            {
                int currentDay = (int)DateTime.Now.DayOfWeek;
                var query = from mu in context.MajorUserPermissions
                            join p in context.Permissions on mu.PermissionId equals p.Id
                            join pm in context.Majors on mu.ParentMajorId equals pm.Id
                            join m in context.Majors on mu.MajorId equals m.Id
                            where mu.IsActive != 100
                                  && mu.DayInWeek == currentDay
                                  && mu.GroupId == groupId
                                  && mu.UserId == userId
                            orderby p.PermissionName, p.PermissionType ascending
                            select new PermissionModel
                            {
                                Id = p.Id,
                                ParentMajorId = pm.Id,
                                ParentMajorName = pm.MajorName,
                                MajorId = m.Id,
                                MajorName = m.MajorName,
                                PermissionType = p.PermissionType,
                                PermissionName = p.PermissionName,
                                IsActive = p.IsActive
                            };

                if (!string.IsNullOrEmpty(parentMajorId))
                {
                    query = query.Where(x => x.ParentMajorId == parentMajorId);
                }

                if (!string.IsNullOrEmpty(majorId))
                {
                    query = query.Where(x => x.MajorId == majorId);
                }

                listPer = await query.Distinct().ToListAsync();
                return listPer;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return listPer;
            }
        }


        public async Task<ApprovalStepSetting> GetFirstApprovalStep(string companyId, string majorId, string permissionId)
        {
            using var context = _context.CreateDbContext();

            var countDetp = await (from p in context.ApprovalDeptSettings
                                   where p.CompanyId.Equals(companyId)
                                       && p.MajorId.Equals(majorId)
                                       && p.IsActive != 100
                                   select new { p.DeptId }).Distinct().CountAsync();

            var countstep = await (from p in context.ApprovalStepSettings
                                   where p.CompanyId.Equals(companyId)
                                       && p.MajorId.Equals(majorId)
                                       && p.IsActive != 100
                                   select new { p.DeptId }).Distinct().CountAsync();

            if (countDetp != countstep)
            {
                throw new Exception($"Bạn chưa cài đặt đủ thứ tự duyệt cho phòng ban của nghiệp vụ này.");
            }
            else
            {
                var entity = await (from p in context.ApprovalStepSettings
                                    join q in context.ApprovalDeptSettings on p.MajorId equals q.MajorId
                                    join chinhanh in context.ChiNhanhs on p.CompanyId equals chinhanh.Id
                                    join parentMajor in context.Majors on p.ParentMajorId equals parentMajor.Id
                                    join major in context.Majors on p.MajorId equals major.Id
                                    join department in context.Departments on p.DeptId equals department.Id
                                    join permission in context.Permissions on p.PermissionId equals permission.Id
                                    where p.CompanyId.Equals(companyId)
                                        && q.CompanyId.Equals(companyId)
                                        && p.MajorId.Equals(majorId)
                                        && p.PermissionId.Equals(permissionId)
                                        && p.ApprovalStep == 1
                                        && q.ApprovalStep == 1
                                        && p.DeptId == q.DeptId
                                        && p.IsActive != 100
                                    select new ApprovalStepSetting
                                    {
                                        Id = p.Id,
                                        IdMain = p.IdMain,
                                        CompanyId = chinhanh.TenChiNhanh,
                                        DeptId = department.DeptName,
                                        ParentMajorId = parentMajor.MajorName,
                                        MajorId = major.MajorName,
                                        PermissionId = permission.PermissionName,
                                        Content = p.Content,
                                        ApprovalStep = p.ApprovalStep,
                                        GroupId = p.GroupId,
                                        Ordinarily = p.Ordinarily,
                                        CreateAt = p.CreateAt,
                                        CreateBy = p.CreateBy,
                                        IsActive = p.IsActive,
                                        ApprovalUserId = p.ApprovalUserId,
                                        DateApproval = p.DateApproval,
                                        DepartmentId = p.DeptId,
                                        DepartmentOrder = p.DepartmentOrder,
                                        ApprovalOrder = p.ApprovalOrder,
                                        ApprovalId = p.ApprovalId,
                                        LastApprovalId = p.LastApprovalId,
                                        IsStatus = p.IsStatus
                                    }).FirstOrDefaultAsync();
                if (entity == null)
                {
                    throw new Exception($"Bạn chưa cài đặt duyệt cho nghiệp vụ này.");
                }
                return entity;
            }
        }

        public async Task<ApprovalStepSetting> GetNextApprovalStep(string companyId, string majorId, string permissionId, string departmentId, int departmentOrder, int approvalOrder)
        {
            using var context = _context.CreateDbContext();

            var entity = await (from p in context.ApprovalStepSettings
                                where p.CompanyId.Equals(companyId)
                                    && p.MajorId.Equals(majorId)
                                    && p.PermissionId.Equals(permissionId)
                                    && p.DeptId.Equals(departmentId)
                                    && p.ApprovalStep == approvalOrder + 1
                                    && p.IsActive != 100
                                select p).FirstOrDefaultAsync();
            if (entity == null)
            {
                var depentity = await (from p in context.ApprovalDeptSettings
                                       where p.CompanyId.Equals(companyId)
                                           && p.MajorId.Equals(majorId)
                                           && p.ApprovalStep.Equals(departmentOrder + 1)
                                           && p.IsActive != 100
                                       select p).FirstOrDefaultAsync();

                entity = await (from p in context.ApprovalStepSettings
                                where p.CompanyId.Equals(companyId)
                                    && p.MajorId.Equals(majorId)
                                    && p.PermissionId.Equals(permissionId)
                                    && p.DeptId.Equals(depentity.DeptId)
                                    && p.ApprovalStep == 1
                                    && p.IsActive != 100
                                select p).FirstOrDefaultAsync();
            }
            if (entity == null)
            {
                throw new Exception($"Bạn chưa cài đặt duyệt cho nghiệp vụ này.");
            }
            return entity;
        }

        public async Task<ApprovalStepSetting> GetLastApprovalStep(string companyId, string majorId, string permissionId)
        {
            using var context = _context.CreateDbContext();

            var query = await (from p in context.ApprovalDeptSettings
                               where p.CompanyId.Equals(companyId)
                               && p.MajorId.Equals(majorId)
                               select p).OrderByDescending(p => p.ApprovalStep).Take(1).FirstOrDefaultAsync();

            var entity = await (from p in context.ApprovalStepSettings
                                where p.CompanyId.Equals(companyId)
                                    && p.MajorId.Equals(majorId)
                                    && p.PermissionId.Equals(permissionId)
                                    && p.DeptId.Equals(query.DeptId)
                                    && p.DeptOrder.Equals(query.ApprovalStep)
                                    && p.IsActive != 100
                                select p).OrderByDescending(p => p.ApprovalStep).Take(1).FirstOrDefaultAsync();

            if (query == null || entity == null)
            {
                throw new Exception($"Bạn chưa cài đặt duyệt cho nghiệp vụ này.");
            }
            return entity;
        }

        public async Task<List<ApprovalStaffSetting>> GetApprovalStaffByApprovalStepId(string approvalStepId)
        {
            using var context = _context.CreateDbContext();
            var entity = await (from p in context.ApprovalStaffSettings
                                join q in context.ApplicationUsers on p.UserId equals q.Id
                                where p.ApprovalStepId.Equals(approvalStepId)
                                && p.IsActive != 100
                                && q.IsActive != 100
                                select new ApprovalStaffSetting
                                {
                                    Id = p.Id,
                                    CompanyId = p.CompanyId,
                                    DeptId = p.DeptId,
                                    UserId = p.UserId,
                                    ParentMajorId = p.ParentMajorId,
                                    MajorId = p.MajorId,
                                    ApprovalStepId = p.ApprovalStepId,
                                    Content = p.Content,
                                    GroupId = p.GroupId,
                                    CreateAt = p.CreateAt,
                                    CreateBy = q.Email,
                                    IsActive = p.IsActive
                                }).ToListAsync();
            return entity;
        }


    }
}
