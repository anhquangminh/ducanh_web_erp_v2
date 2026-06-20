using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_RewardType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RewardTypeName { get; set; } = "";
    public bool IsFinancialImpact { get; set; }
    public string Description { get; set; } = "";
}

public partial class NhanSu_DM_RewardType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RewardTypeName { get; set; } = "";
    public bool IsFinancialImpact { get; set; }
    public string Description { get; set; } = "";
}
