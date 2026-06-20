using DucAnh2025.Models.Accounts;
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
    public class NhomNhanVienController : ControllerBase
    {

        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba";

        private readonly IQLNV_NhomNhanVienRepository _nhomNhanVienService;
        private readonly IQLNV_NhanVienRepository _nhanVienService;
        private readonly PermissionService _permissionService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IQLNV_NhanVienRepository _nhanvienService;

        public NhomNhanVienController(IQLNV_NhomNhanVienRepository nhomNhanVienRepository,
            IQLNV_NhanVienRepository nhanVienService,
            PermissionService permissionService,
            IApplicationUserRepository applicationUserService,
            IPhanQuyenRepository phanQuyenRepository,
            IChiNhanhRepository chiNhanhRepository,
            IQLNV_NhanVienRepository qLNV_NhanVienRepository)
        {
            _nhomNhanVienService = nhomNhanVienRepository;
            _nhanVienService = nhanVienService;
            _permissionService = permissionService;
            _applicationUserService = applicationUserService;
            _phanQuyenService = phanQuyenRepository;
            _chiNhanhService = chiNhanhRepository;
            _nhanvienService = qLNV_NhanVienRepository;
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<QLNV_NhomNhanVien>>> GetById(string id)
        {
            var nhomNhanVien = await _nhomNhanVienService.GetById(id);
            if (nhomNhanVien == null)
            {
                return Ok(new ApiResponse<QLNV_NhomNhanVien>(false, "Không tìm tháy nhóm nhân viên", null));
            }
            return Ok(new ApiResponse<QLNV_NhomNhanVien>(true, "Thành công", nhomNhanVien));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhomNhanVien>>>> GetAllNhomNhanViens(string groupId)
        {
            var nhomNhanViens = await _nhomNhanVienService.GetAll(groupId);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhomNhanVien>>(true, "Thành công", nhomNhanViens));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QLNV_NhomNhanVien>>> CreateNhomNhanVien([FromBody] QLNV_NhomNhanVien input, [FromQuery] string userName)
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
                    return Ok(new ApiResponse<QLNV_NhomNhanVien>(false, $"Bạn không có quyền thêm nhóm nhân viên", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "8e01b3b4-21cc-4f76-9c09-2147ef0b4012");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "8e01b3b4-21cc-4f76-9c09-2147ef0b4012");

                // Check tồn tại
                if (await _nhomNhanVienService.CheckExist("", input))
                {
                    return Ok(new ApiResponse<QLNV_NhomNhanVien>(false, $"Đã tồn tại {input.TenNhom}", input));
                }
                // Check model
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<QLNV_NhomNhanVien>(false, "Dữ liệu không hợp lệ", null));
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
                await _nhomNhanVienService.Insert(input, userName);

                return Ok(new ApiResponse<QLNV_NhomNhanVien>(true, "Tạo nhóm thành công", input));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        private async Task<ApiResponse<QLNV_NhomNhanVien>?> CheckReferencesAsync(QLNV_NhomNhanVien input)
        {
            if (!await ExistsAsync(_chiNhanhService.GetById(input.CompanyId)))
                return new ApiResponse<QLNV_NhomNhanVien>(false, "Không tìm thấy chi nhánh đã chọn", null);

            if (!await ExistsAsync(_nhanvienService.GetById(input.Id_QuanLy)))
                return new ApiResponse<QLNV_NhomNhanVien>(false, "Không tìm thấy người quản lý đã chọn", null);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNhomNhanVien(string id, [FromQuery] string userName, [FromBody] QLNV_NhomNhanVien input)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                // Kiểm tra hợp lệ ParentId và id_QuanLy
                var ParentIdValid = await _chiNhanhService.CheckStatus(input.CompanyId, "");
                var nhanvien = await _nhanvienService.GetById(input.Id_QuanLy);
                ApplicationUser quanLy =  _applicationUserService.GetByUserName(nhanvien.TaiKhoan);
                var QuanLyValid = await _applicationUserService.CheckStatus(quanLy.Id, "");

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
                        var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec");
                        var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec");
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

                        await _nhomNhanVienService.Update(input, "");
                        return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                    }
                }
                else
                {
                    bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 4);
                    if (!hasAddPermission)
                        return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền cập nhật nhóm nhân viên", null));

                    input.IsActive = 1;
                    var checkSave = await _nhomNhanVienService.CheckExist(id,input);
                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, _majorId, "430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec");
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, _majorId, "430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec");
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

                    await _nhomNhanVienService.Update(input, "");
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
        public async Task<IActionResult> DeleteNhomNhanVien(string id, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _nhomNhanVienService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy loại chi nhánh", null));

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));
                }

                // Kiểm tra quyền sửa (permissionType = 5)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 5);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền xóa nhóm nhân viên", null));


                bool isInUse = await _nhomNhanVienService.IsIdInUse(id);
                if (isInUse)
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không thể xoá - đã liên kết dữ liệu", null));
                }


                string[] ids = { id };
                var isValid = await _nhomNhanVienService.CheckExclusive(ids, baseTime);
                if (!isValid)
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));
                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, _majorId, "2483777c-815d-4a54-af1a-58468bfd51a1");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, _majorId, "2483777c-815d-4a54-af1a-58468bfd51a1");

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

                await _nhomNhanVienService.Update(query, "");

                return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>>> GetByVM(string groupId, [FromBody] QLNV_NhomNhanVienModel input)
        {
            try
            {
                var nhanViens = await _nhomNhanVienService.GetByVM(groupId, input);
                return Ok(new ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>(true, "Thành công", nhanViens));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_NhomNhanVien>(false, "Lỗi: " + ex.Message, null));
            }
        }
        [HttpGet("CheckExist")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExist(string id, QLNV_NhomNhanVien input)
        {
            var exists = await _nhomNhanVienService.CheckExist(id, input);
            return Ok(new ApiResponse<bool>(true, "Thành công", exists));
        }

        [HttpGet("IsIdInUse/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> IsIdInUse(string id)
        {
            var isInUse = await _nhomNhanVienService.IsIdInUse(id);
            return Ok(new ApiResponse<bool>(true, "Thành công", isInUse));
        }

        [HttpGet("CheckStatus")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckStatus(string ids, string name)
        {
            var status = await _nhomNhanVienService.CheckStatus(ids, name);
            return Ok(new ApiResponse<bool>(true, "Thành công", status));
        }

        [HttpGet("CheckExclusive")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExclusive(string[] ids, DateTime baseTime)
        {
            var exclusive = await _nhomNhanVienService.CheckExclusive(ids, baseTime);
            return Ok(new ApiResponse<bool>(true, "Thành công", exclusive));
        }
        [HttpGet("GetNhomNhanVienByTaiKhoanAsync")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>>> GetNhomNhanVienByTaiKhoanAsync(string groupId, string companyId, string taiKhoan)
        {
            var nhomNhanViens = await _nhomNhanVienService.GetNhomNhanVienByTaiKhoanAsync(groupId, companyId, taiKhoan);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>(true, "Thành công", nhomNhanViens));
        }
        [HttpGet("GetNhomNhanVienByCVDGAsync")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>>> GetNhomNhanVienByCVDGAsync(string groupId, string taiKhoan)
        {
            var nhomNhanViens = await _nhomNhanVienService.GetNhomNhanVienByCVDGAsync(groupId, taiKhoan);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhomNhanVienModel>>(true, "Thành công", nhomNhanViens));
        }


        [HttpPost("Duyet")]
        public async Task<ActionResult<ApiResponse<object>>> Duyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _nhomNhanVienService.GetById(id);
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
                            entity.IsActive == 0 ? "8e01b3b4-21cc-4f76-9c09-2147ef0b4012"
                                : entity.IsActive == 1 ? "430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec"
                                : entity.IsActive == 2 ? "2483777c-815d-4a54-af1a-58468bfd51a1"
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

                    await _nhomNhanVienService.Approval(entity, userId);
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
                var entity = await _nhomNhanVienService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy ", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _nhomNhanVienService.NoApproval(entity, userId);
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
