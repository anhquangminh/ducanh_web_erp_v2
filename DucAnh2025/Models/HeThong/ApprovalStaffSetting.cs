using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class ApprovalStaffSetting
    {
        [Key]
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string DeptId { get; set; }
        public string UserId { get; set; }
        public string ParentMajorId { get; set; }
        public string MajorId { get; set; }
        public string ApprovalStepId { get; set; }
        public string Content { get; set; }
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; }
        public string CreateBy { get; set; }
        public int IsActive { get; set; }
    }
}
