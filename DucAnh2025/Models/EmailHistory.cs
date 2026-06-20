using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models
{
    public class EmailHistory
    {
        [Key]
        public string Id { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string? CompanyId { get; set; }
        public string? UserId { get; set; }
        public string? ParentMajorId { get; set; }
        public string? MajorId { get; set; }
        public string? IdCheck { get; set; }
        public string? IdLog { get; set; }
        public bool IsMail { get; set; } = false;
        public bool IsNotification { get; set; } = false;
        public bool IsSMS { get; set; } = false;
        public string GroupId { get; set; }
        public DateTime CreateAt { get; set; }
        public string? CreateBy { get; set; }
        public int IsRead { get; set; }
    }
}
