using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<NotificationEntity>> GetUnreadNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => !n.IsRead && (n.TargetUserId == null || n.TargetUserId == userId))
            .Include(n => n.NotificationType)
            .ThenInclude(nt => nt.TargetGroup)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<NotificationEntity>> GetNotificationsForUserAsync(string userId, bool includeRead = false)
    {
        var query = _context.Notifications
            .Where(n => n.TargetUserId == userId || n.TargetUserId == null); // Ensure proper filtering logic

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .Include(n => n.NotificationType)
            .ThenInclude(nt => nt.TargetGroup)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }


    public async Task<List<NotificationEntity>> GetNotificationsByTypeAsync(int notificationTypeId)
    {
        return await _context.Notifications
            .Where(n => n.NotificationTypeId == notificationTypeId)
            .Include(n => n.NotificationType)
            .ThenInclude(nt => nt.TargetGroup)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }




    public async Task<List<NotificationEntity>> GetNotificationsForUserAndRolesAsync(
        string userId,
        IEnumerable<string> userRoles,
        bool includeRead = false)
    {

        IQueryable<NotificationEntity> query = _context.Notifications
            .Where(n => n.TargetUserId == null || n.TargetUserId == userId)
            .Include(n => n.NotificationType)
                .ThenInclude(nt => nt.TargetGroup);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();

        notifications = notifications.Where(n =>
            !_context.NotificationDismissals.Any(nd => nd.NotificationId == n.Id && nd.UserId == userId)
        ).ToList();

        foreach (var notification in notifications)
        {
            var groupRoles = notification.NotificationType.TargetGroup.Roles
                .Split(',')
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList();

            Console.WriteLine($"DEBUG: Notification ID {notification.Id} target group roles: {string.Join(",", groupRoles)}");
            Console.WriteLine($"DEBUG: User ID {userId} roles: {string.Join(",", userRoles)}");
        }

        var filteredNotifications = notifications.Where(n =>
        {
            var groupRoles = n.NotificationType.TargetGroup.Roles
                .Split(',')
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList();

            if (!groupRoles.Any())
                return true;

            return userRoles.Any(userRole =>
                groupRoles.Any(gr => string.Equals(gr, userRole, StringComparison.OrdinalIgnoreCase))
            );
        }).ToList();

        Console.WriteLine($"DEBUG: Returning {filteredNotifications.Count} notifications for user {userId}");

        return filteredNotifications;
    }

    //public async Task<List<NotificationEntity>> GetNotificationsForUserAndRolesAsync(
    //    string userId,
    //    IEnumerable<string> userRoles,
    //    bool includeRead = false)
    //{
    //    // Build the basic query.
    //    IQueryable<NotificationEntity> query = _context.Notifications
    //        .Where(n => n.TargetUserId == null || n.TargetUserId == userId)
    //        .Include(n => n.NotificationType)
    //        .ThenInclude(nt => nt.TargetGroup);

    //    if (!includeRead)
    //    {
    //        query = query.Where(n => !n.IsRead);
    //    }

    //    var notifications = await query
    //        .OrderByDescending(n => n.CreatedAt)
    //        .ToListAsync();

    //    // Filter out notifications dismissed by this user.
    //    notifications = notifications.Where(n =>
    //        !_context.NotificationDismissals.Any(nd => nd.NotificationId == n.Id && nd.UserId == userId)
    //    ).ToList();

    //    // Filter notifications based on the target group's Roles (as before).
    //    var filteredNotifications = notifications.Where(n =>
    //    {
    //        var groupRoles = n.NotificationType.TargetGroup.Roles
    //            .Split(',')
    //            .Select(r => r.Trim())
    //            .Where(r => !string.IsNullOrEmpty(r))
    //            .ToList();

    //        if (!groupRoles.Any())
    //            return true;

    //        return userRoles.Any(userRole => groupRoles.Contains(userRole));
    //    }).ToList();

    //    return filteredNotifications;
    //}


    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n => !n.IsRead && (n.TargetUserId == null || n.TargetUserId == userId))
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DismissNotificationAsync(int notificationId, string userId)
    {
        bool alreadyDismissed = await _context.NotificationDismissals
            .AnyAsync(nd => nd.NotificationId == notificationId && nd.UserId == userId);
        if (!alreadyDismissed)
        {
            var dismissal = new NotificationDismissalEntity
            {
                NotificationId = notificationId,
                UserId = userId,
                DismissedAt = DateTime.UtcNow
            };
            _context.NotificationDismissals.Add(dismissal);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DismissAllNotificationsForUserAsync(string userId)
    {
        var notificationsToDismiss = await _context.Notifications
            .Where(n => (n.TargetUserId == null || n.TargetUserId == userId) &&
                        !_context.NotificationDismissals.Any(nd => nd.NotificationId == n.Id && nd.UserId == userId))
            .ToListAsync();

        foreach (var notification in notificationsToDismiss)
        {
            var dismissal = new NotificationDismissalEntity
            {
                NotificationId = notification.Id,
                UserId = userId,
                DismissedAt = DateTime.UtcNow
            };
            _context.NotificationDismissals.Add(dismissal);
        }
        await _context.SaveChangesAsync();
    }

}
