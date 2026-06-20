using System.ComponentModel.DataAnnotations;

public partial class ApprovalControl
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập chi nhánh")]
    public string CompanyId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập nghiệp vụ cha")]
    public string ParentMajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập nghiệp vụ con")]
    public string MajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập phòng ban")]
    public string DeptId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập người dùng")]
    public string UserId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập loại quyền")]
    public string PermissionId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập lượt duyệt")]
    public string ApprovalStepId { get; set; } = "";
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
public partial class ApprovalControl_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CompanyId { get; set; } = "";
    public string ParentMajorId { get; set; } = "";
    public string MajorId { get; set; } = "";
    public string DeptId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string PermissionId { get; set; } = "";
    public string ApprovalStepId { get; set; } = "";
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

