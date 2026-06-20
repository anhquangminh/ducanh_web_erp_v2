using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class Kho_DM_DonVi : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenDonVi { get; set; } = "";
}

public partial class Kho_DM_DonVi_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenDonVi { get; set; } = "";
}
