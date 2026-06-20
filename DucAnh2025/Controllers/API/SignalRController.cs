using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DucAnh2025.Services.SignalR;

[Route("api/[controller]")]
[ApiController]
public class SignalRController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendTestNotification([FromBody] TestNotificationModel model)
    {
        // Gửi thông báo realtime tới userId
        await _hubContext.Clients.User(model.UserId).SendAsync("ReceiveNotification", new
        {
            title = model.Title,
            body = model.Body
        });
        return Ok(new { success = true });
    }

    [HttpPost("send-multi")]
    public async Task<IActionResult> SendTestNotificationMulti([FromBody] TestNotificationMultiModel model)
    {
        if (model.UserIds == null || !model.UserIds.Any())
            return BadRequest(new { success = false, message = "Danh sách UserId rỗng." });

        foreach (var userId in model.UserIds)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                title = model.Title,
                body = model.Body
            });
        }
        return Ok(new { success = true });
    }

}

public class TestNotificationModel
{
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public class TestNotificationMultiModel
{
    public List<string> UserIds { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}