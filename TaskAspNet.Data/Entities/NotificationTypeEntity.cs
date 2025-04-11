
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Data.Entities;

public class NotificationTypeEntity
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? DefaultMessageTemplate { get; set; }

    public bool IsActive { get; set; } = true;

    public int TargetGroupId { get; set; }
    public NotificationTargetGroupEntity? TargetGroup { get; set; }

    public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
}
