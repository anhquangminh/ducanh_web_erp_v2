using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.NhanSu;
public partial class DM_ChucVu
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập chức vụ")]
    public string ChucVu { get; set; } = "";
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
}
public partial class DM_ChucVu_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ChucVu { get; set; } = "";
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
    public string IdChung { get; set; } = "";
    public bool IsValid { get; set; } = false;
}