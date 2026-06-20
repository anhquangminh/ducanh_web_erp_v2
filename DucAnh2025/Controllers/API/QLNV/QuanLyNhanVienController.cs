using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.Services.HeThong;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.QLNV
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuanLyNhanVienController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "fcf752d9-c19a-496d-bba9-f0864928f32b";

        private readonly IQLNV_QuanLyNhanVienRepository _quanLyNhanVienService;
        private readonly PermissionService _permissionService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IQLNV_NhomNhanVienRepository _nhomNhanVienService;
        private readonly IQLNV_NhanVienRepository _nhanvienService;

        public QuanLyNhanVienController(IQLNV_QuanLyNhanVienRepository quanLyNhanVienRepository,
            PermissionService permissionService,
            IPhanQuyenRepository phanQuyenRepository,
            IApplicationUserRepository applicationUserRepository,
            IQLNV_NhomNhanVienRepository qLNV_NhomNhanVienRepository,
            IQLNV_NhanVienRepository qLNV_NhanVienRepository
            )
        {
            _quanLyNhanVienService = quanLyNhanVienRepository;
            _permissionService = permissionService;
            _phanQuyenService = phanQuyenRepository;
            _applicationUserService = applicationUserRepository;
            _nhomNhanVienService = qLNV_NhomNhanVienRepository;
            _nhanvienService = qLNV_NhanVienRepository;
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<QLNV_QuanLyNhanVien>>> GetById(string id)
        {
            try
            {
                var quanLyNhanVien = await _quanLyNhanVienService.GetById(id);
                if (quanLyNhanVien == null)
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không tìm thấy thông tin", null));
                }
                return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(true, "Thành công", quanLyNhanVien));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CongViec>(false, "Lỗi " + ex.Message, null));
            }

        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QLNV_QuanLyNhanVien>>> Create([FromBody] QLNV_QuanLyNhanVien input, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                input.CreateBy = user.Id;
                input.CreateAt = DateTime.Now;
                bool isEdit = !string.IsNullOrEmpty(input.Id) && input.Ordinarily > 0;

                // Kiểm tra quyền thêm (permissionType = 3)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 3);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, $"Bạn không có quyền thêm nhóm nhân viên", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "bcc95d1d-766f-4e81-84da-4d3ef4edd1cf");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "bcc95d1d-766f-4e81-84da-4d3ef4edd1cf");

                // Check tồn tại
                if (await _quanLyNhanVienService.CheckExist("", input))
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, $"Đã tồn tại ", input));
                }
                // Check model
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Dữ liệu không hợp lệ", null));
                }

                // Check liên kết: Chi nhánh, Bộ phận, Chức vụ, Chuyên môn
                var checkResult = await CheckReferencesAsync(input);
                if (checkResult != null)
                {
                    return Ok(checkResult);
                }

                // Tạo nhân viên
                input.Id = Guid.NewGuid().ToString();
                input.ApprovalId = firstApproval.Id;
                input.LastApprovalId = lastApproval.Id;
                input.ApprovalOrder = 1;
                input.DepartmentOrder = 1;
                input.Ordinarily = 0;
                input.IsStatus = firstApproval.Content;
                input.IsActive = 0;
                input.ApprovalUserId = null;
                input.DateApproval = null;
                input.ApprovalDept = null;
                await _quanLyNhanVienService.Insert(input, userName);

                return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(true, "Thêm nhân viên vào nhóm thành công", input));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }

            //try
            //{
            //    // Kiểm tra quyền sửa (permissionType = 3)
            //    _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
            //    bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 3);
            //    if (!hasAddPermission)
            //        return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền thêm nhân viên vào nhóm", null));


            //    if (!ModelState.IsValid)
            //    {
            //        return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Dữ liệu không hợp lệ", null));
            //    }

            //    var isExist = await _quanLyNhanVienService.CheckExist("", quanLyNhanVien);
            //    if (isExist)
            //    {
            //        return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Đã tồn tại nhân viên trong nhóm", null));
            //    }

            //    quanLyNhanVien.Id = Guid.NewGuid().ToString();
            //    await _quanLyNhanVienService.Insert(quanLyNhanVien, userName);
            //    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(true, "Thêm nhân viên thành công", quanLyNhanVien));
            //}
            //catch (Exception ex)
            //{

            //    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Lỗi " + ex.Message, null));
            //}

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromQuery] string userName, [FromBody] QLNV_QuanLyNhanVien input)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                // Kiểm tra hợp lệ ParentId và id_QuanLy
                var nhomIdValid = await _nhomNhanVienService.CheckStatus(input.Id_NhomNhanVien, "");
                var nhanvienIdValid = await _applicationUserService.CheckStatus(input.Id_NhanVien, "");

                input.GroupId = groupId;
                input.CreateBy = userId;
                input.CreateAt = DateTime.Now;

                bool editcheck = (input.Ordinarily > 0) || (input.Ordinarily == 0 && input.IsActive == 3);

                if (string.IsNullOrEmpty(input.Id))
                    input.Id = Guid.NewGuid().ToString();

                if (editcheck)
                {
                    string[] ids = { id };
                    var isValid = await _nhomNhanVienService.CheckExclusive(ids, baseTime);

                    bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 4);
                    if (!hasAddPermission)
                        return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền cập nhật nhóm nhân viên", null));

                    if (isValid)
                    {
                        var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "475f8637-7871-49f2-8214-5f8cfc245d1c");
                        var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "475f8637-7871-49f2-8214-5f8cfc245d1c");
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

                        await _quanLyNhanVienService.Update(input, "");
                        return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                    }
                }
                else
                {
                    bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 4);
                    if (!hasAddPermission)
                        return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền cập nhật nhóm nhân viên", null));

                    input.IsActive = 0;
                    var checkSave = await _quanLyNhanVienService.CheckExist(id, input);
                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "475f8637-7871-49f2-8214-5f8cfc245d1c");
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "475f8637-7871-49f2-8214-5f8cfc245d1c");
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

                    await _quanLyNhanVienService.Update(input, "");
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
        public async Task<IActionResult> DeleteQuanLyNhanVien(string id, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _quanLyNhanVienService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy", null));

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));
                }

                // Kiểm tra quyền sửa (permissionType = 5)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 5);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền xóa ", null));


                bool isInUse = await _quanLyNhanVienService.IsIdInUse(id);
                if (isInUse)
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không thể xoá - đã liên kết dữ liệu", null));
                }


                string[] ids = { id };
                var isValid = await _quanLyNhanVienService.CheckExclusive(ids, baseTime);
                if (!isValid)
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));
                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, _majorId, "4c7ac566-5404-4cc9-9f3a-7292980bfbc7");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, _majorId, "4c7ac566-5404-4cc9-9f3a-7292980bfbc7");

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

                await _quanLyNhanVienService.Update(query, "");

                return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_QuanLyNhanVienModel>>>> GetByVM(string groupId, QLNV_QuanLyNhanVienModel input)
        {
            try
            {
                var quanLyNhanViens = await _quanLyNhanVienService.GetByVM(groupId, input);
                return Ok(new ApiResponse<IEnumerable<QLNV_QuanLyNhanVienModel>>(true, "Thành công", quanLyNhanViens));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CongViec>(false, "Lỗi " + ex.Message, null));
            }

        }

        private async Task<ApiResponse<QLNV_QuanLyNhanVien>?> CheckReferencesAsync(QLNV_QuanLyNhanVien input)
        {
            if (!await ExistsAsync(_nhomNhanVienService.GetById(input.Id_NhomNhanVien)))
                return new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không tìm thấy nhóm đã chọn", null);

            if (!await ExistsAsync(_nhanvienService.GetById(input.Id_NhanVien)))
                return new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không tìm thấy người đã chọn", null);

            return null;
        }
        private async Task<bool> ExistsAsync<T>(Task<T> task)
        {
            try
            {
                var result = await task;
                return result != null;
            }
            catch
            {
                return false;
            }
        }


        [HttpPost("Duyet")]
        public async Task<ActionResult<ApiResponse<object>>> Duyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _quanLyNhanVienService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy", null));

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
                            entity.IsActive == 0 ? "bcc95d1d-766f-4e81-84da-4d3ef4edd1cf"
                                : entity.IsActive == 1 ? "475f8637-7871-49f2-8214-5f8cfc245d1c"
                                : entity.IsActive == 2 ? "4c7ac566-5404-4cc9-9f3a-7292980bfbc7"
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

                    await _quanLyNhanVienService.Approval(entity, userId);
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
                var entity = await _quanLyNhanVienService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _quanLyNhanVienService.NoApproval(entity, userId);
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
