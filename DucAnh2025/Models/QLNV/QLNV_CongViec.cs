using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_CongViecs")]
    public class QLNV_CongViec
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_NguoiGiaoViec { get; set; }
       
        [Required(ErrorMessage = "Vui lòng nhập nhóm công việc!")]
        public string NhomCongViec { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu!")]
        public DateTime NgayBatDau { get; set; }= DateTime.UtcNow;

        public DateTime? NgayKetThuc { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mức độ ưu tiên!")]
        public string MucDoUuTien { get; set; }
        public string? TuDanhGia { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tiến độ!")]
        [Range(0, 10, ErrorMessage = "Tiến độ phải nằm trong khoảng từ 0 đến 10!")]
        public double TienDo { get; set; } = 0;
        [Required(ErrorMessage = "Vui lòng nhập lặp lại!")]
        public string LapLai { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên công việc !")]
        public string TenCongViec { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung công việc!")]
        public string NoiDungCongViec { get; set; }
        public string? FileDinhKem { get; set; }="";
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
