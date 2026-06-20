using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_LeaveType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LeaveTypeName { get; set; } = "";
    public bool IsDeductible { get; set; }
    public decimal DefaultQuota { get; set; }
    public string LegalBasis { get; set; } = "";
}

public partial class NhanSu_DM_LeaveType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LeaveTypeName { get; set; } = "";
    public bool IsDeductible { get; set; }
    public decimal DefaultQuota { get; set; }
    public string LegalBasis { get; set; } = "";
}
