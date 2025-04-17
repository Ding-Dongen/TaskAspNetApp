using TaskAspNet.Business.Dtos;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Factories;

public static class ClientUpdateFactory
{
    public static void Update(ClientEntity entity, ClientDto dto)
    {
        entity.ClientName = dto.ClientName;
        entity.Email = dto.Email;
        entity.Notes = dto.Notes;

        entity.Addresses = dto.Addresses.Select(a => new MemberAddressEntity
        {
            Id = a.Id,
            Address = a.Address,
            ZipCode = a.ZipCode,
            City = a.City,
            AddressType = a.AddressType,
            ClientId = entity.Id
        }).ToList();

        entity.Phones = dto.Phones.Select(p => new MemberPhoneEntity
        {
            Id = p.Id,
            Phone = p.Phone,
            PhoneType = p.PhoneType,
            ClientId = entity.Id
        }).ToList();
    }
}
