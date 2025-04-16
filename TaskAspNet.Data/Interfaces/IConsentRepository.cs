using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Interfaces;

public interface IConsentRepository : IBaseRepository<Consent>
{
    Task<Consent?> GetByUserIdAsync(string userId);
}
