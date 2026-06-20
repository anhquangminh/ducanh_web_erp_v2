using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class Permission
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Bạn phải nhập mã nghiệp vụ")]
        public string MajorId { get; set; } = "";

        [Required(ErrorMessage = "Bạn phải chọn loại quyền")]
        [Range(0, int.MaxValue, ErrorMessage = "Bạn phải chọn loại quyền hợp lệ.")]
        public int PermissionType { get; set; } = 0;

        public string PermissionName { get; set; } = "";
        //public string GroupId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "test.vp@gmail.com";
        public int IsActive { get; set; } = 1;
    }
}
