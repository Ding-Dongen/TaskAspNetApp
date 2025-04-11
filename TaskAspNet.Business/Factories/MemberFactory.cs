using TaskAspNet.Data.Entities;
using TaskAspNet.Business.Dtos;


namespace TaskAspNet.Business.Factories;

public static class MemberFactory
{
    public static MemberEntity CreateEntity(MemberDto dto)
    {
        var dateOfBirth = new DateTime(dto.Year, dto.Month, dto.Day);

        string profileImageUrl = null!;
        if (!string.IsNullOrEmpty(dto.ImageData?.SelectedImage))
        {
            profileImageUrl = $"/images/membericon/{dto.ImageData.SelectedImage}";
        }
        else if (!string.IsNullOrEmpty(dto.ImageData?.CurrentImage))
        {
            profileImageUrl = dto.ImageData.CurrentImage;
        }

        
        return new MemberEntity
        {
            Id = dto.Id, 
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            JobTitleId = dto.JobTitleId,
            DateOfBirth = dateOfBirth,
            ProfileImageUrl = profileImageUrl,
            UserId = dto.UserId,

            // Convert addresses and pghones
            Addresses = dto.Addresses.Select(a => new MemberAddressEntity
            {
                Id = a.Id,  
                Address = a.Address,
                ZipCode = a.ZipCode,
                City = a.City,
                AddressType = a.AddressType
            }).ToList(),

            
            Phones = dto.Phones.Select(p => new MemberPhoneEntity
            {
                Id = p.Id, 
                Phone = p.Phone,
                PhoneType = p.PhoneType
            }).ToList()
        };
    }

    public static MemberDto CreateDto(MemberEntity entity)
    {
        
        return new MemberDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            JobTitleId = entity.JobTitleId,
            JobTitle = entity.JobTitle != null
                ? new JobTitleDto { Id = entity.JobTitle.Id, Title = entity.JobTitle.Title }
                : null,

            DateOfBirth = entity.DateOfBirth,
            Day = entity.DateOfBirth.Day,
            Month = entity.DateOfBirth.Month,
            Year = entity.DateOfBirth.Year,
            ProfileImageUrl = entity.ProfileImageUrl,
            UserId = entity.UserId,

            ImageData = new UploadSelectImgDto
            {
                CurrentImage = !string.IsNullOrEmpty(entity.ProfileImageUrl) 
                    ? entity.ProfileImageUrl
                    : "/images/membericon/default.png"
            },

            // Convert entity addresses phones
            Addresses = entity.Addresses?.Select(a => new MemberAddressDto
            {
                Id = a.Id,
                Address = a.Address,
                ZipCode = a.ZipCode,
                City = a.City,
                AddressType = a.AddressType,
                MemberId = entity.Id
            }).ToList() ?? new List<MemberAddressDto>(),

            
            Phones = entity.Phones?.Select(p => new MemberPhoneDto
            {
                Id = p.Id,
                Phone = p.Phone,
                PhoneType = p.PhoneType,
                MemberId = entity.Id
            }).ToList() ?? new List<MemberPhoneDto>()
        };
    }
}
