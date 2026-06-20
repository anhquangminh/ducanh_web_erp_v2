namespace DucAnh2025.ViewModels.NhanSu.DanhMuc;
public partial class NhanSu_DM_WorkTypeModel : Chung
{
    public string WorkTypeName { get; set; } = "";
    public string WorkTypeGroup { get; set; } = "";
    public decimal SalaryFactor { get; set; }
    public string Description { get; set; } = "";
}
