using AspNetCoreGeneratedDocument;
using DucAnh2025.ViewModels.Kho;

namespace DucAnh2025.ViewModels;
public partial class Kho_NhapkhoNhienLieuModel : Kho_HDMuaNhienLieuModel
{
    public string Id_TramTron { get; set; } = "";
    public DateTime? NgayNhapKho { get; set; }
    public int ChotSoCoCay { get; set; }
    public int SoPhieu { get; set; }
    public string BienSo { get; set; } = "";
    public string SoHopDong { get; set; } = "";
    public string Id_LoaiNhienLieu { get; set; } = "";
    public string Id_NhanHieu { get; set; } = "";
    public string Id_DonVi { get; set; } = "";
    public decimal SLNhapKhoCoThue { get; set; }
    public decimal SLNhapKhoKhongThue { get; set; }
    public string DonViSauQuyDoi { get; set; } = "";
    public decimal SoLuongThucCoThue { get; set; }
    public decimal SoLuongThucKhongThue { get; set; }
    public decimal TongSoLuong { get; set; }
    public decimal DonGiaCoThue { get; set; }
    public decimal DonGiaKhongThue { get; set; }
    public decimal ThanhTienCoThue { get; set; }
    public decimal ThanhTienKhongThue { get; set; }
    public decimal TongThanhTien { get; set; }
    public string BienSoXe { get; set; } = "";
    public decimal DonGiaCuocCoThue { get; set; }
    public decimal DonGiaCuocKhongThue { get; set; }
    public decimal ThanhTienCuocCoThue { get; set; }
    public decimal ThanhTienCuocKhongThue { get; set; }
    public decimal TongTienHangCoThue { get; set; }
    public decimal TongTienHangKhongThue { get; set; }
    public decimal TongTienHang { get; set; }
}
