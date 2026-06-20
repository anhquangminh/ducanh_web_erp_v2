using DucAnh2025.SeedWork;
namespace DucAnh2025.ViewModels.Accounts;
public partial class ApplicationUserModel : PagingParameters
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Dob { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string CompanyId { get; set; }
    public string DeptId { get; set; }
    public int IsFirstLogin { get; set; }
    public string GroupId { get; set; }
    public int Ordinarily { get; set; }
    public DateTime CreateAt { get; set; }
    public string CreateBy { get; set; }
    public int IsActive { get; set; }
    public string ApprovalUserId { get; set; }
    public DateTime DateApproval { get; set; }
    public string DepartmentId { get; set; }
    public int DepartmentOrder { get; set; }
    public int ApprovalOrder { get; set; }
    public string ApprovalId { get; set; }
    public string LastApprovalId { get; set; }
    public string IsStatus { get; set; }
}
