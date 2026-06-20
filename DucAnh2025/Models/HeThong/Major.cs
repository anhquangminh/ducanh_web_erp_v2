using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class Major
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ParentId { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập tên nghiệp vụ!")]
        public string MajorName { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập thứ tự sắp xếp!")]
        public int Order { get; set; }
        public string? Table { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
        public int IsActive { get; set; } = 1;
    }
}
