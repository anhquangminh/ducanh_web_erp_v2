using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_DM_LoaiNhaCungCap : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenLoaiNhaCungCap { get; set; } = "";
}

public partial class Kho_DM_LoaiNhaCungCap_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenLoaiNhaCungCap { get; set; } = "";
}
