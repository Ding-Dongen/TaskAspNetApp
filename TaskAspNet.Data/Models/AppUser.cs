

using Microsoft.AspNetCore.Identity;

namespace TaskAspNet.Data.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
