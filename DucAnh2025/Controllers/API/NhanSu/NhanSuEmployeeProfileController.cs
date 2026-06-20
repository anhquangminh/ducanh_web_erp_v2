

using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DucAnh2025.Controllers.API.NhanSu
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NhanSuEmployeeProfileController : ControllerBase
    {
        private readonly IApprovalTaskRepository _approvalTaskService;
        private readonly INhanSu_EmployeeProfileRepository _nhansuemployeeprofileService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IPermissionRepository _permissionService;
        private readonly ApplicationDbContext _context;

        public NhanSuEmployeeProfileController(IApprovalTaskRepository approvalTaskRepository,
            INhanSu_EmployeeProfileRepository nhansuemployeeprofileRepository,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService,
            IPermissionRepository permissionService,
            ApplicationDbContext context)
        {
            _approvalTaskService = approvalTaskRepository;
            _nhansuemployeeprofileService = nhansuemployeeprofileRepository;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
            _permissionService = permissionService;
            _context = context;
        }

        public string parentMajorId = "09564cf6-71a5-4721-bc27-4c54fafd112b";
        public string tableName = "NhanSu_EmployeeProfiles";

        [HttpGet("GetSummary")]
        public async Task<IActionResult> GetSummary()
        {
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            // Query only once for all active employees
            var activeEmployeesQuery = _context.NhanSu_EmployeeProfiles.Where(e => e.IsActive == 3);

            var totalEmployees = await activeEmployeesQuery.CountAsync();
            var newHiresThisMonth = await activeEmployeesQuery.CountAsync(e => e.HireDate >= firstDayOfMonth);

            // Group by DepartmentId, handle null/empty as "Chưa có phòng ban"
            var byDepartmentRaw = await activeEmployeesQuery
                .GroupBy(e => string.IsNullOrEmpty(e.DepartmentId) ? "Chưa có phòng ban" : e.DepartmentId)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            // Optionally, you can join with Department table to get department names if needed

            // Gender statistics
            var genderStats = await activeEmployeesQuery
                .GroupBy(e => e.Gender)
                .Select(g => new { Gender = g.Key, Count = g.Count() })
                .ToListAsync();

            // Status mapping
            var statusMap = new Dictionary<int, string>
            {
                { 0, "Chờ duyệt thêm mới" },
                { 1, "Chờ duyệt sửa" },
                { 2, "Chờ duyệt xóa" },
                { 3, "Đã duyệt" },
                { 90, "Không duyệt" },
                { 100, "Đã duyệt xóa" }
            };

            // All employees by status (not just active)
            var byStatusRaw = await _context.NhanSu_EmployeeProfiles
                .GroupBy(e => e.IsActive)
                .Select(g => new { IsActive = g.Key, Count = g.Count() })
                .ToListAsync();

            var byStatusWithLabel = byStatusRaw
                .Select(s => new
                {
                    Status = statusMap.ContainsKey(s.IsActive) ? statusMap[s.IsActive] : $"Trạng thái {s.IsActive}",
                    Count = s.Count
                })
                .ToList();

            return Ok(new
            {
                totalEmployees,
                newHiresThisMonth,
                byDepartment = byDepartmentRaw,
                genderStats,
                byStatus = byStatusWithLabel
            });
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByVM(string groupId, [FromBody] NhanSu_EmployeeProfileModel? input = null)
        {
            try
            {
                var model = input ?? new NhanSu_EmployeeProfileModel();
                var list = await _nhansuemployeeprofileService.GetAllByVM(model, groupId);
                return Ok(new ApiResponse<IEnumerable<object>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _nhansuemployeeprofileService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<object>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(string id)
        {
            try
            {
                var result = await _nhansuemployeeprofileService.GetById(id);
                return Ok(new ApiResponse<object>(true, "Thành công", result));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }
        [HttpGet("GetDetail")]
        public async Task<ActionResult<ApiResponse<object>>> GetDetail(string id)
        {
            try
            {
                var result = await _nhansuemployeeprofileService.GetDetails(id);
                return Ok(new ApiResponse<object>(true, "Thành công", result));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("GetHistory")]
        public async Task<ActionResult<ApiResponse<object>>> GetHistory(string id)
        {
            try
            {
                var result = await _nhansuemployeeprofileService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Thành công", result));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] NhanSu_EmployeeProfile input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var now = DateTime.Now;

                input.GroupId = groupId;
                input.CreateBy = userId;
                input.CreateAt = now;

                bool isEditSpecial = !string.IsNullOrEmpty(input.Id) && input.Ordinarily == 0 && input.IsActive == 3;
                bool editCheck = isEdit || isEditSpecial;

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng này !", null));

                var firstPer = listPer.First();
                var majorId = firstPer.MajorId;
                var idTao = listPer.FirstOrDefault(p => p.PermissionType == 3)?.Id;
                var idSua = listPer.FirstOrDefault(p => p.PermissionType == 4)?.Id;

                if (editCheck)
                {
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idSua))
                        return Ok(new ApiResponse<object>(false, "Ban không có quyền sửa !", null));

                    if (!await _nhansuemployeeprofileService.CheckExclusive(new[] { input.Id }, now))
                        return Ok(new ApiResponse<object>(false, "Không thể sửa dữ liêụ !", null));

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, majorId, idSua);
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, majorId, idSua);

                    input.DepartmentId = firstApproval.DepartmentId;
                    input.ApprovalId = firstApproval.Id;
                    input.LastApprovalId = lastApproval.Id;
                    input.Ordinarily = input.IsActive == 3 ? input.Ordinarily + 1 : input.Ordinarily;
                    input.IsActive = 1;
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.IsStatus = firstApproval.Content;
                    input.ApprovalUserId = null;
                    input.DateApproval = null;
                    input.ApprovalDept = null;
                    await _nhansuemployeeprofileService.CheckEdit(input);
                    await _nhansuemployeeprofileService.Update(input, "");

                    string content = await BuildEmailContentEdit(input.Id);

                    var dataApproval = await _approvalTaskService.GetByOriginalId(input.Id);
                    if (dataApproval == null)
                    {
                        await _approvalTaskService.Insert(CreateApprovalTask("Hồ sơ- sửa !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    else
                    {
                        await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Hồ sơ- sửa !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }

                    await InsertEmailHistories(content, "Hồ sơ- sửa !", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công !", null));
                }
                else
                {
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idTao))
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền thêm mới !", null));

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, majorId, idTao);
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, majorId, idTao);

                    input.Id = Guid.NewGuid().ToString();
                    input.DepartmentId = firstApproval.DepartmentId;
                    input.ApprovalId = firstApproval.Id;
                    input.LastApprovalId = lastApproval.Id;
                    input.IsActive = 0;
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.Ordinarily = 0;
                    input.IsStatus = firstApproval.Content;
                    input.ApprovalUserId = null;
                    input.DateApproval = null;
                    input.ApprovalDept = null;
                    await _nhansuemployeeprofileService.CheckSave(input);
                    await _nhansuemployeeprofileService.Insert(input, "");
                    string content = await BuildEmailContentAdd(input.Id);
                    await _approvalTaskService.Insert(CreateApprovalTask("Hồ sơThêm mơí !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    await InsertEmailHistories(content, "Hồ sơ - Thêm", input.GroupId, groupId, firstApproval.Id, input.Id, userId);
                    return Ok(new ApiResponse<object>(true, "Thành công", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Ok(new ApiResponse<object>(false, "Không xác định đưọc dữ liẹu cần xóa !", null));

            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _nhansuemployeeprofileService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy dữ liệu !", null));

                if (query.IsActive == 2)
                    return Ok(new ApiResponse<object>(false, "Đang chờ duyệt !", null));

                if (!await _nhansuemployeeprofileService.CheckDelete(query))
                    return Ok(new ApiResponse<object>(false, "Không thể xóa - đã liên kết dữ liệu !", null));

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng này !", null));

                var majorId = listPer.FirstOrDefault()?.MajorId;
                var deletePermissionId = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (!await _phanQuyenService.CheckPermission(user.GroupId, query.GroupId, user, deletePermissionId))
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa !", null));

                if (!await _nhansuemployeeprofileService.CheckExclusive(new[] { id }, baseTime))
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại , dữ liệu đã bị thay đôi !", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, majorId, deletePermissionId);
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, majorId, deletePermissionId);

                query.DepartmentId = firstApproval.DepartmentId;
                query.ApprovalId = firstApproval.Id;
                query.LastApprovalId = lastApproval.Id;
                query.IsActive = 2;
                query.ApprovalOrder = 1;
                query.DepartmentOrder = 1;
                query.CreateAt = DateTime.Now;
                query.CreateBy = userId;
                query.IsStatus = firstApproval.Content;
                query.ApprovalUserId = null;
                query.DateApproval = null;
                query.ApprovalDept = null;

                await _nhansuemployeeprofileService.Update(query, "");

                var content = await BuildEmailContentDelete(id);
                var dataApproval = await _approvalTaskService.GetByOriginalId(id);
                if (dataApproval == null)
                {
                    await _approvalTaskService.Insert(CreateApprovalTask("Hồ sơ - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }
                else
                {
                    await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Hồ sơ - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }

                await InsertEmailHistories(content, "Hồ sơ - xóa", query.GroupId, groupId, firstApproval.Id, query.Id, userId);

                return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("Duyet")]
        public async Task<ActionResult<ApiResponse<object>>> Duyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _nhansuemployeeprofileService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "không tìm thấy thông tin đã chọn !", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt !", null));

                string thongbao = entity.IsStatus;

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt duyệt cho chức năng naỳ !", null));
                var MajorId = listPer.FirstOrDefault()?.MajorId;
                var id_tao = listPer.FirstOrDefault(p => p.PermissionType == 3)?.Id;
                var id_sua = listPer.FirstOrDefault(p => p.PermissionType == 4)?.Id;
                var id_xoa = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (entity.IsActive != 3)
                {
                    if (entity.ApprovalId == entity.LastApprovalId)
                    {
                        if (entity.IsActive == 0 || entity.IsActive == 1)
                        {
                            entity.ApprovalUserId = userId;
                            entity.DateApproval = DateTime.Now;
                            entity.ApprovalDept = entity.DepartmentId;
                            entity.DepartmentId = null;
                            entity.ApprovalId = null;
                            entity.ApprovalOrder = 0;
                            entity.DepartmentOrder = 0;
                            entity.LastApprovalId = null;
                            entity.IsActive = 3;
                            entity.IsStatus = "Đã duyệt";
                        }
                        else if (entity.IsActive == 2)
                        {
                            entity.ApprovalUserId = userId;
                            entity.DateApproval = DateTime.Now;
                            entity.ApprovalDept = entity.DepartmentId;
                            entity.DepartmentId = null;
                            entity.ApprovalId = null;
                            entity.ApprovalOrder = 0;
                            entity.DepartmentOrder = 0;
                            entity.LastApprovalId = null;
                            entity.IsActive = 100;
                            entity.IsStatus = "Đã duyệt xóa ";
                        }
                    }
                    else
                    {
                        var nextApproval = await _phanQuyenService.GetNextApprovalStep(
                            entity.GroupId,
                            MajorId,
                            entity.IsActive == 0 ? id_tao
                                : entity.IsActive == 1 ? id_sua
                                : entity.IsActive == 2 ? id_xoa
                                : "",
                            entity.DepartmentId,
                            entity.DepartmentOrder,
                            entity.ApprovalOrder
                        );
                        entity.DepartmentId = nextApproval.DeptId;
                        entity.IsStatus = nextApproval.Content;
                        entity.ApprovalId = nextApproval.Id;
                        entity.ApprovalOrder = nextApproval.ApprovalStep;
                        entity.DepartmentOrder = nextApproval.DeptOrder;
                        entity.ApprovalUserId = userId;
                        entity.DateApproval = DateTime.Now;
                        entity.ApprovalDept = entity.DepartmentId;
                    }

                    var rowApproval = await _approvalTaskService.GetByOriginalId(entity.Id);
                    rowApproval.ApprovalUserId = entity.ApprovalUserId;
                    rowApproval.DateApproval = entity.DateApproval;
                    rowApproval.ApprovalDept = entity.ApprovalDept;
                    rowApproval.DepartmentId = entity.DepartmentId ?? "";
                    rowApproval.ApprovalId = entity.ApprovalId ?? "";
                    rowApproval.ApprovalOrder = entity.ApprovalOrder;
                    rowApproval.DepartmentOrder = entity.DepartmentOrder;
                    rowApproval.LastApprovalId = entity.LastApprovalId ?? "";
                    rowApproval.IsActive = entity.IsActive;
                    rowApproval.IsStatus = entity.IsStatus;
                    await _approvalTaskService.Update(rowApproval, userId);

                    await _nhansuemployeeprofileService.Approval(entity, userId);
                    return Ok(new ApiResponse<object>(true, "Đã duyệt !" + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt !", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("HuyDuyet")]
        public async Task<ActionResult<ApiResponse<object>>> HuyDuyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _nhansuemployeeprofileService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn !", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt !", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _nhansuemployeeprofileService.NoApproval(entity, userId);
                    var dataNoApproval = await _nhansuemployeeprofileService.GetById(entity.Id);

                    var rowApproval = await _approvalTaskService.GetByOriginalId(entity.Id);
                    rowApproval.ApprovalUserId = dataNoApproval.ApprovalUserId;
                    rowApproval.DateApproval = dataNoApproval.DateApproval;
                    rowApproval.ApprovalDept = dataNoApproval.ApprovalDept;
                    rowApproval.DepartmentId = dataNoApproval.DepartmentId ?? "";
                    rowApproval.ApprovalId = dataNoApproval.ApprovalId ?? "";
                    rowApproval.ApprovalOrder = dataNoApproval.ApprovalOrder;
                    rowApproval.DepartmentOrder = dataNoApproval.DepartmentOrder;
                    rowApproval.LastApprovalId = dataNoApproval.LastApprovalId ?? "";
                    rowApproval.IsActive = dataNoApproval.IsActive;
                    rowApproval.IsStatus = dataNoApproval.IsStatus;
                    await _approvalTaskService.Update(rowApproval, userId);

                    return Ok(new ApiResponse<object>(true, "Đã hủy duyệt !" + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt không thể hủy duyệt !", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        private async Task<string> BuildEmailContentEdit(string id)
        {
            var isValidModel = await _nhansuemployeeprofileService.GetHistoryIsValidEdit(id);
            var sb = new StringBuilder();

            sb.AppendLine("Thông tin sửa NhanSuEmployeeProfile</h3>")
             .AppendLine("<table class=\"table table-hover table-bordered\">")
             .AppendLine("  <thead class=\"bg-info\">")
             .AppendLine("    <tr>")
             .AppendLine("     <th>Họ và tên</th>")
             .AppendLine("     <th>Giới tính</th>")
             .AppendLine("     <th>Ngày sinh</th>")
             .AppendLine("     <th>Số CCCD/CMND</th>")
             .AppendLine("     <th>Ngày cấp CCCD</th>")
             .AppendLine("     <th>Nơi cấp CCCD</th>")
             .AppendLine("     <th>Mã số Thuế TNCN</th>")
             .AppendLine("     <th>Số sổ BHXH</th>")
             .AppendLine("     <th>Điện thoại di động</th>")
             .AppendLine("     <th>Email cá nhân</th>")
             .AppendLine("     <th>Địa chỉ thường trú</th>")
             .AppendLine("     <th>Địa chỉ hiện tại</th>")
             .AppendLine("     <th>Tên người liên hệ khẩn cấp</th>")
             .AppendLine("     <th>Mã người liên hệ khẩn cấp</th>")
             .AppendLine("     <th>ID Phòng ban</th>")
             .AppendLine("     <th>ID Chức danh/Vị trí</th>")
             .AppendLine("     <th>Mã Quản lý trực tiếp</th>")
             .AppendLine("     <th>Email công việc</th>")
             .AppendLine("     <th>Ngày vào công ty</th>")
             .AppendLine("     <th>Ngày chính thức</th>")
             .AppendLine("     <th>Loại Hợp đồng hiện tại</th>")
             .AppendLine("     <th>Ngày hết hạn HĐ</th>")
             .AppendLine("     <th>Tình trạng làm việc</th>")
             .AppendLine("     <th>Mức Lương cơ bản hiện tại</th>")
             .AppendLine("     <th>Số Tài khoản Ngân hàng</th>")
             .AppendLine("     <th>Tên Ngân hàng</th>")
             .AppendLine("     <th>Phương thức thanh toán</th>")
             .AppendLine("     <th>Số lượng người phụ thuộc</th>")
             .AppendLine("     <th>GroupId</th>")
             .AppendLine("     <th>Thứ tự</th>")
             .AppendLine("     <th>Ngày tạo</th>")
             .AppendLine("     <th>Người tạo</th>")
             .AppendLine("     <th>IsActive</th>")
             .AppendLine("     <th>Người duyệt</th>")
             .AppendLine("     <th>Ngày duyệt</th>")
             .AppendLine("     <th>Phòng ban duyệt</th>")
             .AppendLine("     <th>Phòng ban duyệt tiếp</th>")
             .AppendLine("     <th>Thứ tự phòng ban</th>")
             .AppendLine("     <th>Thứ tự duyệt</th>")
             .AppendLine("     <th>ID duyệt</th>")
             .AppendLine("     <th>Id duyệt cuối</th>")
             .AppendLine("    </tr>")
             .AppendLine("  </thead>")
             .AppendLine("  <tbody>");

            if (isValidModel.Any())
            {
                var oldData = isValidModel.FirstOrDefault();
                var newData = isValidModel.Count > 1 ? isValidModel[1] : null;

                sb.AppendLine("    <tr>")
                  .AppendLine("      <td class=\"text-center\">Dữ liệu cũ !</td>")
                  .AppendLine($"      <td>{oldData?.FullName}</td>")
                  .AppendLine($"      <td>{oldData?.Gender}</td>")
                  .AppendLine($"      <td>{oldData?.DateOfBirth}</td>")
                  .AppendLine($"      <td>{oldData?.IdentityCardNumber}</td>")
                  .AppendLine($"      <td>{oldData?.IdentityCardIssueDate}</td>")
                  .AppendLine($"      <td>{oldData?.IdentityCardIssuePlace}</td>")
                  .AppendLine($"      <td>{oldData?.TaxCode}</td>")
                  .AppendLine($"      <td>{oldData?.SocialInsuranceNumber}</td>")
                  .AppendLine($"      <td>{oldData?.MobilePhone}</td>")
                  .AppendLine($"      <td>{oldData?.PersonalEmail}</td>")
                  .AppendLine($"      <td>{oldData?.PermanentAddress}</td>")
                  .AppendLine($"      <td>{oldData?.CurrentAddress}</td>")
                  .AppendLine($"      <td>{oldData?.EmergencyContactName}</td>")
                  .AppendLine($"      <td>{oldData?.EmergencyContactPhone}</td>")
                  .AppendLine($"      <td>{oldData?.DepartmentFrId}</td>")
                  .AppendLine($"      <td>{oldData?.ChucVuId}</td>")
                  .AppendLine($"      <td>{oldData?.ManagerId}</td>")
                  .AppendLine($"      <td>{oldData?.WorkEmail}</td>")
                  .AppendLine($"      <td>{oldData?.HireDate}</td>")
                  .AppendLine($"      <td>{oldData?.OfficialDate}</td>")
                  .AppendLine($"      <td>{oldData?.CurrentContractType}</td>")
                  .AppendLine($"      <td>{oldData?.ContractExpirationDate}</td>")
                  .AppendLine($"      <td>{oldData?.WorkStatusId}</td>")
                  .AppendLine($"      <td>{oldData?.CurrentBasicSalary}</td>")
                  .AppendLine($"      <td>{oldData?.BankAccountNumber}</td>")
                  .AppendLine($"      <td>{oldData?.BankName}</td>")
                  .AppendLine($"      <td>{oldData?.SalaryPaymentMethod}</td>")
                  .AppendLine($"      <td>{oldData?.TaxDependentsCount}</td>")
                  .AppendLine($"      <td>{oldData?.GroupId}</td>")
                  .AppendLine($"      <td>{oldData?.Ordinarily}</td>")
                  .AppendLine($"      <td>{oldData?.CreateAt}</td>")
                  .AppendLine($"      <td>{oldData?.CreateBy}</td>")
                  .AppendLine($"      <td>{oldData?.IsActive}</td>")
                  .AppendLine($"      <td>{oldData?.ApprovalUserId}</td>")
                  .AppendLine($"      <td>{oldData?.DateApproval}</td>")
                  .AppendLine($"      <td>{oldData?.ApprovalDept}</td>")
                  .AppendLine($"      <td>{oldData?.DepartmentId}</td>")
                  .AppendLine($"      <td>{oldData?.DepartmentOrder}</td>")
                  .AppendLine($"      <td>{oldData?.ApprovalOrder}</td>")
                  .AppendLine($"      <td>{oldData?.ApprovalId}</td>")
                  .AppendLine($"      <td>{oldData?.LastApprovalId}</td>")
                  .AppendLine("    </tr>");

                sb.AppendLine("    <tr>")
                  .AppendLine("      <td class=\"text-center\">Dữ liệu mới !</td>")
                  .AppendLine($"      <td>{newData?.FullName}</td>")
                  .AppendLine($"      <td>{newData?.Gender}</td>")
                  .AppendLine($"      <td>{newData?.DateOfBirth}</td>")
                  .AppendLine($"      <td>{newData?.IdentityCardNumber}</td>")
                  .AppendLine($"      <td>{newData?.IdentityCardIssueDate}</td>")
                  .AppendLine($"      <td>{newData?.IdentityCardIssuePlace}</td>")
                  .AppendLine($"      <td>{newData?.TaxCode}</td>")
                  .AppendLine($"      <td>{newData?.SocialInsuranceNumber}</td>")
                  .AppendLine($"      <td>{newData?.MobilePhone}</td>")
                  .AppendLine($"      <td>{newData?.PersonalEmail}</td>")
                  .AppendLine($"      <td>{newData?.PermanentAddress}</td>")
                  .AppendLine($"      <td>{newData?.CurrentAddress}</td>")
                  .AppendLine($"      <td>{newData?.EmergencyContactName}</td>")
                  .AppendLine($"      <td>{newData?.EmergencyContactPhone}</td>")
                  .AppendLine($"      <td>{newData?.DepartmentFrId}</td>")
                  .AppendLine($"      <td>{newData?.ChucVuId}</td>")
                  .AppendLine($"      <td>{newData?.ManagerId}</td>")
                  .AppendLine($"      <td>{newData?.WorkEmail}</td>")
                  .AppendLine($"      <td>{newData?.HireDate}</td>")
                  .AppendLine($"      <td>{newData?.OfficialDate}</td>")
                  .AppendLine($"      <td>{newData?.CurrentContractType}</td>")
                  .AppendLine($"      <td>{newData?.ContractExpirationDate}</td>")
                  .AppendLine($"      <td>{newData?.WorkStatusId}</td>")
                  .AppendLine($"      <td>{newData?.CurrentBasicSalary}</td>")
                  .AppendLine($"      <td>{newData?.BankAccountNumber}</td>")
                  .AppendLine($"      <td>{newData?.BankName}</td>")
                  .AppendLine($"      <td>{newData?.SalaryPaymentMethod}</td>")
                  .AppendLine($"      <td>{newData?.TaxDependentsCount}</td>")
                  .AppendLine($"      <td>{newData?.GroupId}</td>")
                  .AppendLine($"      <td>{newData?.Ordinarily}</td>")
                  .AppendLine($"      <td>{newData?.CreateAt}</td>")
                  .AppendLine($"      <td>{newData?.CreateBy}</td>")
                  .AppendLine($"      <td>{newData?.IsActive}</td>")
                  .AppendLine($"      <td>{newData?.ApprovalUserId}</td>")
                  .AppendLine($"      <td>{newData?.DateApproval}</td>")
                  .AppendLine($"      <td>{newData?.ApprovalDept}</td>")
                  .AppendLine($"      <td>{newData?.DepartmentId}</td>")
                  .AppendLine($"      <td>{newData?.DepartmentOrder}</td>")
                  .AppendLine($"      <td>{newData?.ApprovalOrder}</td>")
                  .AppendLine($"      <td>{newData?.ApprovalId}</td>")
                  .AppendLine($"      <td>{newData?.LastApprovalId}</td>")
                  .AppendLine("    </tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class=\"text-danger\">Không có dữ liêụ !</td></tr>");
            }

            sb.AppendLine("  </tbody>")
              .AppendLine("</table>");

            return sb.ToString();
        }

        private async Task<string> BuildEmailContentAdd(string id)
        {
            var detail = await _nhansuemployeeprofileService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Thêm - NhanSuEmployeeProfile</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
              .AppendLine("  <thead class=\"bg-info\">")
              .AppendLine("    <tr>")
              .AppendLine("     <th>Họ và tên</th>")
              .AppendLine("     <th>Giới tính</th>")
              .AppendLine("     <th>Ngày sinh</th>")
              .AppendLine("     <th>Số CCCD/CMND</th>")
              .AppendLine("     <th>Ngày cấp CCCD</th>")
              .AppendLine("     <th>Nơi cấp CCCD</th>")
              .AppendLine("     <th>Mã số Thuế TNCN</th>")
              .AppendLine("     <th>Số sổ BHXH</th>")
              .AppendLine("     <th>Điện thoại di động</th>")
              .AppendLine("     <th>Email cá nhân</th>")
              .AppendLine("     <th>Địa chỉ thường trú</th>")
              .AppendLine("     <th>Địa chỉ hiện tại</th>")
              .AppendLine("     <th>Tên người liên hệ khẩn cấp</th>")
              .AppendLine("     <th>Mã người liên hệ khẩn cấp</th>")
              .AppendLine("     <th>ID Phòng ban</th>")
              .AppendLine("     <th>ID Chức danh/Vị trí</th>")
              .AppendLine("     <th>Mã Quản lý trực tiếp</th>")
              .AppendLine("     <th>Email công việc</th>")
              .AppendLine("     <th>Ngày vào công ty</th>")
              .AppendLine("     <th>Ngày chính thức</th>")
              .AppendLine("     <th>Loại Hợp đồng hiện tại</th>")
              .AppendLine("     <th>Ngày hết hạn HĐ</th>")
              .AppendLine("     <th>Tình trạng làm việc</th>")
              .AppendLine("     <th>Mức Lương cơ bản hiện tại</th>")
              .AppendLine("     <th>Số Tài khoản Ngân hàng</th>")
              .AppendLine("     <th>Tên Ngân hàng</th>")
              .AppendLine("     <th>Phương thức thanh toán</th>")
              .AppendLine("     <th>Số lượng người phụ thuộc</th>")
              .AppendLine("     <th>GroupId</th>")
              .AppendLine("     <th>Thứ tự</th>")
              .AppendLine("     <th>Ngày tạo</th>")
              .AppendLine("     <th>Người tạo</th>")
              .AppendLine("     <th>IsActive</th>")
              .AppendLine("     <th>Người duyệt</th>")
              .AppendLine("     <th>Ngày duyệt</th>")
              .AppendLine("     <th>Phòng ban duyệt</th>")
              .AppendLine("     <th>Phòng ban duyệt tiếp</th>")
              .AppendLine("     <th>Thứ tự phòng ban</th>")
              .AppendLine("     <th>Thứ tự duyệt</th>")
              .AppendLine("     <th>ID duyệt</th>")
              .AppendLine("     <th>Id duyệt cuối</th>")
              .AppendLine("    </tr>")
              .AppendLine("  </thead>")
              .AppendLine("  <tbody>");
            sb.AppendLine($"    <tr>")
              .AppendLine($"     <td>{detail.FullName}</td>")
              .AppendLine($"     <td>{detail.Gender}</td>")
              .AppendLine($"     <td>{detail.DateOfBirth}</td>")
              .AppendLine($"     <td>{detail.IdentityCardNumber}</td>")
              .AppendLine($"     <td>{detail.IdentityCardIssueDate}</td>")
              .AppendLine($"     <td>{detail.IdentityCardIssuePlace}</td>")
              .AppendLine($"     <td>{detail.TaxCode}</td>")
              .AppendLine($"     <td>{detail.SocialInsuranceNumber}</td>")
              .AppendLine($"     <td>{detail.MobilePhone}</td>")
              .AppendLine($"     <td>{detail.PersonalEmail}</td>")
              .AppendLine($"     <td>{detail.PermanentAddress}</td>")
              .AppendLine($"     <td>{detail.CurrentAddress}</td>")
              .AppendLine($"     <td>{detail.EmergencyContactName}</td>")
              .AppendLine($"     <td>{detail.EmergencyContactPhone}</td>")
              .AppendLine($"     <td>{detail.DepartmentFrId}</td>")
              .AppendLine($"     <td>{detail.ChucVuId}</td>")
              .AppendLine($"     <td>{detail.ManagerId}</td>")
              .AppendLine($"     <td>{detail.WorkEmail}</td>")
              .AppendLine($"     <td>{detail.HireDate}</td>")
              .AppendLine($"     <td>{detail.OfficialDate}</td>")
              .AppendLine($"     <td>{detail.CurrentContractType}</td>")
              .AppendLine($"     <td>{detail.ContractExpirationDate}</td>")
              .AppendLine($"     <td>{detail.WorkStatusId}</td>")
              .AppendLine($"     <td>{detail.CurrentBasicSalary}</td>")
              .AppendLine($"     <td>{detail.BankAccountNumber}</td>")
              .AppendLine($"     <td>{detail.BankName}</td>")
              .AppendLine($"     <td>{detail.SalaryPaymentMethod}</td>")
              .AppendLine($"     <td>{detail.TaxDependentsCount}</td>")
              .AppendLine($"     <td>{detail.GroupId}</td>")
              .AppendLine($"     <td>{detail.Ordinarily}</td>")
              .AppendLine($"     <td>{detail.CreateAt}</td>")
              .AppendLine($"     <td>{detail.CreateBy}</td>")
              .AppendLine($"     <td>{detail.IsActive}</td>")
              .AppendLine($"     <td>{detail.ApprovalUserId}</td>")
              .AppendLine($"     <td>{detail.DateApproval}</td>")
              .AppendLine($"     <td>{detail.ApprovalDept}</td>")
              .AppendLine($"     <td>{detail.DepartmentId}</td>")
              .AppendLine($"     <td>{detail.DepartmentOrder}</td>")
              .AppendLine($"     <td>{detail.ApprovalOrder}</td>")
              .AppendLine($"     <td>{detail.ApprovalId}</td>")
              .AppendLine($"     <td>{detail.LastApprovalId}</td>")
              .AppendLine($"    </tr>")
              .AppendLine("  </tbody></table>");
            return sb.ToString();
        }

        private async Task<string> BuildEmailContentDelete(string id)
        {
            var detail = await _nhansuemployeeprofileService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Xóa - NhanSuEmployeeProfile</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
              .AppendLine("  <thead class=\"bg-info\">")
              .AppendLine("    <tr>")
              .AppendLine("     <th>Họ và tên</th>")
              .AppendLine("     <th>Giới tính</th>")
              .AppendLine("     <th>Ngày sinh</th>")
              .AppendLine("     <th>Số CCCD/CMND</th>")
              .AppendLine("     <th>Ngày cấp CCCD</th>")
              .AppendLine("     <th>Nơi cấp CCCD</th>")
              .AppendLine("     <th>Mã số Thuế TNCN</th>")
              .AppendLine("     <th>Số sổ BHXH</th>")
              .AppendLine("     <th>Điện thoại di động</th>")
              .AppendLine("     <th>Email cá nhân</th>")
              .AppendLine("     <th>Địa chỉ thường trú</th>")
              .AppendLine("     <th>Địa chỉ hiện tại</th>")
              .AppendLine("     <th>Tên người liên hệ khẩn cấp</th>")
              .AppendLine("     <th>Mã người liên hệ khẩn cấp</th>")
              .AppendLine("     <th>ID Phòng ban</th>")
              .AppendLine("     <th>ID Chức danh/Vị trí</th>")
              .AppendLine("     <th>Mã Quản lý trực tiếp</th>")
              .AppendLine("     <th>Email công việc</th>")
              .AppendLine("     <th>Ngày vào công ty</th>")
              .AppendLine("     <th>Ngày chính thức</th>")
              .AppendLine("     <th>Loại Hợp đồng hiện tại</th>")
              .AppendLine("     <th>Ngày hết hạn HĐ</th>")
              .AppendLine("     <th>Tình trạng làm việc</th>")
              .AppendLine("     <th>Mức Lương cơ bản hiện tại</th>")
              .AppendLine("     <th>Số Tài khoản Ngân hàng</th>")
              .AppendLine("     <th>Tên Ngân hàng</th>")
              .AppendLine("     <th>Phương thức thanh toán</th>")
              .AppendLine("     <th>Số lượng người phụ thuộc</th>")
              .AppendLine("     <th>GroupId</th>")
              .AppendLine("     <th>Thứ tự</th>")
              .AppendLine("     <th>Ngày tạo</th>")
              .AppendLine("     <th>Người tạo</th>")
              .AppendLine("     <th>IsActive</th>")
              .AppendLine("     <th>Người duyệt</th>")
              .AppendLine("     <th>Ngày duyệt</th>")
              .AppendLine("     <th>Phòng ban duyệt</th>")
              .AppendLine("     <th>Phòng ban duyệt tiếp</th>")
              .AppendLine("     <th>Thứ tự phòng ban</th>")
              .AppendLine("     <th>Thứ tự duyệt</th>")
              .AppendLine("     <th>ID duyệt</th>")
              .AppendLine("     <th>Id duyệt cuối</th>")
              .AppendLine("    </tr>")
              .AppendLine("  </thead>")
              .AppendLine("  <tbody>");
            sb.AppendLine($"    <tr>")
              .AppendLine($"     <td>{detail.FullName}</td>")
              .AppendLine($"     <td>{detail.Gender}</td>")
              .AppendLine($"     <td>{detail.DateOfBirth}</td>")
              .AppendLine($"     <td>{detail.IdentityCardNumber}</td>")
              .AppendLine($"     <td>{detail.IdentityCardIssueDate}</td>")
              .AppendLine($"     <td>{detail.IdentityCardIssuePlace}</td>")
              .AppendLine($"     <td>{detail.TaxCode}</td>")
              .AppendLine($"     <td>{detail.SocialInsuranceNumber}</td>")
              .AppendLine($"     <td>{detail.MobilePhone}</td>")
              .AppendLine($"     <td>{detail.PersonalEmail}</td>")
              .AppendLine($"     <td>{detail.PermanentAddress}</td>")
              .AppendLine($"     <td>{detail.CurrentAddress}</td>")
              .AppendLine($"     <td>{detail.EmergencyContactName}</td>")
              .AppendLine($"     <td>{detail.EmergencyContactPhone}</td>")
              .AppendLine($"     <td>{detail.DepartmentFrId}</td>")
              .AppendLine($"     <td>{detail.ChucVuId}</td>")
              .AppendLine($"     <td>{detail.ManagerId}</td>")
              .AppendLine($"     <td>{detail.WorkEmail}</td>")
              .AppendLine($"     <td>{detail.HireDate}</td>")
              .AppendLine($"     <td>{detail.OfficialDate}</td>")
              .AppendLine($"     <td>{detail.CurrentContractType}</td>")
              .AppendLine($"     <td>{detail.ContractExpirationDate}</td>")
              .AppendLine($"     <td>{detail.WorkStatusId}</td>")
              .AppendLine($"     <td>{detail.CurrentBasicSalary}</td>")
              .AppendLine($"     <td>{detail.BankAccountNumber}</td>")
              .AppendLine($"     <td>{detail.BankName}</td>")
              .AppendLine($"     <td>{detail.SalaryPaymentMethod}</td>")
              .AppendLine($"     <td>{detail.TaxDependentsCount}</td>")
              .AppendLine($"     <td>{detail.GroupId}</td>")
              .AppendLine($"     <td>{detail.Ordinarily}</td>")
              .AppendLine($"     <td>{detail.CreateAt}</td>")
              .AppendLine($"     <td>{detail.CreateBy}</td>")
              .AppendLine($"     <td>{detail.IsActive}</td>")
              .AppendLine($"     <td>{detail.ApprovalUserId}</td>")
              .AppendLine($"     <td>{detail.DateApproval}</td>")
              .AppendLine($"     <td>{detail.ApprovalDept}</td>")
              .AppendLine($"     <td>{detail.DepartmentId}</td>")
              .AppendLine($"     <td>{detail.DepartmentOrder}</td>")
              .AppendLine($"     <td>{detail.ApprovalOrder}</td>")
              .AppendLine($"     <td>{detail.ApprovalId}</td>")
              .AppendLine($"     <td>{detail.LastApprovalId}</td>")
              .AppendLine($"    </tr>")
              .AppendLine("  </tbody></table>");
            return sb.ToString();
        }

        private ApprovalTask CreateApprovalTask(string title, string content, NhanSu_EmployeeProfile input, string tableName, string parentMajorId, string majorId, string userId)
        {
            return new ApprovalTask
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Content = content,
                IsActive = input.IsActive,
                OriginalId = input.Id,
                RelatedTable = tableName,
                ParentMajorId = parentMajorId,
                MajorId = majorId,
                CompanyId = input.GroupId,
                GroupId = input.GroupId,
                Ordinarily = input.Ordinarily,
                ApprovalUserId = input.ApprovalUserId,
                DateApproval = input.DateApproval,
                ApprovalDept = input.ApprovalDept,
                DepartmentId = input.DepartmentId ?? "",
                DepartmentOrder = input.DepartmentOrder,
                ApprovalOrder = input.ApprovalOrder,
                ApprovalId = input.ApprovalId ?? "",
                LastApprovalId = input.LastApprovalId ?? "",
                IsStatus = input.IsStatus,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
        }

        private ApprovalTask UpdateApprovalTask(string Id, string title, string content, NhanSu_EmployeeProfile input, string tableName, string parentMajorId, string majorId, string userId)
        {
            return new ApprovalTask
            {
                Id = Id,
                Title = title,
                Content = content,
                IsActive = input.IsActive,
                OriginalId = input.Id,
                RelatedTable = tableName,
                ParentMajorId = parentMajorId,
                MajorId = majorId,
                CompanyId = input.GroupId,
                GroupId = input.GroupId,
                Ordinarily = input.Ordinarily,
                ApprovalUserId = input.ApprovalUserId,
                DateApproval = input.DateApproval,
                ApprovalDept = input.ApprovalDept,
                DepartmentId = input.DepartmentId ?? "",
                DepartmentOrder = input.DepartmentOrder,
                ApprovalOrder = input.ApprovalOrder,
                ApprovalId = input.ApprovalId ?? "",
                LastApprovalId = input.LastApprovalId ?? "",
                IsStatus = input.IsStatus,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
        }

        private async Task InsertEmailHistories(string content, string subject, string companyId, string groupId, string approvalId, string idCheck, string userId)
        {
            var listEmail = await _emailHistoryService.GetUserPermission(companyId, approvalId);
            var listInsert = listEmail.Select(item => new EmailHistory
            {
                Id = Guid.NewGuid().ToString(),
                Receiver = item.Mail,
                Subject = subject,
                Content = content,
                CompanyId = companyId,
                UserId = item.UserId,
                ParentMajorId = item.ParentMajorId,
                MajorId = item.MajorId,
                IdCheck = idCheck,
                IdLog = "",
                IsMail = true,
                IsNotification = true,
                IsSMS = true,
                GroupId = groupId,
                CreateAt = DateTime.Now,
                CreateBy = userId,
                IsRead = 0
            }).ToList();

            await _emailHistoryService.InsertMulti(listInsert);
        }

    }
}
