using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Interfaces;

public interface INotificationTypeRepository : IBaseRepository<NotificationTypeEntity>
{
    Task<NotificationTypeEntity?> GetByNameAsync(string name);
}
