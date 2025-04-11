using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskAspNet.Web.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user != null)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            var roles = user.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

        var roles = Context.User?.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList() ?? [];

        foreach (var role in roles)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
