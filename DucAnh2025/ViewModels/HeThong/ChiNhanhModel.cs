using DucAnh2025.SeedWork;

public partial class ChiNhanhModel : PagingParameters
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ParentId { get; set; } = "";
    public string TenChiNhanh { get; set; } = "";
    public string CompanyType { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; }
    public DateTime? CreateAt { get; set; }
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; }
    public string ApprovalUserId { get; set; } = "";
    public DateTime? DateApproval { get; set; }
    public string ApprovalDept { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public int DepartmentOrder { get; set; }
    public int ApprovalOrder { get; set; }
    public string ApprovalId { get; set; } = "";
    public string LastApprovalId { get; set; } = "";
    public string IsStatus { get; set; } = "";
}