using Microsoft.AspNetCore.SignalR;

namespace DucAnh2025.Services.SignalR
{
    public class DataUpdateHub : Hub
    {
        public async Task NotifyDataChanged()
        {
            // Gửi thông báo đến tất cả các client
            await Clients.All.SendAsync("ReceiveDataUpdate");
        }
    }
}
