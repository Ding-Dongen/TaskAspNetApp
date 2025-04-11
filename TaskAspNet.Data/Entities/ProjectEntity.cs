using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAspNet.Data.Entities;

public class ProjectEntity
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required]
    public int ClientId { get; set; }

    public ClientEntity? Client { get; set; } 

    [Required]
    public int StatusId { get; set; }

    public ProjectStatusEntity? Status { get; set; } 

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Budget { get; set; }

    public string? ImageUrl { get; set; } 

    public List<ProjectMemberEntity> ProjectMembers { get; set; } = new(); 
}
