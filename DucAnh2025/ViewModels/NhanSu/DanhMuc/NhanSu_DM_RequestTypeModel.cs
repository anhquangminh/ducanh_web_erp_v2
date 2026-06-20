namespace DucAnh2025.ViewModels.NhanSu.DanhMuc;
public partial class NhanSu_DM_RequestTypeModel : Chung
{
    public string RequestTypeName { get; set; } = "";
    public bool IsTimeAttendanceImpact { get; set; }
    public string DefaultProcessId { get; set; } = "";
    public string Description { get; set; } = "";
}
