using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_TerminationReason : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ReasonName { get; set; } = "";
    public string ReasonGroup { get; set; } = "";
}

public partial class NhanSu_DM_TerminationReason_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ReasonName { get; set; } = "";
    public string ReasonGroup { get; set; } = "";
}

