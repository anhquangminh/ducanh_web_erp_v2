namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_EmployeeLeaveQuotaModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public int Year { get; set; }
    public decimal TotalAnnualLeave { get; set; }
    public decimal CarriedOverLeave { get; set; }
    public decimal UsedLeave { get; set; }
    public decimal RemainingLeave { get; set; }
    public decimal OtherLeaveBalance { get; set; }
}
