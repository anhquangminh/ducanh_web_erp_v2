using DucAnh2025.Models.QLNV;

namespace DucAnh2025.ViewModel.QLNV
{
    public class QLNV_CreateCommentRequest
    {
        public string Id_CongViec { get; set; } = "";
        public string? ParentCommentId { get; set; }
        public string NoiDung { get; set; } = "";
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public List<string> MentionUserIds { get; set; } = new();
    }

    public class QLNV_UpdateCommentRequest
    {
        public string NoiDung { get; set; } = "";
        public List<string> MentionUserIds { get; set; } = new();
    }

    public class QLNV_CommentModel : QLNV_CongViecComment
    {
        public List<QLNV_CongViecMention> Mentions { get; set; } = new();
        public int ReplyCount { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class QLNV_TaskAccessPolicyModel
    {
        public bool CanView { get; set; }
        public bool CanComment { get; set; }
        public bool CanManageWatchers { get; set; }
        public bool CanEditAnyComment { get; set; }
        public bool CanDeleteAnyComment { get; set; }
    }

    public class QLNV_CommentsResponseModel
    {
        public List<QLNV_CommentModel> Comments { get; set; } = new();
        public QLNV_TaskAccessPolicyModel Policy { get; set; } = new();
    }

    public class QLNV_TaskUserSuggestionModel
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class QLNV_WatcherRequest
    {
        public string Id_CongViec { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
    }

    public class QLNV_TimelineItemModel
    {
        public string Id { get; set; } = "";
        public string Id_CongViec { get; set; } = "";
        public string Type { get; set; } = "";
        public string EventType { get; set; } = "";
        public string Description { get; set; } = "";
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string ActorUserName { get; set; } = "";
        public DateTime CreateAt { get; set; }
        public object? Data { get; set; }
    }

    public class QLNV_RecordActivityRequest
    {
        public string Id_CongViec { get; set; } = "";
        public string EventType { get; set; } = "";
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string Description { get; set; } = "";
        public string? MetadataJson { get; set; }
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
    }

    public class QLNV_CollaborationResult<T>
    {
        public T Data { get; set; }
        public List<string> TargetUserIds { get; set; } = new();

        public QLNV_CollaborationResult(T data, List<string> targetUserIds)
        {
            Data = data;
            TargetUserIds = targetUserIds;
        }
    }
}
