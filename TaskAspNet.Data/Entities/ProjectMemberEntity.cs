

namespace TaskAspNet.Data.Entities;

public class ProjectMemberEntity
{
    public int ProjectId { get; set; }

    public  ProjectEntity Project { get; set; } 

    public int MemberId { get; set; }

    public  MemberEntity Member { get; set; } 
}
