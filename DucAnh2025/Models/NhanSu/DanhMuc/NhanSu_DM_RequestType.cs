using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_DM_RequestType : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequestTypeName { get; set; } = "";
    public bool IsTimeAttendanceImpact { get; set; }
    public string DefaultProcessId { get; set; } = "";
    public string Description { get; set; } = "";
}

public partial class NhanSu_DM_RequestType_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequestTypeName { get; set; } = "";
    public bool IsTimeAttendanceImpact { get; set; }
    public string DefaultProcessId { get; set; } = "";
    public string Description { get; set; } = "";
}
