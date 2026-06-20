using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class ApprovalRequest
    {
        [Key]
        public string Id { get; set; }
        public string ApprovalId { get; set; }
        public string CurrentId { get; set; }
        public string LogId { get; set; }
        public string RevertId { get; set; }
        public int ApprovalStep { get; set; }
        public string CompanyId { get; set; }
        public string DeptId { get; set; }
        public string MajorId { get; set; }
        public string ScreenId { get; set; }
        public int OperationType { get; set; }
        public int ApprovalSts { get; set; }
        public string Content { get; set; }
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public int IsActive { get; set; }
    }
}
