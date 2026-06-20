namespace DucAnh2025.ViewModels.NhanSu.DanhMuc;
public partial class NhanSu_DM_DisciplineTypeModel : Chung
{
    public string DisciplineTypeName { get; set; } = "";
    public int Level { get; set; }
    public bool IsTerminationRisk { get; set; }
    public string Description { get; set; } = "";
}
