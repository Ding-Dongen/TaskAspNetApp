

using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Interfaces;

public interface IProjectRepository : IBaseRepository<ProjectEntity>
{
    Task<IEnumerable<ProjectEntity>> GetProjectsWithClientsAsync();
    Task<ProjectEntity?> GetProjectByIdAsync(int id);
}
