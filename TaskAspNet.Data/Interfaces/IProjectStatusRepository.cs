

using TaskAspNet.Data.Entities;

namespace TaskAspNet.Data.Interfaces;

public interface IProjectStatusRepository : IBaseRepository<ProjectStatusEntity>
{
    Task<bool> ExistsAsync(int statusId);
}
