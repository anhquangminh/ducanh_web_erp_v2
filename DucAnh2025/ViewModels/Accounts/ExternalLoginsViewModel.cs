using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DucAnh2025.ViewModels.Accounts
{
    public class ExternalLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationScheme> OtherLogins { get; set; }
        public bool ShowRemoveButton { get; set; }
    }
}
