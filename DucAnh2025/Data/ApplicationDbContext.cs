using DucAnh2025.Models;
using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.CongTrinh;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.Kho;
using DucAnh2025.Models.Kho.DanhMuc;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Models.QLNV;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> 
    {
        //private readonly IHubContext<NotificationHub> _hubContext;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<ApprovalTask> ApprovalTasks { get; set; }
        public DbSet<ApprovalTask_Log> ApprovalTask_Logs { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<MajorUserPermission> MajorUserPermissions { get; set; }
        public DbSet<MajorUserPermission_Log> MajorUserPermission_Logs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Department_Log> Department_Logs { get; set; }
        public DbSet<ChiNhanh> ChiNhanhs { get; set; }
        public DbSet<ChiNhanh_Log> ChiNhanh_Logs { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<CompanyType_Log> CompanyType_Logs { get; set; }
        public DbSet<MCompany> MCompanies { get; set; }

        public DbSet<UserFcmToken> UserFcmTokens { get; set; }
        public DbSet<NotificationFireBase> NotificationFireBases { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalDeptSetting> ApprovalDeptSettings { get; set; }
        public DbSet<ApprovalDeptSetting_Log> ApprovalDeptSetting_Logs { get; set; }
        public DbSet<ApprovalStepSetting> ApprovalStepSettings { get; set; }
        public DbSet<ApprovalStepSetting_Log> ApprovalStepSetting_Logs { get; set; }
        public DbSet<ApprovalStaffSetting> ApprovalStaffSettings { get; set; }
        public DbSet<ApprovalControl> ApprovalControls { get; set; }
        public DbSet<ApprovalControl_Log> ApprovalControl_Logs { get; set; }
        public DbSet<PermissionControl> PermissionControls { get; set; }
        public DbSet<PermissionControl_Log> PermissionControl_Logs { get; set; }
        public DbSet<MajorUserApproval> MajorUserApprovals { get; set; }
        public DbSet<MajorUserApproval_Log> MajorUserApproval_Logs { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        
        //Danh muc nhan su
        public DbSet<DM_ChucVu> DM_ChucVus { get; set; }
        public DbSet<DM_ChucVu_Log> DM_ChucVu_Logs { get; set; }
        public DbSet<DM_ChuyenMon> DM_ChuyenMons { get; set; }
        public DbSet<DM_ChuyenMon_Log> DM_ChuyenMon_Logs { get; set; }
        public DbSet<NhanSu_DM_ContractType> NhanSu_DM_ContractTypes { get; set; }
        public DbSet<NhanSu_DM_ContractType_Log> NhanSu_DM_ContractType_Logs { get; set; }   
        public DbSet<NhanSu_DM_WorkStatu> NhanSu_DM_WorkStatus { get; set; }
        public DbSet<NhanSu_DM_WorkStatu_Log> NhanSu_DM_WorkStatu_Logs { get; set; }
        public DbSet<NhanSu_EmployeeProfile> NhanSu_EmployeeProfiles { get; set; }
        public DbSet<NhanSu_EmployeeProfile_Log> NhanSu_EmployeeProfile_Logs { get; set; }
        public DbSet<NhanSu_Contract> NhanSu_Contracts { get; set; }
        public DbSet<NhanSu_Contract_Log> NhanSu_Contract_Logs { get; set; }
        public DbSet<NhanSu_SalaryHistory> NhanSu_SalaryHistorys { get; set; }
        public DbSet<NhanSu_SalaryHistory_Log> NhanSu_SalaryHistory_Logs { get; set; }
        public DbSet<NhanSu_DM_ChangeType> NhanSu_DM_ChangeTypes { get; set; }
        public DbSet<NhanSu_DM_ChangeType_Log> NhanSu_DM_ChangeType_Logs { get; set; }
        public DbSet<NhanSu_DM_TerminationReason> NhanSu_DM_TerminationReasons { get; set; }
        public DbSet<NhanSu_DM_TerminationReason_Log> NhanSu_DM_TerminationReason_Logs { get; set; }
        public DbSet<NhanSu_Termination> NhanSu_Terminations { get; set; }
        public DbSet<NhanSu_Termination_Log> NhanSu_Termination_Logs { get; set; }
        public DbSet<NhanSu_DM_RewardType> NhanSu_DM_RewardTypes { get; set; }
        public DbSet<NhanSu_DM_RewardType_Log> NhanSu_DM_RewardType_Logs { get; set; }
        public DbSet<NhanSu_Reward> NhanSu_Rewards { get; set; }
        public DbSet<NhanSu_Reward_Log> NhanSu_Reward_Logs { get; set; }
        public DbSet<NhanSu_DM_DisciplineType> NhanSu_DM_DisciplineTypes { get; set; }
        public DbSet<NhanSu_DM_DisciplineType_Log> NhanSu_DM_DisciplineType_Logs { get; set; }
        public DbSet<NhanSu_Discipline> NhanSu_Disciplines { get; set; }
        public DbSet<NhanSu_Discipline_Log> NhanSu_Discipline_Logs { get; set; }
        public DbSet<NhanSu_DM_LeaveType> NhanSu_DM_LeaveTypes { get; set; }
        public DbSet<NhanSu_DM_LeaveType_Log> NhanSu_DM_LeaveType_Logs { get; set; }
        public DbSet<NhanSu_DM_RequestType> NhanSu_DM_RequestTypes { get; set; }
        public DbSet<NhanSu_DM_RequestType_Log> NhanSu_DM_RequestType_Logs { get; set; }
        public DbSet<NhanSu_Request> NhanSu_Requests { get; set; }
        public DbSet<NhanSu_Request_Log> NhanSu_Request_Logs { get; set; }
        public DbSet<NhanSu_DM_WorkType> NhanSu_DM_WorkTypes { get; set; }
        public DbSet<NhanSu_DM_WorkType_Log> NhanSu_DM_WorkType_Logs { get; set; }
        public DbSet<NhanSu_TimeSheet> NhanSu_TimeSheets { get; set; }
        public DbSet<NhanSu_TimeSheet_Log> NhanSu_TimeSheet_Logs { get; set; }
        public DbSet<NhanSu_EmployeeLeaveQuota> NhanSu_EmployeeLeaveQuotas { get; set; }
        public DbSet<NhanSu_EmployeeLeaveQuota_Log> NhanSu_EmployeeLeaveQuota_Logs { get; set; }
        public DbSet<NhanSu_AppointmentsHistory> NhanSu_AppointmentsHistorys { get; set; }
        public DbSet<NhanSu_AppointmentsHistory_Log> NhanSu_AppointmentsHistory_Logs { get; set; }

        //QLNV
        public DbSet<QLNV_NhanVien> QLNV_NhanViens { get; set; }
        public DbSet<QLNV_NhanVien_Log> QLNV_NhanVien_Logs { get; set; }
        public DbSet<QLNV_NhomNhanVien> QLNV_NhomNhanViens { get; set; }
        public DbSet<QLNV_NhomNhanVien_Log> QLNV_NhomNhanVien_Logs { get; set; }
        public DbSet<QLNV_QuanLyNhanVien> QLNV_QuanLyNhanViens { get; set; }
        public DbSet<QLNV_QuanLyNhanVien_Log> QLNV_QuanLyNhanVien_Logs { get; set; }
        public DbSet<QLNV_CongViec> QLNV_CongViecs { get; set; }
        public DbSet<QLNV_CongViecCon> QLNV_CongViecCons { get; set; }
        public DbSet<QLNV_CongViecComment> QLNV_CongViecComments { get; set; }
        public DbSet<QLNV_CongViecMention> QLNV_CongViecMentions { get; set; }
        public DbSet<QLNV_CongViecWatcher> QLNV_CongViecWatchers { get; set; }
        public DbSet<QLNV_CongViecActivity> QLNV_CongViecActivities { get; set; }
        public DbSet<QLNV_CongViecEvent> QLNV_CongViecEvents { get; set; }
        public DbSet<QLNV_DanhGia> QLNV_DanhGias { get; set; }
        public DbSet<QLNV_NhanVienThucHien> QLNV_NhanVienThucHiens { get; set; }
        public DbSet<QLNV_ThemNgay> QLNV_ThemNgays { get; set; }

        //Công trình
        public DbSet<CT_DM_TrangThaiThiCong> CT_DM_TrangThaiThiCongs { get; set; }
        public DbSet<CT_DM_TrangThaiThiCong_Log> CT_DM_TrangThaiThiCong_Logs { get; set; }
        public DbSet<CT_DM_HinhThucDayHoGa> CT_DM_HinhThucDayHoGas { get; set; }
        public DbSet<CT_DM_HinhThucDayHoGa_Log> CT_DM_HinhThucDayHoGa_Logs { get; set; }
        public DbSet<CT_DM_LoaiDauNoi> CT_DM_LoaiDauNois { get; set; }
        public DbSet<CT_DM_LoaiDauNoi_Log> CT_DM_LoaiDauNoi_Logs { get; set; }
        public DbSet<CT_DM_HinhThucDapTra> CT_DM_HinhThucDapTras { get; set; }
        public DbSet<CT_DM_HinhThucDapTra_Log> CT_DM_HinhThucDapTra_Logs { get; set; }
        public DbSet<CT_DM_TenLoaiThep> CT_DM_TenLoaiTheps { get; set; }
        public DbSet<CT_DM_TenLoaiThep_Log> CT_DM_TenLoaiThep_Logs { get; set; }
        public DbSet<CT_DM_DanhMucThep> CT_DM_DanhMucTheps { get; set; }
        public DbSet<CT_DM_DanhMucThep_Log> CT_DM_DanhMucThep_Logs { get; set; }
        public DbSet<CT_DM_HangMucCongViec> CT_DM_HangMucCongViecs { get; set; }
        public DbSet<CT_DM_HangMucCongViec_Log> CT_DM_HangMucCongViec_Logs { get; set; }
        public DbSet<CT_DM_HangMucKhoiLuong> CT_DM_HangMucKhoiLuongs { get; set; }
        public DbSet<CT_DM_HangMucKhoiLuong_Log> CT_DM_HangMucKhoiLuong_Logs { get; set; }
        public DbSet<CT_DM_LoaiCauKien> CT_DM_LoaiCauKiens { get; set; }
        public DbSet<CT_DM_LoaiCauKien_Log> CT_DM_LoaiCauKien_Logs { get; set; }
        public DbSet<CT_DM_LoaiKhoiLuong> CT_DM_LoaiKhoiLuongs { get; set; }
        public DbSet<CT_DM_LoaiKhoiLuong_Log> CT_DM_LoaiKhoiLuong_Logs { get; set; }
        public DbSet<CT_DM_ThongTinVatTu> CT_DM_ThongTinVatTus { get; set; }
        public DbSet<CT_DM_ThongTinVatTu_Log> CT_DM_ThongTinVatTu_Logs { get; set; }
        public DbSet<CT_DM_TenCongTac> CT_DM_TenCongTacs { get; set; }
        public DbSet<CT_DM_TenCongTac_Log> CT_DM_TenCongTac_Logs { get; set; }
        public DbSet<CT_DM_TuyenDuong> CT_DM_TuyenDuongs { get; set; }
        public DbSet<CT_DM_TuyenDuong_Log> CT_DM_TuyenDuong_Logs { get; set; }
        public DbSet<CT_DM_LyTrinh> CT_DM_LyTrinhs { get; set; }
        public DbSet<CT_DM_LyTrinh_Log> CT_DM_LyTrinh_Logs { get; set; }
        public DbSet<PKKL_OngNhua_1TtChungNSachDoc> PKKL_OngNhua_1TtChungNSachDocs { get; set; }
        public DbSet<PKKL_OngNhua_1TtChungNSachDoc_Log> PKKL_OngNhua_1TtChungNSachDoc_Logs { get; set; }
        public DbSet<PKKL_OngNhua_2TtChungNSachNgang> PKKL_OngNhua_2TtChungNSachNgangs { get; set; }
        public DbSet<PKKL_OngNhua_2TtChungNSachNgang_Log> PKKL_OngNhua_2TtChungNSachNgang_Logs { get; set; }
        public DbSet<PKKL_OngNhua_3TTinLDatVanTruCHoa> PKKL_OngNhua_3TTinLDatVanTruCHoas { get; set; }
        public DbSet<PKKL_OngNhua_3TTinLDatVanTruCHoa_Log> PKKL_OngNhua_3TTinLDatVanTruCHoa_Logs { get; set; }
        public DbSet<PKKL_OngNhua_4ThongTinChungHGa> PKKL_OngNhua_4ThongTinChungHGas { get; set; }
        public DbSet<PKKL_OngNhua_4ThongTinChungHGa_Log> PKKL_OngNhua_4ThongTinChungHGa_Logs { get; set; }
        public DbSet<PKKL_OngNhua_47TKThepHGa> PKKL_OngNhua_47TKThepHGas { get; set; }
        public DbSet<PKKL_OngNhua_47TKThepHGa_Log> PKKL_OngNhua_47TKThepHGa_Logs { get; set; }
        public DbSet<PKKL_OngNhua_410TKThepTDan> PKKL_OngNhua_410TKThepTDans { get; set; }
        public DbSet<PKKL_OngNhua_410TKThepTDan_Log> PKKL_OngNhua_410TKThepTDan_Logs { get; set; }
        //Kho
        public DbSet<Kho_DM_DanhMucBaoCao> Kho_DM_DanhMucBaoCaos { get; set; }
        public DbSet<Kho_DM_DanhMucBaoCao_Log> Kho_DM_DanhMucBaoCao_Logs { get; set; }
        public DbSet<Kho_DM_TenBaoCao> Kho_DM_TenBaoCaos { get; set; }
        public DbSet<Kho_DM_TenBaoCao_Log> Kho_DM_TenBaoCao_Logs { get; set; }
        public DbSet<Kho_DM_NhomNhienLieu> Kho_DM_NhomNhienLieus { get; set; }
        public DbSet<Kho_DM_NhomNhienLieu_Log> Kho_DM_NhomNhienLieu_Logs { get; set; }
        public DbSet<Kho_DM_LoaiNhienLieu> Kho_DM_LoaiNhienLieus { get; set; }
        public DbSet<Kho_DM_LoaiNhienLieu_Log> Kho_DM_LoaiNhienLieu_Logs { get; set; }
        public DbSet<Kho_DM_NhanHieu> Kho_DM_NhanHieus { get; set; }
        public DbSet<Kho_DM_NhanHieu_Log> Kho_DM_NhanHieu_Logs { get; set; }
        public DbSet<Kho_DM_DonVi> Kho_DM_DonVis { get; set; }
        public DbSet<Kho_DM_DonVi_Log> Kho_DM_DonVi_Logs { get; set; }
        public DbSet<Kho_DM_LoaiNhaCungCap> Kho_DM_LoaiNhaCungCaps { get; set; }
        public DbSet<Kho_DM_LoaiNhaCungCap_Log> Kho_DM_LoaiNhaCungCap_Logs { get; set; }
        public DbSet<Kho_DM_NhaCungCap> Kho_DM_NhaCungCaps { get; set; }
        public DbSet<Kho_DM_NhaCungCap_Log> Kho_DM_NhaCungCap_Logs { get; set; }
        public DbSet<Kho_DM_NhomPhuTung> Kho_DM_NhomPhuTungs { get; set; }
        public DbSet<Kho_DM_NhomPhuTung_Log> Kho_DM_NhomPhuTung_Logs { get; set; }
        public DbSet<kho_DM_LoaiPhuTung> kho_DM_LoaiPhuTungs { get; set; }
        public DbSet<kho_DM_LoaiPhuTung_Log> kho_DM_LoaiPhuTung_Logs { get; set; }
        public DbSet<Kho_HDMuaNhienLieu> Kho_HDMuaNhienLieus { get; set; }
        public DbSet<Kho_HDMuaNhienLieu_Log> Kho_HDMuaNhienLieu_Logs { get; set; }
        public DbSet<Kho_NhapkhoNhienLieu> Kho_NhapkhoNhienLieus { get; set; }
        public DbSet<Kho_NhapkhoNhienLieu_Log> Kho_NhapkhoNhienLieu_Logs { get; set; }
        public DbSet<Kho_XuatKhoNhienLieu> Kho_XuatKhoNhienLieus { get; set; }
        public DbSet<Kho_XuatKhoNhienLieu_Log> Kho_XuatKhoNhienLieu_Logs { get; set; }
        public DbSet<Kho_NhapKhoPhuTung> Kho_NhapKhoPhuTungs { get; set; }
        public DbSet<Kho_NhapKhoPhuTung_Log> Kho_NhapKhoPhuTung_Logs { get; set; }
        public DbSet<Kho_XuatKhoPhuTung> Kho_XuatKhoPhuTungs { get; set; }
        public DbSet<Kho_XuatKhoPhuTung_Log> Kho_XuatKhoPhuTung_Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<EmailHistory>(entity =>
            {
                entity.ToTable(tb => tb.HasTrigger("EmailHistories_SentMail"));

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.CompanyId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreateBy).IsRequired();
                entity.Property(e => e.MajorId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Receiver)
                    .IsRequired()
                    .IsUnicode(false);
                entity.Property(e => e.MajorId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Subject).IsRequired();

            });

            modelBuilder.Entity<QLNV_CongViecComment>()
                .HasIndex(x => new { x.Id_CongViec, x.CreateAt });

            modelBuilder.Entity<QLNV_CongViecMention>()
                .HasIndex(x => new { x.CommentId, x.UserId });

            modelBuilder.Entity<QLNV_CongViecWatcher>()
                .HasIndex(x => new { x.Id_CongViec, x.UserId });

            modelBuilder.Entity<QLNV_CongViecActivity>()
                .HasIndex(x => new { x.Id_CongViec, x.CreateAt });

            modelBuilder.Entity<QLNV_CongViecEvent>()
                .HasIndex(x => new { x.Id_CongViec, x.CreateAt });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Thiết định SignalR
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<EmailHistory>()
                 .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            var result = base.SaveChangesAsync(cancellationToken);


            //if (entries.Any())
            //{
            //    _hubContext.Clients.All.SendAsync("ReceiveMessage", "A new email record has been added.");
            //}

            return result;
        }
    }
}
