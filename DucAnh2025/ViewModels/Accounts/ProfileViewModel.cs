using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.ViewModels.Accounts
{
    public class ProfileViewModel
    {
        public string Username { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
