using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong;

public partial class ApprovalStepSetting
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập idMain")]
    public string IdMain { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập chi nhánh")]
    public string CompanyId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập phòng ban")]
    public string DeptId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập thứ tự phòng ban")]
    public int DeptOrder { get; set; } = 0;
    [Required(ErrorMessage = "Bạn phải nhập nhóm nghiệp vụ")]
    public string ParentMajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập nghiệp vụ")]
    public string MajorId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập quyền")]
    public string PermissionId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập loại duyệt")]
    public string Content { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập thứ tự duyệt")]
    public int ApprovalStep { get; set; } = 0;
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
public partial class ApprovalStepSetting_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string IdMain { get; set; } = "";
    public string CompanyId { get; set; } = "";
    public string DeptId { get; set; } = "";
    public int DeptOrder { get; set; } = 0;
    public string ParentMajorId { get; set; } = "";
    public string MajorId { get; set; } = "";
    public string PermissionId { get; set; } = "";
    public string Content { get; set; } = "";
    public int ApprovalStep { get; set; } = 0;
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
public class ApprovalStepWrapper
{
    public EditContext EditContext { get; set; }
    public ApprovalStepSetting ApprovalRow { get; set; }
    public bool isDuplicate { get; set; } = true;

    public ApprovalStepWrapper(ApprovalStepSetting approvalRow)
    {
        ApprovalRow = approvalRow;
        EditContext = new EditContext(approvalRow);
    }
}

