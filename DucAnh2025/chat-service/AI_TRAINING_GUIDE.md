# DucAnh ERP v2 - Hướng Dẫn Tóm Tắt (AI Assistant Training)

## 📍 Module 1: CHAT

### Mục Đích
- Giao tiếp realtime giữa nhân viên
- Chia sẻ file, reaction tin nhắn
- Quản lý nhóm chat & thành viên

### Cách Sử Dụng Cơ Bản

| Tác Vụ | Bước | Kết Quả |
|--------|------|--------|
| **Tạo nhóm chat** | Bấm "+ Tạo Nhóm" → Chọn thành viên → Gửi | ✓ Nhóm được tạo |
| **Gửi tin nhắn** | Click nhóm → Gõ tin nhắn → Enter | ✓ Tin gửi đi realtime |
| **Gửi file/hình** | Bấm 📎 → Chọn file (max 100MB) → Gửi | ✓ File upload & chia sẻ |
| **Reply tin cũ** | Chuột phải → "Trả lời" → Gõ → Gửi | ✓ Liên kết tin nhắn |
| **Reaction emoji** | Chuột phải tin → 😊 → Chọn emoji | ✓ Phản ứng hiển thị |
| **Xóa cho bản thân** | Chuột phải → "Xóa" → "Xóa cho tôi" | ✓ Chỉ bạn không thấy |
| **Xóa cho tất cả** | Chuột phải → "Xóa" → "Xóa cho tất cả" | ✓ Xóa vĩnh viễn |
| **Ghim tin nhắn** | Chuột phải → "Ghim" (Admin/Owner) | ✓ Tin ở top |
| **Tắt thông báo** | Right-click nhóm → "Tắt thông báo" | ✓ Không có popup |
| **Rời nhóm** | Mở nhóm → ⋮ Tùy chọn → "Rời nhóm" | ✓ Bị xóa khỏi nhóm |

### Cấu Trúc Dữ Liệu (MongoDB)

Conversations (nhóm chat) ├─ type: "private" | "group" ├─ title: "Tên nhóm" (group only) ├─ createdBy: userId ├─ lastMessageAt: Date (dùng cho sorting) └─ isActive: 1/0

ConversationMembers (thành viên) ├─ userId: string ├─ role: "owner" | "admin" | "member" ├─ mutedUntil: Date (tắt thông báo) ├─ lastReadMessageId: ObjectId (tin cuối đã xem) └─ hiddenAt: Date (ẩn khỏi danh sách)

Messages (tin nhắn) ├─ conversationId: ObjectId ├─ senderId: string ├─ body: string ├─ type: "text" | "image" | "file" | "voice" | "video" ├─ attachments: [{url, fileName, mimeType, size}] ├─ mentionedUserIds: [userId] ├─ pinnedAt: Date (nếu ghim) ├─ editedAt: Date ├─ recalledAt: Date (nếu xóa cho tất cả) └─ deletedForUserIds: [userId] (xóa cho cá nhân)

### API Endpoints (Chat Service)

// Cuộc hội thoại
GET    /api/chat/conversations
POST   /api/chat/conversations/private
POST   /api/chat/conversations/group
PATCH  /api/chat/conversations/:id
DELETE /api/chat/conversations/:id

// Tin nhắn
GET    /api/chat/conversations/:id/messages
POST   /api/chat/conversations/:id/messages
PATCH  /api/chat/conversations/:id/messages/:messageId
DELETE /api/chat/conversations/:id/messages/:messageId
POST   /api/chat/conversations/:id/messages/:messageId/reactions
POST   /api/chat/conversations/:id/messages/:messageId/pin
POST   /api/chat/conversations/:id/messages/:messageId/forward

// Thành viên
GET    /api/chat/conversations/:id/members
POST   /api/chat/conversations/:id/members
DELETE /api/chat/conversations/:id/members/:userId

// File
POST   /api/chat/uploads
GET    /api/chat/uploads/:fileName/download

// Tìm kiếm
GET    /api/chat/users/search?q=keyword

### Lỗi Thường Gặp & Cách Khắc Phục
Lỗi	Nguyên Nhân	Cách Khắc Phục
Không gửi được tin	Mất kết nối WebSocket	Refresh trang, check internet
Tin không nhận	Người mình offline	Chờ online hoặc gửi email
Upload file thất bại	File > 100MB hoặc loại không hợp lệ	Giảm dung lượng hoặc đổi format
Không thấy thành viên mới	Chưa refresh list	F5 reload trang
Không có quyền xóa tin	Chỉ sender/owner/admin được xóa	Liên hệ admin
File bị "Quoted" lỗi	Đường dẫn sai hoặc file bị xóa	Tải lại hoặc request file

## 👨‍💼 Module 2: NhanSu (Nhân Sự)

### Mục Đích
Quản lý thông tin nhân viên
Phòng ban, chức vụ, chuyên môn
Hợp đồng, lương, chế độ

