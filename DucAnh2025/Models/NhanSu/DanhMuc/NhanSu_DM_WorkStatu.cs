using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_WorkStatu : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkStatusName { get; set; } = "";
    public string Description { get; set; } = "";
}

public partial class NhanSu_DM_WorkStatu_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkStatusName { get; set; } = "";
    public string Description { get; set; } = "";
}
