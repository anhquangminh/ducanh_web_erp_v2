using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models.Kho.DanhMuc;
public partial class Kho_DM_LoaiNhienLieu : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_NhomNhienLieu { get; set; } = "";
    public string LoaiNhienLieu { get; set; } = "";
}

public partial class Kho_DM_LoaiNhienLieu_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_NhomNhienLieu { get; set; } = "";
    public string LoaiNhienLieu { get; set; } = "";
}

