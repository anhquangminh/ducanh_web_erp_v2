using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_WorkType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkTypeName { get; set; } = "";
    public string WorkTypeGroup { get; set; } = "";
    public decimal SalaryFactor { get; set; }
    public string Description { get; set; } = "";
}

public partial class NhanSu_DM_WorkType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkTypeName { get; set; } = "";
    public string WorkTypeGroup { get; set; } = "";
    public decimal SalaryFactor { get; set; }
    public string Description { get; set; } = "";
}
