namespace DucAnh2025.ViewModels.NhanSu;
public partial class NhanSu_ContractModel : Chung
{
    public string EmployeeId { get; set; } = "";
    public string MaNhanVien { get; set; } = "";
    public string FullName { get; set; } = "";
    public string ContractTypeId { get; set; } = "";
    public string ContractCode { get; set; } = "";
    public string ContractName { get; set; } = "";
    public DateTime? SigningDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string ContractStatusId { get; set; } = "";
    public decimal ContractSalary { get; set; }
    public int DurationMonths { get; set; }
    public string FilePath { get; set; } = "";
    public bool IsESignRequired { get; set; }
    public DateTime? LastRenewalDate { get; set; }
    public string Notes { get; set; } = "";
    public DateTime? AlertDate { get; set; }
    public string SignerId { get; set; } = "";
    public string SignerPosition { get; set; } = "";
}
