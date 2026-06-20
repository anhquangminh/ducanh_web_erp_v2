using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_SalaryHistory : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? EffectiveDate { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalFixedAllowance { get; set; }
    public string AllowanceDetails { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string ChangeReason { get; set; } = "";
}

public partial class NhanSu_SalaryHistory_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? EffectiveDate { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalFixedAllowance { get; set; }
    public string AllowanceDetails { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string ChangeReason { get; set; } = "";
}
