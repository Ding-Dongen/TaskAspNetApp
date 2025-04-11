
using TaskAspNet.Data.Models;

namespace TaskAspNet.Business.Interfaces;

public interface IUserService
{
    Task<bool> CreateUserAsync(UserRegistrationForm form);
    Task<bool> DeleteUserAsync(string userId);
}
