
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsByIdAsync(int id);
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto> AddProjectAsync(ProjectDto project);
    Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto project);
    Task<ProjectDto> DeleteProjectAsync(int id);
    Task<List<MemberDto>> GetProjectMembersAsync(int projectId);
    Task UpdateProjectMembersAsync(int projectId, List<int> memberIds);

}
