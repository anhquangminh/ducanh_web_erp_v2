using DucAnh2025.ViewModels.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using DucAnh2025.Models.Accounts;

namespace DucAnh2025.Controllers
{
    [Authorize]
    public class ManageAccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;

        public ManageAccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new ProfileViewModel
            {
                Username = await _userManager.GetUserNameAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
            };
            return View(model);
        }

        // POST: /ManageAccount/Index
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to set phone number.");
                    return View(model);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewBag.StatusMessage = "Your profile has been updated";
            return View(model);
        }

        // GET: /ManageAccount/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /ManageAccount/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("ChangePasswordConfirmation");
        }

        // GET: /ManageAccount/ChangePasswordConfirmation
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        // GET: /ManageAccount/DeletePersonalData
        [HttpGet]
        public IActionResult DeletePersonalData()
        {
            return View();
        }

        // POST: /ManageAccount/DeletePersonalData
        [HttpPost, ActionName("DeletePersonalData")]
        public async Task<IActionResult> DeletePersonalDataConfirmed(string password)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /ManageAccount/Disable2fa
        [HttpGet]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToAction("TwoFactorAuthentication");
            }

            return View();
        }

        // POST: /ManageAccount/Disable2fa
        [HttpPost, ActionName("Disable2fa")]
        public async Task<IActionResult> Disable2faConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error occurred disabling 2FA.");
                return View();
            }

            return RedirectToAction("TwoFactorAuthentication", new { message = "2FA has been disabled." });
        }

        // GET: /ManageAccount/TwoFactorAuthentication
        [HttpGet]
        public IActionResult TwoFactorAuthentication(string message = null)
        {
            ViewBag.StatusMessage = message;
            return View();
        }

        // GET: /ManageAccount/Email
        [HttpGet]
        public async Task<IActionResult> Email()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new EmailViewModel
            {
                Email = await _userManager.GetEmailAsync(user),
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user)
            };
            return View(model);
        }

        // POST: /ManageAccount/Email
        [HttpPost]
        public async Task<IActionResult> Email(EmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var email = await _userManager.GetEmailAsync(user);
            if (model.Email != email)
            {
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmailChange", "Account", new { userId = user.Id, email = model.Email, code }, protocol: Request.Scheme);
                // Send email with this link
                // await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your email by <a href='{callbackUrl}'>clicking here</a>.");
                ViewBag.StatusMessage = "Confirmation link to change email sent. Please check your email.";
            }
            else
            {
                ViewBag.StatusMessage = "Your email is unchanged.";
            }
            return View(model);
        }

        // GET: /ManageAccount/EnableAuthenticator
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(await _userManager.GetEmailAsync(user), unformattedKey)
            };
            return View(model);
        }

        // POST: /ManageAccount/EnableAuthenticator
        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                model.SharedKey = FormatKey(await _userManager.GetAuthenticatorKeyAsync(user));
                model.AuthenticatorUri = GenerateQrCodeUri(await _userManager.GetEmailAsync(user), await _userManager.GetAuthenticatorKeyAsync(user));
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            return RedirectToAction("GenerateRecoveryCodes");
        }

        // GET: /ManageAccount/ExternalLogins
        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();

            var showRemoveButton = user.PasswordHash != null || userLogins.Count > 1;

            var model = new ExternalLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                ShowRemoveButton = showRemoveButton
            };
            return View(model);
        }

        // POST: /ManageAccount/RemoveLogin
        [HttpPost]
        public async Task<IActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                ViewBag.StatusMessage = "The external login was not removed.";
            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.StatusMessage = "The external login was removed.";
            }
            return RedirectToAction("ExternalLogins");
        }

        // GET: /ManageAccount/GenerateRecoveryCodes
        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToAction("EnableAuthenticator");
            }
            return View();
        }

        // POST: /ManageAccount/GenerateRecoveryCodes
        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodesPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToAction("EnableAuthenticator");
            }
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return View("ShowRecoveryCodes", recoveryCodes);
        }

        [HttpGet]
        public async Task<IActionResult> PersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new PersonalDataViewModel
            {
                Email = await _userManager.GetEmailAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                UserName = await _userManager.GetUserNameAsync(user)
            };
            return View(model);
        }

        // GET: /ManageAccount/ResetAuthenticator
        [HttpGet]
        public IActionResult ResetAuthenticator()
        {
            return View();
        }

        // POST: /ManageAccount/ResetAuthenticator
        [HttpPost, ActionName("ResetAuthenticator")]
        public async Task<IActionResult> ResetAuthenticatorConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
            return RedirectToAction("EnableAuthenticator");
        }

        // GET: /ManageAccount/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        // POST: /ManageAccount/SetPassword
        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been set.";
            return RedirectToAction("Index");
        }

        // GET: /ManageAccount/TwoFactorAuthentication
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return View(model);
        }

        // POST: /ManageAccount/ForgetBrowser
        [HttpPost]
        public async Task<IActionResult> ForgetBrowser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            await _signInManager.ForgetTwoFactorClientAsync();
            TempData["StatusMessage"] = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return RedirectToAction("TwoFactorAuthentication");
        }

        // Helper methods
        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }
            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                _urlEncoder.Encode("YourAppName"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
