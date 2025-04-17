
using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Factories;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Business.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientDto>> GetAllClientsAsync()
    {
        var entities = await _clientRepository.GetAllWithDetailsAsync();
        return ClientFactory.ToDtoList(entities);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var entity = await _clientRepository.GetByIdWithDetailsAsync(id);
        return entity == null ? null : ClientFactory.ToDto(entity);
    }


    public async Task CreateClientAsync(ClientDto dto)
    {
        var client = ClientFactory.FromDto(dto);
        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveAsync();
    }


    public async Task UpdateClientAsync(ClientDto dto)
    {
        var client = await _clientRepository.GetByIdWithDetailsAsync(dto.Id);
        if (client == null) throw new Exception("Client not found.");

        ClientUpdateFactory.Update(client, dto);

        await _clientRepository.UpdateAsync(client);
        await _clientRepository.SaveAsync();
    }


    public async Task DeleteClientAsync(int id)
    {
        var client = await _clientRepository.GetByIdWithDetailsAsync(id);
        if (client == null)
        {
            throw new Exception("Client not found.");
        }

        await _clientRepository.DeleteAsync(client); 
    }

    public async Task<int> EnsureClientAsync(int? clientId, string? clientName)
    {
        if (clientId.HasValue && clientId.Value > 0)
        {
            var existing = await _clientRepository.GetByIdAsync(clientId.Value)
                          ?? throw new Exception($"Client with Id {clientId} not found.");
            return existing.Id;
        }

        if (string.IsNullOrWhiteSpace(clientName))
            throw new Exception("Client name is required.");

        var byName = await _clientRepository.GetByNameAsync(clientName);
        if (byName != null) return byName.Id;

        var created = await _clientRepository.AddAsync(new ClientEntity { ClientName = clientName });
        await _clientRepository.SaveAsync();
        return created.Id;
    }

}
