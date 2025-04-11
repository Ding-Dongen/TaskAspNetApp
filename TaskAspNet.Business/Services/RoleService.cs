
using Microsoft.AspNetCore.Identity;
using TaskAspNet.Data.Models;

namespace TaskAspNet.Business.Services;

public class RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
{

    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task DoesRoleExistAsync()
    {
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    public async Task<bool> AddUserToRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return false;
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> UserHasRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> RemoveUserFromRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return false;
        }
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> AssignUserToAdminAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false; 

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return false; 
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        return result.Succeeded;
    }

    public async Task<bool> RemoveAdminRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false; 

        
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!removeResult.Succeeded)
            {
                return false; 
            }
        }

        
        if (!await _userManager.IsInRoleAsync(user, "User"))
        {
            var addResult = await _userManager.AddToRoleAsync(user, "User");
            return addResult.Succeeded;
        }

        return true; 
    }

}


