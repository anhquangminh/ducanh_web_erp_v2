using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_NhanVienThucHiens")]
    public class QLNV_NhanVienThucHien
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_CongViec { get; set; }
        public string Id_NhanVien { get; set; }
        public string TuDanhGia { get; set; } = "";

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
}
