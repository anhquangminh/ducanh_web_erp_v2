using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_Discipline : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string DisciplineTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string IncidentName { get; set; } = "";
    public DateTime? IncidentDate { get; set; }
    public DateTime? DisciplineDate { get; set; }
    public int DurationDays { get; set; }
    public decimal FinancialImpact { get; set; }
    public string ViolationDetail { get; set; } = "";
    public string SignerId { get; set; } = "";
    public string FilePath { get; set; } = "";
}

public partial class NhanSu_Discipline_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string DisciplineTypeId { get; set; } = "";
    public string DecisionCode { get; set; } = "";
    public string IncidentName { get; set; } = "";
    public DateTime? IncidentDate { get; set; }
    public DateTime? DisciplineDate { get; set; }
    public int DurationDays { get; set; }
    public decimal FinancialImpact { get; set; }
    public string ViolationDetail { get; set; } = "";
    public string SignerId { get; set; } = "";
    public string FilePath { get; set; } = "";
}
