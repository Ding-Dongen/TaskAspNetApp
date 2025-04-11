using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Interfaces;

public interface INotificationRepository : IBaseRepository<NotificationEntity>
{
    Task<List<NotificationEntity>> GetUnreadNotificationsAsync(string userId);
    Task<List<NotificationEntity>> GetNotificationsForUserAsync(string userId, bool includeRead = false);
    Task<List<NotificationEntity>> GetNotificationsByTypeAsync(int notificationTypeId);
    Task<List<NotificationEntity>> GetNotificationsForUserAndRolesAsync(string userId, IEnumerable<string> userRoles, bool includeRead = false);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task DismissNotificationAsync(int notificationId, string userId);
    Task DismissAllNotificationsForUserAsync(string userId);
}
