
using TaskAspNet.Business.ViewModel;
using TaskAspNet.Data.Entities;


namespace TaskAspNet.Services.Interfaces;

public interface IConsentService
{
    Task<Consent?> GetConsentByUserIdAsync(string userId);
    Task SaveUserConsentAsync(string userId, CookieConsentViewModel model);
}
