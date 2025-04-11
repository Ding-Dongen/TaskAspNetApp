
using Microsoft.AspNetCore.Identity;
using TaskAspNet.Data.Models;

namespace TaskAspNet.Business.Services;

public class UserService(UserManager<AppUser> userManager, RoleService roleService)
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleService _roleService = roleService;

    public async Task<bool> CreateUserAsync(UserRegistrationForm form)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));

        if (string.IsNullOrWhiteSpace(form.Email))
            throw new ArgumentNullException(nameof(form.Email));

        
        var existingUser = await _userManager.FindByEmailAsync(form.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        
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

        
        var result = await _userManager.CreateAsync(user, form.Password);

        if (!result.Succeeded)
        {
           
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        
        await _roleService.AddUserToRoleAsync(user.Email, "User");

        
        return true;
    }


    public async Task<bool> DeleteUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found.");
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
