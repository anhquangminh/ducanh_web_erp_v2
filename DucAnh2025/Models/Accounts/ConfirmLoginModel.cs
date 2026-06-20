using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.Accounts
{
    public class ConfirmLoginModel
    {
        [Required(ErrorMessage = "Bạn phải nhập mã xác thực!")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác thực gồm 6 ký tự")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mã xác thực gồm các ký tự từ 0-9")]
        public string Code { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string ErrorMessage { get; set; }
    }
}
