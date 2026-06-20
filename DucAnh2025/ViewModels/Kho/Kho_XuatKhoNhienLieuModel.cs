namespace DucAnh2025.ViewModels;
public partial class Kho_XuatKhoNhienLieuModel : Chung
{
    public string Id_TramTron { get; set; } = "";
    public DateTime? NgayNhapKho { get; set; }
    public int ChotSoCoCay { get; set; }
    public int SoPhieu { get; set; }
    public int ChotSoKMTruocKhiCap { get; set; }
    public int ChieuCaoBinhNhienLieu { get; set; }
    public string DoiTuongNhan { get; set; } = "";
    public string Id_NhomNhienLieu { get; set; } = "";
    public string Id_LoaiNhienLieu { get; set; } = "";
    public string Id_NhanHieu { get; set; } = "";
    public string Id_DonVi { get; set; } = "";
    public decimal KLXuatKhoCoThue { get; set; }
    public decimal KLXuatKhoKhongThue { get; set; }
    public string DonViSauQuyDoi { get; set; } = "";
    public decimal SLThucTeCoThue { get; set; }
    public decimal SLThucTeKhongThue { get; set; }
    public decimal TongSoLuong { get; set; }
    public decimal DonGiaCoThue { get; set; }
    public decimal DonGiaKhongThue { get; set; }
    public decimal ThanhTienCoThue { get; set; }
    public decimal ThanhTienKhongThue { get; set; }
    public decimal ThanhTienTongTien { get; set; }
}
