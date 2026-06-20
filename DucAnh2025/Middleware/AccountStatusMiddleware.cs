using DucAnh2025.Repository;

public class AccountStatusMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccountStatusMiddleware> _logger;

    public AccountStatusMiddleware(RequestDelegate next, ILogger<AccountStatusMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationUserRepository userRepo)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userName = context.User.Identity.Name;

            var appUser = userRepo.GetByUserName(userName);

            _logger.LogInformation("AccountStatusMiddleware: userName={UserName}, appUser={AppUser}",
                userName, appUser == null ? "null" : $"Id={appUser.Id}, IsActive={appUser.IsActive}");

            if (appUser == null || appUser.IsActive == 0)
            {
                context.Session.Clear();

                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Account is inactive or locked.",
                        code = "ACCOUNT_INACTIVE"
                    });
                    return;
                }

                context.Response.Redirect("/Account/Login");
                return;
            }
        }
        else if (context.Request.Path.StartsWithSegments("/api/chat"))
        {
            _logger.LogWarning("AccountStatusMiddleware: unauthenticated chat API request. Path={Path}, HasCookie={HasCookie}",
                context.Request.Path,
                context.Request.Headers.ContainsKey("Cookie"));
        }

        await _next(context);
    }
}
