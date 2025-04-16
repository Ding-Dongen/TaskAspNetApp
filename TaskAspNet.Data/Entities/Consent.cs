

using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAspNet.Data.Entities;

[Table("Consent", Schema = "dbo")]
public class Consent
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public bool IsConsentGiven { get; set; }
    public bool FunctionalCookies { get; set; }
    public bool AnalyticsCookies { get; set; }
    public bool MarketingCookies { get; set; }
    public bool AdvertisingCookies { get; set; }
    public DateTime DateGiven { get; set; }
}
