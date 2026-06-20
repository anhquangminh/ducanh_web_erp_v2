using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhanQuyenCaiDatDuyetController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IApprovalControlRepository _approvalControlService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IPhanQuyenRepository _phanQuyenService;

        public PhanQuyenCaiDatDuyetController(IApprovalControlRepository approvalControlRepository
            ,IEmailHistoryRepository emailHistoryService,
            IApplicationUserRepository applicationUserService,
            IChiNhanhRepository chiNhanhService,
            IPhanQuyenRepository phanQuyenService)
        {
            _approvalControlService = approvalControlRepository;
            _emailHistoryService = emailHistoryService;
            _applicationUserService = applicationUserService;
            _chiNhanhService = chiNhanhService;
            _phanQuyenService = phanQuyenService;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalControlModel>>>> GetByVM(string groupId, [FromBody] ApprovalControlModel input)
        {
            try
            {
                var list = await _approvalControlService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(false, ex.Message, null));
            }
        }

        [HttpPost("CheckEdit")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalControlModel>>>> CheckEdit([FromBody] ApprovalControl input)
        {
            try
            {
                await _approvalControlService.CheckEdit(input);
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(true, "Thành công", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(false, ex.Message, null));
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate([FromBody] ApprovalControl Input, bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                // Kiểm tra hợp lệ các trường liên quan
                var CompanyIdValid = await _chiNhanhService.CheckStatus(Input.CompanyId, "");
                var UserIdValid = await _applicationUserService.CheckStatus(Input.UserId, "");

                Input.GroupId = groupId;
                Input.CreateBy = userId;
                Input.CreateAt = DateTime.Now;
                Input.IsActive = 1;

                bool editcheck = (isEdit && Input.Ordinarily > 0) || (isEdit && Input.Ordinarily == 0 && Input.IsActive == 3);

                if (editcheck)
                {
                    string[] ids = { Input.Id };
                    var isValid = await _approvalControlService.CheckExclusive(ids, baseTime);
                    var checkEdit = await _approvalControlService.CheckEdit(Input);
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, Input.CompanyId, user, "b54436c7-2bd0-47b0-a10f-afa7508c61d5");
                    if (!checkquyen)
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền sửa", null));

                    if (isValid)
                    {
                        Input.Ordinarily = Input.IsActive == 3 ? Input.Ordinarily + 1 : Input.Ordinarily;
                        Input.IsActive = 1;
                        Input.ApprovalOrder = 1;
                        Input.DepartmentOrder = 1;
                        await _approvalControlService.Update(Input, userId);

                        // Build email content
                        string content = "<h3>Thông tin sửa</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th class=\"text-center\" rowspan=\"2\" scope=\"col\"><div class=\"pb-3\">No.</div></th><th><div class=\"pb-3\">Chi nhánh</div></th><th><div class=\"pb-3\">Nghiệp vụ cha</div></th><th><div class=\"pb-3\">Nghiệp vụ</div></th><th><div class=\"pb-3\">Phòng ban</div></th><th><div class=\"pb-3\">Người dùng</div></th><th><div class=\"pb-3\">Loại quyền</div></th><th><div class=\"pb-3\">Duyệt</div></th></tr></thead>";
                        var isValidModel = await _approvalControlService.GetHistoryIsValidEdit(Input.Id);
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
                                content += "<td class=\"text-left\">" + item.DeptId + "</td>";
                                content += "<td class=\"text-left\">" + item.UserId + "</td>";
                                content += "<td class=\"text-left\">" + item.PermissionId + "</td>";
                                content += "<td class=\"text-left\">" + item.ApprovalStepId + "</td>";
                                content += "</tr>";
                            }
                            content += "</tbody>";
                        }
                        else
                        {
                            content += "<tbody><tr><td class=\"text-danger\" colspan=\"8\">Không có dữ liệu</td></tr></tbody>";
                        }
                        content += "</table>";

                        var emailhistory = new EmailHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Receiver = Input.UserId,
                            Subject = "Phân quyền cài đặt thao tác - Sửa.",
                            Content = content,
                            CompanyId = Input.CompanyId,
                            UserId = Input.UserId,
                            ParentMajorId = Input.ParentMajorId,
                            MajorId = Input.MajorId,
                            IdCheck = Input.Id,
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
                        return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                    }
                }
                else
                {
                    // Thêm mới
                    var checkEdit = await _approvalControlService.CheckEdit(Input);
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, Input.CompanyId, user, "49735f27-da2b-430e-82af-be52cc92cc6b");
                    if (!checkquyen)
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền thêm mới", null));

                    Input.ApprovalOrder = 1;
                    Input.DepartmentOrder = 1;
                    Input.Ordinarily = 0;
                    if (isEdit)
                    {
                        await _approvalControlService.Update(Input, userId);
                    }
                    else
                    {
                        await _approvalControlService.Insert(Input, userId);
                    }

                    // Build email content
                    string content = "<h3>Thông tin thêm</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th><div class=\"pb-3\">Chi nhánh</div></th><th><div class=\"pb-3\">Nghiệp vụ cha</div></th><th><div class=\"pb-3\">Nghiệp vụ</div></th><th><div class=\"pb-3\">Phòng ban</div></th><th><div class=\"pb-3\">Người dùng</div></th><th><div class=\"pb-3\">Loại quyền</div></th><th><div class=\"pb-3\">Duyệt</div></th></tr></thead><tbody><tr>";
                    content += "<td class=\"text-left\">" + Input.CompanyId + "</td>";
                    content += "<td class=\"text-left\">" + Input.ParentMajorId + "</td>";
                    content += "<td class=\"text-left\">" + Input.MajorId + "</td>";
                    content += "<td class=\"text-left\">" + Input.DeptId + "</td>";
                    content += "<td class=\"text-left\">" + Input.UserId + "</td>";
                    content += "<td class=\"text-left\">" + Input.PermissionId + "</td>";
                    content += "<td class=\"text-left\">" + Input.ApprovalStepId + "</td>";
                    content += "</tr></tbody></table>";

                    var emailhistory = new EmailHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        Receiver = Input.UserId,
                        Subject = "Phân quyền cài đặt thao tác - Thêm",
                        Content = content,
                        CompanyId = Input.CompanyId,
                        UserId = Input.UserId,
                        ParentMajorId = Input.ParentMajorId,
                        MajorId = Input.MajorId,
                        IdCheck = Input.Id,
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

                    return Ok(new ApiResponse<object>(true, "Lưu thành công.", null));
                }

                return Ok(new ApiResponse<object>(false, "Không xác định được trạng thái thao tác.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalControl>>>> DeletePhanQuyenCaiDatDuyet(string id)
        {
            try
            {
                var item = await _approvalControlService.GetById(id);
                if (item == null)
                    return Ok(new ApiResponse<ApprovalControl>(false, "Không tìm thấy bản ghi", null));

                if (item.IsActive == 2)
                    return Ok(new ApiResponse<ApprovalControl>(true, "Thông tin bạn chọn đang chờ", null));

                if (!await _approvalControlService.CheckDelete(item))
                    return Ok(new ApiResponse<ApprovalControl>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                var isExclusive = await _approvalControlService.CheckExclusive(new[] { id }, DateTime.Now);
                if (!isExclusive)
                    return Ok(new ApiResponse<ApprovalControl>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

                item.IsActive = 100;
                item.CreateAt = DateTime.Now;
                await _approvalControlService.Update(item, "");

                return Ok(new ApiResponse<ApprovalControl>(true, "Xóa thành công", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(false, ex.Message, null));
            }
        }


        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalControl>>>> GetById(string id)
        {
            try
            {
                var list = await _approvalControlService.GetById(id);
                return Ok(new ApiResponse<ApprovalControl>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalControlModel>>(false, ex.Message, null));
            }
        }
        

    }
}
