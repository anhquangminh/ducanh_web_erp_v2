namespace DucAnh2025.Models
{
    public class UserSession
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime LastActive { get; set; }
        public int IsActive { get; set; }

    }
}
