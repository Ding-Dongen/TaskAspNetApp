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

    
    public List<ProjectEntity> Projects { get; set; } = new();
}
