using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TaskAspNet.Data.Entities;

public class MemberPhoneEntity
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    [ProtectedPersonalData]
    public string Phone { get; set; } = string.Empty;

    [StringLength(50)]
    public string PhoneType { get; set; } = "Mobile";

    public int? MemberId { get; set; }
    public MemberEntity? Member { get; set; }

    public int? ClientId { get; set; }
    public ClientEntity? Client { get; set; }
}
