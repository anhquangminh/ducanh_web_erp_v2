using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_CongViecComments")]
    public class QLNV_CongViecComment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_CongViec { get; set; } = "";
        public string? ParentCommentId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận!")]
        public string NoiDung { get; set; } = "";
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; } = "";
        public string? CreateByUserId { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }
        public int IsEdited { get; set; } = 0;
        public int IsActive { get; set; } = 1;
    }
}
