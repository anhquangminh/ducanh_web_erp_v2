using DucAnh2025.Models.Accounts;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DucAnh2025.Controllers
{
    [Authorize]
    public class QLNVController : Controller
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IPhanQuyenRepository _phanQuyenRepository;

        public QLNVController(
            IApplicationUserRepository applicationUserRepository,
            IPhanQuyenRepository phanQuyenRepository)
        {
            _applicationUserRepository = applicationUserRepository;
            _phanQuyenRepository = phanQuyenRepository;
        }

        public async Task<IActionResult> NhanVien()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser user = new ();
            try
            {
                 user = await _applicationUserRepository.GetById(userId);
                if(user ==null)
                    return RedirectToAction("index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("index", "Home");
                
            }
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
            
            return View();
        }
        public async Task<IActionResult> QuanLyNhom()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = new();
            try
            {
                user = await _applicationUserRepository.GetById(userId);
                if (user == null)
                    return RedirectToAction("index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("index", "Home");

            }
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
                "b9100b9e-6be2-45fa-a85c-b1bfc6b313ba"
            );
            var hasNhanVienPermission = permissions.Any(p => p.PermissionName == "Truy cập" && p.IsActive == 3);

            if (!hasNhanVienPermission)
            {
                return RedirectToAction("index", "Home");
            }
            
            return View();
        }
        public IActionResult DanhGia()
        {
            
            return View();
        }
        public IActionResult QuanLyCongViec()
        {
            
            return View();
        }
        public IActionResult CongViecDuocGiao()
        {
            
            return View();
        }
    }
}
