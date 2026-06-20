using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DucAnh2025.Controllers.API
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class FcmController : ControllerBase
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly FirebaseNotificationService _firebaseNotificationService;
        private readonly IEmailHistoryRepository _emailHistoryService;

        public FcmController(IDbContextFactory<ApplicationDbContext> context, IApplicationUserRepository applicationUserService, FirebaseNotificationService firebaseNotificationService, IEmailHistoryRepository emailHistoryService)
        {
            _context = context;
            _applicationUserService = applicationUserService;
            _firebaseNotificationService = firebaseNotificationService;
            _emailHistoryService = emailHistoryService;
        }

        [HttpPost("register-token")]
        public async Task<IActionResult> RegisterFcmToken([FromBody] UserFcmToken fcmToken)
        {
            using var context = _context.CreateDbContext();

            if (string.IsNullOrEmpty(fcmToken.UserId) || string.IsNullOrEmpty(fcmToken.Token))
                return BadRequest("UserId hoặc Token không hợp lệ.");

            // Nếu là username dạng email thì lấy Id
            if (Regex.IsMatch(fcmToken.UserId, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                var user = _applicationUserService.GetByUserName(fcmToken.UserId);
                if (user == null)
                    return BadRequest(new ApiResponse<UserFcmToken>(false, "UserId hoặc Token không hợp lệ.", null));

                fcmToken.UserId = user.Id;
            }

            // Xóa token đã tồn tại (nếu có)
            var existingToken = await context.UserFcmTokens
                .FirstOrDefaultAsync(t => t.Token == fcmToken.Token);

            if (existingToken != null)
            {
                context.UserFcmTokens.Remove(existingToken);
            }

            // Thêm mới
            fcmToken.CreatedAt = DateTime.UtcNow;
            fcmToken.IsActive = true;
            context.UserFcmTokens.Add(fcmToken);

            await context.SaveChangesAsync();
            return Ok(new ApiResponse<UserFcmToken>(true, "Thành công", fcmToken));
        }

        [HttpPost("unregister-token")]
        public async Task<IActionResult> UnregisterFcmToken([FromBody] UserFcmToken fcmToken)
        {
            using var context = _context.CreateDbContext();

            if (string.IsNullOrEmpty(fcmToken.UserId) || string.IsNullOrEmpty(fcmToken.Token))
                return BadRequest("UserId hoặc Token không hợp lệ.");

            // Tìm token đúng user
            var existingToken = await context.UserFcmTokens
                .FirstOrDefaultAsync(t => t.Token == fcmToken.Token || (t.UserId == fcmToken.UserId && t.GroupId == fcmToken.GroupId));


            if (existingToken == null)
                return NotFound(new ApiResponse<UserFcmToken>(false, "Không tìm thấy Token để xóa.", null));

            // Xóa token
            context.UserFcmTokens.Remove(existingToken);
            await context.SaveChangesAsync();

            return Ok(new ApiResponse<UserFcmToken>(true, "Xóa Token thành công.", null));
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotificationToUsers([FromBody] NotificationRequest request)
        {
            using var context = _context.CreateDbContext();

            if (request.UserIds == null || !request.UserIds.Any())
                return BadRequest(new ApiResponse<NotificationRequest>(false, "Danh sách UserId rỗng.", null));

            var realUserIds = new List<string>();

            foreach (var userId in request.UserIds)
            {
                if (Regex.IsMatch(userId, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                {
                    var user = _applicationUserService.GetByUserName(userId);
                    if (user == null)
                        return BadRequest(new ApiResponse<NotificationRequest>(false, "Không tìm thấy user tương ứng với " + userId, null));
                    realUserIds.Add(user.Id);
                }
                else
                {
                    realUserIds.Add(userId);
                }
            }

            // Lấy danh sách token theo danh sách user đã xử lý
            var tokens = await context.UserFcmTokens
                .Where(t => realUserIds.Contains(t.UserId) && t.IsActive)
                .Select(t => t.Token)
                .ToListAsync();

            if (!tokens.Any())
                return BadRequest("Không tìm thấy token hợp lệ.");
            foreach (var token in tokens)
            {
                await _firebaseNotificationService.SendNotificationAsync(token, request.Title, request.Body, request.targetPage, request.targetId);
            }
            return Ok(new ApiResponse<NotificationRequest>(true, "Đã gửi thông báo", request));
        }

        //EmailHistories
        [HttpGet("GetAllNotiByUserId")]
        public async Task<ActionResult<ApiResponse<List<NotificationModel>>>> GetAllNotiByUserId(string userId, int currentPage = 0, int pageSize = 10)
        {
            try
            {
                var notifis = await _emailHistoryService.GetAllNotiByUser(userId, currentPage * pageSize, pageSize);
                return Ok(new ApiResponse<List<NotificationModel>>(true, "Thành công", notifis));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<List<NotificationModel>>(false, "Lỗi " + ex.Message, null));
            }

        }

        [HttpGet("GetUnreadNotiByUserId")]
        public async Task<IActionResult> GetUnreadNotiByUserId(string Id)
        {
            try
            {
                var count = await _emailHistoryService.GetUnreadNotiByUser(Id);
                return Ok(new ApiResponse<int>(true, "Thành công", count));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>(false, "Lỗi " + ex.Message, 0));
            }

        }
        [HttpGet("GetAllCategoriesByUserId")]
        public async Task<ActionResult<ApiResponse<List<(string, string, int)>>>> GetAllCategoriesByUserId(string userId)
        {
            try
            {
                var notifis = await _emailHistoryService.GetAllCategoriesByUser(userId);
                return Ok(new ApiResponse<List<(string, string, int)>>(true, "Thành công", notifis));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<List<(string, string, int)>>(false, "Lỗi " + ex.Message, null));
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] NotificationModel notificationModel)
        {
            try
            {
                await _emailHistoryService.GetById(notificationModel.Id);
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<NotificationModel>(false, "Dữ liệu không hợp lệ", null));
                }
                await _emailHistoryService.Update(notificationModel, "");
                return Ok(new ApiResponse<NotificationModel>(true, "Thành công", notificationModel));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<NotificationModel>(false, "Lỗi " + ex.Message, null));
            }

        }


        [HttpPut("IsReadNotificationSystem")]
        public async Task<IActionResult> IsReadNotificationSystem(string id)
        {
            try
            {
                var data = await _emailHistoryService.GetById(id);
                if (data!=null)
                {
                    data.IsRead = 1;
                    await _emailHistoryService.Update(data, "");
                    return Ok(new ApiResponse<NotificationModel>(true, "Thành công", null));
                }
                return Ok(new ApiResponse<NotificationModel>(true, "Thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<NotificationModel>(false, "Lỗi " + ex.Message, null));
            }

        }
        //notificationFireBase
        [HttpGet("GetAllNotiFireBaseByUserId")]
        public async Task<ActionResult<ApiResponse<List<NotificationFireBase>>>> GetAllNotiFireBaseByUserId(string reciverId, int currentPage = 0, int pageSize = 10)
        {
            try
            {
                var notifis = await _firebaseNotificationService.GetNotificationsByReciverIdAsync(reciverId, currentPage * pageSize, pageSize);
                return Ok(new ApiResponse<List<NotificationFireBase>>(true, "Thành công", notifis));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<List<NotificationFireBase>>(false, "Lỗi " + ex.Message, null));
            }

        }

        [HttpGet("GetUnreadNotiFireBaseByUserId")]
        public async Task<IActionResult> GetUnreadNotiFireBaseByUserId(string Id)
        {
            try
            {
                int count = await _firebaseNotificationService.GetUnreadNotificationCountAsync(Id);
                return Ok(new ApiResponse<int>(true, "Thành công", count));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>(false, "Lỗi " + ex.Message, 0));
            }
        }

        [HttpPut("IsReadFireBaseId")]
        public async Task<IActionResult> IsReadFireBaseId(string Id)
        {
            try
            {
                await _firebaseNotificationService.UpdateIsReadAsync(Id);
                return Ok(new ApiResponse<NotificationFireBase>(true, "Thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<NotificationFireBase>(false, "Lỗi " + ex.Message, null));
            }

        }

        [HttpPut("RemoveNotiFireBaseId")]
        public async Task<IActionResult> RemoveNotiFireBaseId(string Id)
        {
            try
            {
                await _firebaseNotificationService.UpdateIsActiveAsync(Id, 0);
                return Ok(new ApiResponse<NotificationFireBase>(true, "Thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<NotificationFireBase>(false, "Lỗi " + ex.Message, null));
            }

        }


        //old
        [HttpGet("GetAllNotiByUser")]
        public async Task<ActionResult<ApiResponse<List<NotificationModel>>>> GetAllNotiByUser(string userName)
        {
            var notifis = await _emailHistoryService.GetAllNotiByUser(userName);
            if (notifis == null || notifis.Count == 0)
            {
                return NotFound(new ApiResponse<List<NotificationModel>>(false, "Không có thông báo!", null));
            }
            return Ok(new ApiResponse<List<NotificationModel>>(true, "Thành công", notifis));
        }

        [HttpGet("GetAllCategoriesByUser")]
        public async Task<ActionResult<ApiResponse<List<(string, string, int)>>>> GetAllCategoriesByUser(string userName)
        {
            var notifis = await _emailHistoryService.GetAllCategoriesByUser(userName);
            if (notifis == null || notifis.Count == 0)
            {
                return NotFound(new ApiResponse<List<(string, string, int)>>(false, "Không có thông báo!", null));
            }
            return Ok(new ApiResponse<List<(string, string, int)>>(true, "Thành công", notifis));
        }

    }

}
