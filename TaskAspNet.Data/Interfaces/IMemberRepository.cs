using TaskAspNet.Data.Entities;


namespace TaskAspNet.Data.Interfaces;

public interface IMemberRepository : IBaseRepository<MemberEntity>
{
    Task<IEnumerable<MemberEntity>> GetMembersWithJobTitleAsync();
    Task<List<MemberEntity>> SearchMembersAsync(string searchTerm);
    Task<List<JobTitleEntity>> GetAllJobTitlesAsync();
    Task<JobTitleEntity> GetJobTitleByIdAsync(int id);
    Task<MemberEntity> GetMemberByUserIdAsync(string userId);
    Task<MemberEntity> GetMemberByIdWithDetailsAsync(int id);
    void DetachEntity(object entity);
}
