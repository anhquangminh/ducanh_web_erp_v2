using System.ComponentModel.DataAnnotations;
namespace DucAnh2025.Models.NhanSu;
public partial class MCompany
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập thuộc tổ chức")]
    public string? ParentId { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập tên công ty")]
    public string? CompanyName { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập loại")]
    public int? CompanyType { get; set; } = 0;
    [Required(ErrorMessage = "Bạn phải nhập số điện thoại")]
    public string? Phone { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập email")]
    public string? Email { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập địa chỉ")]
    public string? Address { get; set; } = "";
    [Required(ErrorMessage = "Bạn phải nhập groupId")]
    public string? GroupId { get; set; } = "";

    public DateTime? CreateAt { get; set; } = DateTime.Now;

    public string? CreateBy { get; set; } = "";

    public int? IsActive { get; set; } = 0;
    [Required(ErrorMessage = "Bạn phải nhập người duyệt")]
    public string? UserId { get; set; } = "";

    public DateTime? DateApproval { get; set; } = DateTime.Now;

    public string? IdDepartment { get; set; } = "";

    public int? ApprovalOrder { get; set; } = 0;
}