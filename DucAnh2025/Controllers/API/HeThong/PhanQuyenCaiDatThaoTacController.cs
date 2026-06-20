using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.Accounts;
using DucAnh2025.ViewModels.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhanQuyenCaiDatThaoTacController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IPermissionControlRepository _permissionControlService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IPhanQuyenRepository _phanQuyenService;

        public PhanQuyenCaiDatThaoTacController(IPermissionControlRepository permissionControlRepository,
            IEmailHistoryRepository emailHistoryService,
            IApplicationUserRepository applicationUserService,
            IChiNhanhRepository chiNhanhService,
            IPhanQuyenRepository phanQuyenService)
        {
            _permissionControlService = permissionControlRepository;
            _emailHistoryService = emailHistoryService;
            _applicationUserService = applicationUserService;
            _chiNhanhService = chiNhanhService;
            _phanQuyenService = phanQuyenService;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionControlModel>>>> GetByVM(string groupId, [FromBody] PermissionControlModel input)
        {
            try
            {
                var list = await _permissionControlService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<PermissionControlModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<PermissionControlModel>>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionControl>>>> DeletePhanQuyenCaiDatThaoTac(string id)
        {
            try
            {
                var item = await _permissionControlService.GetById(id);
                if (item == null)
                    return Ok(new ApiResponse<PermissionControl>(false, "Không tìm thấy bản ghi", null));

                if (item.IsActive == 2)
                    return Ok(new ApiResponse<PermissionControl>(true, "Thông tin bạn chọn đang chờ", null));

                if (!await _permissionControlService.CheckDelete(item))
                    return Ok(new ApiResponse<PermissionControl>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                var isExclusive = await _permissionControlService.CheckExclusive(new[] { id }, DateTime.Now);
                if (!isExclusive)
                    return Ok(new ApiResponse<PermissionControl>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

                item.IsActive = 100;
                item.CreateAt = DateTime.Now;
                await _permissionControlService.Update(item, "");

                return Ok(new ApiResponse<PermissionControl>(true, "Xóa thành công", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<PermissionControl>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetApplicationUsersForUserId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationUserModel>>>> GetApplicationUsersForUserId(string groupId)
        {
            try
            {
                var list = await _permissionControlService.GetApplicationUsersForUserId(groupId);
                return Ok(new ApiResponse<IEnumerable<ApplicationUserModel>>(true, "Thành công ", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApplicationUserModel>>(false, ex.Message, null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PermissionControl>>> CreateOrUpdatePhanQuyenCaiDatThaoTac([FromBody] PermissionControl input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                // Kiểm tra hợp lệ các trường liên quan
                var CompanyIdValid = await _chiNhanhService.CheckStatus(input.CompanyId, "");
                var UserIdValid = await _applicationUserService.CheckStatus(input.UserId, "");

                input.GroupId = groupId;
                input.CreateBy = userId;
                input.CreateAt = DateTime.Now;

                bool editcheck = (isEdit && input.Ordinarily > 0) || (isEdit && input.Ordinarily == 0 && input.IsActive == 3);

                if (editcheck)
                {
                    string[] ids = { input.Id };
                    var isValid = await _permissionControlService.CheckExclusive(ids, baseTime);
                    var checkEdit = await _permissionControlService.CheckEdit(input);
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, input.CompanyId, user, "ef6f00e4-347e-471d-a9d9-d7661ca6c1b1");
                    if (!checkquyen)
                        return Ok(new ApiResponse<PermissionControl>(false, "Bạn không có quyền sửa", null));

                    if (isValid)
                    {
                        input.Ordinarily = input.IsActive == 3 ? input.Ordinarily + 1 : input.Ordinarily;
                        input.IsActive = 1;
                        input.ApprovalOrder = 1;
                        input.DepartmentOrder = 1;
                        await _permissionControlService.Update(input, "");

                        // Build email content
                        string content = "<h3>Thông tin sửa</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th class=\"text-center\" rowspan=\"2\" scope=\"col\"><div class=\"pb-3\">No.</div></th><th><div class=\"pb-3\">Chi nhánh</div></th><th><div class=\"pb-3\">Nghiệp vụ cha</div></th><th><div class=\"pb-3\">Nghiệp vụ</div></th><th><div class=\"pb-3\">Người dùng</div></th></tr></thead>";
                        var isValidModel = await _permissionControlService.GetHistoryIsValidEdit(input.Id);
                        if (isValidModel.Any())
                        {
                            content += "<tbody>";
                            int stt = 0;
                            foreach (var item in isValidModel)
                            {
                                stt++;
                                content += "<tr>";
                                content += "<td class=\"text-center\" scope=\"row\">" + (stt == 1 ? "Dữ liệu cũ" : "Dữ liệu mới") + "</td>";
                                content += "<td class=\"text-left\">" + item.CompanyId + "</td>";
                                content += "<td class=\"text-left\">" + item.ParentMajorId + "</td>";
                                content += "<td class=\"text-left\">" + item.MajorId + "</td>";
                                content += "<td class=\"text-left\">" + item.UserId + "</td>";
                                content += "</tr>";
                            }
                            content += "</tbody>";
                        }
                        else
                        {
                            content += "<tbody><tr><td class=\"text-danger\" colspan=\"7\">Không có dữ liệu</td></tr></tbody>";
                        }
                        content += "</table>";

                        var emailhistory = new EmailHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Receiver = input.UserId,
                            Subject = "Phân quyền cài đặt - Sửa.",
                            Content = content,
                            CompanyId = input.CompanyId,
                            UserId = input.UserId,
                            ParentMajorId = input.ParentMajorId,
                            MajorId = input.MajorId,
                            IdCheck = input.Id,
                            IdLog = "",
                            IsMail = true,
                            IsNotification = true,
                            IsSMS = true,
                            GroupId = groupId,
                            CreateAt = DateTime.Now,
                            CreateBy = user.Id,
                            IsRead = 0
                        };
                        await _emailHistoryService.Insert(emailhistory, "");

                        return Ok(new ApiResponse<PermissionControl>(true, "Cập nhật thành công.", null));
                    }
                }
                else
                {
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, input.CompanyId, user, "6113e06f-6495-42a3-ad65-8ea440510fa0");
                    if (!checkquyen)
                        return Ok(new ApiResponse<PermissionControl>(false, "Bạn không có quyền thêm mới", null));

                    input.IsActive = 0;
                    var checkSave = await _permissionControlService.CheckSave(input);
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.Ordinarily = 0;

                    if (isEdit)
                        await _permissionControlService.Update(input, "");
                    else
                        await _permissionControlService.Insert(input, "");

                    // Build email content
                    string content = "<h3>Thông tin thêm</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th><div class=\"pb-3\">Chi nhánh</div></th><th><div class=\"pb-3\">Nghiệp vụ cha</div></th><th><div class=\"pb-3\">Nghiệp vụ</div></th><th><div class=\"pb-3\">Người dùng</div></th></tr></thead><tbody><tr>";
                    content += "<td class=\"text-left\">" + input.CompanyId + "</td>";
                    content += "<td class=\"text-left\">" + input.ParentMajorId + "</td>";
                    content += "<td class=\"text-left\">" + input.MajorId + "</td>";
                    content += "<td class=\"text-left\">" + input.UserId + "</td>";
                    content += "</tr></tbody></table>";

                    var emailhistory = new EmailHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Receiver = input.UserId,
                        Subject = "Phân quyền cài đặt - Thêm",
                        Content = content,
                        CompanyId = input.CompanyId,
                        UserId = input.UserId,
                        ParentMajorId = input.ParentMajorId,
                        MajorId = input.MajorId,
                        IdCheck = input.Id,
                        IdLog = "",
                        IsMail = true,
                        IsNotification = true,
                        IsSMS = true,
                        GroupId = groupId,
                        CreateAt = DateTime.Now,
                        CreateBy = user.Id,
                        IsRead = 0
                    };
                    await _emailHistoryService.Insert(emailhistory, "");

                    return Ok(new ApiResponse<PermissionControl>(true, "Lưu thành công.", null));
                }

                return Ok(new ApiResponse<PermissionControl>(false, "Không xác định được trạng thái thao tác.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<PermissionControl>(false, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionControl>>>> GetById(string id)
        {
            try
            {
                var list = await _permissionControlService.GetById(id);
                return Ok(new ApiResponse<PermissionControl>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<PermissionControl>>(false, ex.Message, null));
            }
        }
    }

}
