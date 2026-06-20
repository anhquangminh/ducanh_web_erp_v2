using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_Termination : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? ApplicationDate { get; set; }
    public DateTime? DesiredLeaveDate { get; set; }
    public DateTime? OfficialLeaveDate { get; set; }
    public string ReasonId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string FilePath { get; set; } = "";
    public DateTime? ClearanceDate { get; set; }
    public string HRNotes { get; set; } = "";
}

public partial class NhanSu_Termination_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public DateTime? ApplicationDate { get; set; }
    public DateTime? DesiredLeaveDate { get; set; }
    public DateTime? OfficialLeaveDate { get; set; }
    public string ReasonId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string FilePath { get; set; } = "";
    public DateTime? ClearanceDate { get; set; }
    public string HRNotes { get; set; } = "";
}
