using TaskAspNet.Data.Entities;
using TaskAspNet.Business.Dtos;


namespace TaskAspNet.Business.Factories;

public static class MemberUpdateFactory
{
    public static void UpdateEntity(MemberEntity existing, MemberDto dto)
    {
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.JobTitleId = dto.JobTitleId;
        existing.DateOfBirth = new DateTime(dto.Year, dto.Month, dto.Day);
        existing.UserId = dto.UserId;

        if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl))
        {
            existing.ProfileImageUrl = dto.ProfileImageUrl;
        }


       
        foreach (var dtoAddress in dto.Addresses)
        {
            var match = existing.Addresses.FirstOrDefault(x => x.Id == dtoAddress.Id);

            if (match != null)
            {
                
                match.Address = dtoAddress.Address;
                match.ZipCode = dtoAddress.ZipCode;
                match.City = dtoAddress.City;
                match.AddressType = dtoAddress.AddressType;
            }
            else
            {
                
                existing.Addresses.Add(new MemberAddressEntity
                {
                    Address = dtoAddress.Address,
                    ZipCode = dtoAddress.ZipCode,
                    City = dtoAddress.City,
                    AddressType = dtoAddress.AddressType
                });
            }
        }

       
        existing.Addresses.RemoveAll(existingAddress =>
            !dto.Addresses.Any(dtoAddress => dtoAddress.Id == existingAddress.Id));

        
        foreach (var dtoPhone in dto.Phones)
        {
            var match = existing.Phones.FirstOrDefault(p => p.Id == dtoPhone.Id);

            if (match != null)
            {
                match.Phone = dtoPhone.Phone;
                match.PhoneType = dtoPhone.PhoneType;
            }
            else
            {
                existing.Phones.Add(new MemberPhoneEntity
                {
                    Phone = dtoPhone.Phone,
                    PhoneType = dtoPhone.PhoneType
                });
            }
        }

        
        existing.Phones.RemoveAll(existingPhone =>
            !dto.Phones.Any(dtoPhone => dtoPhone.Id == existingPhone.Id));
    }
}