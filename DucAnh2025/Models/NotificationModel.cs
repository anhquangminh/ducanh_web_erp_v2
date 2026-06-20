namespace DucAnh2025.Models
{
    public class NotificationModel : EmailHistory
    {
        public string MajorName { get; set; }
        public string ParentName { get; set; }
    }
    public class NotificationLog
    {
        public int Id { get; set; }
        public string TargetToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ResponseStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetPage { get; set; } = "";
        public string TargetId { get; set; } = "";
    }

    public class NotificationFireBase
    {
        public string Id { get; set; } = new Guid().ToString();
        public string ReciverId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int IsRead { get; set; } = 0;
        public string Creatby { get; set; } = "system";
        public int IsActive { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public string? TargetPage { get; set; } = "";
        public string? TargetId { get; set; } = "";
    }
}
