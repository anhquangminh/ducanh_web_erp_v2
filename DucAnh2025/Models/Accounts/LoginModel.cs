using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.Accounts
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Bạn phải nhập email!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mật khẩu!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = true;
    }
}
