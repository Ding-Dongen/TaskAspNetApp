
namespace TaskAspNet.Business.Dtos;

public class MemberIndexViewModel
{   
    public List<MemberDto> AllMembers { get; set; } = new();
    public MemberDto CreateMember { get; set; } = new();

}
