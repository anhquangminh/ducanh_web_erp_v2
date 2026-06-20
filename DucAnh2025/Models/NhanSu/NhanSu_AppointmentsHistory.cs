using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_AppointmentsHistory : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string ChangeTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public DateTime? DecisionDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string OldDepartmentId { get; set; } = "";
    public string OldChucVuId { get; set; } = "";
    public string NewDepartmentId { get; set; } = "";
    public string NewChucVuId { get; set; } = "";
    public decimal NewBasicSalary { get; set; }
    public string Content { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string SignerId { get; set; } = "";
}

public partial class NhanSu_AppointmentsHistory_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string ChangeTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public DateTime? DecisionDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string OldDepartmentId { get; set; } = "";
    public string OldChucVuId { get; set; } = "";
    public string NewDepartmentId { get; set; } = "";
    public string NewChucVuId { get; set; } = "";
    public decimal NewBasicSalary { get; set; }
    public string Content { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string SignerId { get; set; } = "";
}
