

using System.ComponentModel.DataAnnotations;

namespace TaskAspNet.Business.Dtos;

public class UpdateProjectMemberDto
{
        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one member must be assigned.")]
        public List<int> MemberIds { get; set; } = new();
}
