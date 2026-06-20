using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.Services;
using DucAnh2025.ViewModels.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Constant = DucAnh2025.Share.Constant;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IApplicationUserRepository _applicationUserService;
    private readonly ILoginService _loginService;
    private readonly IEmailHistoryRepository _emailService;
    private readonly UserState _userState;
    private readonly IUserSessionRepository _userSessionService;
    private readonly IHelperService _helperService;
    private readonly ILogger<AccountController> _logger;
    private readonly ICompanyRepository _companyService;
    private readonly IChiNhanhRepository _chiNhanhService;
    private readonly IDepartmentRepository _departmentService;
    private readonly ICompanyTypeRepository _companyTypeService;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IApplicationUserRepository applicationUserService,
        ILoginService loginService,
        IEmailHistoryRepository emailService,
        UserState userState,
        IUserSessionRepository userSessionService,
        IHelperService helperService,
        ILogger<AccountController> logger,
        ICompanyRepository companyRepository,
        IChiNhanhRepository chiNhanhRepository,
        IDepartmentRepository departmentRepository,
        ICompanyTypeRepository companyTypeRepository,
        IUserStore<IdentityUser> userStore,
        IConfiguration configuration,
        IMemoryCache memoryCache
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _applicationUserService = applicationUserService;
        _loginService = loginService;
        _emailService = emailService;
        _userState = userState;
        _userSessionService = userSessionService;
        _helperService = helperService;
        _logger = logger;
        _companyService = companyRepository;
        _chiNhanhService = chiNhanhRepository;
        _departmentService = departmentRepository;
        _companyTypeService = companyTypeRepository;
        _userStore = userStore;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult QrSignInPage()
    {
        return View();
    }

    [HttpGet]
    [Route("Account/QrSignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> QrSignIn(string sessionId)
    {
        if (_memoryCache.TryGetValue(sessionId, out string userName) && !string.IsNullOrEmpty(userName))
        {
            // Load user từ DB
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return Unauthorized();

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            // Xoá session tránh reuse
            _memoryCache.Remove(sessionId);
            return Redirect("/"); // hoặc redirect tới dashboard
        }

        return Unauthorized();
    }


    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var rememberedEmail = Request.Cookies["RememberedEmail"];
        var rememberedPassword = Request.Cookies["RememberedPassword"];
        return View(new LoginModel
        {
            Email = rememberedEmail ?? "",
            Password = rememberedPassword ?? "",
            RememberMe = !string.IsNullOrEmpty(rememberedEmail)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByNameAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Email không tồn tại trong hệ thống!");
            return View(model);
        }

        var appUser = await _applicationUserService.GetById(user.Id);

        // Check if account is active
        if (appUser != null && appUser.IsActive == 0)
        {
            ModelState.AddModelError("", "Tài khoản của bạn chưa được kích hoạt! Vui lòng kiểm tra mail để kích hoạt tài khoản.");
            return View(model);
        }

        // Check for lockout
        if (user.LockoutEnd != null && user.LockoutEnd.Value > DateTimeOffset.Now)
        {
            return RedirectToAction("Lockout");
        }

        // Password check
        var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordCheck)
        {
            await _userManager.AccessFailedAsync(user);

            // Lockout if needed
            if (await _userManager.GetLockoutEndDateAsync(user) != null)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(1));
                return RedirectToAction("Lockout");
            }

            // Show warning if failed attempts > 0
            var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
            if (accessFailedCount > 0)
            {
                ViewBag.Warning = $"Bạn đã đăng nhập thất bại {user.AccessFailedCount} lần. Còn {5 - user.AccessFailedCount} lần tài khoản sẽ bị khóa.";
            }
            ModelState.AddModelError("", "Mật khẩu không đúng!");
            return View(model);
        }
        else
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);
        }

        // Check for lockout again after update
        if (await _userManager.GetLockoutEndDateAsync(user) != null)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(1));
            return RedirectToAction("Lockout");
        }

        // Check for first login
        if (appUser != null)
        {
            if (appUser.IsFirstLogin == 2)
            {
                _userState.Email = user.Email;
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("ChangePassword", "Account", new { isFirstLogin = 1 });
            }
            if (appUser.IsFirstLogin == 1)
            {
                ViewBag.Warning = "Tài khoản của bạn đã yêu cầu đổi mật khẩu. Xin vui lòng vào email để xác nhận hoặc liên hệ quản trị viên để thay đổi mật khẩu.";
                return View(model);
            }
        }

        // Send verification code
        var code = _loginService.GenerateVerificationCode(user.Email);
        _userState.Email = model.Email;
        _userState.Password = model.Password;
        await SendMail(appUser, code);

        if (model.RememberMe)
        {
            Response.Cookies.Append("RememberedEmail", model.Email, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            // Lưu mật khẩu (không an toàn, chỉ dùng demo)
            Response.Cookies.Append("RememberedPassword", model.Password, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true
            });
        }
        else
        {
            Response.Cookies.Delete("RememberedEmail");
            Response.Cookies.Delete("RememberedPassword");
        }

        TempData["Password"] = model.Password;
        return RedirectToAction("ConfirmLogin", new { email = model.Email, rememberMe = model.RememberMe, returnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Đăng xuất khỏi Identity
        await _signInManager.SignOutAsync();

        // Xóa sessionId và userName khỏi session
        var sessionId = HttpContext.Session.GetString("sessionId");
        HttpContext.Session.Clear();

        // Xóa cookie session nếu có
        if (Request.Cookies.ContainsKey("AspNetCore.Session"))
        {
            Response.Cookies.Delete("AspNetCore.Session");
        }

        // Xóa UserSession trong database nếu có
        if (!string.IsNullOrEmpty(sessionId))
        {
            await _userSessionService.DeleteById(sessionId, "");
        }

        // Chuyển hướng về trang đăng nhập
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ConfirmLogin(string email, bool rememberMe = false, string returnUrl = null)
    {
        var model = new ConfirmLoginModel
        {
            Email = email,
            RememberMe = rememberMe,
            ReturnUrl = returnUrl,
            Code ="555555"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmLogin(ConfirmLoginModel model)
    {

        // Kiểm tra mã xác thực
        if (!_loginService.VerifyCode(model.Email, model.Code))
        {
            model.ErrorMessage = "Mã xác thực không đúng hoặc đã hết hạn.";
            return View(model);
        }

        _loginService.ClearVerificationCode(model.Email);

        // Lấy lại mật khẩu từ TempData (đã lưu sau bước đăng nhập đầu tiên)
        var password = TempData["Password"] as string;
        if (string.IsNullOrEmpty(password))
        {
            model.ErrorMessage = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.";
            return View(model);
        }

        // Đăng nhập
        var result = await _signInManager.PasswordSignInAsync(model.Email, password, model.RememberMe, lockoutOnFailure: true);

        TempData.Remove("Password");

        if (result.Succeeded)
        {
            return RedirectToLocal(model.ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            return RedirectToAction("Lockout");
        }
        else
        {
            model.ErrorMessage = "Lỗi: Thông tin đăng nhập không đúng.";
            return View(model);
        }
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterModel
        {
           Dob = DateTime.UtcNow
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Kiểm tra email đã tồn tại
        if (_applicationUserService.GetByUserName(model.Email) != null)
        {
            ModelState.AddModelError(string.Empty, "Lỗi: Email bạn nhập đã tồn tại trong hệ thống!");
            return View(model);
        }

        // Kiểm tra số điện thoại đã tồn tại
        if (_applicationUserService.IsExistByPhoneNumber(model.PhoneNumber))
        {
            ModelState.AddModelError(string.Empty, "Lỗi: Số điện thoại bạn nhập đã tồn tại trong hệ thống!");
            return View(model);
        }

        string groupID = Guid.NewGuid().ToString();
        string companyTypeId = Guid.NewGuid().ToString();
        string idIT = Guid.NewGuid().ToString();

        var company = CreateCompany(model, groupID);
        var chinhanh = CreateChiNhanh(model, groupID);
        var department = CreateDepartmentDirector(groupID);
        var departmentit = CreateDepartmentIt(groupID, idIT);
        var companyType = CreateCompanyType(groupID);

        var user = CreateUser(model);
        var password = _helperService.GeneratePassword(8);
        Console.WriteLine("Password : ",password);

        await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
        var lockOutStore = GetLockoutStore();
        await lockOutStore.SetLockoutEnabledAsync(user, true, CancellationToken.None);

        var appUser = CreateAppUser(model, company.Id, user);

        await _companyService.Insert(company, "");
        await _chiNhanhService.Insert(chinhanh, "");
        await _departmentService.Insert(department, "");
        await _departmentService.Insert(departmentit, "");
        await _companyTypeService.Insert(companyType, "");

        //xóa tài khoản đã có
        var user11 = await _userManager.FindByEmailAsync(model.Email);
        var result = await _userManager.DeleteAsync(user11);

        var resultUser = await _userManager.CreateAsync(user, password);
        await _applicationUserService.Insert(appUser, "");

        if (!resultUser.Succeeded)
        {
            foreach (var error in resultUser.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        _logger.LogInformation("User created a new account with password.");

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action(
            "ConfirmEmail",
            "Account",
            new { userId, code, returnUrl },
            protocol: Request.Scheme);

        // Gửi mail xác nhận đăng ký
        await SendMail(appUser, callbackUrl, password);

        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation(string email, string returnUrl = null)
    {
        ViewData["Email"] = email;
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code, string? returnUrl = null)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userManager.FindByIdAsync(userId);
        var appUser = await _applicationUserService.GetById(userId);

        bool isSucceed = false;

        if (user == null || appUser == null)
        {
            Response.StatusCode = 404;
            ViewBag.IsSucceed = false;
            return View();
        }

        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
        isSucceed = result.Succeeded;

        if (isSucceed)
        {
            appUser.IsActive = 3;
            await _applicationUserService.Update(appUser, "");
        }
        else
        {
            var company = await _companyService.GetById(appUser.CompanyId);
            if (company != null)
            {
                await _companyService.DeleteById(company.Id, "");
            }
        }

        ViewBag.IsSucceed = isSucceed;
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ChangePassword(string isFirstLogin = null)
    {
        ViewBag.IsFirstLogin = isFirstLogin;
        return View(new ChangePasswordModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model, string isFirstLogin = null)
    {
        ViewBag.IsFirstLogin = isFirstLogin;
        //if (!ModelState.IsValid)
        //{
        //    return View(model);
        //}

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
            return View(model);
        }

        var appUser = _applicationUserService.GetByUserName(user.UserName);
        if (appUser == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            ModelState.AddModelError(string.Empty, "Lỗi bất thường đã xảy ra, vui lòng liên hệ với Người quản trị hệ thống!");
            return View(model);
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            // Gửi email xác nhận đổi mật khẩu
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ChangePasswordEmailConfirm",
                "Account",
                new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            var validateUser = await _applicationUserService.GetById(user.Id);
            validateUser.IsFirstLogin = 1;
            await _applicationUserService.Update(validateUser, "");

            await SendMailChangePass(appUser, callbackUrl);

            return RedirectToAction("ChangePasswordConfirmation");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetJwtToken()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            // Đảm bảo bạn đã cấu hình các giá trị này trong appsettings.json
            _configuration["Jwt:Key"]
        ));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ChangePasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePasswordEmailConfirm(string userId, string code)
    {
        bool isSucceed = false;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            ViewBag.IsSucceed = false;
            return View();
        }

        var user = await _userManager.FindByIdAsync(userId);
        var appUser = await _applicationUserService.GetById(userId);

        if (user == null || appUser == null)
        {
            Response.StatusCode = 404;
            ViewBag.IsSucceed = false;
            return View();
        }

        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
        isSucceed = result.Succeeded;

        if (isSucceed)
        {
            appUser.IsFirstLogin = 0;
            await _applicationUserService.Update(appUser, "");
        }

        ViewBag.IsSucceed = isSucceed;
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
        {
            TempData["StatusMessage"] = "Error: Invalid email change confirmation link.";
            return RedirectToAction("Login", "Account");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.StatusMessage = $"Unable to find user with Id '{userId}'";
            return View();
        }

        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ChangeEmailAsync(user, email, decodedCode);
        if (!result.Succeeded)
        {
            ViewBag.StatusMessage = "Error changing email.";
            return View();
        }

        // Update user name if needed
        var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
        if (!setUserNameResult.Succeeded)
        {
            ViewBag.StatusMessage = "Error changing user name.";
            return View();
        }

        await _signInManager.RefreshSignInAsync(user);
        ViewBag.StatusMessage = "Thank you for confirming your email change.";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            // Optionally add a model error
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Do not reveal that the user does not exist or is not confirmed
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action(
            "ResetPassword",
            "Account",
            new { code },
            protocol: Request.Scheme);

        // Uncomment and implement your email sending logic
        // await _emailSender.SendEmailAsync(email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult InvalidPasswordReset()
    {
        return View();
    }

    [HttpGet]
    public IActionResult InvalidUser()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Lockout()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> LoginWith2fa(string returnUrl = null, bool rememberMe = false)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new LoginWith2faViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to load two-factor authentication user.");
            return View(model);
        }

        var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", await _userManager.GetUserIdAsync(user));
            return Redirect(model.ReturnUrl ?? "/");
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToAction("Lockout");
        }
        else
        {
            _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", await _userManager.GetUserIdAsync(user));
            ModelState.AddModelError(string.Empty, "Error: Invalid authenticator code.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new LoginWithRecoveryCodeViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to load two-factor authentication user.");
            return View(model);
        }

        var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", await _userManager.GetUserIdAsync(user));
            return Redirect(model.ReturnUrl ?? "/");
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToAction("Lockout");
        }
        else
        {
            _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}'", await _userManager.GetUserIdAsync(user));
            ModelState.AddModelError(string.Empty, "Error: Invalid recovery code entered.");
            return View(model);
        }
    }

    // GET: /Account/ResendEmailConfirmation
    [HttpGet]
    public IActionResult ResendEmailConfirmation()
    {
        return View();
    }

    // POST: /Account/ResendEmailConfirmation
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.StatusMessage = "Email is required.";
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is already confirmed
            ViewBag.StatusMessage = "Confirmation email sent. Please check your email.";
            return View();
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

        // await _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

        ViewBag.StatusMessage = "Confirmation email sent. Please check your email.";
        return View();
    }

    // GET: /Account/ResetPassword
    [HttpGet]
    public IActionResult ResetPassword(string code = null)
    {
        if (code == null)
        {
            return RedirectToAction("InvalidPasswordReset");
        }
        var model = new ResetPasswordViewModel { Code = code };
        return View(model);
    }

    // POST: /Account/ResetPassword
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction("ResetPasswordConfirmation");
        }
        var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ResetPasswordAsync(user, decodedCode, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("ResetPasswordConfirmation");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }

    // GET: /Account/ResetPasswordConfirmation
    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    // Hàm gửi mail xác nhận đổi mật khẩu
    private async Task SendMailChangePass(ApplicationUser user, string callBackUrl)
    {
        var emailHistory = new EmailHistory();
        var content = "";
        content += "<div class=\"email-container\">";
        content += "<div class=\"header\">";
        content += "<h1>Xác Nhận Thay Đổi Mật Khẩu</h1>";
        content += "</div>";
        content += "<div class=\"content\">";
        content += "<p>Xin chào,</p>";
        content += "<p>Bạn đã yêu cầu thay đổi mật khẩu cho tài khoản của mình.</p>";
        content += "<p>Vui lòng nhấp vào đường dẫn dưới đây để xác nhận thay đổi mật khẩu:</p>";
        content += $"<p><a href=\"{callBackUrl}\">Xác Nhận Thay Đổi Mật Khẩu</a></p>";
        content += "<p>Nếu bạn không yêu cầu thay đổi mật khẩu, vui lòng bỏ qua email này.</p>";
        content += "<p>Xin chân thành cảm ơn!</p>";
        content += "<p><strong>Lưu ý:</strong></p>";
        content += "<p>Email xác nhận này có hiệu lực trong vòng 1 phút.</p>";
        content += "<p>Đây là email được gửi tự động từ hệ thống, vui lòng không trả lời vào địa chỉ này. Mọi thông tin thắc mắc xin vui lòng liên hệ: <a href=\"mailto:xxxxxxxx@gmail.com\">xxxxxxxx@gmail.com</a></p>";
        content += "</div>";
        content += "<div class=\"footer\">";
        content += "<p>Trân trọng,</p>";
        content += "<p>Công ty cổ phần xây dựng Đức Anh</p>";
        content += "</div>";
        content += "</div>";

        emailHistory.Id = Guid.NewGuid().ToString();
        emailHistory.Receiver = user.Email;
        emailHistory.Subject = "Xác Nhận Thay Đổi Mật Khẩu";
        emailHistory.Content = content;
        emailHistory.CompanyId = user.CompanyId;
        emailHistory.GroupId = user.CompanyId;
        emailHistory.ParentMajorId = "00000000-0000-0000-0000-000000000000";
        emailHistory.MajorId = "52145cca-ac10-4241-8973-b6de05dfeaad";
        emailHistory.IsRead = 1;
        emailHistory.CreateAt = DateTime.Now;
        emailHistory.CreateBy = "System";

        await _emailService.SendEmail(emailHistory);
    }

    private async Task SendMail(ApplicationUser user, string code)
    {
        // Tạo mới đối tượng
        var emailHistory = new EmailHistory();

        // Nội dung mail
        var content = "";

        content += "<div class=\"email-container\">";
        content += "<div class=\"header\">";
        content += "<h1>Xác Thực Đăng Nhập</h1>";
        content += "</div>";
        content += "<div class=\"content\">";
        content += "<p>Xin chào,</p>";
        content += "<p>Mã xác nhận của bạn để đăng nhập vào hệ thống là:</p>";
        content += "<h2>" + code + "</h2>";
        content += "<p>Vui lòng nhập mã này vào biểu mẫu xác nhận để hoàn tất quá trình đăng nhập. Mã này chỉ có hiệu lực trong vòng 1 phút.</p>";
        content += "<p>Xin chân thành cảm ơn!</p>";
        content += "<p><strong>Lưu ý:</strong></p>";
        content += "<p>Tài khoản chỉ có hiệu lực trong vòng 1 tuần.</p>";
        content += "<p>Đây là email được gửi tự động từ hệ thống, vui lòng không trả lời vào địa chỉ này. Mọi thông tin thắc mắc xin vui lòng liên hệ: <a href=\"mailto:xxxxxxxx@gmail.com\">xxxxxxxx@gmail.com</a></p>";
        content += "</div>";
        content += "<div class=\"footer\">";
        content += "<p>Trân trọng,</p>";
        content += "<p>Công ty cổ phần xây dựng Đức Anh</p>";
        content += "</div>";
        content += "</div>";

        // Gán giá trị cho các thuộc tính
        emailHistory.Id = Guid.NewGuid().ToString();
        emailHistory.Receiver = user.Email;
        emailHistory.Subject = "Mã Xác Nhận Đăng Nhập";
        emailHistory.Content = content;
        emailHistory.CompanyId = user.CompanyId;
        emailHistory.GroupId = user.CompanyId;
        emailHistory.MajorId = "b61eacfa-1e38-4167-a813-4ebafb1465ce";
        emailHistory.ParentMajorId = "00000000-0000-0000-0000-000000000000";
        emailHistory.CreateAt = DateTime.Now;
        emailHistory.CreateBy = "System";

        // Thực hiện gửi mail
        await _emailService.SendEmail(emailHistory);
    }
    private async Task SendMail(ApplicationUser user, string callBackUrl, string randomPassword)
    {
        // Tạo mới đối tượng
        var emailHistory = new EmailHistory();

        // Nội dung mail
        string content = "";

        content += "<div class=\"email-container\">";
        content += "<div class=\"header\">";
        content += "<h1>Đăng Ký Tài Khoản Thành Công</h1>";
        content += "</div>";
        content += "<div class=\"content\">";
        content += "";
        content += "<p>Xin chào,</p>";
        content += "<p>Bạn đã đăng ký sử dụng hệ thống quản lý doanh nghiệp thành công. Tài khoản của bạn là:</p>";
        content += "<p><strong>Tài khoản:</strong> " + user.Email + "</p>";
        content += "<p><strong>Mật khẩu:</strong> " + System.Net.WebUtility.HtmlEncode(randomPassword) + "</p>";
        content += "<p>Vui lòng truy cập theo đường dẫn dưới đây và thực hiện đăng nhập, đổi mật khẩu lần đầu tiên để kích hoạt tài khoản.</p>";
        content += "<p><a href=\"" + callBackUrl + "\">Click vào đây để kích hoạt</a></p>";
        content += "<p>Xin chân thành cảm ơn!</p>";
        content += "<p><strong>Lưu ý:</strong></p>";
        content += "<p>Tài khoản chỉ có hiệu lực trong vòng 1 tuần.</p>";
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
        emailHistory.Subject = "Xác Nhận Đăng Ký Tài Khoản";
        emailHistory.Content = content;
        emailHistory.CompanyId = user.CompanyId;
        emailHistory.GroupId = user.CompanyId;
        emailHistory.MajorId = "149d7d51-afde-41a6-b11d-4900d1630535";
        emailHistory.ParentMajorId = "00000000-0000-0000-0000-000000000000";
        emailHistory.IsRead = 1;
        emailHistory.CreateAt = DateTime.Now;
        emailHistory.CreateBy = "system";

        // Thực hiện gửi mail
        await _emailService.SendEmail(emailHistory);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        else
            return RedirectToAction("Index", "Home");
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
    private ApplicationUser CreateAppUser(RegisterModel Input,string companyId, IdentityUser iUser)
    {
        try
        {
            var user = Activator.CreateInstance<ApplicationUser>();
            user.Id = iUser.Id;
            user.UserName = iUser.UserName;
            user.Address = Input.Address;
            user.Dob = (DateTime)Input.Dob;
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.PhoneNumber = Input.PhoneNumber;
            user.IsActive = 0;
            user.IsFirstLogin = 2;
            user.CreateAt = DateTime.Now;
            user.CreateBy = "symtem";
            user.Email = Input.Email;
            user.CompanyId = companyId;
            user.GroupId = companyId;
            user.DeptId = "";

            user.Ordinarily = 1;
            user.ApprovalUserId = "";
            user.DateApproval = DateTime.Now;
            user.DepartmentId = "";
            user.DepartmentOrder = 1;
            user.ApprovalOrder = 1;
            user.ApprovalId = "";
            user.LastApprovalId = "";
            user.IsStatus = "Chờ kích hoạt";
            return user;
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }
    private IdentityUser CreateUser(RegisterModel Input)
    {
        try
        {
            var user = Activator.CreateInstance<IdentityUser>();
            user.PhoneNumber = Input.PhoneNumber;
            return user;
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor.");
        }
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
    private MCompany CreateCompany(RegisterModel Input, string groupID)
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
