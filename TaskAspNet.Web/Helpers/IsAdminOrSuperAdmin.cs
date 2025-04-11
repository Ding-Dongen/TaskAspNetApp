using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAdminOrSuperAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") || user.IsInRole("SuperAdmin");
    }
}
