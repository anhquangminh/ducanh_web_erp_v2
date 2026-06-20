using System.Security.Claims;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.Services;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.QLNV
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class QLNVTaskCollaborationController : ControllerBase
    {
        private readonly IQLNV_TaskCollaborationRepository _collaborationRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly FirebaseNotificationService _firebaseNotificationService;
        private readonly SignalRNotificationService _signalRNotificationService;

        public QLNVTaskCollaborationController(
            IQLNV_TaskCollaborationRepository collaborationRepository,
            IApplicationUserRepository applicationUserRepository,
            FirebaseNotificationService firebaseNotificationService,
            SignalRNotificationService signalRNotificationService)
        {
            _collaborationRepository = collaborationRepository;
            _applicationUserRepository = applicationUserRepository;
            _firebaseNotificationService = firebaseNotificationService;
            _signalRNotificationService = signalRNotificationService;
        }

        [HttpGet("comments")]
        public async Task<ActionResult<ApiResponse<QLNV_CommentsResponseModel>>> GetComments([FromQuery] string idCongViec, [FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            try
            {
                var actor = GetActor();
                var comments = await _collaborationRepository.GetComments(idCongViec, actor.UserName, actor.UserId, skip, take);
                return Ok(new ApiResponse<QLNV_CommentsResponseModel>(true, "Thành công", comments));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CommentsResponseModel>(false, ex.Message, null));
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_TaskUserSuggestionModel>>>> SearchTaskUsers([FromQuery] string idCongViec, [FromQuery] string keyword = "", [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            try
            {
                var actor = GetActor();
                var users = await _collaborationRepository.SearchTaskUsers(idCongViec, actor.UserName, actor.UserId, keyword, skip, take);
                return Ok(new ApiResponse<IEnumerable<QLNV_TaskUserSuggestionModel>>(true, "Thành công", users));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<QLNV_TaskUserSuggestionModel>>(false, ex.Message, new List<QLNV_TaskUserSuggestionModel>()));
            }
        }

        [HttpPost("comments")]
        public async Task<ActionResult<ApiResponse<QLNV_CommentModel>>> CreateComment([FromBody] QLNV_CreateCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Id_CongViec))
                return Ok(new ApiResponse<QLNV_CommentModel>(false, "Thiếu Id công việc cha", null));
            if (string.IsNullOrWhiteSpace(request.NoiDung))
                return Ok(new ApiResponse<QLNV_CommentModel>(false, "Nội dung bình luận không được để trống", null));

            try
            {
                var actor = GetActor();
                var result = await _collaborationRepository.CreateComment(request, actor.UserName, actor.UserId);
                _ = NotifyUsers(result.TargetUserIds, "Bình luận mới", result.Data.NoiDung, "congviec", result.Data.Id_CongViec);
                return Ok(new ApiResponse<QLNV_CommentModel>(true, "Tạo bình luận thành công", result.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CommentModel>(false, ex.Message, null));
            }
        }

        [HttpPut("comments/{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_CommentModel>>> UpdateComment([FromRoute] string id, [FromBody] QLNV_UpdateCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NoiDung))
                return Ok(new ApiResponse<QLNV_CommentModel>(false, "Nội dung bình luận không được để trống", null));

            try
            {
                var actor = GetActor();
                var result = await _collaborationRepository.UpdateComment(id, request, actor.UserName, actor.UserId);
                _ = NotifyUsers(result.TargetUserIds, "Bình luận đã được cập nhật", result.Data.NoiDung, "congviec", result.Data.Id_CongViec);
                return Ok(new ApiResponse<QLNV_CommentModel>(true, "Cập nhật bình luận thành công", result.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CommentModel>(false, ex.Message, null));
            }
        }

        [HttpDelete("comments/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteComment([FromRoute] string id)
        {
            try
            {
                var actor = GetActor();
                var result = await _collaborationRepository.DeleteComment(id, actor.UserName, actor.UserId);
                _ = NotifyUsers(result.TargetUserIds, "Bình luận đã bị xóa", "Một bình luận trong công việc đã bị xóa", "congviec", "");
                return Ok(new ApiResponse<bool>(true, "Xóa bình luận thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<bool>(false, ex.Message, false));
            }
        }

        [HttpGet("watchers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecWatcher>>>> GetWatchers([FromQuery] string idCongViec)
        {
            try
            {
                var actor = GetActor();
                var watchers = await _collaborationRepository.GetWatchers(idCongViec, actor.UserName, actor.UserId);
                return Ok(new ApiResponse<IEnumerable<QLNV_CongViecWatcher>>(true, "Thành công", watchers));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<QLNV_CongViecWatcher>>(false, ex.Message, new List<QLNV_CongViecWatcher>()));
            }
        }

        [HttpPost("watchers")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViecWatcher>>> AddWatcher([FromBody] QLNV_WatcherRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Id_CongViec) || string.IsNullOrWhiteSpace(request.UserId))
                return Ok(new ApiResponse<QLNV_CongViecWatcher>(false, "Thiếu Id công việc hoặc UserId", null));

            try
            {
                var actor = GetActor();
                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    var user = await _applicationUserRepository.GetById(request.UserId);
                    request.UserName = user?.UserName ?? "";
                }

                var result = await _collaborationRepository.AddWatcher(request, actor.UserName, actor.UserId);
                _ = NotifyUsers(result.TargetUserIds, "Theo dõi công việc", $"{result.Data.UserName} đang theo dõi công việc", "congviec", result.Data.Id_CongViec);
                return Ok(new ApiResponse<QLNV_CongViecWatcher>(true, "Thêm người theo dõi thành công", result.Data));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CongViecWatcher>(false, ex.Message, null));
            }
        }

        [HttpDelete("watchers")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveWatcher([FromQuery] string idCongViec, [FromQuery] string userId)
        {
            try
            {
                var actor = GetActor();
                var result = await _collaborationRepository.RemoveWatcher(idCongViec, userId, actor.UserName, actor.UserId);
                _ = NotifyUsers(result.TargetUserIds, "Bỏ theo dõi công việc", "Một người theo dõi đã được xóa khỏi công việc", "congviec", idCongViec);
                return Ok(new ApiResponse<bool>(true, "Xóa người theo dõi thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<bool>(false, ex.Message, false));
            }
        }

        [HttpGet("timeline")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_TimelineItemModel>>>> GetTimeline([FromQuery] string idCongViec, [FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            try
            {
                var actor = GetActor();
                var timeline = await _collaborationRepository.GetTimeline(idCongViec, actor.UserName, actor.UserId, skip, take);
                return Ok(new ApiResponse<IEnumerable<QLNV_TimelineItemModel>>(true, "Thành công", timeline));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<QLNV_TimelineItemModel>>(false, ex.Message, new List<QLNV_TimelineItemModel>()));
            }
        }

        [HttpPost("activities")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViecActivity>>> RecordActivity([FromBody] QLNV_RecordActivityRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Id_CongViec) || string.IsNullOrWhiteSpace(request.EventType))
                return Ok(new ApiResponse<QLNV_CongViecActivity>(false, "Thiếu Id công việc hoặc EventType", null));

            try
            {
                var actor = GetActor();
                var activity = await _collaborationRepository.RecordActivity(request, actor.UserName, actor.UserId);
                return Ok(new ApiResponse<QLNV_CongViecActivity>(true, "Ghi activity thành công", activity));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CongViecActivity>(false, ex.Message, null));
            }
        }

        [HttpGet("events")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecEvent>>>> GetEvents([FromQuery] string idCongViec, [FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            try
            {
                var actor = GetActor();
                var events = await _collaborationRepository.GetEvents(idCongViec, actor.UserName, actor.UserId, skip, take);
                return Ok(new ApiResponse<IEnumerable<QLNV_CongViecEvent>>(true, "Thành công", events));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<QLNV_CongViecEvent>>(false, ex.Message, new List<QLNV_CongViecEvent>()));
            }
        }

        private (string UserName, string? UserId) GetActor()
        {
            var userName = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Name) ?? "";
            var user = string.IsNullOrWhiteSpace(userName) ? null : _applicationUserRepository.GetByUserName(userName);
            return (userName, user?.Id);
        }

        private async Task NotifyUsers(List<string> userIds, string title, string body, string targetPage, string targetId)
        {
            var targets = userIds.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            if (!targets.Any())
                return;

            await _firebaseNotificationService.SendNotificationToMultipleAsync(targets, title, body, targetPage, targetId);
            await _signalRNotificationService.SendToUsersAsync(targets, title, body);
        }
    }
}
