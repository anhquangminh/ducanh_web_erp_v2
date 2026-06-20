using DucAnh2025.Models.Accounts;
using DucAnh2025.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DucAnh2025.Controllers.API
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ApiController]
    [Route("api/chat")]
    public class ChatProxyController : ControllerBase
    {
        private static readonly HashSet<string> HopByHopHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            "Connection", "Keep-Alive", "Proxy-Authenticate", "Proxy-Authorization",
            "TE", "Trailer", "Transfer-Encoding", "Upgrade", "Host"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ILogger<ChatProxyController> _logger;

        public ChatProxyController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IApplicationUserRepository applicationUserRepository,
            ILogger<ChatProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _applicationUserRepository = applicationUserRepository;
            _logger = logger;
        }

        [HttpGet("session")]
        public IActionResult Session()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("Chat session unauthorized: unauthenticated request. HasCookie={HasCookie}",
                    Request.Headers.ContainsKey("Cookie"));
                return Unauthorized(new { success = false, message = "Unauthorized", code = "AUTH_REQUIRED" });
            }

            var userName = User.Identity.Name
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue("sub")
                ?? string.Empty;
            var appUser = _applicationUserRepository.GetByUserName(userName);

            if (appUser == null || appUser.IsActive == 0)
            {
                _logger.LogWarning("Chat session forbidden: inactive account. UserName={UserName}", userName);
                return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = "Account is inactive or locked.", code = "ACCOUNT_INACTIVE" });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = appUser.Id,
                    userName = appUser.UserName,
                    groupId = appUser.GroupId,
                    companyId = appUser.CompanyId,
                    departmentId = !string.IsNullOrWhiteSpace(appUser.DepartmentId) ? appUser.DepartmentId : appUser.DeptId,
                    name = $"{appUser.FirstName} {appUser.LastName}".Trim(),
                    roles = User.Claims
                        .Where(c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                        .Select(c => c.Value)
                        .Distinct()
                        .ToArray()
                }
            });
        }

        //[Route("{**path}")]
        //public async Task<IActionResult> Proxy(string? path)
        //{
        //    if (User.Identity?.IsAuthenticated != true)
        //    {
        //        _logger.LogWarning("Chat proxy unauthorized: unauthenticated request. Path={Path}", Request.Path);
        //        return Unauthorized(new { success = false, message = "Unauthorized", code = "AUTH_REQUIRED" });
        //    }

        //    var userName = User.Identity.Name
        //        ?? User.FindFirstValue(ClaimTypes.Name)
        //        ?? User.FindFirstValue("sub")
        //        ?? string.Empty;

        //    var appUser = _applicationUserRepository.GetByUserName(userName);
        //    if (appUser == null || appUser.IsActive == 0)
        //    {
        //        _logger.LogWarning("Chat proxy forbidden: inactive account. UserName={UserName}, Exists={Exists}, IsActive={IsActive}",
        //            userName, appUser != null, appUser?.IsActive);
        //        return StatusCode(StatusCodes.Status403Forbidden, new { success = false, message = "Account is inactive or locked.", code = "ACCOUNT_INACTIVE" });
        //    }

        //    _logger.LogInformation("Chat proxy authenticated. UserId={UserId}, UserName={UserName}, Claims={Claims}, Path={Path}",
        //        appUser.Id,
        //        appUser.UserName,
        //        User.Claims.Select(c => new { c.Type, c.Value }),
        //        Request.Path);

        //    var client = _httpClientFactory.CreateClient("ChatService");
        //    var query = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty;
        //    var targetPath = string.IsNullOrWhiteSpace(path) ? string.Empty : path.TrimStart('/');
        //    using var upstreamRequest = new HttpRequestMessage(new HttpMethod(Request.Method), $"/api/chat/{targetPath}{query}");

        //    AddForwardedUserHeaders(upstreamRequest, appUser);
        //    CopyRequestHeaders(upstreamRequest);

        //    if (Request.ContentLength.GetValueOrDefault() > 0 || Request.HasFormContentType)
        //    {
        //        upstreamRequest.Content = new StreamContent(Request.Body);
        //        if (!string.IsNullOrWhiteSpace(Request.ContentType))
        //        {
        //            upstreamRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(Request.ContentType);
        //        }
        //    }

        //    using var upstreamResponse = await client.SendAsync(upstreamRequest, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);
        //    var body = await upstreamResponse.Content.ReadAsByteArrayAsync(HttpContext.RequestAborted);

        //    foreach (var header in upstreamResponse.Headers)
        //    {
        //        if (!HopByHopHeaders.Contains(header.Key)) Response.Headers[header.Key] = header.Value.ToArray();
        //    }

        //    foreach (var header in upstreamResponse.Content.Headers)
        //    {
        //        if (!HopByHopHeaders.Contains(header.Key) && !string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
        //        {
        //            Response.Headers[header.Key] = header.Value.ToArray();
        //        }
        //    }

        //    var contentType = upstreamResponse.Content.Headers.ContentType?.ToString() ?? "application/json";
        //    Response.StatusCode = (int)upstreamResponse.StatusCode;
        //    return File(body, contentType);
        //}

        [Route("{**path}")]
        public async Task<IActionResult> Proxy(string? path)
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    _logger.LogWarning(
                        "Chat proxy unauthorized: unauthenticated request. Path={Path}",
                        Request.Path
                    );

                    return Unauthorized(new
                    {
                        success = false,
                        message = "Unauthorized",
                        code = "AUTH_REQUIRED"
                    });
                }

                var userName = User.Identity.Name
                    ?? User.FindFirstValue(ClaimTypes.Name)
                    ?? User.FindFirstValue("sub")
                    ?? string.Empty;

                var appUser = _applicationUserRepository.GetByUserName(userName);

                if (appUser == null || appUser.IsActive == 0)
                {
                    _logger.LogWarning(
                        "Chat proxy forbidden: inactive account. UserName={UserName}, Exists={Exists}, IsActive={IsActive}",
                        userName,
                        appUser != null,
                        appUser?.IsActive
                    );

                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        success = false,
                        message = "Account is inactive or locked.",
                        code = "ACCOUNT_INACTIVE"
                    });
                }

                _logger.LogInformation(
                    "Chat proxy authenticated. UserId={UserId}, UserName={UserName}, Path={Path}",
                    appUser.Id,
                    appUser.UserName,
                    Request.Path
                );

                var client = _httpClientFactory.CreateClient("ChatService");

                var query = Request.QueryString.HasValue
                    ? Request.QueryString.Value
                    : string.Empty;

                var targetPath = string.IsNullOrWhiteSpace(path)
                    ? string.Empty
                    : path.TrimStart('/');

                var targetUrl = $"/api/chat/{targetPath}{query}";

                _logger.LogInformation(
                    "Forwarding request to chat service. Method={Method}, Url={Url}",
                    Request.Method,
                    targetUrl
                );

                using var upstreamRequest = new HttpRequestMessage(
                    new HttpMethod(Request.Method),
                    targetUrl
                );

                AddForwardedUserHeaders(upstreamRequest, appUser, User);
                CopyRequestHeaders(upstreamRequest);

                _logger.LogInformation(
                    "Forward headers: {Headers}",
                    upstreamRequest.Headers.ToDictionary(
                        h => h.Key,
                        h => string.Join(",", h.Value)
                    )
                );

                if (Request.ContentLength.GetValueOrDefault() > 0 || Request.HasFormContentType)
                {
                    upstreamRequest.Content = new StreamContent(Request.Body);

                    if (!string.IsNullOrWhiteSpace(Request.ContentType))
                    {
                        upstreamRequest.Content.Headers.ContentType =
                            MediaTypeHeaderValue.Parse(Request.ContentType);
                    }

                    _logger.LogInformation(
                        "Request has body. ContentType={ContentType}, Length={Length}",
                        Request.ContentType,
                        Request.ContentLength
                    );
                }

                using var upstreamResponse = await client.SendAsync(
                    upstreamRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    HttpContext.RequestAborted
                );

                _logger.LogInformation(
                    "Chat service response received. StatusCode={StatusCode}",
                    (int)upstreamResponse.StatusCode
                );

                var body = await upstreamResponse.Content.ReadAsByteArrayAsync(
                    HttpContext.RequestAborted
                );

                foreach (var header in upstreamResponse.Headers)
                {
                    if (!HopByHopHeaders.Contains(header.Key))
                    {
                        Response.Headers[header.Key] = header.Value.ToArray();
                    }
                }

                foreach (var header in upstreamResponse.Content.Headers)
                {
                    if (
                        !HopByHopHeaders.Contains(header.Key)
                        && !string.Equals(
                            header.Key,
                            "Content-Type",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        Response.Headers[header.Key] = header.Value.ToArray();
                    }
                }

                var contentType =
                    upstreamResponse.Content.Headers.ContentType?.ToString()
                    ?? "application/json";

                Response.StatusCode = (int)upstreamResponse.StatusCode;

                return File(body, contentType);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,
                    "Chat proxy timeout/cancelled. Path={Path}, Method={Method}",
                    Request.Path,
                    Request.Method
                );

                return StatusCode(StatusCodes.Status504GatewayTimeout, new
                {
                    success = false,
                    message = "Chat service timeout",
                    error = ex.Message
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "Chat proxy HTTP error. Path={Path}, Method={Method}",
                    Request.Path,
                    Request.Method
                );

                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    success = false,
                    message = "Cannot connect to chat service",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Chat proxy unexpected error. Path={Path}, Method={Method}",
                    Request.Path,
                    Request.Method
                );

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Internal proxy error",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private void AddForwardedUserHeaders(HttpRequestMessage request, ApplicationUser appUser, ClaimsPrincipal principal)
        {
            var secret = _configuration["ChatService:InternalSecret"] ?? "";
            var roles = principal.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .Distinct();
            request.Headers.Add("X-Chat-Internal-Secret", secret);
            request.Headers.Add("X-Chat-User-Id", appUser.Id);
            request.Headers.Add("X-Chat-User-Name", appUser.UserName);
            request.Headers.Add("X-Chat-User-First-Name", EncodeHeader(appUser.FirstName) ?? "");
            request.Headers.Add("X-Chat-User-Last-Name", EncodeHeader(appUser.LastName) ?? "");
            request.Headers.Add("X-Chat-User-Email", appUser.Email ?? appUser.UserName);
            request.Headers.Add("X-Chat-Company-Id", appUser.CompanyId ?? "");
            request.Headers.Add("X-Chat-Group-Id", appUser.GroupId ?? "");
            request.Headers.Add("X-Chat-Department-Id", !string.IsNullOrWhiteSpace(appUser.DepartmentId) ? appUser.DepartmentId : appUser.DeptId ?? "");
            request.Headers.Add("X-Chat-Roles", string.Join(",", roles));
        }

        private void CopyRequestHeaders(HttpRequestMessage request)
        {
            foreach (var header in Request.Headers)
            {
                if (HopByHopHeaders.Contains(header.Key)) continue;
                if (header.Key.StartsWith("X-Chat-", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(header.Key, "Authorization", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(header.Key, "Cookie", StringComparison.OrdinalIgnoreCase)) continue;
                request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private static string EncodeHeader(string value)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(value)
            );
        }
    }
}
