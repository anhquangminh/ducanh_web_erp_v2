using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_DisciplineType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DisciplineTypeName { get; set; } = "";
    public int Level { get; set; }
    public bool IsTerminationRisk { get; set; }
    public string Description { get; set; } = "";
}

public partial class NhanSu_DM_DisciplineType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DisciplineTypeName { get; set; } = "";
    public int Level { get; set; }
    public bool IsTerminationRisk { get; set; }
    public string Description { get; set; } = "";
}
