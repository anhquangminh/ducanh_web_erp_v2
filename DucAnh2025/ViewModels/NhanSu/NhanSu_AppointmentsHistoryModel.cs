namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_AppointmentsHistoryModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public string ChangeTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public DateTime? DecisionDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string OldDepartmentId { get; set; } = "";
    public string OldChucVuId { get; set; } = "";
    public string NewDepartmentId { get; set; } = "";
    public string NewChucVuId { get; set; } = "";
    public decimal NewBasicSalary { get; set; }
    public string Content { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string SignerId { get; set; } = "";
}
