

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Models;

namespace TaskAspNet.Data.Context;

public class UserDbContext(DbContextOptions<UserDbContext> options) : IdentityDbContext<AppUser>(options)
{
    
}
