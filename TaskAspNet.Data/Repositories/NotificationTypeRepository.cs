using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class NotificationTypeRepository : BaseRepository<NotificationTypeEntity>, INotificationTypeRepository
{
    private readonly AppDbContext _context;

    public NotificationTypeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<NotificationTypeEntity?> GetByNameAsync(string name)
    {
        return await _context.NotificationTypes
            .Include(nt => nt.TargetGroup)
            .FirstOrDefaultAsync(nt => nt.Name == name);
    }
}
