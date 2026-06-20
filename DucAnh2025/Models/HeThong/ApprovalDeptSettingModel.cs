using DucAnh2025.SeedWork;

public partial class ApprovalDeptSettingModel : PagingParameters
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string IdMain { get; set; } = "";
    public string CompanyId { get; set; } = "";
    public string DeptId { get; set; } = "";
    public string ParentMajorId { get; set; } = "";
    public string MajorId { get; set; } = "";
    public int ApprovalStep { get; set; }
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; }
    public DateTime? CreateAt { get; set; }
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; }
    public string ApprovalUserId { get; set; } = "";
    public DateTime? DateApproval { get; set; }
    public string DepartmentId { get; set; } = "";
    public int DepartmentOrder { get; set; }
    public int ApprovalOrder { get; set; }
    public string ApprovalId { get; set; } = "";
    public string LastApprovalId { get; set; } = "";
    public string IsStatus { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string MajorName { get; set; } = "";
    public string ParentName { get; set; } = "";
    public string DeptName { get; set; } = "";
}

public partial class ApprovalDeptSettingData : PagingParameters
{
    public string IdMain { get; set; }
    public string CompanyId { get; set; }
    public string DeptId { get; set; }
    public string ParentMajorId { get; set; }
    public string MajorId { get; set; }

}
