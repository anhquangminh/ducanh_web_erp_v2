using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.Accounts
{
    public partial class ApplicationUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Bạn phải nhập tên đăng nhập")]
        public string UserName { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập họ")]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập tên")]
        public string LastName { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập ngày sinh")]
        public DateTime Dob { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Bạn phải nhập số điện thoại")]
        public string PhoneNumber { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập email")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập địa chỉ")]
        public string Address { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập chi nhánh")]
        public string CompanyId { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập phòng ban")]
        public string DeptId { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập isFirstLogin")]
        public int IsFirstLogin { get; set; } = 0;
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
    public partial class ApplicationUser_Log
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime Dob { get; set; } = DateTime.Now;
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string CompanyId { get; set; } = "";
        public string DeptId { get; set; } = "";
        public int IsFirstLogin { get; set; } = 0;
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
    }

}
