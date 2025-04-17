
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Interfaces;

public interface IClientService
{
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<List<ClientDto>> GetAllClientsAsync();

    Task CreateClientAsync(ClientDto dto);
    Task UpdateClientAsync(ClientDto dto);
    Task DeleteClientAsync(int id);
    Task<int> EnsureClientAsync(int? clientId, string? clientName);

}
