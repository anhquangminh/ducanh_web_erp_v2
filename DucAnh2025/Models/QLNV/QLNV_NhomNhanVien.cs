using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_NhomNhanViens")]
    public class QLNV_NhomNhanVien
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng chọn người quản lý!")]
        public string Id_QuanLy { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm!")]
        public string TenNhom { get; set; }
        public string IconName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh!")]
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; } = 0;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.Now;
        public string? ApprovalDept { get; set; }
        public string? DepartmentId { get; set; }
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; }
        public string? LastApprovalId { get; set; }
        public string IsStatus { get; set; } = "";
    }

    [Table("QLNV_NhomNhanVien_Logs")]
    public class QLNV_NhomNhanVien_Log
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng chọn người quản lý!")]
        public string Id_QuanLy { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm!")]
        public string TenNhom { get; set; }
        public string IconName { get; set; } = "";
        [Required(ErrorMessage = "Vui lòng chọn chi nhánh!")]
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; } = 0;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.Now;
        public string? ApprovalDept { get; set; }
        public string? DepartmentId { get; set; }
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; }
        public string? LastApprovalId { get; set; }
        public string IsStatus { get; set; } = "";
        public string IdChung { get; set; } = "";
        public bool IsValid { get; set; } = false;
    }
}
