using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhanQuyenThaoTacController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "";
        private string _majorId = "";

        private readonly IMajorUserPermissionRepository _majorUserPermissionService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        public PhanQuyenThaoTacController(IMajorUserPermissionRepository majorUserPermissionRepository,
            IApplicationUserRepository applicationUserRepository,
            IChiNhanhRepository chiNhanhService,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService)
        {
            _majorUserPermissionService = majorUserPermissionRepository;
            _applicationUserService = applicationUserRepository;
            _chiNhanhService = chiNhanhService;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserPermissionModel>>>> GetByVM(string groupId, [FromBody] MajorUserPermissionModel input)
        {
            try
            {
                var list = await _majorUserPermissionService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<MajorUserPermissionModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<MajorUserPermissionModel>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetByMainId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserPermission>>>> GetByMainId(string idMain)
        {
            try
            {
                var list = await _majorUserPermissionService.GetByIdMain(idMain);
                return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(true, "Thành công", list));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(false, ex.Message, null));
            }
        }

        //[HttpPost]
        //public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserPermission>>>> CreatePhanQuyenThaoTac(List<MajorUserPermission> input)
        //{
        //    try
        //    {
        //        var CompanyIdValid = await _chiNhanhService.CheckStatus(input[0].CompanyId, "");
        //        var UserIdValid = await _applicationUserService.CheckStatus(input[0].UserId, "");
        //        var checkEdit = await _majorUserPermissionService.CheckEdit(input[0]);
        //        var listAdd = new List<MajorUserPermission>();

        //        await _majorUserPermissionService.UpdateMulti(input, input[0].IdMain);


        //        return Ok(new ApiResponse<MajorUserPermission>(true, "Lưu thành công", null));
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine(ex);
        //        return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(false, ex.Message, null));
        //    }
        //}
        [HttpPost]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserPermission>>>> CreatePhanQuyenThaoTac(List<MajorUserPermission> input)
        {
            try
            {
                // Validate input
                if (input == null || input.Count == 0)
                    return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(false, "Dữ liệu đầu vào không hợp lệ", null));

                // Lấy user hiện tại
                var user = _applicationUserService.GetCurrentUser();
                var groupId = input[0].GroupId ?? user.GroupId;
                var userId = user.Id;

                // Lấy idMain từ input hoặc tạo mới
                var idMain = string.IsNullOrEmpty(input[0].IdMain) ? Guid.NewGuid().ToString() : input[0].IdMain;

                var userSelect = await _applicationUserService.GetById(input[0].UserId);
                // Lấy các giá trị text hiển thị (nếu cần)
                var CompanyIdText = "";
                var ParentMajorIdText = input[0].ParentMajorId;
                var MajorIdText = input[0].MajorId;
                var UserIdText = userSelect.Email ?? "";
                var PermissionIdText = "";
                var DayInWeekText = "";  

                // Validate các trường liên quan
                var CompanyIdValid = await _chiNhanhService.CheckStatus(input[0].CompanyId, CompanyIdText);
                var UserIdValid = await _applicationUserService.CheckStatus(input[0].UserId, UserIdText);

                // Gán các trường chung
                var Input = input[0];
                Input.GroupId = groupId;
                Input.CreateBy = userId;
                Input.CreateAt = DateTime.Now;
                Input.IdMain = idMain;

                // Kiểm tra quyền
                var checkEdit = await _majorUserPermissionService.CheckEdit(Input);
                var checkquyen = await _phanQuyenService.CheckPermission(groupId, Input.CompanyId, user, "04EDB742-5723-49E2-98BC-F85A9F693D18");
                if (!checkquyen)
                    return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(false, "Bạn không có quyền thực hiện thao tác này", null));

                Input.Ordinarily = Input.IsActive == 3 ? Input.Ordinarily + 1 : Input.Ordinarily;
                Input.IsActive = 1;
                Input.ApprovalOrder = 1;
                Input.DepartmentOrder = 1;

                // Lấy danh sách permissionId và dayInWeek từ input
                var permissionIds = input.Select(x => x.PermissionId).Distinct().ToList();
                var dayInWeeks = input.Select(x => x.DayInWeek).Distinct().ToList();

                // Build danh sách cần thêm
                var listAdd = new List<MajorUserPermission>();
                foreach (var permissionId in permissionIds)
                {
                    if (string.IsNullOrEmpty(permissionId)) continue;
                    foreach (var diw in dayInWeeks)
                    {
                        if (diw <= 0) continue;
                        var addItem = new MajorUserPermission
                        {
                            Id = Guid.NewGuid().ToString(),
                            CompanyId = Input.CompanyId,
                            ParentMajorId = Input.ParentMajorId,
                            MajorId = Input.MajorId,
                            UserId = Input.UserId,
                            PermissionId = permissionId,
                            DayInWeek = diw,
                            IdMain = idMain,
                            GroupId = Input.GroupId,
                            Ordinarily = Input.Ordinarily,
                            CreateAt = Input.CreateAt,
                            CreateBy = Input.CreateBy,
                            IsActive = Input.IsActive,
                            ApprovalUserId = Input.ApprovalUserId,
                            DateApproval = Input.DateApproval,
                            DepartmentId = Input.DepartmentId,
                            DepartmentOrder = Input.DepartmentOrder,
                            ApprovalOrder = Input.ApprovalOrder,
                            ApprovalId = Input.ApprovalId,
                            LastApprovalId = Input.LastApprovalId,
                            IsStatus = Input.IsStatus
                        };
                        listAdd.Add(addItem);
                    }
                }

                await _majorUserPermissionService.UpdateMulti(listAdd, idMain);

                // Build PermissionIdText & DayInWeekText for email
                PermissionIdText = string.Join(", ", permissionIds);
                DayInWeekText = string.Join(", ", dayInWeeks.Select(d =>
                     d == 0 ? "Chủ nhật" :
                     d == 1 ? "Thứ 2" :
                     d == 2 ? "Thứ 3" :
                     d == 3 ? "Thứ 4" :
                     d == 4 ? "Thứ 5" :
                     d == 5 ? "Thứ 6" :
                     d == 6 ? "Thứ 7" : d.ToString()
                 ));

                // Build email content
                string content = "";
                content += "<h3>Thông tin sửa</h3>";
                content += "<table class=\"table table-hover table-bordered\">";
                content += "<thead class=\"bg-info\">";
                content += "<tr>";
                content += "<th class=\"text-center\" rowspan=\"2\" scope=\"col\"><div class=\"pb-3\">No.</div></th>";
                content += "<th><div class=\"pb-3\">Chi nhánh</div></th>";
                content += "<th><div class=\"pb-3\">Nghiệp vụ cha</div></th>";
                content += "<th><div class=\"pb-3\">Nghiệp vụ</div></th>";
                content += "<th><div class=\"pb-3\">Người dùng</div></th>";
                content += "<th><div class=\"pb-3\">Loại quyền</div></th>";
                content += "<th><div class=\"pb-3\">Ngày trong tuần</div></th>";
                content += "</tr>";
                content += "</thead>";
                content += "<tbody>";
                content += "<tr>";
                content += "<td class=\"text-center\" scope=\"row\"></td>";
                content += "<td class=\"text-left\">" + CompanyIdText + "</td>";
                content += "<td class=\"text-left\">" + ParentMajorIdText + "</td>";
                content += "<td class=\"text-left\">" + MajorIdText + "</td>";
                content += "<td class=\"text-left\">" + UserIdText + "</td>";
                content += "<td class=\"text-left\">" + PermissionIdText + "</td>";
                content += "<td class=\"text-left\">" + DayInWeekText + "</td>";
                content += "</tr>";
                content += "</tbody>";
                content += "</table>";

                var emailhistory = new EmailHistory()
                {
                    Id = Guid.NewGuid().ToString(),
                    Receiver = UserIdText,
                    Subject = "Quản lý quyền theo người dùng, nghiệp vụ - Sửa.",
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
                return Ok(new ApiResponse<MajorUserPermission>(true, "Lưu thành công", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new ApiResponse<IEnumerable<MajorUserPermission>>(false, ex.Message, null));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserPermission>>>> DeletePhanQuyenThaoTac(string deleteId, string userId)
        {
            try
            {
                // Chỉ lấy trường IsActive để kiểm tra trạng thái
                var query = await _majorUserPermissionService.GetById(deleteId);
                if (query == null)
                {
                    return Ok(new ApiResponse<MajorUserPermission>(false, "Không tìm thấy bản ghi", null));
                }

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<MajorUserPermission>(false, "Thông tin bạn chọn đang chờ ", null));
                }

                string[] ids = { deleteId };
                var isValid = await _majorUserPermissionService.CheckExclusive(ids, DateTime.Now);

                if (!isValid)
                {
                    return Ok(new ApiResponse<MajorUserPermission>(false, "Xóa thất bại", null));
                }

                // Chỉ update các trường cần thiết
                query.IsActive = 100;
                query.CreateAt = DateTime.Now;
                query.CreateBy = userId;
                await _majorUserPermissionService.Update(query, userId);

                // Gửi email/log có thể đưa vào background nếu cần tối ưu thêm
                _ = Task.Run(async () =>
                {
                    var emailsent = await _applicationUserService.GetById(query.UserId);
                    string content = "<h3>Thông tin xóa</h3>";
                    content += "<table class=\"table table-hover table-bordered\">";
                    content += "<thead class=\"bg-info\">";
                    content += "<tr>";
                    content += "<th><div class=\"pb-3\">Chi nhánh</div></th>";
                    content += "<th><div class=\"pb-3\">Nghiệp vụ cha</div></th>";
                    content += "<th><div class=\"pb-3\">Nghiệp vụ</div></th>";
                    content += "<th><div class=\"pb-3\">Người dùng</div></th>";
                    content += "<th><div class=\"pb-3\">Loại quyền</div></th>";
                    content += "<th><div class=\"pb-3\">Ngày trong tuần</div></th>";
                    content += "<th><div class=\"pb-3\">IdMain</div></th>";
                    content += "</tr>";
                    content += "</thead>";
                    content += "<tbody>";
                    content += "<tr>";
                    content += "<td class=\"text-left\">" + query.CompanyId + "</td>";
                    content += "<td class=\"text-left\">" + query.ParentMajorId + "</td>";
                    content += "<td class=\"text-left\">" + query.MajorId + "</td>";
                    content += "<td class=\"text-left\">" + query.UserId + "</td>";
                    content += "<td class=\"text-left\">" + query.PermissionId + "</td>";
                    content += "<td class=\"text-left\">" + query.DayInWeek + "</td>";
                    content += "<td class=\"text-left\">" + query.IdMain + "</td>";
                    content += "</tr>";
                    content += "</tbody>";
                    content += "</table>";

                    var emailhistory = new EmailHistory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Receiver = emailsent?.Email,
                        Subject = "Quản lý quyền theo người dùng, nghiệp vụ - Xóa",
                        Content = content,
                        CompanyId = query.CompanyId,
                        UserId = query.UserId,
                        ParentMajorId = query.ParentMajorId,
                        MajorId = query.MajorId,
                        IdCheck = query.Id,
                        IdLog = "",
                        IsMail = true,
                        IsNotification = true,
                        IsSMS = true,
                        GroupId = query.GroupId,
                        CreateAt = DateTime.Now,
                        CreateBy = query.CreateBy,
                        IsRead = 0
                    };
                    await _emailHistoryService.Insert(emailhistory, "");
                });

                return Ok(new ApiResponse<MajorUserPermission>(true, "Xóa thành công", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new ApiResponse<MajorUserPermission>(false, ex.Message, null));
            }
        }


    }
}
