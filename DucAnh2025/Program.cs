using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Repository.CongTrinh;
using DucAnh2025.Repository.CongTrinh.DanhMuc;
using DucAnh2025.Repository.DanhMuc;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.Repository.Kho;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.Repository.NhanSu.DanhMuc;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.Services;
using DucAnh2025.Services.CongTrinh;
using DucAnh2025.Services.CongTrinh.DanhMuc;
using DucAnh2025.Services.DanhMuc;
using DucAnh2025.Services.HeThong;
using DucAnh2025.Services.Kho;
using DucAnh2025.Services.NhanSu;
using DucAnh2025.Services.NhanSu.DanhMuc;
using DucAnh2025.Services.QLNV;
using DucAnh2025.Services.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.ComponentModel.Design;
using System.Globalization;
using System.Security;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Đặt LicenseContext cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


// SignalR Response Compression
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

// MVC 
builder.Services.AddControllersWithViews();

// Đăng ký SignalR
builder.Services.AddSignalR();

// Add application services
builder.Services.AddSingleton<ToastService>();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB for chat attachments
});

builder.Services.AddMemoryCache();
// Dependency Injection for repositories and services
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IEmailHistoryRepository, EmailHistoryRepository>();
builder.Services.AddScoped<IApprovalTaskRepository, ApprovalTaskRepository>();
builder.Services.AddScoped<ICompanyTypeRepository, CompanyTypeRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IChiNhanhRepository, ChiNhanhRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<FirebaseNotificationService>();
builder.Services.AddScoped<SignalRNotificationService>();

builder.Services.AddScoped<IPhanQuyenRepository, PhanQuyenRepository>();
builder.Services.AddScoped<PermissionService>();

builder.Services.AddScoped<IDM_ChucVuRepository, DM_ChucVuRepository>();
builder.Services.AddScoped<IDM_ChuyenMonRepository, DM_ChuyenMonRepository>();
builder.Services.AddScoped<IMajorUserPermissionRepository, MajorUserPermissionRepository>();
//Hệ thống
builder.Services.AddScoped<IApprovalDeptSettingRepository, ApprovalDeptSettingRepository>();
builder.Services.AddScoped<IApprovalStepSettingRepository, ApprovalStepSettingRepository>();
builder.Services.AddScoped<IMajorRepository, MajorRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IApprovalControlRepository, ApprovalControlRepository>();
builder.Services.AddScoped<IPermissionControlRepository, PermissionControlRepository>();
builder.Services.AddScoped<IMajorUserApprovalReponsitory, MajorUserApprovalReponsitory>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

//Kho
builder.Services.AddScoped<IKho_DM_DanhMucBaoCaoRepository, Kho_DM_DanhMucBaoCaoRepository>();
builder.Services.AddScoped<IKho_DM_TenBaoCaoRepository, Kho_DM_TenBaoCaoRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_NhomNhienLieuRepository, Kho_DM_NhomNhienLieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_LoaiNhienLieuRepository, Kho_DM_LoaiNhienLieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_NhanHieuRepository ,Kho_DM_NhanHieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_DonViRepository , Kho_DM_DonViRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_LoaiNhaCungCapRepository, Kho_DM_LoaiNhaCungCapRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_NhaCungCapRepository, Kho_DM_NhaCungCapRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_DM_NhomPhuTungRepository, Kho_DM_NhomPhuTungRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<Ikho_DM_LoaiPhuTungRepository, kho_DM_LoaiPhuTungRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_HDMuaNhienLieuRepository, Kho_HDMuaNhienLieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
builder.Services.AddScoped<IKho_NhapkhoNhienLieuRepository, Kho_NhapkhoNhienLieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
builder.Services.AddScoped<IKho_XuatKhoNhienLieuRepository, Kho_XuatKhoNhienLieuRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
builder.Services.AddScoped<IKho_NhapKhoPhuTungRepository, Kho_NhapKhoPhuTungRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
builder.Services.AddScoped<IKho_XuatKhoPhuTungRepository, Kho_XuatKhoPhuTungRepository>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           

//QLNV
builder.Services.AddScoped<IQLNV_NhanVienRepository, QLNV_NhanVienRepository>();
builder.Services.AddScoped<IQLNV_NhomNhanVienRepository, QLNV_NhomNhanVienRepository>();
builder.Services.AddScoped<IQLNV_QuanLyNhanVienRepository, QLNV_QuanLyNhanVienRepository>();
builder.Services.AddScoped<IQLNV_CongViecRepository, QLNV_CongViecRepository>();
builder.Services.AddScoped<IQLNV_TaskCollaborationRepository, QLNV_TaskCollaborationRepository>();
builder.Services.AddScoped<IQLNV_DanhGiaRepository, QLNV_DanhGiaRepository>();
builder.Services.AddScoped<IQLNV_NhanVienThucHienRepository, QLNV_NhanVienThucHienRepository>();

