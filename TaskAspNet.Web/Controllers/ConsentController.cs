using Microsoft.AspNetCore.Mvc;
using TaskAspNet.Business.ViewModel;
using TaskAspNet.Services.Interfaces;

public class ConsentController : Controller
{
    private readonly IConsentService _consentService;

    public ConsentController(IConsentService consentService)
    {
        _consentService = consentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User?.Identity?.Name ?? "anonymous";

        var existingConsent = await _consentService.GetConsentByUserIdAsync(userId);

        if (existingConsent == null)
        {
            var vm = new CookieConsentViewModel
            {
                NecessaryCookies = true,
                IsConsentGiven = false,
                FunctionalCookies = false, 
                AnalyticsCookies = false,
                MarketingCookies = false,
                AdvertisingCookies = false
            };
            ViewBag.HasConsent = false; 
            return View(vm);
        }
        else
        {
            var vm = new CookieConsentViewModel
            {
                NecessaryCookies = true,
                IsConsentGiven = existingConsent.IsConsentGiven,
                FunctionalCookies = existingConsent.FunctionalCookies,
                AnalyticsCookies = existingConsent.AnalyticsCookies,
                MarketingCookies = existingConsent.MarketingCookies,
                AdvertisingCookies = existingConsent.AdvertisingCookies
            };
            ViewBag.HasConsent = existingConsent.IsConsentGiven; 
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([FromForm] CookieConsentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            var userId = User?.Identity?.Name ?? "anonymous";

            model.NecessaryCookies = true;

            model.IsConsentGiven = model.FunctionalCookies || model.AnalyticsCookies ||
                                   model.MarketingCookies || model.AdvertisingCookies;

            await _consentService.SaveUserConsentAsync(userId, model);

            Response.Cookies.Append("CookieConsent", "true", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"An error occurred: {ex.Message}"
            });
        }
    }

}
