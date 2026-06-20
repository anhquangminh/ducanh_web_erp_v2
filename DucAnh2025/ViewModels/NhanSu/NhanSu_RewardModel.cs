namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_RewardModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
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
