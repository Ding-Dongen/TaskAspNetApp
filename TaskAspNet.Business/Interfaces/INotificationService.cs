using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Interfaces;

public interface INotificationService
{
    Task<List<NotificationEntity>> GetUnreadNotificationsAsync(string userId);
    Task<List<NotificationEntity>> GetNotificationsForUserAsync(string userId, bool includeRead = false);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task NotifyProjectCreatedAsync(int projectId, string projectName, string createdByUserId);
    Task NotifyProjectUpdatedAsync(int projectId, string projectName);
    Task NotifyMemberAddedToProjectAsync(int projectId, string projectName, string memberName, string? addedByUserId = null);
    Task NotifyMemberRemovedFromProjectAsync(int projectId, string projectName, string memberName, string? removedByUserId = null);

    Task NotifyMemberCreatedAsync(string memberName, string createdByUserId);
    Task NotifyMemberUpdatedAsync(string memberName, string updatedByUserId);

    Task DismissNotificationForUserAsync(int notificationId, string userId);
    Task DismissAllNotificationsForUserAsync(string userId);
}
