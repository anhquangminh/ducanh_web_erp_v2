using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_XuatKhoPhuTung : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_Tram { get; set; } = "";
    public DateTime? NgayXuatKho { get; set; }
    public int SoPhieu { get; set; }
    public string DoiTuongNhan { get; set; } = "";
    public string Id_NhomPhuTung { get; set; } = "";
    public string Id_LoaiPhuTung { get; set; } = "";
    public string Id_NhanHieu { get; set; } = "";
    public string Seri { get; set; } = "";
    public string Id_DonVi { get; set; } = "";
    public decimal KLCoThue { get; set; }
    public decimal KLKhongThue { get; set; }
    public decimal DonGiaCoThue { get; set; }
    public decimal DonGiaKhongThue { get; set; }
    public decimal ThanhTienCoThue { get; set; }
    public decimal ThanhTienKhongThue { get; set; }
    public decimal TongTien { get; set; }
}

public partial class Kho_XuatKhoPhuTung_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_Tram { get; set; } = "";
    public DateTime? NgayXuatKho { get; set; }
    public int SoPhieu { get; set; }
    public string DoiTuongNhan { get; set; } = "";
    public string Id_NhomPhuTung { get; set; } = "";
    public string Id_LoaiPhuTung { get; set; } = "";
    public string Id_NhanHieu { get; set; } = "";
    public string Seri { get; set; } = "";
    public string Id_DonVi { get; set; } = "";
    public decimal KLCoThue { get; set; }
    public decimal KLKhongThue { get; set; }
    public decimal DonGiaCoThue { get; set; }
    public decimal DonGiaKhongThue { get; set; }
    public decimal ThanhTienCoThue { get; set; }
    public decimal ThanhTienKhongThue { get; set; }
    public decimal TongTien { get; set; }
}
