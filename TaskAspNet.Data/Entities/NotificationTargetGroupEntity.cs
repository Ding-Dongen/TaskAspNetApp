
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Data.Entities;

public class NotificationTargetGroupEntity
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Roles { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<NotificationTypeEntity> NotificationTypes { get; set; } = new List<NotificationTypeEntity>();
}
