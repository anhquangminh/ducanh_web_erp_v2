using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class MajorUserApproval
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Bạn phải chọn chi nhánh!")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Bạn phải chọn nghiệp vụ!")]
        public string ParentMajorId { get; set; }
        [Required(ErrorMessage = "Bạn phải chọn phân loại nghiệp vụ!")]
        public string MajorId { get; set; }
        [Required(ErrorMessage = "Bạn phải chọn phòng ban!")]
        public string DeptId { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn người dùng!")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn loại quyền")]
        //[StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Bạn phải chọn loại quyền")]
        public string PermissionId { get; set; } = "";
        public string ApprovalStepId { get; set; } = "";

        //[Required(ErrorMessage = "Bạn phải chọn thứ")]
        //[StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Bạn phải chọn thứ")]
        public string DayinWeek { get; set; }
        public string IdMain { get; set; } = "";
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
        public int IsActive { get; set; } = 0;
        public string ApprovalUserId { get; set; } = "";
        public DateTime DateApproval { get; set; } = DateTime.Now;
        public string DepartmentId { get; set; } = "";
        public int DepartmentOrder { get; set; } = 0;
        public string ApprovalId { get; set; } = "";
        public int ApprovalOrder { get; set; } = 0;
        public string LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";
    }

    public class MajorUserApprovalDetail
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CompanyId { get; set; }
        public string ParentMajorId { get; set; }
        public string MajorId { get; set; }
        public string DeptId { get; set; }
        public string UserId { get; set; }
        public string ApprovalId { get; set; }
        public string ApprovalStepId { get; set; }
        public string IdMain { get; set; }
        public string DayinWeek { get; set; }
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
        public int IsActive { get; set; } = 0;
        public string ApprovalUserId { get; set; } = "";
        public DateTime DateApproval { get; set; } = DateTime.Now;
        public string DepartmentId { get; set; } = "";
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";
    }
    public class MajorUserApproval_Log
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CompanyId { get; set; } = "";
        public string ParentMajorId { get; set; } = "";
        public string MajorId { get; set; } = "";
        public string DeptId { get; set; } = "";
        public string UserId { get; set; } = "";
        public string PermissionId { get; set; } = "";
        public string ApprovalStepId { get; set; } = "";
        public string IdMain { get; set; } = "";
        public string DayinWeek { get; set; } = "";
        public string GroupId { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
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
}
