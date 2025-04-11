using System.ComponentModel.DataAnnotations;

public class MemberPhoneDto
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(50)]
    public string PhoneType { get; set; } = "Mobile";

    public int MemberId { get; set; }
}
