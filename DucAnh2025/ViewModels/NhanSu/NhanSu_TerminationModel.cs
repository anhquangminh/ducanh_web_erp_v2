namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_TerminationModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime? ApplicationDate { get; set; }
    public DateTime? DesiredLeaveDate { get; set; }
    public DateTime? OfficialLeaveDate { get; set; }
    public string ReasonId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string FilePath { get; set; } = "";
    public DateTime? ClearanceDate { get; set; }
    public string HRNotes { get; set; } = "";
}
