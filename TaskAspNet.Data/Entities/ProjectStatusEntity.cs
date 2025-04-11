using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Data.Entities;

public class ProjectStatusEntity
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string StatusName { get; set; } = string.Empty;
}
