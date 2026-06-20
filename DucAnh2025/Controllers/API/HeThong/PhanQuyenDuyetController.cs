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
    public class PhanQuyenDuyetController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IMajorUserApprovalReponsitory _majorUserApprovalService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IEmailHistoryRepository _emailHistoryService;

        public PhanQuyenDuyetController(IMajorUserApprovalReponsitory majorUserApprovalReponsitory
            ,IApplicationUserRepository applicationUserRepository,
            IChiNhanhRepository chiNhanhService,
            IEmailHistoryRepository emailHistoryService)
        {
            _majorUserApprovalService = majorUserApprovalReponsitory;
            _applicationUserService = applicationUserRepository;
            _chiNhanhService = chiNhanhService;
            _emailHistoryService = emailHistoryService;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserApprovalModel>>>> GetByVM(string groupId, [FromBody] MajorUserApprovalModel input)
        {
            try
            {
                var list = await _majorUserApprovalService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<MajorUserApprovalModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<MajorUserApprovalModel>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetByMainId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserApproval>>>> GetByMainId(string idMain)
        {
            try
            {
                var list = await _majorUserApprovalService.GetByIdMain(idMain);
                return Ok(new ApiResponse<IEnumerable<MajorUserApproval>>(true, "Thành công", list));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<MajorUserApproval>>(false, ex.Message, null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserApproval>>>> CreateOrUpdatePhanQuyenDuyet(List<MajorUserApproval> input)
        {
            try
            {
                if (input == null || input.Count == 0)
                    return Ok(new ApiResponse<IEnumerable<MajorUserApproval>>(false, "Dữ liệu đầu vào không hợp lệ", null));

                var user = _applicationUserService.GetCurrentUser();
                var groupId = input[0].GroupId ?? user.GroupId;
                var userId = user.Id;
                var idMain = string.IsNullOrEmpty(input[0].IdMain) ? Guid.NewGuid().ToString() : input[0].IdMain;

                var CompanyIdValid = await _chiNhanhService.CheckStatus(input[0].CompanyId, "");
                var UserIdValid = await _applicationUserService.CheckStatus(input[0].UserId, "");

                // Gán các trường chung
                var baseItem = input[0];
                baseItem.GroupId = groupId;
                baseItem.CreateBy = userId;
                baseItem.CreateAt = DateTime.Now;
                baseItem.IdMain = idMain;
                baseItem.IsActive = 1;

                // Kiểm tra quyền
                var checkEdit = await _majorUserApprovalService.CheckEdit(baseItem);
                var hasPermission = await _majorUserApprovalService.CheckPermission(groupId, baseItem.CompanyId, user, "06ff3867-749b-4350-bb5c-bdb46fc97ed4");
                if (!hasPermission)
                    return Ok(new ApiResponse<IEnumerable<MajorUserApproval>>(false, "Bạn không có quyền thực hiện thao tác này", null));

                // Tạo danh sách cần thêm
                var approvalStepIds = input.Select(x => x.ApprovalStepId).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
                var dayInWeeks = input.Select(x => x.DayinWeek).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();

                var listAdd = new List<MajorUserApproval>();
                foreach (var stepId in approvalStepIds)
                {
                    foreach (var diw in dayInWeeks)
                    {
                        listAdd.Add(new MajorUserApproval
                        {
                            Id = Guid.NewGuid().ToString(),
                            CompanyId = baseItem.CompanyId,
                            ParentMajorId = baseItem.ParentMajorId,
                            MajorId = baseItem.MajorId,
                            DeptId = baseItem.DeptId,
                            UserId = baseItem.UserId,
                            PermissionId = baseItem.PermissionId,
                            ApprovalStepId = stepId,
                            DayinWeek = diw,
                            IdMain = idMain,
                            GroupId = baseItem.GroupId,
                            CreateAt = baseItem.CreateAt,
                            CreateBy = baseItem.CreateBy,
                            IsActive = baseItem.IsActive,
                        });
                    }
                }

                await _majorUserApprovalService.UpdateMulti(listAdd, idMain);

                // Build DayInWeekText for email
                var dayInWeekText = string.Join(", ", dayInWeeks.Select(d =>
                    d == "0" ? "Chủ nhật" :
                    d == "1" ? "Thứ 2" :
                    d == "2" ? "Thứ 3" :
                    d == "3" ? "Thứ 4" :
                    d == "4" ? "Thứ 5" :
                    d == "5" ? "Thứ 6" :
                    d == "6" ? "Thứ 7" : d
                ));

                // Build email content (nên tách ra hàm riêng nếu dùng lại)
                string content = $@"
            <h3>Thông tin sửa</h3>
            <table class='table table-hover table-bordered'>
                <thead class='bg-info'>
                    <tr>
                        <th class='text-center' rowspan='2' scope='col'><div class='pb-3'>No.</div></th>
                        <th><div class='pb-3'>Chi nhánh</div></th>
                        <th><div class='pb-3'>Nghiệp vụ cha</div></th>
                        <th><div class='pb-3'>Nghiệp vụ</div></th>
                        <th><div class='pb-3'>Người dùng</div></th>
                        <th><div class='pb-3'>Loại quyền</div></th>
                        <th><div class='pb-3'>Ngày trong tuần</div></th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class='text-center' scope='row'></td>
                        <td class='text-left'>{baseItem.CompanyId}</td>
                        <td class='text-left'>{baseItem.ParentMajorId}</td>
                        <td class='text-left'>{baseItem.MajorId}</td>
                        <td class='text-left'>{baseItem.UserId}</td>
                        <td class='text-left'>{baseItem.PermissionId}</td>
                        <td class='text-left'>{dayInWeekText}</td>
                    </tr>
                </tbody>
            </table>";

                var emailhistory = new EmailHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    Receiver = baseItem.UserId,
                    Subject = "Phân quyền cài đặt duyệt theo người dùng, nghiệp vụ - Sửa.",
                    Content = content,
                    CompanyId = baseItem.CompanyId,
                    UserId = baseItem.UserId,
                    ParentMajorId = baseItem.ParentMajorId,
                    MajorId = baseItem.MajorId,
                    IdCheck = baseItem.Id,
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

                return Ok(new ApiResponse<MajorUserApproval>(true, "Lưu thành công", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new ApiResponse<IEnumerable<MajorUserApproval>>(false, ex.Message, null));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorUserApproval>>>> DeletePhanQuyenDuyet(string deleteId, string userId)
        {
            try
            {
                // Lấy bản ghi cần xóa (chỉ lấy trường IsActive để kiểm tra nhanh)
                var query = await _majorUserApprovalService.GetById(deleteId);
                if (query == null)
                {
                    return Ok(new ApiResponse<MajorUserApproval>(false, "Không tìm thấy bản ghi", null));
                }

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<MajorUserApproval>(false, "Thông tin bạn chọn đang chờ ", null));
                }

                string[] ids = { deleteId };
                var isValid = await _majorUserApprovalService.CheckExclusive(ids, DateTime.Now);

                if (!isValid)
                {
                    return Ok(new ApiResponse<MajorUserApproval>(false, "Xóa thất bại", null));
                }

                // Chỉ update các trường cần thiết
                query.IsActive = 100;
                query.CreateAt = DateTime.Now;
                query.CreateBy = userId;
                await _majorUserApprovalService.Update(query, userId);

                // Gửi email/log có thể đưa vào background để giảm thời gian trả về
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
                    content += "<td class=\"text-left\">" + query.DayinWeek + "</td>";
                    content += "<td class=\"text-left\">" + query.IdMain + "</td>";
                    content += "</tr>";
                    content += "</tbody>";
                    content += "</table>";

                    var emailhistory = new EmailHistory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Receiver = emailsent?.Email,
                        Subject = "Phân quyền cài đặt - Xóa",
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

                return Ok(new ApiResponse<MajorUserApproval>(true, "Xóa thành công", null));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Ok(new ApiResponse<MajorUserApproval>(false, ex.Message, null));
            }
        }

    }
}
