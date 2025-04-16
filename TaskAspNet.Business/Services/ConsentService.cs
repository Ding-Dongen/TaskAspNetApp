using TaskAspNet.Business.ViewModel;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;
using TaskAspNet.Services.Interfaces;

namespace TaskAspNet.Services;

public class ConsentService : IConsentService
{
    private readonly IConsentRepository _consentRepository;

    public ConsentService(IConsentRepository consentRepository)
    {
        _consentRepository = consentRepository;
    }

    public async Task<Consent?> GetConsentByUserIdAsync(string userId)
    {
        return await _consentRepository.GetByUserIdAsync(userId);
    }

    public async Task SaveUserConsentAsync(string userId, CookieConsentViewModel model)
    {
        try
        {
            Console.WriteLine($"ConsentService: Starting save for user {userId}"); 
            
            var existingConsent = await _consentRepository.GetByUserIdAsync(userId);
            Console.WriteLine($"ConsentService: Existing consent found: {existingConsent != null}"); 

            if (existingConsent == null)
            {
                Console.WriteLine("ConsentService: Creating new consent"); 
                var newConsent = new Consent
                {
                    UserId = userId,
                    IsConsentGiven = (model.FunctionalCookies || model.AnalyticsCookies || model.MarketingCookies || model.AdvertisingCookies),
                    FunctionalCookies = model.FunctionalCookies,
                    AnalyticsCookies = model.AnalyticsCookies,
                    MarketingCookies = model.MarketingCookies,
                    AdvertisingCookies = model.AdvertisingCookies,
                    DateGiven = DateTime.UtcNow
                };
                await _consentRepository.AddAsync(newConsent);
                Console.WriteLine("ConsentService: New consent added successfully");
            }
            else
            {
                Console.WriteLine("ConsentService: Updating existing consent"); 
                existingConsent.IsConsentGiven = (model.FunctionalCookies || model.AnalyticsCookies || model.MarketingCookies || model.AdvertisingCookies);
                existingConsent.FunctionalCookies = model.FunctionalCookies;
                existingConsent.AnalyticsCookies = model.AnalyticsCookies;
                existingConsent.MarketingCookies = model.MarketingCookies;
                existingConsent.AdvertisingCookies = model.AdvertisingCookies;
                existingConsent.DateGiven = DateTime.UtcNow;

                await _consentRepository.UpdateAsync(existingConsent);
                Console.WriteLine("ConsentService: Existing consent updated successfully"); 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ConsentService Error: {ex.Message}");
            Console.WriteLine($"ConsentService Stack Trace: {ex.StackTrace}");
            throw; 
        }
    }

}
