namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_DisciplineModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public string DisciplineTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string IncidentName { get; set; } = "";
    public DateTime? IncidentDate { get; set; }
    public DateTime? DisciplineDate { get; set; }
    public int DurationDays { get; set; }
    public decimal FinancialImpact { get; set; }
    public string ViolationDetail { get; set; } = "";
    public string SignerId { get; set; } = "";
    public string FilePath { get; set; } = "";
}
