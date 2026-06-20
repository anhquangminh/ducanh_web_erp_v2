using Microsoft.AspNetCore.SignalR;
using DucAnh2025.Services.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DucAnh2025.Services
{
    public class SignalRNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // Gửi cho 1 user
        public async Task SendToUserAsync(string userId, string title, string body)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                title,
                body
            });
        }

        // Gửi cho nhiều user
        public async Task SendToUsersAsync(IEnumerable<string> userIds, string title, string body)
        {
            var tasks = new List<Task>();
            foreach (var userId in userIds)
            {
                tasks.Add(
                    _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                    {
                        title,
                        body
                    })
                );
            }
            await Task.WhenAll(tasks);
        }
    }
}