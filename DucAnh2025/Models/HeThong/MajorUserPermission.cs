using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong;
public partial class MajorUserPermission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập chi nhánh")]
    public string CompanyId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập nghiệp vụ cha")]
    public string ParentMajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập nghiệp vụ")]
    public string MajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập người dùng")]
    public string UserId { get; set; } = "";
    public string PermissionId { get; set; } = "";
    public int DayInWeek { get; set; } = 7;
    //[Required(ErrorMessage = "Bạn phải nhập idMain")]
    public string IdMain { get; set; } = "";
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string ApprovalUserId { get; set; } = "";
    public DateTime DateApproval { get; set; } = DateTime.Now;
    public string DepartmentId { get; set; } = "";
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string ApprovalId { get; set; } = "";
    public string LastApprovalId { get; set; } = "";
    public string IsStatus { get; set; } = "";
}
public partial class MajorUserPermission_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CompanyId { get; set; } = "";
    public string ParentMajorId { get; set; } = "";
    public string MajorId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string PermissionId { get; set; } = "";
    public int DayInWeek { get; set; } = 0;
    public string IdMain { get; set; } = "";
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string ApprovalUserId { get; set; } = "";
    public DateTime DateApproval { get; set; } = DateTime.Now;
    public string DepartmentId { get; set; } = "";
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string ApprovalId { get; set; } = "";
    public string LastApprovalId { get; set; } = "";
    public string IsStatus { get; set; } = "";
    public string IdChung { get; set; } = "";
    public bool IsValid { get; set; } = false;
}

