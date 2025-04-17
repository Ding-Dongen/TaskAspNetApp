using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TaskAspNet.Data.Entities;

public class MemberAddressEntity
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [ProtectedPersonalData]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(10)]
    [ProtectedPersonalData]
    public string ZipCode { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [ProtectedPersonalData]
    public string City { get; set; } = string.Empty;

    [StringLength(50)]
    public string AddressType { get; set; } = "Home";

    public int? MemberId { get; set; }
    public MemberEntity? Member { get; set; }

    public int? ClientId { get; set; }
    public ClientEntity? Client { get; set; }
}
