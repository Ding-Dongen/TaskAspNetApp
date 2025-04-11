
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Data.Entities;

public class NotificationEntity
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; } = false;

    public string? CreatedByUserId { get; set; }
    public string? TargetUserId { get; set; }

    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public int NotificationTypeId { get; set; }
    public NotificationTypeEntity? NotificationType { get; set; }

}
