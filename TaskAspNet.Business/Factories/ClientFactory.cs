using TaskAspNet.Business.Dtos;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Factories;

public static class ClientFactory
{
    public static ClientEntity FromDto(ClientDto dto)
    {
        return new ClientEntity
        {
            ClientName = dto.ClientName,
            Email = dto.Email,
            Notes = dto.Notes,
            Addresses = dto.Addresses.Select(a => new MemberAddressEntity
            {
                Address = a.Address,
                ZipCode = a.ZipCode,
                City = a.City,
                AddressType = a.AddressType
            }).ToList(),
            Phones = dto.Phones.Select(p => new MemberPhoneEntity
            {
                Phone = p.Phone,
                PhoneType = p.PhoneType
            }).ToList()
        };
    }

    public static ClientDto ToDto(ClientEntity entity)
    {
        return new ClientDto
        {
            Id = entity.Id,
            ClientName = entity.ClientName,
            Email = entity.Email,
            Notes = entity.Notes,
            Addresses = entity.Addresses?.Select(a => new MemberAddressDto
            {
                Id = a.Id,
                Address = a.Address,
                ZipCode = a.ZipCode,
                City = a.City,
                AddressType = a.AddressType
            }).ToList() ?? new List<MemberAddressDto>(),
            Phones = entity.Phones?.Select(p => new MemberPhoneDto
            {
                Id = p.Id,
                Phone = p.Phone,
                PhoneType = p.PhoneType
            }).ToList() ?? new List<MemberPhoneDto>()
        };
    }

    public static List<ClientDto> ToDtoList(List<ClientEntity> entities)
    {
        return entities.Select(ToDto).ToList();
    }
}
