using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_EmployeeLeaveQuota : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public int Year { get; set; }
    public decimal TotalAnnualLeave { get; set; }
    public decimal CarriedOverLeave { get; set; }
    public decimal UsedLeave { get; set; }
    public decimal RemainingLeave { get; set; }
    public decimal OtherLeaveBalance { get; set; }
}

public partial class NhanSu_EmployeeLeaveQuota_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public int Year { get; set; }
    public decimal TotalAnnualLeave { get; set; }
    public decimal CarriedOverLeave { get; set; }
    public decimal UsedLeave { get; set; }
    public decimal RemainingLeave { get; set; }
    public decimal OtherLeaveBalance { get; set; }
}
