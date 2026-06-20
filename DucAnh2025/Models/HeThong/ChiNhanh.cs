using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models.HeThong;
public partial class ChiNhanh
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập thuộc tổ chức")]
    public string ParentId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập tên chi nhánh")]
    public string TenChiNhanh { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập loại chi nhánh")]
    public string CompanyType { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập điện thoại")]
    public string Phone { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập email")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập địa chỉ")]
    public string Address { get; set; } = "";
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
public partial class ChiNhanh_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ParentId { get; set; } = "";
    public string TenChiNhanh { get; set; } = "";
    public string CompanyType { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
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
