
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Business.Dtos;

public class ProjectStatusDto
{
    public int Id { get; set; }

    [StringLength(50)]
    public string StatusName { get; set; } = null!;
}

