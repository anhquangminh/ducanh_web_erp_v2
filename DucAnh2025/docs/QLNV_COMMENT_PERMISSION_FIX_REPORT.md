# QLNV Comment, Mention, Watcher Permission Fix

## A. Danh sach loi hien tai

1. User suggestion bi lo pham vi
   - File: `wwwroot/js/QLNV/qlnv_task_collaboration.js`
   - Root cause: frontend goi `/api/ApplicationUser/GetAll?groupId=...` de lay user, sau do tu filter tren client.
   - Logic sai: client-side filtering khong phai security boundary; user co the goi API truc tiep de lay user ngoai task/project.

2. Mention chi filter theo `GroupId`
   - File: `Services/QLNV/QLNV_TaskCollaborationRepository.cs`
   - Root cause: `BuildMentionRows` chi kiem tra `ApplicationUsers.GroupId == task.GroupId`.
   - Logic sai: user cung tenant/group nhung khong thuoc nhom cong viec van co the bi mention.

3. Watcher co the them user bat ky
   - File: `Controllers/API/QLNV/QLNVTaskCollaborationController.cs`, `Services/QLNV/QLNV_TaskCollaborationRepository.cs`
   - Root cause: `AddWatcher` nhan `UserId` tu request va khong validate user do co nam trong pham vi task.
   - Logic sai: tao IDOR, co the tag/theo doi user ngoai nhom cong viec.

4. API doc du lieu chua check quyen xem
   - File: `GetComments`, `GetWatchers`, `GetTimeline`, `GetEvents`
   - Root cause: cac API read chi can JWT, khong validate actor co lien quan task.
   - Logic sai: user biet `idCongViec` co the doc comments/timeline/events.

5. Comment permission chua dung business rule
   - File: `CreateComment`, `UpdateComment`, `DeleteComment`
   - Root cause: thieu centralized policy.
   - Logic sai: bat ky JWT user co the comment/reply/edit/delete neu goi API.

## B. Huong fix da trien khai

Backend:
- Them scoped suggestion API: `GET /api/QLNVTaskCollaboration/users?idCongViec=&keyword=&skip=&take=`.
- Moi API comments/watchers/timeline/events deu validate actor bang `BuildTaskAccessContext`.
- `CanView`: admin hoac user nam trong scoped audience cua task.
- `CanComment`: admin hoac nguoi giao viec hoac nguoi nhan viec hoac user da tung comment trong task.
- `CanManageWatchers`: admin hoac nguoi giao viec hoac nguoi nhan viec.
- Mention va watcher deu validate `UserId` nam trong `ScopedUserIds`.
- Events chi tra ve event ma actor nam trong `TargetUserIdsJson`.

Frontend:
- Bo goi `/api/ApplicationUser/GetAll`.
- Select2 watcher va autocomplete `@mention` dung API scoped theo `idCongViec`.
- Them debounce cho mention search.
- Comment composer dung `policy.canComment` tu backend.
- Reply comment khong chen/khong hien ID dai; UI dung indent theo tree.
- Nut xoa comment chi hien khi backend tra `canDelete`.

Database:
- Tiep tuc dung cac bang da co: `QLNV_CongViecComments`, `QLNV_CongViecMentions`, `QLNV_CongViecWatchers`, `QLNV_CongViecActivities`, `QLNV_CongViecEvents`.
- Scope user duoc tinh tu: `QLNV_CongViecs`, `QLNV_NhanVienThucHiens`, `QLNV_QuanLyNhanViens`, `ApplicationUsers`, `UserRoles`, `Roles`, comments va watchers.

Socket/notification:
- Chua broadcast rong.
- Notification target lay tu audience da scope: creator, assignees, watchers, mentions hop le.
- Client ngoai pham vi task khong nhan duoc target ids tu API moi.

## C. Refactor de xuat

Nen tach `TaskAccessContext` thanh service rieng:
- `IQLNVTaskPermissionService`
- `GetAccessContext(taskId, actor)`
- `CanViewTask`
- `CanComment`
- `CanManageWatchers`
- `CanModifyComment`
- `SearchScopedUsers`

Kien truc de xuat:
```text
Controller
  -> PermissionService: validate actor + build policy
  -> CollaborationRepository: write comment/watcher/activity/event
  -> NotificationService: send only scoped targets
```

Policy nen duoc ap dung bat buoc o backend:
- `GET comments/watchers/timeline/events`: `CanView`
- `POST comments`: `CanComment`
- `PUT/DELETE comments`: author hoac admin
- `POST/DELETE watchers`: `CanManageWatchers`
- `MentionUserIds`: subset cua `ScopedUserIds`

## D. Code mau

Middleware/policy validator:
```csharp
var access = await BuildTaskAccessContext(context, task, actorUserName, actorUserId);
if (!access.CanView)
    throw new Exception("Ban khong co quyen truy cap cong viec nay.");
```

Mention query filter:
```csharp
var ids = mentionUserIds.Distinct().ToList();
var unauthorized = ids.Where(x => !access.ScopedUserIds.Contains(x)).ToList();
if (unauthorized.Any())
    throw new Exception("Danh sach mention co nguoi dung khong thuoc pham vi cong viec.");
```

Comment validator:
```csharp
if (!access.CanComment)
    throw new Exception("Ban khong co quyen binh luan trong cong viec nay.");
```

Secure user suggestion API:
```http
GET /api/QLNVTaskCollaboration/users?idCongViec={taskId}&keyword=M&take=20
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "id": "user-id",
      "userName": "user@company.com",
      "firstName": "Minh",
      "lastName": "Nguyen",
      "email": "user@company.com",
      "role": "Nguoi nhan viec"
    }
  ]
}
```

Socket authorization strategy:
```csharp
if (!await permissionService.CanViewTask(taskId, actorUserId))
    return;

await hub.Clients.Users(scopedTargetUserIds).SendAsync("task.comment.created", payload);
```

## Files changed

- `Services/QLNV/QLNV_TaskCollaborationRepository.cs`
- `Repository/QLNV/IQLNV_TaskCollaborationRepository.cs`
- `Controllers/API/QLNV/QLNVTaskCollaborationController.cs`
- `ViewModels/QLNV/QLNV_TaskCollaborationModel.cs`
- `wwwroot/js/QLNV/qlnv_task_collaboration.js`
- `Views/QLNV/QuanLyCongViec.cshtml`