### Chức Năng Chính
Chức Năng	Mô Tả	Truy Cập
NhanVien	Danh sách nhân viên	NhanSu > NhanVien
ChucVu	Quản lý chức vụ	NhanSu > ChucVu
ChuyenMon	Quản lý chuyên môn	NhanSu > ChuyenMon
PhongBan	Quản lý phòng ban	NhanSu > PhongBan
NhanSuContract	Quản lý hợp đồng	NhanSu > Hợp Đồng
NhanSuSalaryHistory	Lịch sử lương	NhanSu > Lương
NhanSuRequest	Yêu cầu nhân sự	NhanSu > Yêu Cầu
NhanSuTimeSheet	Bảng chấm công	NhanSu > Chấm Công
NhanSuEmployeeLeaveQuota	Quản lý ngày phép	NhanSu > Phép
NhanSuTermination	Chấm dứt lao động	NhanSu > Chấm Dứt
NhanSuRewards	Khen thưởng	NhanSu > Khen Thưởng
NhanSuDiscipline	Kỷ luật	NhanSu > Kỷ Luật

### API Endpoints (ASP.NET Backend)
// Nhân viên
GET    /api/nhansu/NhanVien
POST   /api/nhansu/NhanVien
GET    /api/nhansu/NhanVien/:id
PUT    /api/nhansu/NhanVien/:id
DELETE /api/nhansu/NhanVien/:id

// Bộ phận
GET    /api/NhanSu/DepartmentGetAllByVM?groupId=...
POST   /api/NhanSu/DepartmentGetAllByVM

// Chức vụ
GET    /api/nhansu/ChucVuGetAllByVM
POST   /api/nhansu/ChucVuGetAllByVM

// Chuyên môn
GET    /api/nhansu/ChuyenMonGetAllByVM
POST   /api/nhansu/ChuyenMonGetAllByVM

// Chi nhánh
GET    /api/chinhanh/getall?groupId=...
POST   /api/chinhanh/GetByVM

### Lỗi Thường Gặp & Cách Khắc Phục
Lỗi	Nguyên Nhân	Cách Khắc Phục
"Vui lòng chọn chi nhánh"	Dropdown chưa load đủ	Chờ loading, reload trang
"Đã tồn tại email"	Email nhân viên bị trùng	Dùng email khác
"Không tìm thấy bộ phận"	Bộ phận đã bị xóa	Chọn bộ phận khác
"Dữ liệu không hợp lệ"	Thiếu trường bắt buộc	Kiểm tra: Tên, Email, Chức vụ, Phòng ban
"Không có quyền sửa"	Thiếu permission sửa	Liên hệ quản trị viên
Dropdown trống	API chưa trả về dữ liệu	Kiểm tra API, check internet

## 👥 Module 3: QLNV (Quản Lý Nhân Viên)

### Mục Đích
Quản lý danh sách nhân viên công ty
Duyệt/hủy duyệt tuyển dụng
Quản lý trạng thái nhân viên

### Cách Sử Dụng
Tác Vụ	Bước	Kết Quả
Xem danh sách	Mở QLNV > NhanVien	✓ Hiển thị table nhân viên
Thêm nhân viên	Bấm "Thêm nhân viên" → Chọn chi nhánh → Nhập thông tin → "Thêm"	✓ Chờ duyệt (IsActive=0)
Thêm + Tài Khoản	Bấm "Thêm nhân viên + Tài Khoản" → ... → "Thêm"	✓ Tạo cả user login
Sửa nhân viên	Bấm nút Chỉnh sửa (bút) → Thay đổi → "Cập nhật"	✓ Cập nhật thông tin
Duyệt nhân viên	Bấm ✓ Duyệt → Xác nhận	✓ IsActive=3 (được duyệt)
Hủy duyệt	Bấm ✗ Hủy Duyệt → Xác nhận	✓ Quay lại chờ duyệt
Xóa nhân viên	Bấm nút Xóa (thùng rác) → "Xóa cho tất cả" → Duyệt	✓ IsActive=100 (xóa)

### Cấu Trúc Dữ Liệu (SQL Server)
QLNV_NhanVien
├─ Id: GUID (primary key)
├─ TenNhanVien: string
├─ TaiKhoan: string (email)
├─ CompanyId: GUID (chi nhánh)
├─ DepartmentId: GUID (bộ phận)
├─ ChucVuId: GUID (chức vụ)
├─ ChuyenMonId: GUID (chuyên môn)
├─ IsActive: 0=chờ duyệt, 1=từ chối, 2=chờ xóa, 3=đã duyệt, 100=xóa
├─ IsStatus: string ("Chờ duyệt", "Đã duyệt", "Từ chối", etc)
├─ ApprovalId: GUID (bước duyệt hiện tại)
├─ ApprovalOrder: int (thứ tự duyệt)
├─ ApprovalUserId: GUID (người duyệt)
├─ DateApproval: DateTime (ngày duyệt)
└─ CreateBy, CreateAt: audit fields

### API Endpoints (QLNV)
// Danh sách
POST   /api/nhanvien/GetByVM?groupId=...
GET    /api/nhanvien/:id
GET    /api/nhanvien/GetById?id=...

