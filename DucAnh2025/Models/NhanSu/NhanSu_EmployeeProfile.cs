using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models;
public partial class NhanSu_EmployeeProfile : ModelChung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string IdentityCardNumber { get; set; } = "";
    public DateTime? IdentityCardIssueDate { get; set; }
    public string IdentityCardIssuePlace { get; set; } = "";
    public string TaxCode { get; set; } = "";
    public string SocialInsuranceNumber { get; set; } = "";
    public string MobilePhone { get; set; } = "";
    public string PersonalEmail { get; set; } = "";
    public string PermanentAddress { get; set; } = "";
    public string CurrentAddress { get; set; } = "";
    public string EmergencyContactName { get; set; } = "";
    public string EmergencyContactPhone { get; set; } = "";
    public string DepartmentFrId { get; set; } = "";
    public string ChucVuId { get; set; } = "";
    public string ManagerId { get; set; } = "";
    public string WorkEmail { get; set; } = "";
    public DateTime? HireDate { get; set; }
    public DateTime? OfficialDate { get; set; }
    public string CurrentContractType { get; set; } = "";
    public DateTime? ContractExpirationDate { get; set; }
    public string WorkStatusId { get; set; } = "";
    public decimal CurrentBasicSalary { get; set; }
    public string BankAccountNumber { get; set; } = "";
    public string BankName { get; set; } = "";
    public string SalaryPaymentMethod { get; set; } = "";
    public int TaxDependentsCount { get; set; }
}

public partial class NhanSu_EmployeeProfile_Log : ModelChung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime? DateOfBirth { get; set; }
    public string IdentityCardNumber { get; set; } = "";
    public DateTime? IdentityCardIssueDate { get; set; }
    public string IdentityCardIssuePlace { get; set; } = "";
    public string TaxCode { get; set; } = "";
    public string SocialInsuranceNumber { get; set; } = "";
    public string MobilePhone { get; set; } = "";
    public string PersonalEmail { get; set; } = "";
    public string PermanentAddress { get; set; } = "";
    public string CurrentAddress { get; set; } = "";
    public string EmergencyContactName { get; set; } = "";
    public string EmergencyContactPhone { get; set; } = "";
    public string DepartmentFrId { get; set; } = "";
    public string ChucVuId { get; set; } = "";
    public string ManagerId { get; set; } = "";
    public string WorkEmail { get; set; } = "";
    public DateTime? HireDate { get; set; }
    public DateTime? OfficialDate { get; set; }
    public string CurrentContractType { get; set; } = "";
    public DateTime? ContractExpirationDate { get; set; }
    public string WorkStatusId { get; set; } = "";
    public decimal CurrentBasicSalary { get; set; }
    public string BankAccountNumber { get; set; } = "";
    public string BankName { get; set; } = "";
    public string SalaryPaymentMethod { get; set; } = "";
    public int TaxDependentsCount { get; set; }
}
