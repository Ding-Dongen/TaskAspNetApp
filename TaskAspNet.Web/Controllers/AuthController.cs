using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskAspNet.Business.Services;
using TaskAspNet.Data.Models;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Web.Interfaces;

namespace TaskAspNet.Web.Controllers
{
    public class AuthController(
        UserService userService,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IMemberService memberService,
        IWebHostEnvironment webHostEnvironment,
        IGoogleAuthHandler googleAuthHandler) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMemberService _memberService = memberService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly IGoogleAuthHandler _googleAuthHandler = googleAuthHandler;

        public IActionResult CreateAcc()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAcc(UserRegistrationForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var existingUser = await _userManager.FindByEmailAsync(form.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with that email already exists.");
                return View(form);
            }

            var nameParts = form.FullName?.Trim().Split(' ', 2);
            var firstName = nameParts?.Length > 0 ? nameParts[0] : "Unknown";
            var lastName = nameParts?.Length > 1 ? nameParts[1] : "";

            var user = new AppUser
            {
                UserName = form.Email,
                Email = form.Email,
                FirstName = firstName,
                LastName = lastName
            };

            var createResult = await _userManager.CreateAsync(user, form.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(form);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(
                "CompleteProfile",
                "Member",
                new
                {
                    fullName = form.FullName,
                    email = form.Email,
                    userId = user.Id
                }
            );
        }


        public IActionResult LogIn(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserLogInForm form, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid login attempt.";
                return View(form);
            }

            var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(form.Email);
                var roles = await _userManager.GetRolesAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);

                string redirectUrl;

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    redirectUrl = returnUrl;
                }
                else if (roles.Contains("SuperAdmin"))
                {
                    redirectUrl = Url.Action("ManageUsers", "Admin")!;
                }
                else if (roles.Contains("Admin"))
                {
                    redirectUrl = Url.Action("Members", "Admin")!;
                }
                else
                {
                    redirectUrl = Url.Action("Index", "Project")!;
                }

                return View("ExternalLoginRedirect", redirectUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(form);
        }




        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return View("ExternalLogoutRedirect", Url.Action("LogIn", "Auth"));
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                TempData["Error"] = $"External login failed: {remoteError}";
                return RedirectToAction("LogIn");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Could not load external login info.";
                return RedirectToAction("LogIn");
            }

            return await _googleAuthHandler.HandleExternalLoginAsync(info);
        }



    }


}
