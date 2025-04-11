
using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Business.Dtos;

public class ClientDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Client Name is required.")]
    [StringLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
    public string ClientName { get; set; } = null!;
}