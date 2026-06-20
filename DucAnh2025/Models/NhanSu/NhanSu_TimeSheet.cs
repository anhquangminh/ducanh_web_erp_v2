using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models.NhanSu;
public partial class NhanSu_TimeSheet : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? TimeSheetDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string WorkTypeId { get; set; } = "";
    public decimal TotalHours { get; set; }
    public decimal TotalDays { get; set; }
    public string Source { get; set; } = "";
}

public partial class NhanSu_TimeSheet_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? TimeSheetDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string WorkTypeId { get; set; } = "";
    public decimal TotalHours { get; set; }
    public decimal TotalDays { get; set; }
    public string Source { get; set; } = "";
}
