using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_DM_NhomNhienLieu : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhienLieu { get; set; } = "";
}

public partial class Kho_DM_NhomNhienLieu_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenNhienLieu { get; set; } = "";
}

