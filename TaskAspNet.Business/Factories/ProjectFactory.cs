using TaskAspNet.Data.Entities;
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Factories;

public static class ProjectFactory
{
   
    public static ProjectEntity CreateEntity(ProjectDto dto)
    {
        return new ProjectEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ClientId = dto.ClientId.GetValueOrDefault(1),
            StatusId = dto.StatusId,
            Budget = dto.Budget,
            ImageUrl = dto.ImageData?.CurrentImage ?? dto.ImageData?.SelectedImage,
            //ImageUrl = dto.SelectedLogo,
            ProjectMembers = dto.SelectedMemberIds.Select(memberId => new ProjectMemberEntity
            {
                MemberId = memberId
            }).ToList()
        };
    }

    public static ProjectDto CreateDto(ProjectEntity entity)
    {
        if (entity == null) return null!;

        return new ProjectDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            ClientId = entity.ClientId,
            Client = entity.Client != null ? new ClientDto
            {
                Id = entity.Client.Id,
                ClientName = entity.Client.ClientName
            } : new ClientDto(), 

            StatusId = entity.StatusId,
            Status = entity.Status != null ? new ProjectStatusDto
            {
                Id = entity.Status.Id,
                StatusName = entity.Status.StatusName 
            } : new ProjectStatusDto(), 

            Budget = entity.Budget,
            ImageData = new UploadSelectImgDto
            {
                CurrentImage = entity.ImageUrl
            },
            //SelectedLogo = entity.ImageUrl,

            SelectedMemberIds = entity.ProjectMembers.Select(pm => pm.MemberId).ToList(),


             Members = entity.ProjectMembers
                .Where(pm => pm.Member != null) 
                .Select(pm => new MemberDto
                {
                    Id = pm.Member.Id,
                    FirstName = pm.Member.FirstName,
                    LastName = pm.Member.LastName,
                    Email = pm.Member.Email,
                    ImageData = new UploadSelectImgDto
                    {
                        CurrentImage = pm.Member.ProfileImageUrl
                    }
                }).ToList()
        };
    }
}
