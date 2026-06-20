using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DucAnh2025.Controllers
{
    public class KhoController : Controller
    {
        public KhoController(){}
        
        public async Task<IActionResult> Index()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "index");
            return View();
        }
        public async Task<IActionResult> DMDanhMucBaoCao()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMDanhMucBaoCao");
            return View();
        }
        public async Task<IActionResult> DMTenBaoCao()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMTenBaoCao");
            return View();
        }
        public async Task<IActionResult> DMNhomNhienLieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMNhomNhienLieu");
            return View();
        }
        public async Task<IActionResult> DMLoaiNhienLieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMLoaiNhienLieu");
            return View();
        }
        public async Task<IActionResult> DMNhanHieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMNhanHieu");
            return View();
        }
        public async Task<IActionResult> DMDonVi()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMDonVi");
            return View();
        }
        public async Task<IActionResult> DMLoaiNhaCungCap()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMLoaiNhaCungCap");
            return View();
        }
        public async Task<IActionResult> DMNhaCungCap()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMNhaCungCap");
            return View();
        }
        public async Task<IActionResult> DMNhomPhuTung()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMNhomPhuTung");
            return View();
        }
        public async Task<IActionResult> DMLoaiPhuTung()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "DMLoaiPhuTung");
            return View();
        }
        public async Task<IActionResult> KhoHDMuaNhienLieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "KhoHDMuaNhienLieu");
            return View();
        }
        public async Task<IActionResult> NhapkhoNhienLieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "NhapkhoNhienLieu");
            return View();
        }
        public async Task<IActionResult> XuatkhoNhienLieu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "XuatkhoNhienLieu");
            return View();
        }
        public async Task<IActionResult> NhapKhoPhuTung()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "NhapKhoPhuTung");
            return View();
        }
        public async Task<IActionResult> XuatKhoPhuTung()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("Kho", "XuatKhoPhuTung");
            return View();
        }
      
    }
}
