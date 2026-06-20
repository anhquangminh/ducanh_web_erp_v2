using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChiNhanhController : ControllerBase
    {
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly ICompanyTypeRepository _companyTypeService;
        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        public ChiNhanhController(IChiNhanhRepository chiNhanhRepository,
            ICompanyTypeRepository companyTypeRepository,
            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService)
        {
            _chiNhanhService = chiNhanhRepository;
            _companyTypeService = companyTypeRepository;
            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChiNhanh>>>> GetAll([FromQuery] string groupId)
        {
            try
            {
                var chiNhanhs = await _chiNhanhService.GetAll(groupId);
                var Data = new ApiResponse<IEnumerable<ChiNhanh>>(true, "Thành công", chiNhanhs);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByVM(string groupId, [FromBody] ChiNhanhModel input)
        {
            try
            {
                var list = await _chiNhanhService.GetAllByVM(input, groupId);
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
               var result = await _chiNhanhService.GetById(id);
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
                var result = await _chiNhanhService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("CreateChiNhanh")]
        public async Task<ActionResult<ApiResponse<object>>> CreateChiNhanh([FromBody] ChiNhanh input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                // Kiểm tra hợp lệ ParentId và CompanyType
                var ParentIdValid = await _chiNhanhService.CheckStatus(input.ParentId, "");
                var CompanyTypeValid = await _companyTypeService.CheckStatus(input.CompanyType, "");

                input.GroupId = groupId;
                input.CreateBy = userId;
                input.CreateAt = DateTime.Now;

                bool editcheck = (isEdit && input.Ordinarily > 0) || (isEdit && input.Ordinarily == 0 && input.IsActive == 3);

                if (string.IsNullOrEmpty(input.Id))
                    input.Id = Guid.NewGuid().ToString();

                if (editcheck)
                {
                    string[] ids = { input.Id };
                    var isValid = await _chiNhanhService.CheckExclusive(ids, baseTime);
                    var checkEdit = await _chiNhanhService.CheckEdit(input);
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, "25a73733-e932-5cf0-231a-cd2469d2d091");
                    if (!checkquyen)
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền sửa", null));

                    if (isValid)
                    {
                        var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "25a73733-e932-5cf0-231a-cd2469d2d091");
                        var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "25a73733-e932-5cf0-231a-cd2469d2d091");
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

                        await _chiNhanhService.Update(input, "");

                        // Build email content
                        string content = "<h3>Thông tin sửa</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th class=\"text-center\" rowspan=\"2\" scope=\"col\"><div class=\"pb-3\">No.</div></th><th><div class=\"pb-3\">Thuộc tổ chức</div></th><th><div class=\"pb-3\">Tên chi nhánh</div></th><th><div class=\"pb-3\">Loại chi nhánh</div></th><th><div class=\"pb-3\">Điện thoại</div></th><th><div class=\"pb-3\">Email</div></th><th><div class=\"pb-3\">Địa chỉ</div></th></tr></thead>";
                        var isValidModel = await _chiNhanhService.GetHistoryIsValidEdit(input.Id);
                        if (isValidModel.Any())
                        {
                            content += "<tbody>";
                            content += "<tr><td class=\"text-center\" scope=\"row\">Dữ liệu cũ</td><td class=\"text-left\">" + isValidModel[0].ParentId + "</td><td class=\"text-left\">" + isValidModel[0].TenChiNhanh + "</td><td class=\"text-left\">" + isValidModel[0].CompanyType + "</td><td class=\"text-left\">" + isValidModel[0].Phone + "</td><td class=\"text-left\">" + isValidModel[0].Email + "</td><td class=\"text-left\">" + isValidModel[0].Address + "</td></tr>";
                            if (isValidModel.Count > 1)
                                content += "<tr><td class=\"text-center\" scope=\"row\">Dữ liệu mới</td><td class=\"text-left\">" + isValidModel[1].ParentId + "</td><td class=\"text-left\">" + isValidModel[1].TenChiNhanh + "</td><td class=\"text-left\">" + isValidModel[1].CompanyType + "</td><td class=\"text-left\">" + isValidModel[1].Phone + "</td><td class=\"text-left\">" + isValidModel[1].Email + "</td><td class=\"text-left\">" + isValidModel[1].Address + "</td></tr>";
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
                                Subject = "Chi nhánh - Sửa.",
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
                                GroupId = groupId,
                                CreateAt = DateTime.Now,
                                CreateBy = user.Id,
                                IsRead = 0
                            });
                        }
                        await _emailHistoryService.InsertMulti(listInsert);

                        return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                    }
                }
                else
                {
                    var checkquyen = await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, "22cd5c88-a9d1-6189-8d91-d856b4595815");
                    if (!checkquyen)
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền thêm mới", null));

                    input.IsActive = 0;
                    var checkSave = await _chiNhanhService.CheckSave(input);
                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "22cd5c88-a9d1-6189-8d91-d856b4595815");
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "22cd5c88-a9d1-6189-8d91-d856b4595815");
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

                    if (isEdit)
                        await _chiNhanhService.Update(input, "");
                    else
                        await _chiNhanhService.Insert(input, "");

                    // Build email content
                    string content = "<h3>Thông tin thêm</h3><table class=\"table table-hover table-bordered\"><thead class=\"bg-info\"><tr><th><div class=\"pb-3\">Thuộc tổ chức</div></th><th><div class=\"pb-3\">Tên chi nhánh</div></th><th><div class=\"pb-3\">Loại chi nhánh</div></th><th><div class=\"pb-3\">Điện thoại</div></th><th><div class=\"pb-3\">Email</div></th><th><div class=\"pb-3\">Địa chỉ</div></th></tr></thead><tbody><tr><td class=\"text-left\">" + "" + "</td><td class=\"text-left\">" + input.TenChiNhanh + "</td><td class=\"text-left\">" + "" + "</td><td class=\"text-left\">" + input.Phone + "</td><td class=\"text-left\">" + input.Email + "</td><td class=\"text-left\">" + input.Address + "</td></tr></tbody></table>";

                    var listEmail = await _emailHistoryService.GetUserPermission(input.GroupId, firstApproval.Id);
                    var listInsert = new List<EmailHistory>();
                    foreach (var item in listEmail)
                    {
                        listInsert.Add(new EmailHistory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Receiver = item.Mail,
                            Subject = "Chi nhánh - Thêm",
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
                            GroupId = groupId,
                            CreateAt = DateTime.Now,
                            CreateBy = user.Id,
                            IsRead = 0
                        });
                    }
                    await _emailHistoryService.InsertMulti(listInsert);

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
        public async Task<IActionResult> DeleteChiNhanh(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var user = _applicationUserService.GetCurrentUser();
                    var groupId = user.GroupId;
                    var userId = user.Id;
                    var baseTime = DateTime.Now;

                    var query = await _chiNhanhService.GetById(id);
                    if (query.IsActive == 2)
                    {
                        return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));
                    }
                    else
                    {
                        var checkDelete = await _chiNhanhService.CheckDelete(query);
                        if (!checkDelete)
                            return Ok(new ApiResponse<object>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                        var checkquyen = await _phanQuyenService.CheckPermission(groupId, query.GroupId, user, "db666b7c-1e63-19df-dd09-1d860de545a4");
                        if (!checkquyen)
                            return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa", null));

                        string[] ids = { id };
                        var isValid = await _chiNhanhService.CheckExclusive(ids, baseTime);
                        if (!isValid)
                            return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

                        var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "db666b7c-1e63-19df-dd09-1d860de545a4");
                        var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, "74537f8f-a779-4313-ab4d-010d2d4ebab3", "db666b7c-1e63-19df-dd09-1d860de545a4");

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

                        await _chiNhanhService.Update(query, "");

                        var result = await _chiNhanhService.GetDetails(id);
                        string content = "";
                        content += "<h3>Thông tin xóa</h3>";
                        content += "<table class=\"table table-hover table-bordered\">";
                        content += "<thead class=\"bg-info\">";
                        content += "<tr>";
                        content += "<th><div class=\"pb-3\">Thuộc tổ chức</div></th>";
                        content += "<th><div class=\"pb-3\">Tên chi nhánh</div></th>";
                        content += "<th><div class=\"pb-3\">Loại chi nhánh</div></th>";
                        content += "<th><div class=\"pb-3\">Điện thoại</div></th>";
                        content += "<th><div class=\"pb-3\">Email</div></th>";
                        content += "<th><div class=\"pb-3\">Địa chỉ</div></th>";
                        content += "</tr>";
                        content += "</thead>";
                        content += "<tbody>";
                        content += "<tr>";
                        content += $"<td class=\"text-left\">{result.ParentId}</td>";
                        content += $"<td class=\"text-left\">{result.TenChiNhanh}</td>";
                        content += $"<td class=\"text-left\">{result.CompanyType}</td>";
                        content += $"<td class=\"text-left\">{result.Phone}</td>";
                        content += $"<td class=\"text-left\">{result.Email}</td>";
                        content += $"<td class=\"text-left\">{result.Address}</td>";
                        content += "</tr>";
                        content += "</tbody>";
                        content += "</table>";

                        var listEmail = await _emailHistoryService.GetUserPermission(query.GroupId, firstApproval.Id);
                        var listInsert = new List<EmailHistory>();
                        foreach (var item in listEmail)
                        {
                            var emailhistory = new EmailHistory()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Receiver = item.Mail,
                                Subject = "Chi nhánh - Xóa",
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
                }
                return Ok(new ApiResponse<object>(false, "Không xác định được chi nhánh cần xóa.", null));
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
                var entity = await _chiNhanhService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy chi nhánh", null));

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
                            "74537f8f-a779-4313-ab4d-010d2d4ebab3",
                            entity.IsActive == 0 ? "22cd5c88-a9d1-6189-8d91-d856b4595815"
                                : entity.IsActive == 1 ? "25a73733-e932-5cf0-231a-cd2469d2d091"
                                : entity.IsActive == 2 ? "db666b7c-1e63-19df-dd09-1d860de545a4"
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

                    await _chiNhanhService.Approval(entity, userId);
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
                var entity = await _chiNhanhService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy chi nhánh", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _chiNhanhService.NoApproval(entity, userId);
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
