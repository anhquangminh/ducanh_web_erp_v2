using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_DM_NhanHieu : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhanHieu { get; set; } = "";
}

public partial class Kho_DM_NhanHieu_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhanHieu { get; set; } = "";
}
