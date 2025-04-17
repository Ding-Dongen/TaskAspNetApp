

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

    public async Task<ClientEntity?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Clients
            .Include(c => c.Addresses)
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<ClientEntity>> GetAllWithDetailsAsync()
    {
        return await _context.Clients
            .Include(c => c.Phones)
            .Include(c => c.Addresses)
            .ToListAsync();
    }

}
