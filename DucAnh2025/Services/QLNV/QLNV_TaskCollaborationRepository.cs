using System.Text.Json;
using DucAnh2025.Data;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.QLNV
{
    public class QLNV_TaskCollaborationRepository : IQLNV_TaskCollaborationRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public QLNV_TaskCollaborationRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public async Task<QLNV_CommentsResponseModel> GetComments(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanView(access);

            take = Math.Clamp(take, 1, 100);

            var comments = await context.QLNV_CongViecComments
                .AsNoTracking()
                .Where(x => x.Id_CongViec == idCongViec && x.IsActive != 100)
                .OrderBy(x => x.CreateAt)
                .Skip(Math.Max(skip, 0))
                .Take(take)
                .Select(x => new QLNV_CommentModel
                {
                    Id = x.Id,
                    Id_CongViec = x.Id_CongViec,
                    ParentCommentId = x.ParentCommentId,
                    NoiDung = x.NoiDung,
                    CompanyId = x.CompanyId,
                    GroupId = x.GroupId,
                    CreateAt = x.CreateAt,
                    CreateBy = x.CreateBy,
                    CreateByUserId = x.CreateByUserId,
                    UpdateAt = x.UpdateAt,
                    UpdateBy = x.UpdateBy,
                    IsEdited = x.IsEdited,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            var commentIds = comments.Select(x => x.Id).ToList();
            var mentions = await context.QLNV_CongViecMentions
                .AsNoTracking()
                .Where(x => commentIds.Contains(x.CommentId) && x.IsActive != 100)
                .ToListAsync();

            var replyCounts = await context.QLNV_CongViecComments
                .AsNoTracking()
                .Where(x => x.ParentCommentId != null && commentIds.Contains(x.ParentCommentId) && x.IsActive != 100)
                .GroupBy(x => x.ParentCommentId)
                .Select(x => new { ParentCommentId = x.Key, Count = x.Count() })
                .ToListAsync();

            foreach (var comment in comments)
            {
                comment.Mentions = mentions.Where(x => x.CommentId == comment.Id).ToList();
                comment.ReplyCount = replyCounts.FirstOrDefault(x => x.ParentCommentId == comment.Id)?.Count ?? 0;
                comment.CanEdit = CanModifyComment(access, comment);
                comment.CanDelete = CanModifyComment(access, comment);
            }

            return new QLNV_CommentsResponseModel
            {
                Comments = comments,
                Policy = access.ToPolicy()
            };
        }

        public async Task<List<QLNV_TaskUserSuggestionModel>> SearchTaskUsers(string idCongViec, string actorUserName, string? actorUserId, string keyword = "", int skip = 0, int take = 20)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanView(access);

            take = Math.Clamp(take, 1, 50);
            var ids = access.ScopedUserIds.ToList();
            var query = context.ApplicationUsers
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && x.IsActive != 100);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(x =>
                    x.UserName.Contains(keyword) ||
                    x.FirstName.Contains(keyword) ||
                    x.LastName.Contains(keyword) ||
                    x.Email.Contains(keyword));
            }

            var users = await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.UserName)
                .Skip(Math.Max(skip, 0))
                .Take(take)
                .Select(x => new QLNV_TaskUserSuggestionModel
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                })
                .ToListAsync();

            foreach (var user in users)
                user.Role = access.GetUserRoleLabel(user.Id);

            return users;
        }

        public async Task<QLNV_TaskAccessPolicyModel> GetTaskPolicy(string idCongViec, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            return access.ToPolicy();
        }

        public async Task<QLNV_CollaborationResult<QLNV_CommentModel>> CreateComment(QLNV_CreateCommentRequest request, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            var task = await GetActiveTask(context, request.Id_CongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanComment(access);

            if (!string.IsNullOrWhiteSpace(request.ParentCommentId))
            {
                var parentExists = await context.QLNV_CongViecComments
                    .AnyAsync(x => x.Id == request.ParentCommentId && x.Id_CongViec == task.Id && x.IsActive != 100);
                if (!parentExists)
                    throw new Exception("Bình luận cha không hợp lệ.");
            }

            var now = DateTime.UtcNow;
            var comment = new QLNV_CongViecComment
            {
                Id = Guid.NewGuid().ToString(),
                Id_CongViec = task.Id,
                ParentCommentId = string.IsNullOrWhiteSpace(request.ParentCommentId) ? null : request.ParentCommentId,
                NoiDung = request.NoiDung.Trim(),
                CompanyId = string.IsNullOrWhiteSpace(request.CompanyId) ? task.CompanyId : request.CompanyId,
                GroupId = string.IsNullOrWhiteSpace(request.GroupId) ? task.GroupId : request.GroupId,
                CreateAt = now,
                CreateBy = actorUserName,
                CreateByUserId = actorUserId,
                IsActive = 1
            };

            context.QLNV_CongViecComments.Add(comment);
            var mentionRows = await BuildMentionRows(context, task, access, comment.Id, request.MentionUserIds, actorUserName, now);
            context.QLNV_CongViecMentions.AddRange(mentionRows);

            context.QLNV_CongViecActivities.Add(BuildActivity(task, "task.comment.created", "Bình luận mới", actorUserName, actorUserId, now, metadata: new { commentId = comment.Id }));

            var targets = await GetTaskAudience(context, task, access, request.MentionUserIds);
            context.QLNV_CongViecEvents.Add(BuildEvent(task, "task.comment.created", actorUserName, targets, new
            {
                commentId = comment.Id,
                taskId = task.Id,
                body = comment.NoiDung,
                mentions = request.MentionUserIds
            }, now));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new QLNV_CollaborationResult<QLNV_CommentModel>(
                await BuildCommentModel(context, comment.Id, access),
                targets
            );
        }

        public async Task<QLNV_CollaborationResult<QLNV_CommentModel>> UpdateComment(string id, QLNV_UpdateCommentRequest request, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            var comment = await context.QLNV_CongViecComments.FirstOrDefaultAsync(x => x.Id == id && x.IsActive != 100);
            if (comment == null)
                throw new Exception("Không tìm thấy bình luận.");

            var task = await GetActiveTask(context, comment.Id_CongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            if (!CanModifyComment(access, comment))
                throw new Exception("Bạn không có quyền chỉnh sửa bình luận này.");

            var oldValue = comment.NoiDung;
            var now = DateTime.UtcNow;

            comment.NoiDung = request.NoiDung.Trim();
            comment.UpdateAt = now;
            comment.UpdateBy = actorUserName;
            comment.IsEdited = 1;

            var oldMentions = await context.QLNV_CongViecMentions
                .Where(x => x.CommentId == comment.Id && x.IsActive != 100)
                .ToListAsync();
            foreach (var mention in oldMentions)
                mention.IsActive = 100;

            var mentionRows = await BuildMentionRows(context, task, access, comment.Id, request.MentionUserIds, actorUserName, now);
            context.QLNV_CongViecMentions.AddRange(mentionRows);

            context.QLNV_CongViecActivities.Add(BuildActivity(task, "task.comment.updated", "Cập nhật bình luận", actorUserName, actorUserId, now, "NoiDung", oldValue, comment.NoiDung, new { commentId = comment.Id }));

            var targets = await GetTaskAudience(context, task, access, request.MentionUserIds);
            context.QLNV_CongViecEvents.Add(BuildEvent(task, "task.comment.updated", actorUserName, targets, new
            {
                commentId = comment.Id,
                taskId = task.Id,
                oldValue,
                newValue = comment.NoiDung,
                mentions = request.MentionUserIds
            }, now));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new QLNV_CollaborationResult<QLNV_CommentModel>(
                await BuildCommentModel(context, comment.Id, access),
                targets
            );
        }

        public async Task<QLNV_CollaborationResult<bool>> DeleteComment(string id, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            var comment = await context.QLNV_CongViecComments.FirstOrDefaultAsync(x => x.Id == id && x.IsActive != 100);
            if (comment == null)
                throw new Exception("Không tìm thấy bình luận.");

            var task = await GetActiveTask(context, comment.Id_CongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            if (!CanModifyComment(access, comment))
                throw new Exception("Bạn không có quyền xóa bình luận này.");

            var now = DateTime.UtcNow;
            comment.IsActive = 100;
            comment.UpdateAt = now;
            comment.UpdateBy = actorUserName;

            var mentions = await context.QLNV_CongViecMentions
                .Where(x => x.CommentId == comment.Id && x.IsActive != 100)
                .ToListAsync();
            foreach (var mention in mentions)
                mention.IsActive = 100;

            context.QLNV_CongViecActivities.Add(BuildActivity(task, "task.comment.deleted", "Xóa bình luận", actorUserName, actorUserId, now, metadata: new { commentId = comment.Id }));
            var targets = await GetTaskAudience(context, task, access, new List<string>());
            context.QLNV_CongViecEvents.Add(BuildEvent(task, "task.comment.deleted", actorUserName, targets, new { commentId = comment.Id, taskId = task.Id }, now));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new QLNV_CollaborationResult<bool>(true, targets);
        }

        public async Task<List<QLNV_CongViecWatcher>> GetWatchers(string idCongViec, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanView(access);

            return await context.QLNV_CongViecWatchers
                .AsNoTracking()
                .Where(x => x.Id_CongViec == idCongViec && x.IsActive != 100)
                .OrderBy(x => x.CreateAt)
                .ToListAsync();
        }

        public async Task<QLNV_CollaborationResult<QLNV_CongViecWatcher>> AddWatcher(QLNV_WatcherRequest request, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            var task = await GetActiveTask(context, request.Id_CongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanManageWatchers(access);

            if (!access.ScopedUserIds.Contains(request.UserId))
                throw new Exception("Người dùng này không thuộc phạm vi công việc nên không thể theo dõi.");

            var targetUser = await context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId && x.IsActive != 100);
            if (targetUser == null)
                throw new Exception("Không tìm thấy người dùng.");

            var now = DateTime.UtcNow;
            var watcher = await context.QLNV_CongViecWatchers
                .FirstOrDefaultAsync(x => x.Id_CongViec == task.Id && x.UserId == request.UserId);

            if (watcher == null)
            {
                watcher = new QLNV_CongViecWatcher
                {
                    Id = Guid.NewGuid().ToString(),
                    Id_CongViec = task.Id,
                    UserId = targetUser.Id,
                    UserName = targetUser.UserName,
                    CompanyId = string.IsNullOrWhiteSpace(request.CompanyId) ? task.CompanyId : request.CompanyId,
                    GroupId = string.IsNullOrWhiteSpace(request.GroupId) ? task.GroupId : request.GroupId,
                    CreateAt = now,
                    CreateBy = actorUserName,
                    IsActive = 1
                };
                context.QLNV_CongViecWatchers.Add(watcher);
            }
            else
            {
                watcher.UserName = targetUser.UserName;
                watcher.IsActive = 1;
            }

            context.QLNV_CongViecActivities.Add(BuildActivity(task, "task.watcher.added", "Thêm người theo dõi", actorUserName, actorUserId, now, metadata: new { watcher.UserId, watcher.UserName }));
            var targets = await GetTaskAudience(context, task, access, new List<string> { targetUser.Id });
            context.QLNV_CongViecEvents.Add(BuildEvent(task, "task.watcher.added", actorUserName, targets, new { taskId = task.Id, watcher.UserId, watcher.UserName }, now));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new QLNV_CollaborationResult<QLNV_CongViecWatcher>(watcher, targets);
        }

        public async Task<QLNV_CollaborationResult<bool>> RemoveWatcher(string idCongViec, string userId, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanManageWatchers(access);

            var watcher = await context.QLNV_CongViecWatchers
                .FirstOrDefaultAsync(x => x.Id_CongViec == idCongViec && x.UserId == userId && x.IsActive != 100);

            if (watcher == null)
                throw new Exception("Không tìm thấy người theo dõi.");

            var now = DateTime.UtcNow;
            watcher.IsActive = 100;
            context.QLNV_CongViecActivities.Add(BuildActivity(task, "task.watcher.removed", "Xóa người theo dõi", actorUserName, actorUserId, now, metadata: new { watcher.UserId, watcher.UserName }));
            var targets = await GetTaskAudience(context, task, access, new List<string> { userId });
            context.QLNV_CongViecEvents.Add(BuildEvent(task, "task.watcher.removed", actorUserName, targets, new { taskId = task.Id, watcher.UserId, watcher.UserName }, now));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new QLNV_CollaborationResult<bool>(true, targets);
        }

        public async Task<List<QLNV_TimelineItemModel>> GetTimeline(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanView(access);

            take = Math.Clamp(take, 1, 100);

            var activities = await context.QLNV_CongViecActivities
                .AsNoTracking()
                .Where(x => x.Id_CongViec == idCongViec && x.IsActive != 100)
                .Select(x => new QLNV_TimelineItemModel
                {
                    Id = x.Id,
                    Id_CongViec = x.Id_CongViec,
                    Type = "activity",
                    EventType = x.EventType,
                    Description = x.Description,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue,
                    ActorUserName = x.ActorUserName,
                    CreateAt = x.CreateAt,
                    Data = x
                })
                .ToListAsync();

            var commentsResponse = await GetComments(idCongViec, actorUserName, actorUserId, 0, 1000);
            var commentItems = commentsResponse.Comments.Select(x => new QLNV_TimelineItemModel
            {
                Id = x.Id,
                Id_CongViec = x.Id_CongViec,
                Type = "comment",
                EventType = "task.comment",
                Description = x.NoiDung,
                ActorUserName = x.CreateBy,
                CreateAt = x.CreateAt,
                Data = x
            });

            return activities
                .Concat(commentItems)
                .OrderByDescending(x => x.CreateAt)
                .Skip(Math.Max(skip, 0))
                .Take(take)
                .ToList();
        }

        public async Task<QLNV_CongViecActivity> RecordActivity(QLNV_RecordActivityRequest request, string actorUserName, string? actorUserId)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, request.Id_CongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanManageWatchers(access);

            var now = DateTime.UtcNow;

            var activity = BuildActivity(
                task,
                request.EventType,
                request.Description,
                actorUserName,
                actorUserId,
                now,
                request.FieldName,
                request.OldValue,
                request.NewValue,
                string.IsNullOrWhiteSpace(request.MetadataJson) ? null : JsonSerializer.Deserialize<object>(request.MetadataJson));

            context.QLNV_CongViecActivities.Add(activity);
            var targets = await GetTaskAudience(context, task, access, new List<string>());
            context.QLNV_CongViecEvents.Add(BuildEvent(task, request.EventType, actorUserName, targets, request, now));
            await context.SaveChangesAsync();
            return activity;
        }

        public async Task<List<QLNV_CongViecEvent>> GetEvents(string idCongViec, string actorUserName, string? actorUserId, int skip = 0, int take = 50)
        {
            using var context = _context.CreateDbContext();
            var task = await GetActiveTask(context, idCongViec);
            var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
            EnsureCanView(access);

            take = Math.Clamp(take, 1, 100);
            return await context.QLNV_CongViecEvents
                .AsNoTracking()
                .Where(x => x.Id_CongViec == idCongViec && x.IsActive != 100 && x.TargetUserIdsJson.Contains(access.ActorUserId))
                .OrderByDescending(x => x.CreateAt)
                .Skip(Math.Max(skip, 0))
                .Take(take)
                .ToListAsync();
        }

        private static async Task<QLNV_CongViec> GetActiveTask(ApplicationDbContext context, string idCongViec)
        {
            var task = await context.QLNV_CongViecs.FirstOrDefaultAsync(x => x.Id == idCongViec && x.IsActive != 100);
            if (task == null)
                throw new Exception("Không tìm thấy công việc cha.");
            return task;
        }

        private static async Task<TaskAccessContext> BuildTaskAccessContext(ApplicationDbContext context, QLNV_CongViec task, string actorUserName, string? actorUserId)
        {
            var actor = await context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsActive != 100 &&
                    ((!string.IsNullOrWhiteSpace(actorUserId) && x.Id == actorUserId) || x.UserName == actorUserName));

            if (actor == null)
                return TaskAccessContext.Denied();

            var roleNames = await (
                from userRole in context.UserRoles.AsNoTracking()
                join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                where userRole.UserId == actor.Id
                select role.Name
            ).ToListAsync();
            var isAdmin = roleNames.Any(IsAdminRole);

            var groupAdminIds = await (
                from user in context.ApplicationUsers.AsNoTracking()
                join userRole in context.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
                join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                where user.GroupId == task.GroupId && user.IsActive != 100 && role.Name.Contains("Admin")
                select user.Id
            ).Distinct().ToListAsync();

            var creatorIds = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => x.IsActive != 100 && (
                    x.UserName == task.Id_NguoiGiaoViec ||
                    x.Id == task.Id_NguoiGiaoViec ||
                    x.UserName == task.CreateBy ||
                    x.Id == task.CreateBy))
                .Select(x => x.Id)
                .ToListAsync();

            var assigneeIds = await (
                from nvth in context.QLNV_NhanVienThucHiens.AsNoTracking()
                join nv in context.QLNV_NhanViens.AsNoTracking() on nvth.Id_NhanVien equals nv.Id
                join user in context.ApplicationUsers.AsNoTracking() on nv.TaiKhoan equals user.UserName
                where nvth.Id_CongViec == task.Id && nvth.IsActive != 100 && nv.IsActive != 100 && user.IsActive != 100
                select user.Id
            ).Distinct().ToListAsync();

            var teamMemberIds = await (
                from member in context.QLNV_QuanLyNhanViens.AsNoTracking()
                join nv in context.QLNV_NhanViens.AsNoTracking() on member.Id_NhanVien equals nv.Id
                join user in context.ApplicationUsers.AsNoTracking() on nv.TaiKhoan equals user.UserName
                where member.Id_NhomNhanVien == task.NhomCongViec &&
                      member.IsActive != 100 &&
                      nv.IsActive != 100 &&
                      user.IsActive != 100 &&
                      user.GroupId == task.GroupId
                select user.Id
            ).Distinct().ToListAsync();

            var watcherIds = await context.QLNV_CongViecWatchers
                .AsNoTracking()
                .Where(x => x.Id_CongViec == task.Id && x.IsActive != 100)
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();

            var commenterIds = await context.QLNV_CongViecComments
                .AsNoTracking()
                .Where(x => x.Id_CongViec == task.Id && x.IsActive != 100)
                .Select(x => x.CreateByUserId ?? "")
                .Where(x => x != "")
                .Distinct()
                .ToListAsync();

            var commenterNames = await context.QLNV_CongViecComments
                .AsNoTracking()
                .Where(x => x.Id_CongViec == task.Id && x.IsActive != 100 && x.CreateByUserId == null)
                .Select(x => x.CreateBy)
                .Distinct()
                .ToListAsync();

            if (commenterNames.Any())
            {
                var idsFromNames = await context.ApplicationUsers
                    .AsNoTracking()
                    .Where(x => commenterNames.Contains(x.UserName) && x.IsActive != 100)
                    .Select(x => x.Id)
                    .ToListAsync();
                commenterIds.AddRange(idsFromNames);
            }

            var scopedIds = teamMemberIds
                .Concat(groupAdminIds)
                .Concat(creatorIds)
                .Concat(assigneeIds)
                .Concat(watcherIds)
                .Concat(commenterIds)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToHashSet();

            return new TaskAccessContext
            {
                ActorUserId = actor.Id,
                ActorUserName = actor.UserName,
                IsAdmin = isAdmin,
                CreatorIds = creatorIds.ToHashSet(),
                AssigneeIds = assigneeIds.ToHashSet(),
                TeamMemberIds = teamMemberIds.ToHashSet(),
                WatcherIds = watcherIds.ToHashSet(),
                CommenterIds = commenterIds.ToHashSet(),
                GroupAdminIds = groupAdminIds.ToHashSet(),
                ScopedUserIds = scopedIds,
                CanView = isAdmin || scopedIds.Contains(actor.Id),
                CanComment = isAdmin || creatorIds.Contains(actor.Id) || assigneeIds.Contains(actor.Id) || commenterIds.Contains(actor.Id),
                CanManageWatchers = isAdmin || creatorIds.Contains(actor.Id) || assigneeIds.Contains(actor.Id)
            };
        }

        private static bool IsAdminRole(string? roleName)
        {
            return !string.IsNullOrWhiteSpace(roleName) &&
                   roleName.Contains("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<List<QLNV_CongViecMention>> BuildMentionRows(ApplicationDbContext context, QLNV_CongViec task, TaskAccessContext access, string commentId, List<string> mentionUserIds, string actorUserName, DateTime now)
        {
            var ids = mentionUserIds.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            if (!ids.Any())
                return new List<QLNV_CongViecMention>();

            var unauthorized = ids.Where(x => !access.ScopedUserIds.Contains(x)).ToList();
            if (unauthorized.Any())
                throw new Exception("Danh sách mention có người dùng không thuộc phạm vi công việc.");

            var users = await context.ApplicationUsers
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && x.IsActive != 100)
                .Select(x => new { x.Id, x.UserName })
                .ToListAsync();

            if (users.Count != ids.Count)
                throw new Exception("Danh sách mention không hợp lệ.");

            return users.Select(x => new QLNV_CongViecMention
            {
                Id = Guid.NewGuid().ToString(),
                Id_CongViec = task.Id,
                CommentId = commentId,
                UserId = x.Id,
                UserName = x.UserName,
                GroupId = task.GroupId,
                CreateAt = now,
                CreateBy = actorUserName,
                IsActive = 1
            }).ToList();
        }

        private static async Task<List<string>> GetTaskAudience(ApplicationDbContext context, QLNV_CongViec task, TaskAccessContext access, List<string> extraUserIds)
        {
            var validExtraIds = (extraUserIds ?? new List<string>())
                .Where(x => access.ScopedUserIds.Contains(x))
                .ToList();

            var targets = access.CreatorIds
                .Concat(access.AssigneeIds)
                .Concat(access.WatcherIds)
                .Concat(validExtraIds)
                .Where(x => !string.IsNullOrWhiteSpace(x) && access.ScopedUserIds.Contains(x))
                .Distinct()
                .ToList();

            if (!targets.Any())
            {
                targets = await context.ApplicationUsers
                    .AsNoTracking()
                    .Where(x => access.ScopedUserIds.Contains(x.Id) && x.GroupId == task.GroupId && x.IsActive != 100)
                    .Select(x => x.Id)
                    .Take(50)
                    .ToListAsync();
            }

            return targets;
        }

        private async Task<QLNV_CommentModel> BuildCommentModel(ApplicationDbContext context, string commentId, TaskAccessContext access)
        {
            var comment = await context.QLNV_CongViecComments.AsNoTracking().FirstAsync(x => x.Id == commentId);
            var mentions = await context.QLNV_CongViecMentions.AsNoTracking()
                .Where(x => x.CommentId == comment.Id && x.IsActive != 100)
                .ToListAsync();
            var replyCount = await context.QLNV_CongViecComments.AsNoTracking()
                .CountAsync(x => x.ParentCommentId == comment.Id && x.IsActive != 100);

            return new QLNV_CommentModel
            {
                Id = comment.Id,
                Id_CongViec = comment.Id_CongViec,
                ParentCommentId = comment.ParentCommentId,
                NoiDung = comment.NoiDung,
                CompanyId = comment.CompanyId,
                GroupId = comment.GroupId,
                CreateAt = comment.CreateAt,
                CreateBy = comment.CreateBy,
                CreateByUserId = comment.CreateByUserId,
                UpdateAt = comment.UpdateAt,
                UpdateBy = comment.UpdateBy,
                IsEdited = comment.IsEdited,
                IsActive = comment.IsActive,
                Mentions = mentions,
                ReplyCount = replyCount,
                CanEdit = CanModifyComment(access, comment),
                CanDelete = CanModifyComment(access, comment)
            };
        }

        private static bool CanModifyComment(TaskAccessContext access, QLNV_CongViecComment comment)
        {
            return access.IsAdmin ||
                   (!string.IsNullOrWhiteSpace(comment.CreateByUserId) && comment.CreateByUserId == access.ActorUserId) ||
                   (!string.IsNullOrWhiteSpace(comment.CreateBy) && comment.CreateBy == access.ActorUserName);
        }

        private static void EnsureCanView(TaskAccessContext access)
        {
            if (!access.CanView)
                throw new Exception("Bạn không có quyền truy cập công việc này.");
        }

        private static void EnsureCanComment(TaskAccessContext access)
        {
            EnsureCanView(access);
            if (!access.CanComment)
                throw new Exception("Bạn không có quyền bình luận trong công việc này.");
        }

        private static void EnsureCanManageWatchers(TaskAccessContext access)
        {
            EnsureCanView(access);
            if (!access.CanManageWatchers)
                throw new Exception("Bạn không có quyền quản lý người theo dõi công việc này.");
        }

        private static QLNV_CongViecActivity BuildActivity(QLNV_CongViec task, string eventType, string description, string actorUserName, string? actorUserId, DateTime now, string? fieldName = null, string? oldValue = null, string? newValue = null, object? metadata = null)
        {
            return new QLNV_CongViecActivity
            {
                Id = Guid.NewGuid().ToString(),
                Id_CongViec = task.Id,
                EventType = eventType,
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Description = description,
                ActorUserId = actorUserId,
                ActorUserName = actorUserName,
                MetadataJson = metadata == null ? null : JsonSerializer.Serialize(metadata),
                CompanyId = task.CompanyId,
                GroupId = task.GroupId,
                CreateAt = now,
                IsActive = 1
            };
        }

        private static QLNV_CongViecEvent BuildEvent(QLNV_CongViec task, string eventName, string actorUserName, List<string> targetUserIds, object payload, DateTime now)
        {
            return new QLNV_CongViecEvent
            {
                Id = Guid.NewGuid().ToString(),
                Id_CongViec = task.Id,
                EventName = eventName,
                PayloadJson = JsonSerializer.Serialize(payload),
                TargetUserIdsJson = JsonSerializer.Serialize(targetUserIds.Distinct().ToList()),
                CompanyId = task.CompanyId,
                GroupId = task.GroupId,
                CreateAt = now,
                CreateBy = actorUserName,
                IsActive = 1
            };
        }

        private class TaskAccessContext
        {
            public string ActorUserId { get; set; } = "";
            public string ActorUserName { get; set; } = "";
            public bool IsAdmin { get; set; }
            public bool CanView { get; set; }
            public bool CanComment { get; set; }
            public bool CanManageWatchers { get; set; }
            public HashSet<string> CreatorIds { get; set; } = new();
            public HashSet<string> AssigneeIds { get; set; } = new();
            public HashSet<string> TeamMemberIds { get; set; } = new();
            public HashSet<string> WatcherIds { get; set; } = new();
            public HashSet<string> CommenterIds { get; set; } = new();
            public HashSet<string> GroupAdminIds { get; set; } = new();
            public HashSet<string> ScopedUserIds { get; set; } = new();

            public static TaskAccessContext Denied() => new();

            public QLNV_TaskAccessPolicyModel ToPolicy()
            {
                return new QLNV_TaskAccessPolicyModel
                {
                    CanView = CanView,
                    CanComment = CanComment,
                    CanManageWatchers = CanManageWatchers,
                    CanEditAnyComment = IsAdmin,
                    CanDeleteAnyComment = IsAdmin
                };
            }

            public string GetUserRoleLabel(string userId)
            {
                if (GroupAdminIds.Contains(userId)) return "Admin nhóm";
                if (CreatorIds.Contains(userId)) return "Người giao việc";
                if (AssigneeIds.Contains(userId)) return "Người nhận việc";
                if (TeamMemberIds.Contains(userId)) return "Thành viên nhóm";
                if (WatcherIds.Contains(userId)) return "Người theo dõi";
                if (CommenterIds.Contains(userId)) return "Đã bình luận";
                return "Thành viên";
            }
        }
    }
}
