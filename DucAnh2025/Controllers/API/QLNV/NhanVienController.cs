using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.Services;
using DucAnh2025.Services.HeThong;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.QLNV
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IQLNV_NhanVienRepository _nhanVienRepository;
        private readonly IChiNhanhRepository _chiNhanhRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDM_ChucVuRepository _dM_ChucVuRepository;
        private readonly IDM_ChuyenMonRepository _dM_ChuyenMonRepository;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IEmailHistoryRepository _emailService;
        private readonly IHelperService _helperService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PermissionService _permissionService;
        private readonly IPhanQuyenRepository _phanQuyenService;

        public NhanVienController(IQLNV_NhanVienRepository nhanVienRepository,
            IApplicationUserRepository applicationUserService,
            IChiNhanhRepository chiNhanhService,
            IDepartmentRepository departmentService,
            IDM_ChucVuRepository dM_ChucVuService,
            IDM_ChuyenMonRepository dM_ChuyenMonService,
            IEmailHistoryRepository emailService,
            IHelperService helperService,
            UserManager<IdentityUser> userManager,
            PermissionService permissionService,
            IPhanQuyenRepository phanQuyenService)
        {
            _nhanVienRepository = nhanVienRepository;
            _applicationUserService = applicationUserService;
            _chiNhanhRepository = chiNhanhService;
            _departmentRepository = departmentService;
            _dM_ChucVuRepository = dM_ChucVuService;
            _dM_ChuyenMonRepository = dM_ChuyenMonService;
            _emailService = emailService;
            _helperService = helperService;
            _userManager = userManager;
            _permissionService = permissionService;
            _phanQuyenService = phanQuyenService;
        }


        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVienModel>>>> GetByVM(string groupId, [FromBody] QLNV_NhanVienModel input)
        {
            try
            {
                var nhanViens = await _nhanVienRepository.GetByVM(groupId, input);
                return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienModel>>(true, "Thành công", nhanViens));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienModel>>(false, ex.Message, null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVien>>> GetNhanVienById(string id)
        {
            var nhanVien = await _nhanVienRepository.GetById(id);
            if (nhanVien == null)
            {
                return Ok(new ApiResponse<QLNV_NhanVien>(false, "Không tìm thấy nhân viên", null));
            }
            return Ok(new ApiResponse<QLNV_NhanVien>(true, "Thành công", nhanVien));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVien>>>> GetAllNhanViens(string groupId)
        {
            var nhanViens = await _nhanVienRepository.GetAll(groupId);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVien>>(true, "Thành công", nhanViens));
        }

        [HttpPost("CreateNhanVienNotTaiKhoan")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVien>>> CreateNhanVienNotTaiKhoan(QLNV_NhanVien nhanVien, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                nhanVien.CreateBy = user.Id;
                nhanVien.CreateAt = DateTime.Now;
                bool isEdit = !string.IsNullOrEmpty(nhanVien.Id) && nhanVien.Ordinarily > 0;

                // Kiểm tra quyền thêm (permissionType = 3)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 3);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền thêm nhân viên", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(nhanVien.GroupId, _majorId, "be25c8c3-5728-43ed-b82a-ab29d3fe4a61");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(nhanVien.GroupId, _majorId, "be25c8c3-5728-43ed-b82a-ab29d3fe4a61");

                // Check tồn tại
                if (await _nhanVienRepository.CheckExist("", nhanVien))
                {
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Đã tồn tại {nhanVien.TaiKhoan}", nhanVien));
                }
                // Check model
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, "Dữ liệu không hợp lệ", null));
                }

                // Check liên kết: Chi nhánh, Bộ phận, Chức vụ, Chuyên môn
                var checkResult = await CheckReferencesAsync(nhanVien);
                if (checkResult != null)
                {
                    return Ok(checkResult);
                }

                // Tạo nhân viên
                nhanVien.Id = Guid.NewGuid().ToString();
                nhanVien.ApprovalId = firstApproval.Id;
                nhanVien.LastApprovalId = lastApproval.Id;
                nhanVien.ApprovalOrder = 1;
                nhanVien.DepartmentOrder = 1;
                nhanVien.Ordinarily = 0;
                nhanVien.IsStatus = firstApproval.Content;
                nhanVien.ApprovalUserId = null;
                nhanVien.DateApproval = null;
                nhanVien.ApprovalDept = null;
                await _nhanVienRepository.Insert(nhanVien, userName);

                return Ok(new ApiResponse<QLNV_NhanVien>(true, "Tạo nhân viên thành công", nhanVien));
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
                var result = await _nhanVienRepository.GetById(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVien>>> CreateNhanVien(QLNV_NhanVien nhanVien, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                nhanVien.CreateBy = user.Id;
                nhanVien.CreateAt = DateTime.Now;
                bool isEdit = !string.IsNullOrEmpty(nhanVien.Id) && nhanVien.Ordinarily > 0;

                // Kiểm tra quyền thêm (permissionType = 3)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 3);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền thêm nhân viên", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(nhanVien.GroupId, _majorId, "be25c8c3-5728-43ed-b82a-ab29d3fe4a61");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(nhanVien.GroupId, _majorId, "be25c8c3-5728-43ed-b82a-ab29d3fe4a61");

                // Check tồn tại
                if (await _nhanVienRepository.CheckExist("", nhanVien))
                {
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Đã tồn tại {nhanVien.TaiKhoan}", nhanVien));
                }
                // Check model
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, "Dữ liệu không hợp lệ", null));
                }

                // Check liên kết: Chi nhánh, Bộ phận, Chức vụ, Chuyên môn
                var checkResult = await CheckReferencesAsync(nhanVien);
                if (checkResult != null)
                {
                    return Ok(checkResult);
                }

                //// Check user tồn tại
                //var userExists = _applicationUserService.GetByUserName(nhanVien.TaiKhoan) != null;

                //// Tạo user nếu chưa có
                //if (!userExists)
                //{
                //    var registerSuccess = await RegisterUser(nhanVien, userName);
                //    if (!registerSuccess)
                //    {
                //        return Ok(new ApiResponse<QLNV_NhanVien>(false, "Đăng ký tài khoản thất bại", null));
                //    }
                //}

                // Tạo nhân viên
                nhanVien.Id = Guid.NewGuid().ToString();
                nhanVien.ApprovalId = firstApproval.Id;
                nhanVien.LastApprovalId = lastApproval.Id;
                nhanVien.ApprovalOrder = 1;
                nhanVien.DepartmentOrder = 1;
                nhanVien.Ordinarily = 0;
                nhanVien.IsActive = 0;
                nhanVien.IsStatus = firstApproval.Content;
                nhanVien.ApprovalUserId = null;
                nhanVien.DateApproval = null;
                nhanVien.ApprovalDept = null;
                await _nhanVienRepository.Insert(nhanVien, userName);

                return Ok(new ApiResponse<QLNV_NhanVien>(true, "Tạo nhân viên thành công", nhanVien));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }


            //// Kiểm tra quyền thêm (permissionType = 3)
            //_userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
            //bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken,_parentMajorId, _majorId, 3);
            //if (!hasAddPermission)
            //    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền thêm nhân viên", null));

            //// Check tồn tại
            //if (await _nhanVienRepository.CheckExist("", nhanVien))
            //{
            //    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Đã tồn tại {nhanVien.TaiKhoan}", nhanVien));
            //}
            //// Check model
            //if (!ModelState.IsValid)
            //{
            //    return Ok(new ApiResponse<QLNV_NhanVien>(false, "Dữ liệu không hợp lệ", null));
            //}

            //// Check liên kết: Chi nhánh, Bộ phận, Chức vụ, Chuyên môn
            //var checkResult = await CheckReferencesAsync(nhanVien);
            //if (checkResult != null)
            //{
            //    return Ok(checkResult);
            //}

            //// Check user tồn tại
            //var userExists = _applicationUserService.GetByUserName(nhanVien.TaiKhoan) != null;

            //// Tạo user nếu chưa có
            //if (!userExists)
            //{
            //    var registerSuccess = await RegisterUser(nhanVien, userName);
            //    if (!registerSuccess)
            //    {
            //        return Ok(new ApiResponse<QLNV_NhanVien>(false, "Đăng ký tài khoản thất bại", null));
            //    }
            //}

            //// Tạo nhân viên
            //nhanVien.Id = Guid.NewGuid().ToString();
            //await _nhanVienRepository.Insert(nhanVien, userName);

            //return Ok(new ApiResponse<QLNV_NhanVien>(true, "Tạo nhân viên thành công", nhanVien));
        }
        private async Task<ApiResponse<QLNV_NhanVien>?> CheckReferencesAsync(QLNV_NhanVien nhanVien)
        {
            if (!await ExistsAsync(_chiNhanhRepository.GetById(nhanVien.CompanyId)))
                return new ApiResponse<QLNV_NhanVien>(false, "Không tìm thấy chi nhánh đã chọn", null);

            if (!await ExistsAsync(_departmentRepository.GetById(nhanVien.DepartmentId)))
                return new ApiResponse<QLNV_NhanVien>(false, "Không tìm thấy bộ phận đã chọn", null);

            if (!await ExistsAsync(_dM_ChucVuRepository.GetById(nhanVien.ChucVuId)))
                return new ApiResponse<QLNV_NhanVien>(false, "Không tìm thấy chức vụ đã chọn", null);

            if (!await ExistsAsync(_dM_ChuyenMonRepository.GetById(nhanVien.ChuyenMonId)))
                return new ApiResponse<QLNV_NhanVien>(false, "Không tìm thấy chuyên môn đã chọn", null);

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
        public async Task<bool> RegisterUser(QLNV_NhanVien Input, string userName)
        {
            var existingUser = await _userManager.FindByNameAsync(Input.TaiKhoan);
            if (existingUser != null)
            {
                var deleteResult = await _userManager.DeleteAsync(existingUser);
                if (!deleteResult.Succeeded)
                {
                    return false;
                }
            }

            //var password = _helperService.GeneratePassword(8);
            var password = "Da!123456";
            var iUser = new IdentityUser
            {
                UserName = Input.TaiKhoan,
                Email = Input.TaiKhoan,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(iUser, password);
            if (!result.Succeeded)
            {
                return false;
            }

            var appUser = CreateAppUser(Input.GroupId, Input.CompanyId, Input.DepartmentId, iUser, userName, Input.TenNhanVien);
            await _applicationUserService.Insert(appUser, "");

            var callbackUrl = "http://117.4.35.44:8080/Account/Login";
            await SendMail(appUser, callbackUrl, password);

            return true;
        }
        private ApplicationUser CreateAppUser(string groupId, string companyId, string deptId, IdentityUser iUser, string userName, string tenNhanVien)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Id = iUser.Id,
                    UserName = iUser.UserName,
                    Email = iUser.Email,
                    FirstName = tenNhanVien,
                    LastName = "",
                    Address = "",
                    Dob = DateTime.Now,
                    PhoneNumber = "0000000000",
                    IsActive = 3,
                    IsFirstLogin = 0,
                    CreateAt = DateTime.Now,
                    CreateBy = userName,
                    CompanyId = companyId,
                    GroupId = groupId,
                    DeptId = deptId,
                    Ordinarily = 1,
                    ApprovalUserId = "",
                    DateApproval = DateTime.Now,
                    DepartmentId = "",
                    DepartmentOrder = 1,
                    ApprovalOrder = 1,
                    ApprovalId = "",
                    LastApprovalId = "",
                    IsStatus = "Đã kích hoạt"
                };

                return user;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private async Task SendMail(ApplicationUser user, string callBackUrl, string randomPassword)
        {
            // Tạo mới đối tượng
            var emailHistory = new EmailHistory();

            // Nội dung mail
            string content = "";

            content += "<div class=\"email-container\">";
            content += "<div class=\"header\">";
            content += "<h1>Tài khoản sử dụng hệ thống</h1>";
            content += "</div>";
            content += "<div class=\"content\">";
            content += "";
            content += "<p>Xin chào,</p>";
            content += "<p>Bạn đã đăng ký sử dụng hệ thống quản lý doanh nghiệp thành công. Tài khoản của bạn là:</p>";
            content += "<p><strong>Tài khoản:</strong> " + user.Email + "</p>";
            content += "<p><strong>Mật khẩu:</strong> " + System.Net.WebUtility.HtmlEncode(randomPassword) + "</p>";
            content += "<p>Vui lòng truy cập theo đường dẫn dưới đây";
            content += "<p><a href=\"" + callBackUrl + "\">Click vào đây để sử dụng hệ thống</a></p>";
            content += "<p>Xin chân thành cảm ơn!</p>";
            content += "<p>Đây là email được gửi tự động từ hệ thống, vui lòng không trả lời vào địa chỉ này. Mọi thông tin thắc mắc xin vui lòng liên hệ: <a href=\"mailto:xxxxxxxx@gmail.com\">xxxxxxxx@gmail.com</a></p>";
            content += "</div>";
            content += "<div class=\"footer\">";
            content += "";
            content += "<p>Trân trọng,</p>";
            content += "<p>Công ty cổ phần xây dựng Đức Anh</p>";
            content += "</div>";
            content += "</div>";

            // Gán giá trị cho các thuộc tính
            emailHistory.Id = Guid.NewGuid().ToString();
            emailHistory.Receiver = user.Email;
            emailHistory.Subject = "Tài Khoản sử dụng hệ thống";
            emailHistory.Content = content;
            emailHistory.CompanyId = user.CompanyId;
            emailHistory.GroupId = user.CompanyId;
            emailHistory.MajorId = "";
            emailHistory.ParentMajorId = "";
            emailHistory.IsRead = 1;
            emailHistory.CreateAt = DateTime.Now;
            emailHistory.CreateBy = "system";

            // Thực hiện gửi mail
            await _emailService.SendEmail(emailHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNhanVien(string id, QLNV_NhanVien nhanVien, [FromQuery] string userName)
        {
            try
            {
                nhanVien.IsActive = 1;
                // Kiểm tra quyền sửa (permissionType = 4)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 4);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền cập nhật nhân viên", null));

                TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime baseTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

                bool isValid = await _nhanVienRepository.CheckExclusive([nhanVien.Id], baseTime);

                if (isValid)
                {
                    await _nhanVienRepository.Update(nhanVien, userName);
                    return Ok(new ApiResponse<QLNV_NhanVien>(true, "Cập nhật nhân viên thành công", nhanVien));
                }

                return Ok(new ApiResponse<QLNV_NhanVien>(false, "Dữ liệu đã bị thay đổi bởi người khác", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_NhanVien>(false, "Lỗi: " + ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhanVien(string id, [FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _nhanVienRepository.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy loại chi nhánh", null));

                if (query.IsActive == 2)
                {
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));
                }

                // Kiểm tra quyền xóa (permissionType = 5)
                _userNameFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name;
                bool hasAddPermission = await _permissionService.HasPermissionAsync(_userNameFromToken, _parentMajorId, _majorId, 5);
                if (!hasAddPermission)
                    return Ok(new ApiResponse<QLNV_NhanVien>(false, $"Bạn không có quyền xóa nhân viên", null));


                bool isInUse = await _nhanVienRepository.IsIdInUse(id);
                if (isInUse)
                {
                    return Ok(new ApiResponse<QLNV_QuanLyNhanVien>(false, "Không thể xoá - đã liên kết dữ liệu", null));
                }


                string[] ids = { id };
                var isValid = await _nhanVienRepository.CheckExclusive(ids, baseTime);
                if (!isValid)
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));
                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, _majorId, "4e3af927-cca3-4390-a718-64a4a4c20b5a");
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, _majorId, "4e3af927-cca3-4390-a718-64a4a4c20b5a");

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

                await _nhanVienRepository.Update(query, "");

                return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        // Các phương thức khác từ IQLNV_NhanVienRepository
        [HttpGet("CheckExist")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExist(string id, QLNV_NhanVien input)
        {
            var exists = await _nhanVienRepository.CheckExist(id, input);
            return Ok(new ApiResponse<bool>(true, "Thành công", exists));
        }

        [HttpGet("IsIdInUse/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> IsIdInUse(string id)
        {
            var isInUse = await _nhanVienRepository.IsIdInUse(id);
            return Ok(new ApiResponse<bool>(true, "Thành công", isInUse));
        }

        [HttpGet("CheckStatus")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckStatus(string ids, string name)
        {
            var status = await _nhanVienRepository.CheckStatus(ids, name);
            return Ok(new ApiResponse<bool>(true, "Thành công", status));
        }

        [HttpGet("CheckExclusive")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExclusive(string[] ids, DateTime baseTime)
        {
            var exclusive = await _nhanVienRepository.CheckExclusive(ids, baseTime);
            return Ok(new ApiResponse<bool>(true, "Thành công", exclusive));
        }

        [HttpGet("GetNhanVienIsQuanLy")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVien>>>> GetNhanVienIsQuanLy(string groupId, bool isQuanLy)
        {
            var nhanViens = await _nhanVienRepository.GetNhanVienIsQuanLy(groupId, isQuanLy);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVien>>(true, "Thành công", nhanViens));
        }

        [HttpGet("GetNhanVienByNhom")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVien>>>> GetNhanVienByNhom(string groupId, string companyId, string Id_NhomNhanVien)
        {
            var nhanViens = await _nhanVienRepository.GetNhanVienByNhom(groupId, companyId, Id_NhomNhanVien);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVien>>(true, "Thành công", nhanViens));
        }

        [HttpGet("GetNhanVienNotQL")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVien>>>> GetNhanVienNotQL(string groupId, string companyId, string Id_NhomNhanVien)
        {
            var nhanViens = await _nhanVienRepository.GetNhanVienNotQL(groupId, companyId, Id_NhomNhanVien);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVien>>(true, "Thành công", nhanViens));
        }

        [HttpPost("Duyet")]
        public async Task<ActionResult<ApiResponse<object>>> Duyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _nhanVienRepository.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy loại chi nhánh", null));

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

                            // Check user tồn tại
                            var userExists = _applicationUserService.GetByUserName(entity.TaiKhoan) != null;

                            // Tạo user nếu chưa có
                            if (!userExists)
                            {
                                var registerSuccess = await RegisterUser(entity, userId);
                                if (!registerSuccess)
                                {
                                    return Ok(new ApiResponse<object>(false, "Đăng ký tài khoản thất bại", null));
                                }
                            }
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

                            
                            var existingUser = await _userManager.FindByNameAsync(entity.TaiKhoan);
                            if (existingUser != null)
                            {
                                ApplicationUser applicationUser = new();

                                applicationUser = await _applicationUserService.GetById(existingUser.Id);
                                if (applicationUser.CreateBy != "symtem")
                                {
                                    var deleteResult = await _userManager.DeleteAsync(existingUser);
                                    if (deleteResult.Succeeded)
                                    {
                                        await _applicationUserService.DeleteById(existingUser.Id, userId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var nextApproval = await _phanQuyenService.GetNextApprovalStep(
                            entity.GroupId,
                            _majorId,
                            entity.IsActive == 0 ? "be25c8c3-5728-43ed-b82a-ab29d3fe4a61"
                                : entity.IsActive == 1 ? "f08e2d74-528b-487c-98d3-33c988878947"
                                : entity.IsActive == 2 ? "4e3af927-cca3-4390-a718-64a4a4c20b5a"
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

                    await _nhanVienRepository.Approval(entity, userId);
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
                var entity = await _nhanVienRepository.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy loại chi nhánh", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _nhanVienRepository.NoApproval(entity, userId);
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
