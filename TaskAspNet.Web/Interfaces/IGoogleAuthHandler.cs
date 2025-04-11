using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TaskAspNet.Web.Interfaces;

public interface IGoogleAuthHandler
{
    Task<IActionResult> HandleExternalLoginAsync(ExternalLoginInfo info);
}
