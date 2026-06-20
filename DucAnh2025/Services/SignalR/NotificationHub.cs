using Microsoft.AspNetCore.SignalR;

namespace DucAnh2025.Services.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
