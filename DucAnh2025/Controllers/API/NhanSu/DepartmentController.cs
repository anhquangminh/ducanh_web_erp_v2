using DucAnh2025.Models;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.NhanSu
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {

        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "f2ee47e5-04b5-4f19-a5ac-3311e0122140";
        private string _majorId = "2beef307-7f25-4ae2-bd19-8bbfecd42334";

        private readonly IDepartmentRepository _departmentService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;

        public DepartmentController(IDepartmentRepository departmentRepository,
            ICompanyTypeRepository companyTypeRepository,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService)
        {
            _departmentService = departmentRepository;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentModel>>>> GetByVM(string groupId, [FromBody] DepartmentModel input)
        {
            try
            {
                var list = await _departmentService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<DepartmentModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<DepartmentModel>>(false, ex.Message, null));
            }
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Department>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _departmentService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<Department>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<Department>>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] Department input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                input.GroupId = input.GroupId ?? user.CompanyId;
                input.CreateBy = user.Id;
                input.CreateAt = DateTime.Now;


                bool editcheck = (isEdit && input.Ordinarily > 0) || (isEdit && input.Ordinarily == 0 && input.IsActive == 3);

                if (string.IsNullOrEmpty(input.Id))
                    input.Id = Guid.NewGuid().ToString();

                if (editcheck)
                {
                    string[] ids = { input.Id };
                    await _departmentService.CheckExclusive(ids, input.CreateAt ?? DateTime.Now);
                    await _departmentService.CheckEdit(input);
                    await _phanQuyenService.CheckPermission(input.GroupId, input.GroupId, user, "41566fcc-d0e8-482d-9408-e1744399ee9a");

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "41566fcc-d0e8-482d-9408-e1744399ee9a");
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "41566fcc-d0e8-482d-9408-e1744399ee9a");

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

                    await _departmentService.Update(input, "");

                    // Build email content
                    var isValidModel = await _departmentService.GetHistoryIsValidEdit(input.Id);
                    string content = "<h3>Thông tin sửa</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th class=\"text-center\" rowspan=\"2\" scope=\"col\"><div class=\"pb-3\">No.</div></th><th><div class=\"pb-3\">Tên </div></th></tr></thead>";
                    if (isValidModel.Any())
                    {
                        content += "<tbody>";
                        content += "<tr><td class=\"text-center\" scope=\"row\">Dữ liệu cũ</td><td class=\"text-left\">" + isValidModel[0].DeptName + "</td></tr>";
                        if (isValidModel.Count > 1)
                            content += "<tr><td class=\"text-center\" scope=\"row\">Dữ liệu mới</td><td class=\"text-left\">" + isValidModel[1].DeptName + "</td></tr>";
                        content += "</tbody>";
                    }
                    else
                    {
                        content += "<tbody><tr><td class=\"text-danger\" colspan=\"7\">Không có dữ liệu</td></tr></tbody>";
                    }

                    var listEmail = await _emailHistoryService.GetUserPermission(input.GroupId, firstApproval.Id);
                    var listInsert = new List<EmailHistory>();
                    foreach (var item in listEmail)
                    {
                        listInsert.Add(new EmailHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Receiver = item.Mail,
                            Subject = "Phòng ban - Sửa.",
                            Content = content,
                            CompanyId = input.GroupId,
                            UserId = item.UserId,
                            ParentMajorId = item.ParentMajorId,
                            MajorId = item.MajorId,
                            IdCheck = input.Id,
                            IdLog = "",
                            IsMail = true,
                            IsNotification = true,
                            IsSMS = true,
                            GroupId = input.GroupId,
                            CreateAt = DateTime.Now,
                            CreateBy = user.Id,
                            IsRead = 0
                        });
                    }
                    await _emailHistoryService.InsertMulti(listInsert);

                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                }
                else
                {
                    await _phanQuyenService.CheckPermission(input.GroupId, input.GroupId, user, "93ec3a5d-ce6f-41ed-b187-2f45ae048bd8");
                    input.IsActive = 0;
                    await _departmentService.CheckSave(input);

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "93ec3a5d-ce6f-41ed-b187-2f45ae048bd8");
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "93ec3a5d-ce6f-41ed-b187-2f45ae048bd8");

                    input.DepartmentId = firstApproval.DepartmentId;
                    input.ApprovalId = firstApproval.Id;
                    input.LastApprovalId = lastApproval.Id;
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.Ordinarily = 0;
                    input.IsStatus = firstApproval.Content;
                    input.ApprovalUserId = null;
                    input.DateApproval = null;
                    input.ApprovalDept = null;

                    await _departmentService.Insert(input, "");

                    // Build email content
                    string content = "<h3>Thông tin thêm</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th><div class=\"pb-3\">Tên </div></th></tr></thead><tbody><tr><td class=\"text-left\">" + input.DeptName + "</td></tr></tbody></table>";

                    var listEmail = await _emailHistoryService.GetUserPermission(input.GroupId, firstApproval.Id);
                    var listInsert = new List<EmailHistory>();
                    foreach (var item in listEmail)
                    {
                        listInsert.Add(new EmailHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Receiver = item.Mail,
                            Subject = "Chức vụ - Thêm",
                            Content = content,
                            CompanyId = input.GroupId,
                            UserId = item.UserId,
                            ParentMajorId = item.ParentMajorId,
                            MajorId = item.MajorId,
                            IdCheck = input.Id,
                            IdLog = "",
                            IsMail = true,
                            IsNotification = true,
                            IsSMS = true,
                            GroupId = input.GroupId,
                            CreateAt = DateTime.Now,
                            CreateBy = user.Id,
                            IsRead = 0
                        });
                    }
                    await _emailHistoryService.InsertMulti(listInsert);

                    return Ok(new ApiResponse<object>(true, "Lưu thành công.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(string id)
        {
            try
            {
                var result = await _departmentService.GetById(id);
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
                var result = await _departmentService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _departmentService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn", null));

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));
                }

                var checkDelete = await _departmentService.CheckDelete(query);
                if (!checkDelete)
                    return Ok(new ApiResponse<object>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                var checkquyen = await _phanQuyenService.CheckPermission(groupId, groupId, user, "558d30b4-baf0-4bbf-add4-23ab42b53701");
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa", null));

                string[] ids = { id };
                var isValid = await _departmentService.CheckExclusive(ids, baseTime);
                if (!isValid)
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, _majorId, "558d30b4-baf0-4bbf-add4-23ab42b53701");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, _majorId, "558d30b4-baf0-4bbf-add4-23ab42b53701");

                query.DepartmentId = firstApproval.DepartmentId;
                query.ApprovalId = firstApproval.Id;
                query.LastApprovalId = lastApproval.Id;
                query.IsActive = 2;
                query.ApprovalOrder = 1;
                query.DepartmentOrder = 1;
                query.CreateAt = DateTime.Now;
                query.CreateBy = user.UserName;
                query.IsStatus = firstApproval.Content;
                query.ApprovalUserId = null;
                query.DateApproval = null;
                query.ApprovalDept = null;

                await _departmentService.Update(query, "");

                var result = await _departmentService.GetDetails(id);
                string content = "<h3>Thông tin xóa</h3>";
                content += "<table class=\"table table-hover table-bordered\">";
                content += "<thead class=\"bg-info\"><tr><th><div class=\"pb-3\">Tên </div></th></tr></thead>";
                content += "<tbody><tr><td class=\"text-left\">" + result?.DeptName ?? query.DeptName + "</td></tr></tbody></table>";

                var listEmail = await _emailHistoryService.GetUserPermission(query.GroupId, firstApproval.Id);
                var listInsert = new List<EmailHistory>();
                foreach (var item in listEmail)
                {
                    var emailhistory = new EmailHistory()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Receiver = item.Mail,
                        Subject = "Phòng ban - Xóa",
                        Content = content,
                        CompanyId = query.GroupId,
                        UserId = item.UserId,
                        ParentMajorId = item.ParentMajorId,
                        MajorId = item.MajorId,
                        IdCheck = query.Id,
                        IdLog = "",
                        IsMail = true,
                        IsNotification = true,
                        IsSMS = true,
                        GroupId = groupId,
                        CreateAt = DateTime.Now,
                        CreateBy = user.Id,
                        IsRead = 0
                    };
                    listInsert.Add(emailhistory);
                }
                await _emailHistoryService.InsertMulti(listInsert);

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
                var entity = await _departmentService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy ", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt", null));

                string thongbao = entity.IsStatus;

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
                            _majorId,
                            entity.IsActive == 0 ? "93ec3a5d-ce6f-41ed-b187-2f45ae048bd8"
                                : entity.IsActive == 1 ? "41566fcc-d0e8-482d-9408-e1744399ee9a"
                                : entity.IsActive == 2 ? "558d30b4-baf0-4bbf-add4-23ab42b53701"
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

                    await _departmentService.Approval(entity, userId);
                    return Ok(new ApiResponse<object>(true, "Đã duyệt " + thongbao.ToLower(), null));
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
                var entity = await _departmentService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy ", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _departmentService.NoApproval(entity, userId);
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

    }
}
