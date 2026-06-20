namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_TimeSheetModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime? TimeSheetDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string WorkTypeId { get; set; } = "";
    public decimal TotalHours { get; set; }
    public decimal TotalDays { get; set; }
    public string Source { get; set; } = "";
}
