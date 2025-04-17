using System.ComponentModel.DataAnnotations;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Dtos;

public class ClientDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Client Name is required.")]
    [StringLength(100, ErrorMessage = "Client name cannot exceed 100 characters.")]
    public string ClientName { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }

    public List<MemberAddressDto> Addresses { get; set; } = new();
    public List<MemberPhoneDto> Phones { get; set; } = new();
}