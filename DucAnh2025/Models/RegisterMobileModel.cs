using DucAnh2025.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models
{
    

    public class RegisterMobileModel : RegisterModel
    {
        [Required(ErrorMessage = "Bạn phải nhập mật khẩu!")]
        public string Password { get; set; }
    }
}
