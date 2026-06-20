using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.Services;
using DucAnh2025.Share;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace DucAnh2025.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoginService _loginService;
        private readonly IEmailHistoryRepository _emailService;
        private readonly UserState _userState;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IConfiguration _configuration;
        private readonly ICompanyRepository _companyService;
        private readonly IChiNhanhRepository _chiNhanhService;
        private readonly IDepartmentRepository _departmentService;
        private readonly ICompanyTypeRepository _companyTypeService;
        private readonly IHelperService _helperService;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IMajorUserPermissionRepository _majorUserPermissionRepository;
        private readonly IDM_ChucVuRepository _chucVuRepository;
        private readonly IDM_ChuyenMonRepository _chuyenMonRepository;
        private readonly IMemoryCache _memoryCache;

        public UserController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoginService loginService,
            IEmailHistoryRepository emailService,
            UserState userState,
            IApplicationUserRepository applicationUserService,
            IConfiguration configuration,
            ICompanyRepository companyService,
            IChiNhanhRepository chiNhanhService,
            IDepartmentRepository departmentService,
            ICompanyTypeRepository companyTypeService,
            IHelperService helperService,
            IUserStore<IdentityUser> userStore,
            IMajorUserPermissionRepository majorUserPermissionRepository,
            IDM_ChuyenMonRepository chuyenMonRepository,
            IDM_ChucVuRepository chucVuRepository,
            IMemoryCache memoryCache
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _loginService = loginService;
            _emailService = emailService;
            _userState = userState;
            _configuration = configuration;
            _applicationUserService = applicationUserService;
            _companyService = companyService;
            _chiNhanhService = chiNhanhService;
            _departmentService = departmentService;
            _companyTypeService = companyTypeService;
            _helperService = helperService;
            _userStore = userStore;
            _majorUserPermissionRepository = majorUserPermissionRepository;
            _chuyenMonRepository = chuyenMonRepository;
            _chucVuRepository = chucVuRepository;
            _memoryCache = memoryCache;
        }



        //---Đăng nhập bẳng qrcode---
        [HttpGet("qr-session")]
        [AllowAnonymous]
        public IActionResult GetQrSession()
        {
            var sessionId = Guid.NewGuid().ToString();
            // Lưu sessionId vào cache/memory với trạng thái "chưa xác thực"
            _memoryCache.Set<string>(sessionId, null, TimeSpan.FromMinutes(2));
            return Ok(new { sessionId });
        }

        [HttpPost("qr-login")]
        [Authorize(AuthenticationSchemes = "JwtBearer")]
        public IActionResult QrLogin(string sessionId)
        {
            // Lấy userName từ JWT claims
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            // Kiểm tra sessionId có tồn tại trong cache không
            if (!_memoryCache.TryGetValue(sessionId, out string _))
            {
                return BadRequest(new { error = "SessionId không hợp lệ hoặc đã hết hạn." });
            }

            // Đánh dấu sessionId đã xác thực với userName
            _memoryCache.Set(sessionId, userName, TimeSpan.FromMinutes(2));
            return Ok();
        }


        [HttpGet("qr-session-status")]
        [AllowAnonymous]
        public IActionResult CheckQrSession(string sessionId)
        {
            if (_memoryCache.TryGetValue(sessionId, out string userName) && !string.IsNullOrEmpty(userName))
            {
                return Ok(new { authenticated = true, userName });
            }
            return Ok(new { authenticated = false });
        }
        //---end đăng nhập bẳng qrcode---

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    return BadRequest(new ApiResponse<string>(false, "Email không tồn tại trong hệ thống!", null));
                }

                var notActiveUser = await _applicationUserService.GetById(user.Id);

                if (notActiveUser != null && notActiveUser.IsActive == 0)
                {
                    return BadRequest(new ApiResponse<string>(false, "Tài khoản của bạn chưa được kích hoạt! Vui lòng kiểm tra mail để kích hoạt tài khoản.", null));
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordCheck)
                {
                    await _userManager.AccessFailedAsync(user);

                    if (await _userManager.GetLockoutEndDateAsync(user) != null)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(1));
                        return BadRequest(new ApiResponse<string>(false, "Tài khoản đã bị khóa.", null));
                    }

                    var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    if (accessFailedCount > 0)
                    {
                        return BadRequest(new ApiResponse<string>(false, $"Bạn đã đăng nhập thất bại {user.AccessFailedCount} lần. Còn {5 - user.AccessFailedCount} lần tài khoản sẽ bị khóa.", null));
                    }

                    return BadRequest(new ApiResponse<string>(false, "Mật khẩu không đúng!", null));
                }
                else if (user.LockoutEnd != null && user.LockoutEnd.Value > DateTimeOffset.Now)
                {
                    return BadRequest(new ApiResponse<string>(false, "Tài khoản đã bị khóa.", null));
                }
                else
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                }

                var validateUser = await _applicationUserService.GetById(user.Id);
                if (validateUser.IsFirstLogin == 2)
                {
                    _userState.Email = user.Email;
                    return Ok(new ApiResponse<object>(true, "Đăng nhập thành công", new { RedirectUrl = "/Account/ChangePassword", IsFirstLogin = true }));
                }
                if (validateUser.IsFirstLogin == 1)
                {
                    return BadRequest(new ApiResponse<string>(false, "Tài khoản của bạn đã yêu cầu đổi mật khẩu. Xin vui lòng vào email để xác nhận hoặc liên hệ quản trị viên để thay đổi mật khẩu.", null));
                }

                var code = _loginService.GenerateVerificationCode(user.Email);
                _userState.Email = model.Email;
                _userState.Password = model.Password;
                await SendMail(validateUser, code);

                var token = GenerateJwtToken(user);
                var expirationTime = DateTime.Now.AddDays(7);

                return Ok(new ApiResponse<object>(true, "Đăng nhập thành công", new
                {
                    Token = token,
                    Expiration = expirationTime
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(false, ex.Message, null));
            }
        }

        [Authorize(AuthenticationSchemes = "Identity.Application")]
        [HttpGet("GetJwtToken")]
        public async Task<IActionResult> GetJwtToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            // Lấy thông tin user từ ApplicationUserService
            var userInfor = _applicationUserService.GetByUserName(user.UserName);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                userInfor = new
                {
                    id = userInfor.Id,
                    userName = userInfor.UserName,
                    groupId = userInfor.GroupId,
                    companyId = userInfor.CompanyId,
                    name = $"{userInfor.FirstName} {userInfor.LastName}".Trim()
                }
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Thời gian hết hạn có thể được điều chỉnh
            var expirationTime = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task SendMail(ApplicationUser user, string code)
        {
            var emailHistory = new EmailHistory
            {
                Id = Guid.NewGuid().ToString(),
                Receiver = user.Email,
                Subject = "Mã Xác Nhận Đăng Nhập",
                Content = $@"
                    <div class='email-container'>
                        <div class='header'>
                            <h1>Xác Thực Đăng Nhập</h1>
                        </div>
                        <div class='content'>
                            <p>Xin chào,</p>
                            <p>Mã xác nhận của bạn để đăng nhập vào hệ thống là:</p>
                            <h2>{code}</h2>
                            <p>Vui lòng nhập mã này vào biểu mẫu xác nhận để hoàn tất quá trình đăng nhập. Mã này chỉ có hiệu lực trong vòng 1 phút.</p>
                            <p>Xin chân thành cảm ơn!</p>
                            <p><strong>Lưu ý:</strong></p>
                            <p>Tài khoản chỉ có hiệu lực trong vòng 1 tuần.</p>
                            <p>Đây là email được gửi tự động từ hệ thống, vui lòng không trả lời vào địa chỉ này. Mọi thông tin thắc mắc xin vui lòng liên hệ: <a href='mailto:xxxxxxxx@gmail.com'>xxxxxxxx@gmail.com</a></p>
                        </div>
                        <div class='footer'>
                            <p>Trân trọng,</p>
                            <p>Công ty cổ phần xây dựng Đức Anh</p>
                        </div>
                    </div>",
                CompanyId = user.CompanyId,
                GroupId = user.CompanyId,
                MajorId = "dce7329a-e7a1-4ef9-9ece-b45537c4128c",
                ParentMajorId = "",
                CreateAt = DateTime.Now,
                CreateBy = "System"
            };

            await _emailService.SendEmail(emailHistory);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterMobileModel Input)
        {
            try
            {
                var specialCharRegex = new Regex(@"[\W_]");
                if (!specialCharRegex.IsMatch(Input.Password))
                    return Ok(new ApiResponse<RegisterMobileModel>(false, "Mật khẩu phải chứa ít nhất một ký tự đặc biệt!", null));

                // Kiểm tra song song
                var checkEmailTask = Task.Run(() => _applicationUserService.GetByUserName(Input.Email));
                var checkPhoneTask = Task.Run(() => _applicationUserService.IsExistByPhoneNumber(Input.PhoneNumber));
                var existedUserTask = _userManager.FindByEmailAsync(Input.Email);

                await Task.WhenAll(checkEmailTask, checkPhoneTask, existedUserTask);

                if (checkEmailTask.Result != null)
                    return Ok(new ApiResponse<RegisterMobileModel>(false, "Email đã tồn tại!", null));
                if (checkPhoneTask.Result)
                    return Ok(new ApiResponse<RegisterMobileModel>(false, "Số điện thoại đã tồn tại!", null));

                string groupID = Guid.NewGuid().ToString();
                string idIT = Guid.NewGuid().ToString();
                var company = CreateCompany(Input, groupID);
                var chinhanh = CreateChiNhanh(Input, groupID);
                var department = CreateDepartmentDirector(groupID);
                var departmentit = CreateDepartmentIt(groupID, idIT);
                var companyType = CreateCompanyType(groupID);
                var user = CreateUser(Input);

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await GetEmailStore().SetEmailAsync(user, Input.Email, CancellationToken.None);
                await GetLockoutStore().SetLockoutEnabledAsync(user, true, CancellationToken.None);

                var appUser = CreateAppUser(Input, company.Id, user);
                appUser.IsActive = 1;
                appUser.IsStatus = "Đã kích hoạt";

                // Insert song song các thực thể
                await Task.WhenAll(
                    _companyService.Insert(company, ""),
                    _chiNhanhService.Insert(chinhanh, ""),
                    _departmentService.Insert(department, ""),
                    _departmentService.Insert(departmentit, ""),
                    _companyTypeService.Insert(companyType, "")
                );

                if (existedUserTask.Result != null)
                    await _userManager.DeleteAsync(existedUserTask.Result);

                var resultUser = await _userManager.CreateAsync(user, Input.Password);
                if (!resultUser.Succeeded)
                    return Ok(new ApiResponse<RegisterMobileModel>(false, "Tạo tài khoản thất bại!", null));

                await _applicationUserService.Insert(appUser, "");

                var permissionMajorPairs = new (string PermissionId, string MajorId)[]
                {
                    ("2483777c-815d-4a54-af1a-58468bfd51a1", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"),
                    ("4c7ac566-5404-4cc9-9f3a-7292980bfbc7", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("4e3af927-cca3-4390-a718-64a4a4c20b5a", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("430f04ce-e8a1-4d72-83ba-e1ab5c3b99ec", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"),
                    ("475f8637-7871-49f2-8214-5f8cfc245d1c", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("f08e2d74-528b-487c-98d3-33c988878947", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("bcc95d1d-766f-4e81-84da-4d3ef4edd1cf", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("be25c8c3-5728-43ed-b82a-ab29d3fe4a61", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("8e01b3b4-21cc-4f76-9c09-2147ef0b4012", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"),
                    ("07f134f4-196e-40db-a1d6-07b6b1558d1b", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"),
                    ("e665cdda-d747-4bf9-a2b2-39c39772973d", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("aad8617e-a30a-4ad1-ad92-47bfe8fd027b", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("c3a7d3bc-50ca-4ccf-a2ac-470b973df561", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("295935cf-f21e-4500-b59f-eefedebd50d7", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("a4660fa9-afa6-48a6-b992-d8f81f8c553a", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"),
                    ("2d313114-60bd-402c-85fc-be6e8fc0f5c6", "fcf752d9-c19a-496d-bba9-f0864928f32b"),
                    ("02797a75-39f3-4cb6-914f-b670c4cfd660", "2105f7e7-1d45-4369-85a9-fdd185c3490b"),
                    ("ba3b4bc1-cc09-49b5-af51-67bd8cbc52f2", "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba")
                };

                string idMain = Guid.NewGuid().ToString();
                var listPermissions = new List<MajorUserPermission>();

                for (int day = 1; day <= 7; day++) // 1: Monday → 7: Sunday
                {
                    foreach (var (permId, majorId) in permissionMajorPairs)
                    {
                        listPermissions.Add(new MajorUserPermission
                        {
                            Id = Guid.NewGuid().ToString(),
                            CompanyId = company.Id,
                            ParentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84",
                            MajorId = majorId,
                            UserId = appUser.Id,
                            PermissionId = permId,
                            DayInWeek = day,
                            IdMain = idMain,
                            GroupId = company.Id,
                            Ordinarily = 0,
                            CreateAt = DateTime.Now,
                            CreateBy = "system",
                            IsActive = 1,
                            ApprovalUserId = "",
                            DateApproval = DateTime.Now,
                            DepartmentId = "",
                            DepartmentOrder = 1,
                            ApprovalOrder = 1,
                            ApprovalId = "",
                            LastApprovalId = "",
                            IsStatus = ""
                        });
                    }
                }

                await _majorUserPermissionRepository.InsertMulti(listPermissions);

                var chucVus = new List<DM_ChucVu>
                {
                    new DM_ChucVu
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChucVu = "Giám đốc",
                        GroupId = company.Id,
                        Ordinarily = 0,
                        CreateAt = DateTime.Now,
                        CreateBy = user.Id,
                        IsActive = 3,
                        ApprovalUserId =  user.Id,
                        DateApproval = DateTime.Now,
                        ApprovalDept = "",
                        DepartmentId = "",
                        DepartmentOrder = 0,
                        ApprovalOrder = 0,
                        ApprovalId = null,
                        LastApprovalId = null,
                        IsStatus = "Đã duyệt"
                    },
                    new DM_ChucVu
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChucVu = "Trưởng phòng",
                        GroupId = company.Id,
                        Ordinarily = 0,
                        CreateAt = DateTime.Now,
                        ApprovalDept = "",
                        CreateBy =  user.Id,
                        IsActive = 3,
                        ApprovalUserId =  user.Id,
                        DateApproval = DateTime.Now,
                        DepartmentId = "",
                        DepartmentOrder = 0,
                        ApprovalOrder = 0,
                        ApprovalId = null,
                        LastApprovalId = null,
                        IsStatus = "Đã duyệt"
                    },
                    new DM_ChucVu
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChucVu = "Nhân viên",
                        GroupId = company.Id,
                        Ordinarily = 0,
                        CreateAt =DateTime.Now,
                        CreateBy =  user.Id,
                        IsActive = 3,
                        ApprovalUserId =  user.Id,
                        ApprovalDept = "",
                        DateApproval = DateTime.Now,
                        DepartmentId = "",
                        DepartmentOrder = 0,
                        ApprovalOrder = 0,
                        ApprovalId = null,
                        LastApprovalId = null,
                        IsStatus = "Đã duyệt"
                    }
                };
                await _chucVuRepository.Insert(chucVus[0], "");
                await _chucVuRepository.Insert(chucVus[1], "");
                await _chucVuRepository.Insert(chucVus[2], "");

                var chuyenMons = new List<DM_ChuyenMon>
                {
                    new DM_ChuyenMon {
                        Id = Guid.NewGuid().ToString(),
                        ChuyenMon = "Nhân viên",
                        GroupId = company.Id,
                        Ordinarily = 1,
                        CreateAt =DateTime.Now,
                        CreateBy =  user.Id,
                        IsActive = 3,
                        ApprovalUserId =  user.Id,
                        DateApproval = DateTime.Now,
                        ApprovalDept = "",
                        DepartmentId = "",
                        DepartmentOrder = 0,
                        ApprovalOrder = 0,
                        ApprovalId = null,
                        LastApprovalId = null,
                        IsStatus = "Đã duyệt"
                    },
                    new DM_ChuyenMon {
                        Id = Guid.NewGuid().ToString(),
                        ChuyenMon = "Kỹ sư",
                        GroupId = company.Id,
                        Ordinarily = 1,
                        CreateAt =DateTime.Now,
                        CreateBy =  user.Id,
                        IsActive = 3,
                        ApprovalUserId =  user.Id,
                        DateApproval = DateTime.Now,
                        ApprovalDept = "",
                        DepartmentId = "",
                        DepartmentOrder = 0,
                        ApprovalOrder = 0,
                        ApprovalId = null,
                        LastApprovalId = null,
                        IsStatus = "Đã duyệt"
                    }
                };
                await _chuyenMonRepository.Insert(chuyenMons[0], "");
                await _chuyenMonRepository.Insert(chuyenMons[1], "");

                return Ok(new ApiResponse<RegisterMobileModel>(true, "Tạo tài khoản thành công!", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<RegisterMobileModel>(false, $"Lỗi: {ex.Message}", null));
            }

        }

        [Authorize(AuthenticationSchemes = "JwtBearer")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var appUser = _applicationUserService.GetByUserName(model.Email);

            if (user == null || appUser == null)
                return Ok(new ApiResponse<RegisterMobileModel>(false, "Tài khoản không hợp lệ.", null));

            // Xác thực mật khẩu hiện tại
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isPasswordValid)
                return Ok(new ApiResponse<RegisterMobileModel>(false, "Mật khẩu hiện tại không đúng.", null));

            // Xóa mật khẩu cũ
            var removePassResult = await _userManager.RemovePasswordAsync(user);
            if (!removePassResult.Succeeded)
                return Ok(new ApiResponse<RegisterMobileModel>(false, "Không thể xóa mật khẩu cũ.", null));

            // Gán mật khẩu mới
            var addPassResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPassResult.Succeeded)
                return Ok(new ApiResponse<RegisterMobileModel>(false, "Không thể cập nhật mật khẩu mới.", null));

            return Ok(new ApiResponse<RegisterMobileModel>(true, "Đổi mật khẩu thành công.", null));
        }

        [HttpDelete("DeleteCurrentUser")]
        public async Task<IActionResult> DeleteCurrentUser([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new ApiResponse<string>(false, "Email không được để trống.", null));

                // Find IdentityUser by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound(new ApiResponse<string>(false, "Không tìm thấy tài khoản.", null));

                // Delete from Identity
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest(new ApiResponse<string>(false, "Xóa tài khoản thất bại.", null));

                // Delete from ApplicationUser (if exists)
                var appUser = await _applicationUserService.GetById(user.Id);
                if (appUser != null)
                {
                    await _applicationUserService.DeleteById(user.Id, "system");
                }

                return Ok(new ApiResponse<string>(true, "Xóa tài khoản thành công.", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(false, "Lỗi: " + ex.Message, null));
            }
        }


        private ApplicationUser CreateAppUser(RegisterMobileModel Input, string companyId, IdentityUser iUser)
        {
            var user = new ApplicationUser
            {
                Id = iUser.Id,
                UserName = iUser.UserName,
                Address = Input.Address,
                Dob = (DateTime)Input.Dob,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                PhoneNumber = Input.PhoneNumber,
                IsActive = 3,
                IsFirstLogin = 0,
                CreateAt = DateTime.Now,
                CreateBy = "system",
                Email = Input.Email,
                CompanyId = companyId,
                GroupId = companyId,
                DeptId = "",
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
        private IdentityUser CreateUser(RegisterMobileModel input)
        {
            try
            {
                var user = new IdentityUser
                {
                    UserName = input.Email,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber,
                    EmailConfirmed = true
                };
                return user;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
        private IUserLockoutStore<IdentityUser> GetLockoutStore()
        {
            if (!_userManager.SupportsUserLockout)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserLockoutStore<IdentityUser>)_userStore;
        }
        private CompanyType CreateCompanyType(string groupID)
        {
            try
            {
                var chinhanh = Activator.CreateInstance<CompanyType>();
                chinhanh.Id = groupID;
                chinhanh.TenLoaiChiNhanh = "Hội sở";
                chinhanh.GroupId = groupID;
                chinhanh.Ordinarily = 1;
                chinhanh.CreateAt = DateTime.Now;
                chinhanh.CreateBy = "symtem";
                chinhanh.IsActive = 3;
                chinhanh.ApprovalUserId = "";
                chinhanh.DateApproval = DateTime.Now;
                chinhanh.DepartmentId = "";
                chinhanh.DepartmentOrder = 1;
                chinhanh.ApprovalOrder = 1;
                chinhanh.ApprovalId = "";
                chinhanh.LastApprovalId = "";
                chinhanh.IsStatus = "Đã duyệt";
                return chinhanh;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(CompanyType)}'. " +
                    $"Ensure that '{nameof(CompanyType)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private ChiNhanh CreateChiNhanh(RegisterModel Input, string groupID)
        {
            try
            {
                var chinhanh = Activator.CreateInstance<ChiNhanh>();
                chinhanh.Id = groupID;
                chinhanh.ParentId = groupID;
                chinhanh.TenChiNhanh = Input.CompanyName;
                chinhanh.CompanyType = groupID;
                chinhanh.Phone = Input.PhoneNumber;
                chinhanh.Email = Input.Email;
                chinhanh.Address = Input.Address;
                chinhanh.GroupId = groupID;
                chinhanh.Ordinarily = 1;
                chinhanh.CreateAt = DateTime.Now;
                chinhanh.CreateBy = "symtem";
                chinhanh.IsActive = 3;
                chinhanh.ApprovalDept = "";
                chinhanh.ApprovalUserId = "";
                chinhanh.DateApproval = DateTime.Now;
                chinhanh.DepartmentId = "";
                chinhanh.DepartmentOrder = 1;
                chinhanh.ApprovalOrder = 1;
                chinhanh.ApprovalId = "";
                chinhanh.LastApprovalId = "";
                chinhanh.IsStatus = "Đã duyệt";
                return chinhanh;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ChiNhanh)}'. " +
                    $"Ensure that '{nameof(ChiNhanh)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private MCompany CreateCompany(RegisterMobileModel Input, string groupID)
        {
            try
            {
                var company = Activator.CreateInstance<MCompany>();
                company.Id = groupID;
                company.ParentId = groupID;
                company.CompanyName = Input.CompanyName;
                company.CompanyType = Constant.HEAD_COMPANY;
                company.Phone = Input.PhoneNumber;
                company.Email = Input.Email;
                company.Address = Input.Address;
                company.GroupId = groupID;
                company.CreateAt = DateTime.Now;
                company.CreateBy = "symtem";
                company.IsActive = 3;
                return company;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(MCompany)}'. " +
                    $"Ensure that '{nameof(MCompany)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private Department CreateDepartmentDirector(string groupID)
        {
            try
            {
                var department = Activator.CreateInstance<Department>();
                department.Id = groupID;
                department.CompanyId = groupID;
                department.DeptName = "Ban giám đốc";
                department.Phone = "0975666999";
                department.Email = "da@gmail.com";
                department.GroupId = groupID;
                department.Ordinarily = 0;
                department.CreateAt = DateTime.Now;
                department.CreateBy = "symtem";
                department.IsActive = 3;
                department.ApprovalDept = "";
                department.ApprovalUserId = "";
                department.DateApproval = DateTime.Now;
                department.DepartmentId = "";
                department.DepartmentOrder = 1;
                department.ApprovalOrder = 1;
                department.ApprovalId = "";
                department.LastApprovalId = "";
                department.IsStatus = "Đã duyệt";
                return department;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Department)}'. " +
                    $"Ensure that '{nameof(Department)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private Department CreateDepartmentIt(string groupID, string idIT)
        {
            try
            {
                var department = Activator.CreateInstance<Department>();
                department.Id = idIT;
                department.CompanyId = groupID;
                department.DeptName = "Công nghệ thông tin";
                department.Phone = "0975666999";
                department.Email = "viethau.nd@gmail.com";
                department.GroupId = groupID;
                department.Ordinarily = 0;
                department.CreateAt = DateTime.Now;
                department.CreateBy = "symtem";
                department.IsActive = 3;
                department.ApprovalDept = "";
                department.ApprovalUserId = "";
                department.DateApproval = DateTime.Now;
                department.DepartmentId = "";
                department.DepartmentOrder = 1;
                department.ApprovalOrder = 1;
                department.ApprovalId = "";
                department.LastApprovalId = "";
                department.IsStatus = "Đã duyệt";
                return department;
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Department)}'. " +
                    $"Ensure that '{nameof(Department)}' is not an abstract class and has a parameterless constructor.");
            }
        }
    }
}
