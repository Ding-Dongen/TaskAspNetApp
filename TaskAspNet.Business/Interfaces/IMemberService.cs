using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetMembersByIdAsync(int id);
    Task<MemberDto?> GetMemberByIdAsync(int id);

    Task<IEnumerable<MemberDto>> GetAllMembersAsync();
    Task<MemberDto> AddMemberAsync(MemberDto member);
    Task<MemberDto> UpdateMemberAsync(int id, MemberDto member);
    Task<MemberDto> DeleteMemberAsync(int id);
    Task<List<MemberDto>> SearchMembersAsync(string searchTerm);
    Task<List<JobTitleDto>> GetAllJobTitlesAsync();
    Task<MemberDto> GetMemberByUserIdAsync(string userId);
}
