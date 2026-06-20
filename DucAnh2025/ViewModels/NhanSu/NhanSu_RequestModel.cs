namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_RequestModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public string RequestTypeId { get; set; } = "";
    public DateTime? RequestDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalDuration { get; set; }
    public string LeaveTypeId { get; set; } = "";
    public string Reason { get; set; } = "";
    public string CurrentApproverId { get; set; } = "";
}
