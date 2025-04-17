using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

public interface IClientRepository : IBaseRepository<ClientEntity>
{
    Task<ClientEntity?> GetByNameAsync(string clientName);
    Task<ClientEntity?> GetByIdWithDetailsAsync(int id);

    Task<List<ClientEntity>> GetAllWithDetailsAsync();
}
