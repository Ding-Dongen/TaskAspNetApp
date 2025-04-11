using Microsoft.AspNetCore.SignalR;
using TaskAspNet.Data.Entities;
using TaskAspNet.Web.Hubs;
using TaskAspNet.Web.Interfaces;

namespace TaskAspNet.Web.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(NotificationEntity notification)
    {
        if (notification.TargetUserId != null)
        {
            await _hubContext.Clients.Group(notification.TargetUserId).SendAsync("ReceiveNotification", notification);
        }
        else
        {
            var notificationType = notification.NotificationType;
            if (notificationType?.TargetGroup != null)
            {
                var roles = notificationType.TargetGroup.Roles.Split(',')
                    .Select(r => r.Trim())
                    .Where(r => !string.IsNullOrEmpty(r))
                    .ToList();

                if (roles.Any())
                {
                    foreach (var role in roles)
                    {
                        await _hubContext.Clients.Group(role).SendAsync("ReceiveNotification", notification);
                    }
                }
                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                }
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            }
        }
    }

}
