using DucAnh2025.SeedWork;

public partial class ApprovalControlModel : PagingParameters
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CompanyId { get; set; }
    public string ParentMajorId { get; set; }
    public string MajorId { get; set; }
    public string DeptId { get; set; }
    public string UserId { get; set; }
    public string PermissionId { get; set; } = "";
    public string ApprovalStepId { get; set; } = "";
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
    public bool IsValid { get; set; } = false;
}

