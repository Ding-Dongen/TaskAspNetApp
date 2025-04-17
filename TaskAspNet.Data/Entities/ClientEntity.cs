using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskAspNet.Data.Entities;

public class ClientEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [ProtectedPersonalData]
    public string ClientName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();
    public ICollection<MemberAddressEntity> Addresses { get; set; } = new List<MemberAddressEntity>();
    public ICollection<MemberPhoneEntity> Phones { get; set; } = new List<MemberPhoneEntity>();
}
