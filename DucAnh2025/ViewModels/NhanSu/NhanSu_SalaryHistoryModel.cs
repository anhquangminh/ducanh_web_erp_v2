namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_SalaryHistoryModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime? EffectiveDate { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalFixedAllowance { get; set; }
    public string AllowanceDetails { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string ChangeReason { get; set; } = "";
}
