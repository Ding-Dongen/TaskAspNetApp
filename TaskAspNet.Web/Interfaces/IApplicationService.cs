using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Interfaces;

public interface IApplicationService
{
    Task<(bool Success, MemberDto? Member, string? ErrorMessage)> CreateMemberAsync(MemberDto memberDto);
    Task<(bool Success, MemberDto? Member, string? ErrorMessage)> UpdateMemberAsync(int id, MemberDto memberDto);
}
