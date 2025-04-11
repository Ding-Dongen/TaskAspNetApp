

using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Repositories;

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity>(context), IClientRepository
{

    private readonly AppDbContext _context = context;

    public async Task<ClientEntity?> GetByNameAsync(string clientName)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.ClientName == clientName);
    }
}
