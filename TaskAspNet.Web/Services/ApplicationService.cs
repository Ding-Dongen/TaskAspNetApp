using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using TaskAspNet.Data.Models;

namespace TaskAspNet.Business.Services;


public class ApplicationService : IApplicationService
{
    private readonly IMemberService _memberService;  
    private readonly IFileService _fileService;
    private readonly UserManager<AppUser> _userManager;

    public ApplicationService(IMemberService memberService, IFileService fileService, UserManager<AppUser> userManager)
    {
        _memberService = memberService;
        _fileService = fileService;
        _userManager = userManager;
    }

    public async Task<(bool Success, MemberDto? Member, string? ErrorMessage)> CreateMemberAsync(MemberDto memberDto)
    {
        try
        {
            if (string.IsNullOrEmpty(memberDto.UserId))
            {
                var existingUser = await _userManager.FindByEmailAsync(memberDto.Email);
                if (existingUser != null)
                {
                    return (false, null, "That email is already in use by another user.");
                }

                var generatedPassword = GenerateSecurePassword();
                var identityUser = new AppUser
                {
                    UserName = memberDto.Email,
                    Email = memberDto.Email,
                    FirstName = memberDto.FirstName ?? "",
                    LastName = memberDto.LastName ?? ""
                };

                var createResult = await _userManager.CreateAsync(identityUser, generatedPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return (false, null, $"Failed to create Identity user: {errors}");
                }

                await _userManager.AddToRoleAsync(identityUser, "User");

                memberDto.UserId = identityUser.Id;
            }
            else
            {
                // If we already have a user, skip re-creation checks entirely.
                // Identity user is created in a prior step; we just create the domain member now.
            }

            string? imagePath = await _fileService.SaveImageAsync(memberDto.ImageData, "members")
                               ?? "/images/membericon/default.png";
            memberDto.ProfileImageUrl = imagePath;

            var createdMember = await _memberService.AddMemberAsync(memberDto);
            if (createdMember == null)
            {
                return (false, null, "Failed to add the member to the database.");
            }

            return (true, createdMember, null);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }





    public async Task<(bool Success, MemberDto? Member, string? ErrorMessage)> UpdateMemberAsync(int id, MemberDto memberDto)
    {
        try
        {
            var existing = await _memberService.GetMembersByIdAsync(id);
            if (existing == null || !existing.Any())
                return (false, null, "Member not found.");

            if (!string.IsNullOrEmpty(memberDto.UserId))
            {
                var user = await _userManager.FindByIdAsync(memberDto.UserId);
                if (user != null)
                {
                    user.FirstName = memberDto.FirstName;
                    user.LastName = memberDto.LastName;
                    user.Email = memberDto.Email;
                    user.UserName = memberDto.Email;

                    var identityResult = await _userManager.UpdateAsync(user);
                    if (!identityResult.Succeeded)
                    {
                        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                        return (false, null, $"Failed to update user: {errors}");
                    }
                }
            }

            string? imagePath = await _fileService.SaveImageAsync(memberDto.ImageData, "members");
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                memberDto.ProfileImageUrl = imagePath;
                memberDto.ImageData.CurrentImage = imagePath;
            }
            else if (!string.IsNullOrWhiteSpace(memberDto.ImageData?.SelectedImage))
            {
                var fileNameOnly = Path.GetFileName(memberDto.ImageData.SelectedImage);

                memberDto.ProfileImageUrl = $"/images/membericon/{fileNameOnly}";
                memberDto.ImageData.CurrentImage = memberDto.ProfileImageUrl;
            }



            var updated = await _memberService.UpdateMemberAsync(id, memberDto);
            if (updated == null)
                return (false, null, "Failed to update member.");

            return (true, updated, null);
        }
        catch (Exception ex)
        {
            var error = ex.InnerException?.Message ?? ex.Message;
            return (false, null, $"Exception: {error}");
        }
    }





    private string GenerateSecurePassword(int length = 12)
    {
        const string upper = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@$?_";

        var random = new Random();

        var passwordChars = new List<char>
{
    upper[random.Next(upper.Length)],
    lower[random.Next(lower.Length)],
    digits[random.Next(digits.Length)],
    special[random.Next(special.Length)]
};

        string allChars = upper + lower + digits + special;
        while (passwordChars.Count < length)
        {
            passwordChars.Add(allChars[random.Next(allChars.Length)]);
        }

        return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
    }


}