//Công trình
builder.Services.AddScoped<IDM_TrangThaiThiCongRepository, DM_TrangThaiThiCongRepository>();
builder.Services.AddScoped<IDM_HinhThucDayHoGaRepository, DM_HinhThucDayHoGaRepository>();
builder.Services.AddScoped<IDM_LoaiDauNoiRepository, DM_LoaiDauNoiRepository>();
builder.Services.AddScoped<IDM_HinhThucDapTraRepository, DM_HinhThucDapTraRepository>();
builder.Services.AddScoped<IDM_TenLoaiThepRepository, DM_TenLoaiThepRepository>();
builder.Services.AddScoped<IDM_DanhMucThepRepository, DM_DanhMucThepRepository>();
builder.Services.AddScoped<IDM_HangMucCongViecRepository, DM_HangMucCongViecRepository>();
builder.Services.AddScoped<IDM_HangMucKhoiLuongRepository, DM_HangMucKhoiLuongRepository>();
builder.Services.AddScoped<IDM_LoaiCauKienRepository, DM_LoaiCauKienRepository>();
builder.Services.AddScoped<IDM_LoaiKhoiLuongRepository, DM_LoaiKhoiLuongRepository>();
builder.Services.AddScoped<IDM_ThongTinVatTuRepository, DM_ThongTinVatTuRepository>();
builder.Services.AddScoped<IDM_TenCongTacRepository, DM_TenCongTacRepository>();
builder.Services.AddScoped<IDM_TuyenDuongRepository, DM_TuyenDuongRepository>();
builder.Services.AddScoped<IDM_LyTrinhRepository, DM_LyTrinhRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_1TtChungNSachDocRepository, PKKL_OngNhua_1TtChungNSachDocRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_2TtChungNSachNgangRepository, PKKL_OngNhua_2TtChungNSachNgangRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_3TTinLDatVanTruCHoaRepository, PKKL_OngNhua_3TTinLDatVanTruCHoaRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_4ThongTinChungHGaRepository,PKKL_OngNhua_4ThongTinChungHGaRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_47TKThepHGaRepository, PKKL_OngNhua_47TKThepHGaRepository>();
builder.Services.AddScoped<IPKKL_OngNhua_410TKThepTDanRepository, PKKL_OngNhua_410TKThepTDanRepository>();

//Nhân sự
builder.Services.AddScoped<INhanSu_DM_ContractTypeRepository, NhanSu_DM_ContractTypeRepository>();
builder.Services.AddScoped<INhanSu_DM_WorkStatuRepository, NhanSu_DM_WorkStatuRepository>();
builder.Services.AddScoped<INhanSu_EmployeeProfileRepository, NhanSu_EmployeeProfileRepository>();
builder.Services.AddScoped<INhanSu_ContractRepository, NhanSu_ContractRepository>();
builder.Services.AddScoped<INhanSu_SalaryHistoryRepository, NhanSu_SalaryHistoryRepository>();
builder.Services.AddScoped<INhanSu_DM_ChangeTypeRepository, NhanSu_DM_ChangeTypeRepository>();
builder.Services.AddScoped<INhanSu_AppointmentsHistoryRepository, NhanSu_AppointmentsHistoryRepository>();
builder.Services.AddScoped<INhanSu_DM_TerminationReasonRepository, NhanSu_DM_TerminationReasonRepository>();
builder.Services.AddScoped<INhanSu_TerminationRepository, NhanSu_TerminationRepository>();
builder.Services.AddScoped<INhanSu_DM_RewardTypeRepository, NhanSu_DM_RewardTypeRepository>();
builder.Services.AddScoped<INhanSu_RewardRepository, NhanSu_RewardRepository>();
builder.Services.AddScoped<INhanSu_DM_DisciplineTypeRepository, NhanSu_DM_DisciplineTypeRepository>();
builder.Services.AddScoped<INhanSu_DisciplineRepository, NhanSu_DisciplineRepository>();
builder.Services.AddScoped<INhanSu_DM_LeaveTypeRepository, NhanSu_DM_LeaveTypeRepository>();
builder.Services.AddScoped<INhanSu_DM_RequestTypeRepository, NhanSu_DM_RequestTypeRepository>();
builder.Services.AddScoped<INhanSu_RequestRepository, NhanSu_RequestRepository>();
builder.Services.AddScoped<INhanSu_DM_WorkTypeRepository, NhanSu_DM_WorkTypeRepository>();
builder.Services.AddScoped<INhanSu_TimeSheetRepository, NhanSu_TimeSheetRepository>();
builder.Services.AddScoped<INhanSu_EmployeeLeaveQuotaRepository, NhanSu_EmployeeLeaveQuotaRepository>();

// Token lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromMinutes(30));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity lockout
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// Sử dụng AddDefaultIdentity cho Razor Pages Identity UI
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình LoginPath cho Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Đường dẫn trang đăng nhập
    options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi bị từ chối truy cập (nếu có)
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("ChatService", client =>
{
    var baseUrl = builder.Configuration["ChatService:BaseUrl"] ?? "http://localhost:3000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Cấu hình song song Cookie (Identity) và JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme; // Cookie cho web
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})

.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        NameClaimType = ClaimTypes.Name,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    // Nếu bạn đã cài Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore thì mở dòng sau:
    // app.UseMigrationsEndPoint();
    // Nếu không, dùng DeveloperExceptionPage:
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

// Đặt compression trước static files để tối ưu
app.UseResponseCompression();
app.UseStaticFiles();

app.UseRouting();
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseSession();
app.UseAuthentication();
app.UseMiddleware<AccountStatusMiddleware>();
app.UseAuthorization();


// SignalR
app.MapHub<NotificationHub>("/notificationHub");

// Route cho khu vực (area) Admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

var cultureInfo = new CultureInfo("en-CA"); // yyyy-MM-dd

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = new[] { cultureInfo },
    SupportedUICultures = new[] { cultureInfo }
};

app.UseRequestLocalization(localizationOptions);


app.Run();

