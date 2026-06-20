using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DucAnh2025.Controllers
{
    public class NhanSuController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IPhanQuyenRepository _phanQuyenRepository;

        public NhanSuController(
            IApplicationUserRepository applicationUserRepository,
            IPhanQuyenRepository phanQuyenRepository)
        {
            _applicationUserRepository = applicationUserRepository;
            _phanQuyenRepository = phanQuyenRepository;
        }
        public async Task<IActionResult> NhanVien()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("index", "Home");
            var user = await _applicationUserRepository.GetById(userId);
            var groupId = user?.GroupId;

            // Nếu chưa đăng nhập hoặc thiếu groupId
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(groupId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var permissions = await _phanQuyenRepository.GetAllPermissionByGroupId(
                groupId,
                userId,
                "249ff511-8f10-45e8-bf8f-29b0ada5ab84",
                "2105f7e7-1d45-4369-85a9-fdd185c3490b"
            );
            var hasNhanVienPermission = permissions.Any(p => p.PermissionName == "Truy cập" && p.IsActive == 3);

            if (!hasNhanVienPermission)
            {
                return RedirectToAction("index", "Home");
            }
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanVien");
            return View();
        }

        public async Task<IActionResult> ChucVu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "ChucVu");
            return View();
        }
        public async Task<IActionResult> ChuyenMon()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "ChuyenMon");
            return View();
        }
        public async Task<IActionResult> PhongBan()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "PhongBan");
            return View();
        }
        //danh mục
        public async Task<IActionResult> DMContractType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMContractType");
            return View();
        }
        public async Task<IActionResult> DMWorkStatus()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMWorkStatus");
            return View();
        }
        public async Task<IActionResult> DMChangeType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMChangeType");
            return View();
        }
        public async Task<IActionResult> DMTerminationReason()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMTerminationReason");
            return View();
        }
        public async Task<IActionResult> DMRewardType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMRewardType");
            return View();
        }
        public async Task<IActionResult> DMDisciplineType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMDisciplineType");
            return View();
        }
        public async Task<IActionResult> DMLeaveType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMLeaveType");
            return View();
        }
        public async Task<IActionResult> DMRequestType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMRequestType");
            return View();
        }
        public async Task<IActionResult> DMWorkType()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "DMWorkType");
            return View();
        }


        public async Task<IActionResult> NhanSuDashboard()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuDashboard");
            return View();;
        }
        public async Task<IActionResult> NhanSuEmployeeProfile()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuEmployeeProfile");
            return View();;
        }
        public async Task<IActionResult> NhanSuContract()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuContract");
            return View();
        }
        public async Task<IActionResult> NhanSuSalaryHistory()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuSalaryHistory");
            return View();
        }
        public async Task<IActionResult> NhanSuAppointmentsHistorys()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuAppointmentsHistorys");
            return View();
        }
        public async Task<IActionResult> NhanSuTermination()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuTermination");
            return View();
        }
        public async Task<IActionResult> NhanSuRewards()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuRewards");
            return View();
        }
        public async Task<IActionResult> NhanSuDiscipline()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuDiscipline");
            return View();
        }
        public async Task<IActionResult> NhanSuRequest()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuRequest");
            return View();
        }
        public async Task<IActionResult> NhanSuTimeSheet()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuTimeSheet");
            return View();
        }
        public async Task<IActionResult> NhanSuEmployeeLeaveQuota()
        {

            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("NhanSu", "NhanSuEmployeeLeaveQuota");
            return View();
        }
       
    }
}
