using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_ChangeType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ChangeTypeName { get; set; } = "";
    public bool IsSalaryImpact { get; set; }
    public bool IsPositionImpact { get; set; }
}

public partial class NhanSu_DM_ChangeType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ChangeTypeName { get; set; } = "";
    public bool IsSalaryImpact { get; set; }
    public bool IsPositionImpact { get; set; }
}
