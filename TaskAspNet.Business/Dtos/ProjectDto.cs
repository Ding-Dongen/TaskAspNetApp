
using System.ComponentModel.DataAnnotations;


namespace TaskAspNet.Business.Dtos;

public class ProjectDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string Description { get; set; } = null!;


    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }


    public ClientDto Client { get; set; } = new ClientDto();
    public int? ClientId { get; set; }

    [Required]
    public int StatusId { get; set; }
    public ProjectStatusDto? Status { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value.")]
    public decimal Budget { get; set; } = 0;

    public UploadSelectImgDto ImageData { get; set; } = new UploadSelectImgDto();

    //[Required(ErrorMessage = "Select a file to upload.")]
    //public IFormFile ImageFile { get; set; } = null!;
    //public string? SelectedLogo { get; set; }

    public List<int> SelectedMemberIds { get; set; } = new();
    public List<MemberDto> Members { get; set; } = [];

    public string? CreatedByUserId { get; set; }
}

