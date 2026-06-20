namespace DucAnh2025.Models
{
    public class UserFcmToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
    public class NotificationRequest
    {
        public List<string> UserIds { get; set; } = new();
        public string Title { get; set; }
        public string Body { get; set; }
        public string targetPage { get; set; } = "";
        public string targetId { get; set; } = "";
    }
}
