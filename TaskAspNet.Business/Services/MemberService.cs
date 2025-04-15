using TaskAspNet.Business.Factories;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Data.Interfaces;
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Services;

public class MemberService(IMemberRepository memberRepository, INotificationService notificationService) : IMemberService
{
    private readonly IMemberRepository _memberRepository = memberRepository;
    private readonly INotificationService _notificationService = notificationService;


    public async Task<MemberDto> AddMemberAsync(MemberDto memberDto)
{
    var memberEntity = MemberFactory.CreateEntity(memberDto);
    
    await _memberRepository.AddAsync(memberEntity);
    await _memberRepository.SaveAsync();

        var fullName = $"{memberDto.FirstName} {memberDto.LastName}";
        
        var createdByUserId = "[the user ID of who created this member]";

        await _notificationService.NotifyMemberCreatedAsync(fullName, createdByUserId);

        return MemberFactory.CreateDto(memberEntity);
}


    //public async Task<MemberDto> AddMemberAsync(MemberDto member)
    //{
    //    var memberEntity = MemberFactory.CreateEntity(member);
    //    await _memberRepository.AddAsync(memberEntity);
    //    await _memberRepository.SaveAsync();
    //    return member;
    //}

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var members = await _memberRepository.GetMembersWithJobTitleAsync();
        return members.Select(MemberFactory.CreateDto).ToList();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByIdAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        return member is not null ? [MemberFactory.CreateDto(member)] : [];
    }


    public async Task<MemberDto> UpdateMemberAsync(int id, MemberDto memberDto)
    {
        var existingEntity = await _memberRepository.GetMemberByIdWithDetailsAsync(id);
        if (existingEntity == null) return null!;

        
        MemberUpdateFactory.UpdateEntity(existingEntity, memberDto);

        await _memberRepository.SaveAsync();

        await _notificationService.NotifyMemberUpdatedAsync(
       memberName: $"{existingEntity.FirstName} {existingEntity.LastName}",
       updatedByUserId: memberDto.UserId 
   );

        return MemberFactory.CreateDto(existingEntity);
    }



    //public async Task<MemberDto> UpdateMemberAsync(int id, MemberDto memberDto)
    //{
    //    var member = await _memberRepository.GetByIdAsync(id);
    //    if (member is null) return null!;

    //    // ✅ Detach the tracked entity to avoid EF tracking conflict
    //    _memberRepository.DetachEntity(member);

    //    // Rebuild a new entity from DTO (your factory logic)
    //    var updatedMember = MemberFactory.CreateEntity(memberDto);
    //    updatedMember.Id = id;

    //    await _memberRepository.UpdateAsync(updatedMember);
    //    await _memberRepository.SaveAsync();

    //    return memberDto;
    //}

    //public async Task<MemberDto> UpdateMemberAsync(int id, MemberDto memberDto)
    //{
    //    var member = await _memberRepository.GetByIdAsync(id);
    //    if (member is null) return null!;

    //    var updatedMember = MemberFactory.CreateEntity(memberDto);
    //    updatedMember.Id = id;

    //    await _memberRepository.UpdateAsync(updatedMember);
    //    await _memberRepository.SaveAsync();

    //    return memberDto;
    //}
    public async Task<MemberDto> DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member is not null)
        {
            await _memberRepository.DeleteAsync(member);
            await _memberRepository.SaveAsync();
        }
        return null!;
    }

    public async Task<List<MemberDto>> SearchMembersAsync(string searchTerm)
    {
        var entities = await _memberRepository.SearchMembersAsync(searchTerm);

        var dtos = entities.Select(m => new MemberDto
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Email = m.Email,
            ImageData = new UploadSelectImgDto
            {
                CurrentImage = m.ProfileImageUrl
            }
        }).ToList();

        return dtos;
    }

    public async Task<List<JobTitleDto>> GetAllJobTitlesAsync()
    {
        var jobTitleEntities = await _memberRepository.GetAllJobTitlesAsync();
        return jobTitleEntities.Select(e => new JobTitleDto
        {
            Id = e.Id,
            Title = e.Title
        }).ToList();
    }

    public async Task<MemberDto> GetMemberByUserIdAsync(string userId)
    {
        var member = await _memberRepository.GetMemberByUserIdAsync(userId);
        return member != null ? MemberFactory.CreateDto(member) : null!;
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        var member = await _memberRepository.GetMemberByIdWithDetailsAsync(id);
        return member != null ? MemberFactory.CreateDto(member) : null;
    }

}
