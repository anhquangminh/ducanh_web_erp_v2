using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.ViewModels.Accounts
{
   
    public class LoginWithRecoveryCodeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