// Tạo
POST   /api/nhanvien
POST   /api/nhanvien/CreateNhanVienNotTaiKhoan

// Cập nhật
PUT    /api/nhanvien/:id?userName=...

// Xóa
DELETE   /api/nhanvien/:id?userName=...

// Duyệt
POST   /api/nhanvien/Duyet?id=...
POST   /api/nhanvien/HuyDuyet?id=...

// Kiểm tra
GET    /api/nhanvien/CheckExist
GET    /api/nhanvien/IsIdInUse/:id

### Trạng Thái (IsActive)
┌─────────┬──────────────────────┬─────────────────────┐
│ Value   │ Mô Tả                │ Hành Động            │
├─────────┼──────────────────────┼─────────────────────┤
│ 0       │ Chờ duyệt (bước 1)    │ Duyệt → Chấp nhận    │
│ 1       │ Chờ duyệt (bước 2+)   │ Duyệt lần lượt       │
│ 2       │ Chờ xóa              │ Duyệt xóa            │
│ 3       │ ✓ Đã duyệt (chính thức)│ Chỉnh sửa/xóa      │
│ 100     │ ✗ Đã xóa (vĩnh viễn)  │ Không thể khôi phục  │
└─────────┴──────────────────────┴─────────────────────┘

### Quy Trình Duyệt (Workflow)
Người dùng tạo nhân viên
      ↓
IsActive = 0 (Chờ duyệt)
      ↓
Duyệt lần 1 → Đi bước 2 (nếu có)
      ↓
Duyệt lần 2 → ... (tùy config)
      ↓
IsActive = 3 ✓ (Đã duyệt)
      ↓
Tạo tài khoản (nếu chưa có)

### Lỗi Thường Gặp & Cách Khắc Phục
Lỗi	Nguyên Nhân	Cách Khắc Phục
"Bạn không có quyền"	Thiếu quyền Thêm/Sửa/Xóa	Yêu cầu cấp quyền từ Admin
"Không tìm thấy bộ phận"	Bộ phận không tồn tại	Tạo bộ phận trước
"Đã tồn tại"	Email/tài khoản bị trùng	Dùng email khác
"Dữ liệu đã bị thay đổi"	Người khác đang edit	Refresh và thử lại
"Không thể xóa"	Nhân viên được liên kết dữ liệu	Xóa dữ liệu liên kết trước
"Thông tin chờ"	IsActive=2 (chờ xóa)	Chờ duyệt xóa hoặc hủy
Dropdown load chậm	API lag hoặc mạng yếu	Kiểm tra speed internet

## 🔑 Quyền Hạn (Permission)

### Các Loại Quyền (PermissionType)
3 = Thêm (Create)
4 = Sửa (Update)
5 = Xóa (Delete)

### Cách Check Quyền
C#
// Backend sẽ check:
bool hasPermission = await _permissionService
  .HasPermissionAsync(userName, parentMajorId, majorId, permissionType);

// Nếu false → Trả về:
{
  "success": false,
  "message": "Bạn không có quyền [thêm/sửa/xóa]"
}

### Cách Cấp Quyền
Code
Vào: HeThong > Phân Quyền
  → Chọn User
  → Chọn Module (NhanVien, QLNV, etc)
  → Check: Thêm ☑, Sửa ☑, Xóa ☑
  → Lưu

## 🚨 Lỗi Chung & Troubleshooting

1. Lỗi API "401 Unauthorized"
   → JWT Token hết hạn hoặc không hợp lệ
   → Cách khắc phụ: Đăng xuất & đăng nhập lại

2. Lỗi "Không load được dữ liệu"
   → API không response hoặc SQL Server down
   → Cách khắc phụ: F5 reload, kiểm tra API, restart backend

3. Lỗi Dropdown trống
   → API chưa trả dữ liệu hoặc lag
   → Cách khắc phụ: Chọn cha trước (Chi nhánh → Bộ phận → Chức vụ)

4. Modal không đóng sau khi lưu
   → Lỗi JavaScript hoặc API chưa return success
   → Cách khắc phụ: F12 console kiểm tra error, liên hệ dev

5. Dữ liệu không update
   → Lỗi concurrent edit (người khác đang edit)
   → Cách khắc phụ: Refresh trang F5, thử lại

## 📞 Hỗ Trợ Nhanh

Q: Quên mật khẩu?
A: Bấm "Quên mật khẩu" → Nhập email → Check email để reset

Q: Không thêm được nhân viên?
A: Kiểm tra: Chi nhánh, Bộ phận, Chức vụ, Chuyên môn đã chọn chưa?

Q: Tin nhắn chat không gửi?
A: Check internet → Refresh trang → Thử lại

Q: Muốn tăng quyền cho người khác?
A: Vào HeThong > Phân Quyền → Cấp quyền

Q: Dữ liệu cũ đâu rồi?
A: Nó được lưu trên server → Có thể recover từ Admin