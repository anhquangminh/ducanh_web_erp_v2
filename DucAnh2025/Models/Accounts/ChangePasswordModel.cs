using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.Accounts
{
    public class ChangePasswordModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [StringLength(16, ErrorMessage = "Mật khẩu mới phải có độ dài tối thiểu {2} ký tự.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Mật khẩu mới phải chứa ít nhất một chữ thường, một chữ hoa, một chữ số và một ký tự đặc biệt.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
