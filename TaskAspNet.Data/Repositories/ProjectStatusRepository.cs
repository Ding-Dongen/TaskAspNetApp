

using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class ProjectStatusRepository(AppDbContext context) : BaseRepository<ProjectStatusEntity>(context), IProjectStatusRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> ExistsAsync(int statusId)
    {
        return await _context.ProjectStatuses.AnyAsync(s => s.Id == statusId);
    }
}
