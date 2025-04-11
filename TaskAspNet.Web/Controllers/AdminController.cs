using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Business.Services;
using TaskAspNet.Data.Models;

namespace TaskAspNet.Web.Controllers;
public class AdminController(IMemberService memberService, UserManager<AppUser> userManager, RoleService roleService, UserService userService) : Controller
{
    private readonly IMemberService _memberService = memberService;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleService _roleService = roleService;
    private readonly UserService _userService = userService;

    [Authorize(Roles = "Admin, User, SuperAdmin")]
    public async Task<IActionResult> Members()
    {
        var members = await _memberService.GetAllMembersAsync();
        return View(members);
    }
    [Authorize(Roles = "Admin, SuperAdmin")]
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new Dictionary<string, string>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.FirstOrDefault() ?? "User"; 
        }

        ViewData["UserRoles"] = userRoles;
        return View(users);
    }
    [Authorize(Roles = "Admin, SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> AssignAdmin(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user selection.";
            return RedirectToAction("ManageUsers");
        }

        var success = await _roleService.AssignUserToAdminAsync(userId);

        if (success)
        {
            TempData["Success"] = "User has been assigned as Admin.";
            TempData["RoleChangedUserId"] = userId;
        }
        else
        {
            TempData["Error"] = "Failed to assign Admin role.";
        }

        return RedirectToAction("ManageUsers");
    }
    [Authorize(Roles = "Admin, SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user selection.";
            return RedirectToAction("ManageUsers");
        }

        var success = await _roleService.RemoveAdminRoleAsync(userId);

        if (success)
        {
            TempData["Success"] = "Admin role has been removed.";
            TempData["RoleChangedUserId"] = userId;

        }
        else
        {
            TempData["Error"] = "Failed to remove Admin role.";
        }

        return RedirectToAction("ManageUsers");
    }
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user selection.";
            return RedirectToAction("ManageUsers");
        }

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member != null)
        {
            await _memberService.DeleteMemberAsync(member.Id);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            TempData["Success"] = "User and related member deleted.";
        }
        else
        {
            TempData["Error"] = "User not found.";
        }

        return RedirectToAction("ManageUsers");
    }



}
