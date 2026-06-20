using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models
{
    public class ApprovalTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int IsActive { get; set; } = 0;

        public string OriginalId { get; set; } = "";
        public string RelatedTable { get; set; } = "";

        public string ParentMajorId { get; set; } = "";
        public string MajorId { get; set; } = "";

        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; } = 0;

        public string? ApprovalUserId { get; set; } = "";
        public DateTime? DateApproval { get; set; } = DateTime.Now;
        public string? ApprovalDept { get; set; } = "";
        public string? DepartmentId { get; set; } = "";
        public int? DepartmentOrder { get; set; } =0;
        public int? ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; } = "";
        public string? LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";

        public DateTime? CreateAt { get; set; }= DateTime.Now;
        public string CreateBy { get; set; } = "";
    }
    public class ApprovalTaskModel
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int IsActive { get; set; } = 0;

        public string OriginalId { get; set; } = "";
        public string RelatedTable { get; set; } = "";

        public string ParentMajorId { get; set; } = "";
        public string ParentName { get; set; } = "";
        public string MajorId { get; set; } = "";
        public string MajorName { get; set; } = "";

        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; } = 0;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.MinValue;
        public string? ApprovalDept { get; set; }
        public string? DepartmentId { get; set; } = "";
        public int? DepartmentOrder { get; set; } = 0;
        public int? ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; } = "";
        public string? LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";

        public DateTime? CreateAt { get; set; } = DateTime.MinValue;
        public string CreateBy { get; set; } = "";
    }

    public class ApprovalTask_Log
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int IsActive { get; set; } = 0;

        public string OriginalId { get; set; } = "";
        public string RelatedTable { get; set; } = "";

        public string ParentMajorId { get; set; } = "";
        public string MajorId { get; set; } = "";

        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; } = 0;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.MinValue;
        public string? ApprovalDept { get; set; }
        public string? DepartmentId { get; set; } = "";
        public int? DepartmentOrder { get; set; } = 0;
        public int? ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; } = "";
        public string? LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";

        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "";

        public string IdChung { get; set; } = "";
        public bool IsValid { get; set; } = false;
    }

    }
