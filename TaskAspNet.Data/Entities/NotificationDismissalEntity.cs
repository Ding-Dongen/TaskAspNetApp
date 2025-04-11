
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAspNet.Data.Entities;

public class NotificationDismissalEntity
{
    [Key, Column(Order = 0)]
    public int NotificationId { get; set; }

    [Key, Column(Order = 1)]
    public string UserId { get; set; } = string.Empty;

    public DateTime DismissedAt { get; set; }

    public NotificationEntity Notification { get; set; } = null!;
}
