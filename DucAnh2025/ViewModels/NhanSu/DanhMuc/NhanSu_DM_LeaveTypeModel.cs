namespace DucAnh2025.ViewModels.NhanSu.DanhMuc;
public partial class NhanSu_DM_LeaveTypeModel : Chung
{
    public string LeaveTypeName { get; set; } = "";
    public bool IsDeductible { get; set; }
    public decimal DefaultQuota { get; set; }
    public string LegalBasis { get; set; } = "";
}
