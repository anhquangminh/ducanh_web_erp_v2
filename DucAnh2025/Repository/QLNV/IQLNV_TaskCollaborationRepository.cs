using DucAnh2025.Models.QLNV;
using DucAnh2025.ViewModel.QLNV;

namespace DucAnh2025.Repository.QLNV
{
    public interface IQLNV_TaskCollaborationRepository
    {
        Task<QLNV_CommentsResponseModel> GetComments(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50);
        Task<List<QLNV_TaskUserSuggestionModel>> SearchTaskUsers(string idCongViec, string actorUserName, string? actorUserId, string keyword = "", int skip = 0, int take = 20);
        Task<QLNV_TaskAccessPolicyModel> GetTaskPolicy(string idCongViec, string actorUserName, string? actorUserId);
        Task<QLNV_CollaborationResult<QLNV_CommentModel>> CreateComment(QLNV_CreateCommentRequest request, string actorUserName, string? actorUserId);
        Task<QLNV_CollaborationResult<QLNV_CommentModel>> UpdateComment(string id, QLNV_UpdateCommentRequest request, string actorUserName, string? actorUserId);
        Task<QLNV_CollaborationResult<bool>> DeleteComment(string id, string actorUserName, string? actorUserId);

        Task<List<QLNV_CongViecWatcher>> GetWatchers(string idCongViec, string actorUserName, string? actorUserId);
        Task<QLNV_CollaborationResult<QLNV_CongViecWatcher>> AddWatcher(QLNV_WatcherRequest request, string actorUserName, string? actorUserId);
        Task<QLNV_CollaborationResult<bool>> RemoveWatcher(string idCongViec, string userId, string actorUserName, string? actorUserId);

        Task<List<QLNV_TimelineItemModel>> GetTimeline(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50);
        Task<QLNV_CongViecActivity> RecordActivity(QLNV_RecordActivityRequest request, string actorUserName, string? actorUserId);
        Task<List<QLNV_CongViecEvent>> GetEvents(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50);
    }
}
