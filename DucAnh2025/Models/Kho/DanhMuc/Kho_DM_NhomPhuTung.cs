using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_DM_NhomPhuTung : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhomPhuTung { get; set; } = "";
}

public partial class Kho_DM_NhomPhuTung_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhomPhuTung { get; set; } = "";
}
