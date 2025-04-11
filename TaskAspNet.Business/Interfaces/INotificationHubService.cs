using TaskAspNet.Data.Entities;

namespace TaskAspNet.Web.Interfaces;

public interface INotificationHubService
{

    Task SendNotificationAsync(NotificationEntity notification);
}
