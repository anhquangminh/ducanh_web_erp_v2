using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.CongTrinh.DanhMuc;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DucAnh2025.Controllers.API.CongTrinh.DanhMuc
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DMHinhThucDapTraController : ControllerBase
    {
        private readonly IApprovalTaskRepository _approvalTaskService;
        private readonly IDM_HinhThucDapTraRepository _hinhThucDapTraService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IPermissionRepository _permissionService;
        public DMHinhThucDapTraController(IApprovalTaskRepository approvalTaskRepository ,
            IDM_HinhThucDapTraRepository dM_HinhThucDapTraRepository,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService,
            IPermissionRepository permissionService)
        {
            _approvalTaskService = approvalTaskRepository;
            _hinhThucDapTraService = dM_HinhThucDapTraRepository;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
            _permissionService = permissionService;
        }

        public string parentMajorId = "85fe739a-0bfa-4f8e-9ab9-55a85e5670d9";
        public string tableName = "CT_DM_HinhThucDapTras";

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _hinhThucDapTraService.GetAll(groupId);
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
                var result = await _hinhThucDapTraService.GetById(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

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
                var result = await _hinhThucDapTraService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CT_DM_HinhThucDapTra input, [FromQuery] bool isEdit)
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


                // Lấy quyền
                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));

                var firstPer = listPer.First();
                var majorId = firstPer.MajorId;
                var idTao = listPer.FirstOrDefault(p => p.PermissionType == 3)?.Id;
                var idSua = listPer.FirstOrDefault(p => p.PermissionType == 4)?.Id;

                if (editCheck)
                {
                    // Quyền sửa
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idSua))
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền sửa", null));

                    // Check dữ liệu
                    if (!await _hinhThucDapTraService.CheckExclusive(new[] { input.Id }, now))
                        return Ok(new ApiResponse<object>(false, "Không thể sửa dữ liệu", null));

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

                    await _hinhThucDapTraService.Update(input, "");

                    // Nội dung email
                    string content = await BuildEmailContentEdit(input.Id);
                    string contentApp = await BuildEmailContentAppEdit(input.Id);

                    // Tạo ApprovalTask
                    var dataApproval = await _approvalTaskService.GetByOriginalId(input.Id);
                    if (dataApproval == null)
                    {
                        await _approvalTaskService.Insert(CreateApprovalTask("Danh mục hình thức đắp trả - Sửa", contentApp, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    else
                    {
                        await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Danh mục hình thức đắp trả - Sửa", contentApp, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    
                    // Gửi email
                    await InsertEmailHistories(content, "Danh mục hình thức đắp trả - Sửa.", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                }
                else
                {
                    // Quyền thêm
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idTao))
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền thêm mới", null));

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
                    await _hinhThucDapTraService.Insert(input, "");
                    string content = await BuildEmailContentAdd(input.Id);
                    string contentApp = await BuildEmailContentAppAdd(input.Id);
                    await _approvalTaskService.Insert(CreateApprovalTask("Danh mục hình thức đắp trả - Thêm mới", contentApp, input, tableName, parentMajorId, majorId, userId), userId);

                    await InsertEmailHistories(content, "Danh mục hình thức đắp trả - Thêm", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

                    return Ok(new ApiResponse<object>(true, "Lưu thành công.", null));
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
                return Ok(new ApiResponse<object>(false, "Không xác định được dữ liệu cần xóa.", null));

            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _hinhThucDapTraService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy dữ liệu.", null));

                if (query.IsActive == 2)
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));

                if (!await _hinhThucDapTraService.CheckDelete(query))
                    return Ok(new ApiResponse<object>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));

                var majorId = listPer.FirstOrDefault()?.MajorId;
                var deletePermissionId = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (!await _phanQuyenService.CheckPermission(user.GroupId, query.GroupId, user, deletePermissionId))
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa", null));

                if (!await _hinhThucDapTraService.CheckExclusive(new[] { id }, baseTime))
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

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

                await _hinhThucDapTraService.Update(query, "");

                var content = await BuildEmailContentDelete(id);
                var contentApp = await BuildEmailContentAppDelete(id);
                var dataApproval = await _approvalTaskService.GetByOriginalId(id);
                if (dataApproval == null)
                {
                    await _approvalTaskService.Insert(CreateApprovalTask("Danh mục hình thức đắp trả - xóa", contentApp, query, tableName, parentMajorId, majorId, userId), userId);
                }
                else
                {
                    await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Danh mục hình thức đắp trả - xóa", contentApp, query, tableName, parentMajorId, majorId, userId), userId);
                }
                // Hàm gửi email đã có → dùng lại
                await InsertEmailHistories(content, "Danh mục hình thức đắp trả - xóa", query.GroupId, groupId, firstApproval.Id, query.Id, userId);

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
                var entity = await _hinhThucDapTraService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt", null));

                string thongbao = entity.IsStatus;

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));
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
                            entity.IsStatus = "Đã duyệt xóa";
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

                    await _hinhThucDapTraService.Approval(entity, userId);
                    return Ok(new ApiResponse<object>(true, "Đã duyệt " + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt.", null));
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
                var entity = await _hinhThucDapTraService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _hinhThucDapTraService.NoApproval(entity, userId);
                    var dataNoApproval = await _hinhThucDapTraService.GetById(entity.Id);

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

                    return Ok(new ApiResponse<object>(true, "Đã hủy duyệt " + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt, không thể hủy duyệt.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        private async Task<string> BuildEmailContentEdit(string id)
        {
            var isValidModel = await _hinhThucDapTraService.GetHistoryIsValidEdit(id);
            var sb = new StringBuilder();

            sb.AppendLine("<h3>Thông tin sửa - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
              .AppendLine("  <thead class=\"bg-info\">")
              .AppendLine("    <tr>")
              .AppendLine("      <th class=\"text-center\">Loại dữ liệu</th>")
              .AppendLine("      <th>Tên danh mục</th>")
              .AppendLine("      <th>Trạng thái</th>")
              .AppendLine("      <td>Người thao tác</td>")
              .AppendLine("      <td>Thời gian</td>")
              .AppendLine("    </tr>")
              .AppendLine("  </thead>")
              .AppendLine("  <tbody>");

            if (isValidModel.Any())
            {
                var oldData = isValidModel.FirstOrDefault();
                var newData = isValidModel.Count > 1 ? isValidModel[1] : null;

                sb.AppendLine("    <tr>")
                  .AppendLine("      <td class=\"text-center\">Dữ liệu cũ</td>")
                  .AppendLine($"    <td>{oldData?.TenDanhMuc ?? ""}</td>")
                  .AppendLine($"    <td>{oldData?.IsStatus}</td>")
                  .AppendLine($"    <td>{oldData?.CreateBy}</td>")
                  .AppendLine($"    <td>{oldData?.CreateAt}</td>")
                  .AppendLine("    </tr>");

                // Dòng dữ liệu mới
                sb.AppendLine("    <tr>")
                  .AppendLine("      <td class=\"text-center\">Dữ liệu mới</td>")
                  .AppendLine($"     <td>{newData?.TenDanhMuc ?? ""}</td>")
                  .AppendLine($"     <td>{oldData?.IsStatus}</td>")
                  .AppendLine($"     <td>{oldData?.CreateBy}</td>")
                  .AppendLine($"     <td>{oldData?.CreateAt}</td>")
                  .AppendLine("    </tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class=\"text-danger\" colspan=\"6\">Không có dữ liệu</td></tr>");
            }

            sb.AppendLine("  </tbody>")
              .AppendLine("</table>");

            return sb.ToString();
        }
        private async Task<string> BuildEmailContentAdd(string id)
        {
            var detail = await _hinhThucDapTraService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Thêm - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
                .AppendLine("  <thead class=\"bg-info\">")
                .AppendLine("    <tr>")
                .AppendLine("     <th>Tên danh mục</th>")
                .AppendLine("     <th>Trạng thái</th>")
                .AppendLine("     <td>Người thao tác</td>")
                .AppendLine("     <td>Thời gian</td>")
                .AppendLine("   </tr>")
                .AppendLine("  </thead>")
                .AppendLine("  <tbody>")
                .AppendLine($"    <tr>")
                .AppendLine($"     <td>{detail.TenDanhMuc}</td>")
                .AppendLine($"     <td>{detail.IsStatus}</td>")
                .AppendLine($"     <td>{detail.CreateBy}</td>")
                .AppendLine($"     <td>{detail.CreateAt}</td>")
                .AppendLine($"    </tr>")
                .AppendLine("  </tbody></table>");
            return sb.ToString();
        }
        private async Task<string> BuildEmailContentDelete(string id)
        {
            var detail = await _hinhThucDapTraService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Xóa - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
            .AppendLine("  <thead class=\"bg-info\">")
            .AppendLine("    <tr>")
            .AppendLine("     <th>Tên danh mục</th>")
            .AppendLine("     <th>Trạng thái</th>")
            .AppendLine("     <td>Người thao tác</td>")
            .AppendLine("     <td>Thời gian</td>")
            .AppendLine("   </tr>")
            .AppendLine("  </thead>")
            .AppendLine("  <tbody>")
            .AppendLine($"    <tr>")
            .AppendLine($"     <td>{detail.TenDanhMuc}</td>")
            .AppendLine($"     <td>{detail.IsStatus}</td>")
            .AppendLine($"     <td>{detail.CreateBy}</td>")
            .AppendLine($"     <td>{detail.CreateAt}</td>")
            .AppendLine($"    </tr>")
            .AppendLine("  </tbody></table>");

            return sb.ToString();
        }


        private async Task<string> BuildEmailContentAppEdit(string id)
        {
            var isValidModel = await _hinhThucDapTraService.GetHistoryIsValidEdit(id);
            var sb = new StringBuilder();

            sb.AppendLine("<h3 style='color:#0d6efd;'>Thông tin sửa - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class='table table-hover table-bordered' style='border-collapse: collapse; width: 80%; text-align: center;'>")
              .AppendLine("  <thead style='background-color: #e0f0ff;'>")
              .AppendLine("    <tr>")
              .AppendLine("      <th>Loại dữ liệu</th>")
              .AppendLine("      <th>Tên danh mục</th>")
              .AppendLine("      <th>Trạng thái</th>")
              .AppendLine("      <th>Người thao tác</th>")
              .AppendLine("      <th>Thời gian</th>")
              .AppendLine("    </tr>")
              .AppendLine("  </thead>")
              .AppendLine("  <tbody>");

            if (isValidModel != null && isValidModel.Any())
            {
                var oldData = isValidModel.FirstOrDefault();
                var newData = await _hinhThucDapTraService.GetDetails(id);

                // --- Dữ liệu cũ ---
                sb.AppendLine("    <tr style='background-color:#fff3cd;'>")
                  .AppendLine("      <td><b>Dữ liệu cũ</b></td>")
                  .AppendLine($"     <td>{oldData?.TenDanhMuc ?? ""}</td>")
                  .AppendLine($"     <td>{oldData?.IsStatus}</td>")
                  .AppendLine($"     <td>{oldData?.CreateBy}</td>")
                  .AppendLine($"     <td>{oldData?.CreateAt}</td>")
                  .AppendLine("    </tr>");

                // --- Dữ liệu mới ---
                sb.AppendLine("    <tr style='background-color:#d1e7dd;'>")
                  .AppendLine("      <td><b>Dữ liệu mới</b></td>")
                  .AppendLine($"     <td>{newData?.TenDanhMuc ?? ""}</td>")
                  .AppendLine($"     <td>{newData?.IsStatus}</td>")
                  .AppendLine($"     <td>{newData?.CreateBy}</td>")
                  .AppendLine($"     <td>{newData?.CreateAt}</td>")
                  .AppendLine("    </tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class='text-danger' colspan='5'>Không có dữ liệu</td></tr>");
            }

            sb.AppendLine("  </tbody>")
              .AppendLine("</table>");

            return sb.ToString();
        }
        private async Task<string> BuildEmailContentAppAdd(string id)
        {
            var detail = await _hinhThucDapTraService.GetDetails(id);
            var sb = new StringBuilder();

            sb.AppendLine("<h3 style='color:#198754;'>Thêm - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class='table table-hover table-bordered' style='border-collapse: collapse; width: 80%; text-align: center;'>")
              .AppendLine("  <thead style='background-color: #e0f0ff;'>")
              .AppendLine("    <tr>")
              .AppendLine("      <th>Tên danh mục</th>")
              .AppendLine("      <th>Trạng thái</th>")
              .AppendLine("      <th>Người thao tác</th>")
              .AppendLine("      <th>Thời gian</th>")
              .AppendLine("    </tr>")
              .AppendLine("  </thead>")
              .AppendLine("  <tbody>");

            if (detail != null)
            {
                sb.AppendLine("    <tr style='background-color:#d1e7dd;'>") // xanh nhạt
                  .AppendLine($"     <td>{detail.TenDanhMuc ?? ""}</td>")
                  .AppendLine($"     <td>{detail.IsStatus}</td>")
                  .AppendLine($"     <td>{detail.CreateBy}</td>")
                  .AppendLine($"     <td>{detail.CreateAt}</td>")
                  .AppendLine("    </tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class='text-danger' colspan='4'>Không có dữ liệu</td></tr>");
            }

            sb.AppendLine("  </tbody>")
              .AppendLine("</table>");

            return sb.ToString();
        }
        private async Task<string> BuildEmailContentAppDelete(string id)
        {
            var detail = await _hinhThucDapTraService.GetDetails(id);
            var sb = new StringBuilder();

            sb.AppendLine("<h3 style='color:#c9302c;'>Xóa - Danh mục hình thức đắp trả</h3>")
              .AppendLine("<table class='table table-hover table-bordered' style='width: 60%; border-collapse: collapse;'>")
              .AppendLine("  <tbody>");

            if (detail != null)
            {
                sb.AppendLine("    <tr><th style='width:35%; background-color:#f8d7da;'>Tên danh mục</th><td>" + detail.TenDanhMuc + "</td></tr>")
                  .AppendLine("    <tr><th style='background-color:#f8d7da;'>Trạng thái</th><td>" + detail.IsStatus + "</td></tr>")
                  .AppendLine("    <tr><th style='background-color:#f8d7da;'>Người thao tác</th><td>" + detail.CreateBy + "</td></tr>")
                  .AppendLine("    <tr><th style='background-color:#f8d7da;'>Thời gian</th><td>" + detail.CreateAt + "</td></tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class='text-danger' colspan='2'>Không có dữ liệu</td></tr>");
            }

            sb.AppendLine("  </tbody>")
              .AppendLine("</table>");

            return sb.ToString();
        }


        private ApprovalTask CreateApprovalTask(string title, string content, CT_DM_HinhThucDapTra input, string tableName, string parentMajorId, string majorId, string userId)
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
        private ApprovalTask UpdateApprovalTask(string Id, string title, string content, CT_DM_HinhThucDapTra input, string tableName, string parentMajorId, string majorId, string userId)
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
