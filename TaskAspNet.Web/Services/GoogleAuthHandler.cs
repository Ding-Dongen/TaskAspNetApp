using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Data.Models;
using TaskAspNet.Web.Interfaces;

namespace TaskAspNet.Web.Services;

public class GoogleAuthHandler : IGoogleAuthHandler
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMemberService _memberService;

    public GoogleAuthHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMemberService memberService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _memberService = memberService;
    }

    public async Task<IActionResult> HandleExternalLoginAsync(ExternalLoginInfo info)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

        if (string.IsNullOrEmpty(email))
        {
            return new RedirectToActionResult("LogIn", "Auth", null);
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (signInResult.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var member = await _memberService.GetMemberByUserIdAsync(user.Id);

            if (member != null)
            {
                return new RedirectToActionResult("Index", "Project", null); 
            }

            return new RedirectToActionResult("CompleteProfile", "Member", new
            {
                fullName = $"{user.FirstName} {user.LastName}",
                email = user.Email,
                userId = user.Id
            });
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);

            var member = await _memberService.GetMemberByUserIdAsync(existingUser.Id);
            if (member != null)
            {
                return new RedirectToActionResult("Index", "Project", null);
            }

            return new RedirectToActionResult("CompleteProfile", "Member", new
            {
                fullName = $"{existingUser.FirstName} {existingUser.LastName}",
                email = existingUser.Email,
                userId = existingUser.Id
            });
        }

        var newUser = new AppUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            return new RedirectToActionResult("LogIn", "Auth", null);
        }

        await _userManager.AddLoginAsync(newUser, info);
        await _userManager.AddToRoleAsync(newUser, "User");
        await _signInManager.SignInAsync(newUser, isPersistent: false);

        return new RedirectToActionResult("CompleteProfile", "Member", new
        {
            fullName = $"{firstName} {lastName}",
            email = email,
            userId = newUser.Id
        });
    }
}
