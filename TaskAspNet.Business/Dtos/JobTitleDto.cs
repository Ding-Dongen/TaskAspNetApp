using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Business.Dtos;

public class JobTitleDto
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;
}

