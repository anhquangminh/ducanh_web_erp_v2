using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_Request : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string RequestTypeId { get; set; } = "";
    public DateTime? RequestDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalDuration { get; set; }
    public string LeaveTypeId { get; set; } = "";
    public string Reason { get; set; } = "";
    public string CurrentApproverId { get; set; } = "";
}

public partial class NhanSu_Request_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string RequestTypeId { get; set; } = "";
    public DateTime? RequestDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalDuration { get; set; }
    public string LeaveTypeId { get; set; } = "";
    public string Reason { get; set; } = "";
    public string CurrentApproverId { get; set; } = "";
}
