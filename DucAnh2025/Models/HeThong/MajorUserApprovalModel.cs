using DucAnh2025.SeedWork;

namespace DucAnh2025.Models.HeThong
{
    public class MajorUserApprovalModel : PagingParameters
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ParentMajorId { get; set; }
        public string ParentMajorName { get; set; }
        public string MajorId { get; set; }
        public string MajorName { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PermissionId { get; set; }
        public string ApprovalStepId { get; set; }
        public string ApprovalName { get; set; }
        public string IdMain { get; set; }
        public string DayinWeek { get; set; }
        public string DayInWeekText { get; set; }
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
        public int IsActive { get; set; } = 0;
        public string ApprovalUserId { get; set; } = "";
        public DateTime DateApproval { get; set; } = DateTime.Now;
        public string DepartmentId { get; set; } = "";
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string ApprovalId { get; set; }
        public string LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";
    }
}
