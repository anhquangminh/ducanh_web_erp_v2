using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_Reward : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string RewardTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string RewardName { get; set; } = "";
    public DateTime? RewardDate { get; set; }
    public string RewardForm { get; set; } = "";
    public decimal RewardValue { get; set; }
    public string AchievementDetail { get; set; } = "";
    public string SignerId { get; set; } = "";
    public string FilePath { get; set; } = "";
}

public partial class NhanSu_Reward_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string RewardTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string RewardName { get; set; } = "";
    public DateTime? RewardDate { get; set; }
    public string RewardForm { get; set; } = "";
    public decimal RewardValue { get; set; }
    public string AchievementDetail { get; set; } = "";
    public string SignerId { get; set; } = "";
    public string FilePath { get; set; } = "";
}
