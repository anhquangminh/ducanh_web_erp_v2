using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class kho_DM_LoaiPhuTung : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_NhomPhuTung { get; set; } = "";
    public string TenLoaiPhuTung { get; set; } = "";
}

public partial class kho_DM_LoaiPhuTung_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_NhomPhuTung { get; set; } = "";
    public string TenLoaiPhuTung { get; set; } = "";
}
