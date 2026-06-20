using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DucAnh2025.Controllers.API.NhanSu
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NhanSuTerminationController : ControllerBase
    {
        private readonly IApprovalTaskRepository _approvalTaskService;
        private readonly INhanSu_TerminationRepository _nhansuterminationService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IPermissionRepository _permissionService;

        public NhanSuTerminationController(IApprovalTaskRepository approvalTaskRepository,
            INhanSu_TerminationRepository nhansuterminationRepository,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService,
            IPermissionRepository permissionService)
        {
            _approvalTaskService = approvalTaskRepository;
            _nhansuterminationService = nhansuterminationRepository;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
            _permissionService = permissionService;
        }

        public string parentMajorId = "09564cf6-71a5-4721-bc27-4c54fafd112b";
        public string tableName = "NhanSu_Terminations";

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByVM(string groupId, [FromBody] NhanSu_TerminationModel? input = null)
        {
            try
            {
                var model = input ?? new NhanSu_TerminationModel();
                var list = await _nhansuterminationService.GetAllByVM(model, groupId);
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
                var list = await _nhansuterminationService.GetAll(groupId);
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
                var result = await _nhansuterminationService.GetById(id);
                return Ok(new ApiResponse<object>(true, "Thành công .", result));
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
                var result = await _nhansuterminationService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công .", result));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] NhanSu_Termination input, [FromQuery] bool isEdit)
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
                        return Ok(new ApiResponse<object>(false, "Banj không có quyền sửa !", null));

                    if (!await _nhansuterminationService.CheckExclusive(new[] { input.Id }, now))
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

                    await _nhansuterminationService.Update(input, "");

                    string content = await BuildEmailContentEdit(input.Id);

                    var dataApproval = await _approvalTaskService.GetByOriginalId(input.Id);
                    if (dataApproval == null)
                    {
                        await _approvalTaskService.Insert(CreateApprovalTask("Nghỉ việc- sửa !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    else
                    {
                        await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Nghỉ việc- sửa !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }

                    await InsertEmailHistories(content, "Nghỉ việc- sửa !", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

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
                    await _nhansuterminationService.Insert(input, "");
                    string content = await BuildEmailContentAdd(input.Id);
                    await _approvalTaskService.Insert(CreateApprovalTask("Nghỉ việcThêm mơí !", content, input, tableName, parentMajorId, majorId, userId), userId);
                    await InsertEmailHistories(content, "Nghỉ việc - Thêm", input.GroupId, groupId, firstApproval.Id, input.Id, userId);
                    return Ok(new ApiResponse<object>(true, "Lưu thành công .", null));
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

                var query = await _nhansuterminationService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy dữ liệu !", null));

                if (query.IsActive == 2)
                    return Ok(new ApiResponse<object>(false, "Đang chờ duyệt !", null));

                if (!await _nhansuterminationService.CheckDelete(query))
                    return Ok(new ApiResponse<object>(false, "Không thể xóa - đã liên kết dữ liệu !", null));

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng này !", null));

                var majorId = listPer.FirstOrDefault()?.MajorId;
                var deletePermissionId = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (!await _phanQuyenService.CheckPermission(user.GroupId, query.GroupId, user, deletePermissionId))
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa !", null));

                if (!await _nhansuterminationService.CheckExclusive(new[] { id }, baseTime))
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

                await _nhansuterminationService.Update(query, "");

                var content = await BuildEmailContentDelete(id);
                var dataApproval = await _approvalTaskService.GetByOriginalId(id);
                if (dataApproval == null)
                {
                    await _approvalTaskService.Insert(CreateApprovalTask("Nghỉ việc - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }
                else
                {
                    await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "Nghỉ việc - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }

                await InsertEmailHistories(content, "Nghỉ việc - xóa", query.GroupId, groupId, firstApproval.Id, query.Id, userId);

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
                var entity = await _nhansuterminationService.GetById(id);
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

                    await _nhansuterminationService.Approval(entity, userId);
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
                var entity = await _nhansuterminationService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn !", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt !", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _nhansuterminationService.NoApproval(entity, userId);
                    var dataNoApproval = await _nhansuterminationService.GetById(entity.Id);

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
            var isValidModel = await _nhansuterminationService.GetHistoryIsValidEdit(id);
            var sb = new StringBuilder();

            sb.AppendLine("Thông tin sửa NhanSuTermination</h3>")
             .AppendLine("<table class=\"table table-hover table-bordered\">")
             .AppendLine("  <thead class=\"bg-info\">")
             .AppendLine("    <tr>")
             .AppendLine("     <th>Mã nhân viên</th>")
             .AppendLine("     <th>Ngày nộp đơn</th>")
             .AppendLine("     <th>Ngày nghỉ việc mong muốn</th>")
             .AppendLine("     <th>Ngày nghỉ việc chính thức</th>")
             .AppendLine("     <th>Lý do nghỉ việc</th>")
             .AppendLine("     <th>Mã Quyết định nghỉ việc</th>")
             .AppendLine("     <th>Đường dẫn File Đơn/QĐ</th>")
             .AppendLine("     <th>Ngày thanh lý</th>")
             .AppendLine("     <th>Ghi chú từ HR</th>")
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
                  .AppendLine($"      <td>{oldData?.EmployeeId}</td>")
                  .AppendLine($"      <td>{oldData?.ApplicationDate}</td>")
                  .AppendLine($"      <td>{oldData?.DesiredLeaveDate}</td>")
                  .AppendLine($"      <td>{oldData?.OfficialLeaveDate}</td>")
                  .AppendLine($"      <td>{oldData?.ReasonId}</td>")
                  .AppendLine($"      <td>{oldData?.DecisionCode}</td>")
                  .AppendLine($"      <td>{oldData?.FilePath}</td>")
                  .AppendLine($"      <td>{oldData?.ClearanceDate}</td>")
                  .AppendLine($"      <td>{oldData?.HRNotes}</td>")
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
                  .AppendLine($"      <td>{newData?.EmployeeId}</td>")
                  .AppendLine($"      <td>{newData?.ApplicationDate}</td>")
                  .AppendLine($"      <td>{newData?.DesiredLeaveDate}</td>")
                  .AppendLine($"      <td>{newData?.OfficialLeaveDate}</td>")
                  .AppendLine($"      <td>{newData?.ReasonId}</td>")
                  .AppendLine($"      <td>{newData?.DecisionCode}</td>")
                  .AppendLine($"      <td>{newData?.FilePath}</td>")
                  .AppendLine($"      <td>{newData?.ClearanceDate}</td>")
                  .AppendLine($"      <td>{newData?.HRNotes}</td>")
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
            var detail = await _nhansuterminationService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Thêm - NhanSuTermination</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
              .AppendLine("  <thead class=\"bg-info\">")
              .AppendLine("    <tr>")
              .AppendLine("     <th>Mã nhân viên</th>")
              .AppendLine("     <th>Ngày nộp đơn</th>")
              .AppendLine("     <th>Ngày nghỉ việc mong muốn</th>")
              .AppendLine("     <th>Ngày nghỉ việc chính thức</th>")
              .AppendLine("     <th>Lý do nghỉ việc</th>")
              .AppendLine("     <th>Mã Quyết định nghỉ việc</th>")
              .AppendLine("     <th>Đường dẫn File Đơn/QĐ</th>")
              .AppendLine("     <th>Ngày thanh lý</th>")
              .AppendLine("     <th>Ghi chú từ HR</th>")
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
              .AppendLine($"     <td>{detail.EmployeeId}</td>")
              .AppendLine($"     <td>{detail.ApplicationDate}</td>")
              .AppendLine($"     <td>{detail.DesiredLeaveDate}</td>")
              .AppendLine($"     <td>{detail.OfficialLeaveDate}</td>")
              .AppendLine($"     <td>{detail.ReasonId}</td>")
              .AppendLine($"     <td>{detail.DecisionCode}</td>")
              .AppendLine($"     <td>{detail.FilePath}</td>")
              .AppendLine($"     <td>{detail.ClearanceDate}</td>")
              .AppendLine($"     <td>{detail.HRNotes}</td>")
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
            var detail = await _nhansuterminationService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Xóa - NhanSuTermination</h3>")
              .AppendLine("<table class=\"table table-hover table-bordered\">")
              .AppendLine("  <thead class=\"bg-info\">")
              .AppendLine("    <tr>")
              .AppendLine("     <th>Mã nhân viên</th>")
              .AppendLine("     <th>Ngày nộp đơn</th>")
              .AppendLine("     <th>Ngày nghỉ việc mong muốn</th>")
              .AppendLine("     <th>Ngày nghỉ việc chính thức</th>")
              .AppendLine("     <th>Lý do nghỉ việc</th>")
              .AppendLine("     <th>Mã Quyết định nghỉ việc</th>")
              .AppendLine("     <th>Đường dẫn File Đơn/QĐ</th>")
              .AppendLine("     <th>Ngày thanh lý</th>")
              .AppendLine("     <th>Ghi chú từ HR</th>")
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
              .AppendLine($"     <td>{detail.EmployeeId}</td>")
              .AppendLine($"     <td>{detail.ApplicationDate}</td>")
              .AppendLine($"     <td>{detail.DesiredLeaveDate}</td>")
              .AppendLine($"     <td>{detail.OfficialLeaveDate}</td>")
              .AppendLine($"     <td>{detail.ReasonId}</td>")
              .AppendLine($"     <td>{detail.DecisionCode}</td>")
              .AppendLine($"     <td>{detail.FilePath}</td>")
              .AppendLine($"     <td>{detail.ClearanceDate}</td>")
              .AppendLine($"     <td>{detail.HRNotes}</td>")
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

        private ApprovalTask CreateApprovalTask(string title, string content, NhanSu_Termination input, string tableName, string parentMajorId, string majorId, string userId)
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

        private ApprovalTask UpdateApprovalTask(string Id, string title, string content, NhanSu_Termination input, string tableName, string parentMajorId, string majorId, string userId)
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

