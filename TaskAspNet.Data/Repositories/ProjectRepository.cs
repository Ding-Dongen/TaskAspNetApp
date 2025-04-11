using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context), IProjectRepository
{

    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<ProjectEntity>> GetProjectsWithClientsAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Status)
            .Include(p => p.ProjectMembers)
                .ThenInclude(pm => pm.Member)
            .ToListAsync();
    }

    public async Task<ProjectEntity?> GetProjectByIdAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.ProjectMembers) 
            .ThenInclude(pm => pm.Member)
            .ThenInclude(m => m.JobTitle)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }
}
