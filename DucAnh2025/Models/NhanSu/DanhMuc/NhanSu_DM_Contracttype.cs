using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_ContractType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContractTypeName { get; set; } = "";
    public string Description { get; set; } = "";
    public int DurationMonths { get; set; }
    public int WarningDays { get; set; }
    public int IsRenewable { get; set; }
    public string NextContractTypeId { get; set; } = "";
    public decimal ProbationSalaryRate { get; set; }
}

public partial class NhanSu_DM_ContractType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContractTypeName { get; set; } = "";
    public string Description { get; set; } = "";
    public int DurationMonths { get; set; }
    public int WarningDays { get; set; }
    public int IsRenewable { get; set; }
    public string NextContractTypeId { get; set; } = "";
    public decimal ProbationSalaryRate { get; set; }
}
