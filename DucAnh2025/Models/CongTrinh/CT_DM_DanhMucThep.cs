
using System.ComponentModel.DataAnnotations;

public class CT_DM_DanhMucThep
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải chọn tên loại thép")]
    public string Id_TenLoaiThep { get; set; } = "";

    [Required(ErrorMessage = "Bạn phải nhập đường kính")]
    public string DuongKinh { get; set; } = "";

    [Required(ErrorMessage = "Bạn phải nhập trọng lượng")]
    public double TrongLuong { get; set; } = 0;

    [Required(ErrorMessage = "Bạn phải nhập đơn vị")]
    public string DonVi { get; set; } = "";

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

public partial class CT_DM_DanhMucThep_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Id_TenLoaiThep { get; set; } = "";
    public string DuongKinh { get; set; } = "";
    public double TrongLuong { get; set; } = 0;
    public string DonVi { get; set; } = "";

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
