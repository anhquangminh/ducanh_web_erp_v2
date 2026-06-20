using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers
{
    public class CongTrinhController : Controller
    {
        public IActionResult DmTrangThaiThiCong()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmTrangThaiThiCong");
            return View();
        }
        public IActionResult DmHinhThucDayHoGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmHinhThucDayHoGa");
            return View();
        }
        public IActionResult DmLoaiDauNoi()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmLoaiDauNoi");
            return View();
        }
        public IActionResult DmHinhThucDapTra()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmHinhThucDapTra");
            return View();
        }
        public IActionResult DmTenLoaiThep()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmTenLoaiThep");
            return View();
        }
        public IActionResult DmDanhMucThep()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "CaiDatPhongBanDuyet");
            return View();
        }
        public IActionResult DmHangMucCongViec()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmHangMucCongViec");
            return View();
        }
        public IActionResult DmHangMucKhoiLuong()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmHangMucKhoiLuong");
            return View();
        }
        public IActionResult DmLoaiCauKien()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmLoaiCauKien");
            return View();
        }
        public IActionResult DmLoaiKhoiLuong()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmLoaiKhoiLuong");
            return View();
        }
        public IActionResult DmTenCongTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmTenCongTac");
            return View();
        }
        public IActionResult DmThongTinVatTu()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmThongTinVatTu");
            return View();
        }
        public IActionResult DmTuyenDuong()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmTuyenDuong");
            return View();
        }
        public IActionResult DmLyTrinh()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "DmLyTrinh");
            return View();
        }


        public IActionResult PKKL_OngNhua_1TtChungNSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1TtChungNSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_1LTrinhNSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_1LTrinhNSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_2CDoDapNSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_2CDoDapNSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_3CDoDapNSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_3CDoDapNSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_4KLDaoDapOngNhuaDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_4KLDaoDapOngNhuaDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_5THKLXDungNuocSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_5THKLXDungNuocSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_6THKLDaoNSachDoc()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_6THKLDaoNSachDoc");
            return View();
        }
        public IActionResult PKKL_OngNhua_1_7THKLDaoDapTheoLoaiKL()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_1_7THKLDaoDapTheoLoaiKL");
            return View();
        }

        public IActionResult PKKL_OngNhua_2TtChungNSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2TtChungNSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_1LTrinhNSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_1LTrinhNSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_2CDoDaoNSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_2CDoDaoNSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_3CDoDapNSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_3CDoDapNSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_4DapTraOngNhuaNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_4DapTraOngNhuaNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_5THKLNuocSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_5THKLNuocSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_6THKLDaoNSachNgang()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_6THKLDaoNSachNgang");
            return View();
        }
        public IActionResult PKKL_OngNhua_2_7THKLDaoDapTheoLoaiKL()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_2_7THKLDaoDapTheoLoaiKL");
            return View();
        }



        public IActionResult PKKL_OngNhua_3TTinLDatVanTruCHoa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_3TTinLDatVanTruCHoa");
            return View();
        }
        public IActionResult PKKL_OngNhua_3_1THKLVanTruCHoa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_3_1THKLVanTruCHoa");
            return View();
        }

        public IActionResult PKKL_OngNhua_4ThongTinChungHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4ThongTinChungHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_1CaoDoKCauHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_1CaoDoKCauHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_1aCaoDoDapHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_1aCaoDoDapHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_2KLdaoDapHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_2KLdaoDapHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_3THKLDaoDapTTuyen()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_3THKLDaoDapTTuyen");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_4KTHHHG()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_4KTHHHG");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_5HinhThucDauNoi()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_5HinhThucDauNoi");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_6KHopTDanHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_6KHopTDanHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_7KLHGaTheoTenHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_7KLHGaTheoTenHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_7aKLHGaTheoTenCTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_7aKLHGaTheoTenCTac");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_8KTHHTDan()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_8KTHHTDan");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_9KLTDHGTheoLTrinh()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_9KLTDHGTheoLTrinh");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_9aTHKLTDHGTheoTenCTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_9aTHKLTDHGTheoTenCTac");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_9bTHKLTDHVTTuyen()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_9bTHKLTDHVTTuyen");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_9cTHKLTDHGTheoLoaiKL()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_9cTHKLTDHGTheoLoaiKL");
            return View();
        }
       

        public IActionResult PKKL_OngNhua_4_7bTHKLHGaTTuyen()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_7bTHKLHGaTTuyen");
            return View();
        }
        public IActionResult PKKL_OngNhua_4_7cTHKLHGaTheoLoaiKL()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_4_7cTHKLHGaTheoLoaiKL");
            return View();
        }

        public IActionResult PKKL_OngNhua_47TKThepHGa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_47TKThepHGa");
            return View();
        }
        public IActionResult PKKL_OngNhua_47_aTHThepHGaTheoCTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_47_aTHThepHGaTheoCTac");
            return View();
        }
        public IActionResult PKKL_OngNhua_47_bTHThepHGaTheoLThep()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_47_bTHThepHGaTheoLThep");
            return View();
        }
        public IActionResult PKKL_OngNhua_47_cTHThepHGaTheoCDai()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_47_cTHThepHGaTheoCDai");
            return View();
        }

        public IActionResult PKKL_OngNhua_410TKThepTDan()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410TKThepTDan");
            return View();
        }
        public IActionResult PKKL_OngNhua_410_aTHThepTheoLoaiTDHG()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410_aTHThepTheoLoaiTDHG");
            return View();
        }
        public IActionResult PKKL_OngNhua_410_bTHThepTDHVTenCTac()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410_bTHThepTDHVTenCTac");
            return View();
        }
        public IActionResult PKKL_OngNhua_410_cTHDKinhThepTDHV()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410_cTHDKinhThepTDHV");
            return View();
        }
        public IActionResult PKKL_OngNhua_410_dTHThepTDHVCDai()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410_dTHThepTDHVCDai");
            return View();
        }
        public IActionResult PKKL_OngNhua_410_eTHCDaiThanhTDHV()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_410_eTHCDaiThanhTDHV");
            return View();
        }
        public IActionResult PKKL_OngNhua_5THKLongPKien()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_5THKLongPKien");
            return View();
        }
        public IActionResult PKKL_OngNhua_5_1THKLVanTruCHoa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_5_1THKLVanTruCHoa");
            return View();
        }
        public IActionResult PKKL_OngNhua_5_2THKLVanTruCHoa()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_5_2THKLVanTruCHoa");
            return View();
        }
        public IActionResult PKKL_OngNhua_5_3THKLXayDung()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_5_3THKLXayDung");
            return View();
        }
        public IActionResult PKKL_OngNhua_5_4THKLThep()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "PKKL_OngNhua_5_4THKLThep");
            return View();
        }

        public IActionResult QuanLyCongTrinh()
        {
            ViewBag.Breadcrumbs = BreadcrumbHelper.GetBreadcrumb("CongTrinh", "QuanLyCongTrinh");
            return View();
        }
    }
}
