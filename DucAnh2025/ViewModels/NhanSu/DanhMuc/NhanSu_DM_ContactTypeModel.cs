namespace DucAnh2025.ViewModels.NhanSu.DanhMuc;
public partial class NhanSu_DM_ContractTypeModel : Chung
{
    public string ContractTypeName { get; set; } = "";
    public string Description { get; set; } = "";
    public int DurationMonths { get; set; }
    public int WarningDays { get; set; }
    public int IsRenewable { get; set; }
    public string NextContractTypeId { get; set; } = "";
    public decimal ProbationSalaryRate { get; set; }
}
