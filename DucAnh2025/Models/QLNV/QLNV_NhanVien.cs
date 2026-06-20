using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_NhanViens")]
    public class QLNV_NhanVien
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng chọn tên nhân viên!")]
        public string TenNhanVien { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tài khoản!")]
        [EmailAddress(ErrorMessage = "Tài khoản phải là một địa chỉ email hợp lệ!")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh!")]
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";


        [Required(ErrorMessage = "Vui lòng chọn bộ phận!")]
        public string DepartmentId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn chức vụ!")]
        public string ChucVuId { get; set; } = "";
        [Required(ErrorMessage = "Vui lòng chọn chuyên môn!")]
        public string ChuyenMonId { get; set; } = "";


        public int Ordinarily { get; set; } = 0;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.Now;
        public string? ApprovalDept { get; set; }
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; }
        public string? LastApprovalId { get; set; }
        public string IsStatus { get; set; } = "";
    }
    
    [Table("QLNV_NhanVien_Logs")]
    public class QLNV_NhanVien_Log
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng chọn tên nhân viên!")]
        public string TenNhanVien { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tài khoản!")]
        [EmailAddress(ErrorMessage = "Tài khoản phải là một địa chỉ email hợp lệ!")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh!")]
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn bộ phận!")]
        public string DepartmentId { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn chức vụ!")]
        public string ChucVuId { get; set; } = "";
        [Required(ErrorMessage = "Vui lòng chọn chuyên môn!")]
        public string ChuyenMonId { get; set; } = "";

        public int Ordinarily { get; set; } = 0;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

        public string? ApprovalUserId { get; set; }
        public DateTime? DateApproval { get; set; } = DateTime.Now;
        public string? ApprovalDept { get; set; }
        public int DepartmentOrder { get; set; } = 0;
        public int ApprovalOrder { get; set; } = 0;
        public string? ApprovalId { get; set; }
        public string? LastApprovalId { get; set; }
        public string IsStatus { get; set; } = "";
        public string IdChung { get; set; } = "";
        public bool IsValid { get; set; } = false;
    }
}
