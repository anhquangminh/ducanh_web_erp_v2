using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.ViewModels.Accounts
{
    public class EmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
