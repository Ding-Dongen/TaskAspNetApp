using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Dtos;

public class MemberDto
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }

    [Required]
    public int JobTitleId { get; set; }
    public JobTitleDto? JobTitle { get; set; }

    
    [Required, DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    
    [Required]
    public int Day { get; set; }
    [Required]
    public int Month { get; set; }
    [Required]
    public int Year { get; set; }

    public string? ProfileImageUrl { get; set; }
    public UploadSelectImgDto ImageData { get; set; } = new UploadSelectImgDto();

    
    public List<MemberAddressDto> Addresses { get; set; } = new List<MemberAddressDto>();

    
    public List<MemberPhoneDto> Phones { get; set; } = new List<MemberPhoneDto>();

    
    public List<ProjectMemberEntity> ProjectMembers { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();

   
    public List<SelectListItem> AvailableJobTitles { get; set; } = new();
    public string? UserId { get; set; }
}
