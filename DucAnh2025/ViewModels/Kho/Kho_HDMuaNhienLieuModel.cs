namespace DucAnh2025.ViewModels.Kho;
public partial class Kho_HDMuaNhienLieuModel : Chung
{
    public string Id_TenTram { get; set; } = "";
    public DateTime? NgayKyHopDong { get; set; }
    public string SoHopDong { get; set; } = "";
    public string Id_LoaiNhaCungCap { get; set; } = "";
    public string Id_NhaCungcap { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public string MaSoThue { get; set; } = "";
    public string Id_NhomNhienLieu { get; set; } = "";
    public string Id_LoaiNhienLieu { get; set; } = "";
    public string Id_NhanHieu { get; set; } = "";
    public string Id_DonVi { get; set; } = "";
    public decimal DonGiaBGCoThue { get; set; }
    public decimal DonGiaBGKhongThue { get; set; }
    public decimal DonGiaKBCoThue { get; set; }
    public decimal DonGiaKBKhongThue { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
}
