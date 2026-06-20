using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers
{
    [Authorize]
    public class HeThongController : Controller
    {
        public IActionResult CaiDatPhongBanDuyet()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "CaiDatPhongBanDuyet");
            return View();
        }
        public IActionResult CaiDatSoLuotDuyet()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "CaiDatSoLuotDuyet");
            return View();
        }

        public IActionResult PhanQuyenCaiDatDuyet()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "PhanQuyenCaiDatDuyet");
            return View();
        }
        public IActionResult PhanQuyenCaiDatThaoTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "PhanQuyenCaiDatThaoTac");
            return View();
        }
        public IActionResult PhanQuyenDuyet()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "PhanQuyenDuyet");
            return View();
        }
        public IActionResult PhanQuyenThaoTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "PhanQuyenThaoTac");
            return View();
        }
        public IActionResult LoaiChiNhanh()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "LoaiChiNhanh");
            return View();
        }
        public IActionResult ChiNhanh()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "ChiNhanh");
            return View();
        }
        public IActionResult NghiepVu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "NghiepVu");
            return View();
        }
        public IActionResult DanhSachQuyen()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "DanhSachQuyen");
            return View();
        }
        public IActionResult UserRoleManager()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("HeThong", "UserRoleManager");
            return View();
        }
        
    }
}
