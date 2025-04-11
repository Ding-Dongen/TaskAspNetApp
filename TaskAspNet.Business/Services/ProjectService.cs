using TaskAspNet.Business.Factories;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Data.Interfaces;
using TaskAspNet.Data.Entities;
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Services;

public class ProjectService(IProjectRepository projectRepository, IClientRepository clientRepository, IProjectStatusRepository statusRepository, INotificationService notificationService, IMemberRepository memberRepository) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IProjectStatusRepository _statusRepository = statusRepository;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IMemberRepository _memberRepository = memberRepository;

    public async Task<ProjectDto> AddProjectAsync(ProjectDto projectDto)
    {
        if (string.IsNullOrWhiteSpace(projectDto.Client.ClientName))
            throw new Exception("Client Name is required to create a project.");

        var statusExists = await _statusRepository.ExistsAsync(projectDto.StatusId);
        if (!statusExists)
            throw new Exception($"Status ID {projectDto.StatusId} is invalid.");

        var client = await _clientRepository.GetByNameAsync(projectDto.Client.ClientName);
        if (client == null)
        {
            client = new ClientEntity { ClientName = projectDto.Client.ClientName };
            client = await _clientRepository.AddAsync(client);
            await _clientRepository.SaveAsync();
        }
        projectDto.ClientId = client.Id;

        var projectEntity = ProjectFactory.CreateEntity(projectDto);

        await _projectRepository.AddAsync(projectEntity);
        await _projectRepository.SaveAsync();

        var fullName = $"{projectDto.Name}";
        var createdByUserId = "[the user ID of who created this member]";

        await _notificationService.NotifyProjectCreatedAsync(projectEntity.Id, fullName, createdByUserId);

        return ProjectFactory.CreateDto(projectEntity);
    }



    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetProjectsWithClientsAsync();
        return projects.Select(ProjectFactory.CreateDto).ToList();
    }

    public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto)
    {
        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject is null) return null!;

     
        if (!string.IsNullOrWhiteSpace(projectDto.Client?.ClientName))
        {
            var client = await _clientRepository.GetByNameAsync(projectDto.Client.ClientName);
            if (client == null)
            {
                client = new ClientEntity { ClientName = projectDto.Client.ClientName };
                client = await _clientRepository.AddAsync(client);
                await _clientRepository.SaveAsync();
            }
            projectDto.ClientId = client.Id;
        }

        
        var updatedProject = ProjectFactory.CreateEntity(projectDto);
        updatedProject.Id = id; 

    
        existingProject.Name = updatedProject.Name;
        existingProject.Description = updatedProject.Description;
        existingProject.StartDate = updatedProject.StartDate;
        existingProject.EndDate = updatedProject.EndDate;
        existingProject.StatusId = updatedProject.StatusId;
        existingProject.Budget = updatedProject.Budget;
        existingProject.ClientId = updatedProject.ClientId;
        existingProject.ImageUrl = updatedProject.ImageUrl;
        
   
        if (projectDto.SelectedMemberIds?.Any() == true)
        {
            existingProject.ProjectMembers.Clear();
            existingProject.ProjectMembers = updatedProject.ProjectMembers;
        }

        await _projectRepository.SaveAsync();

        await _notificationService.NotifyProjectUpdatedAsync(
        existingProject.Id,
        existingProject.Name
    );


        return ProjectFactory.CreateDto(existingProject);
    }
    public async Task<ProjectDto> DeleteProjectAsync(int id)
    {
       
        var projectEntity = await _projectRepository.GetByIdAsync(id);
        if (projectEntity == null)
        {
            throw new Exception("Project does not exist");
        }

 
        var deletedProjectDto = ProjectFactory.CreateDto(projectEntity);

        
        await _projectRepository.DeleteAsync(projectEntity);


        await _projectRepository.SaveAsync();


        return deletedProjectDto;
    }


    public async Task<IEnumerable<ProjectDto>> GetProjectsByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        return project is not null ? [ProjectFactory.CreateDto(project)] : Enumerable.Empty<ProjectDto>();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        var projectEntity = await _projectRepository.GetProjectByIdAsync(id);
        return projectEntity != null ? ProjectFactory.CreateDto(projectEntity) : null;
    }


    public async Task<List<MemberDto>> GetProjectMembersAsync(int projectId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null) return new List<MemberDto>();

        return project.ProjectMembers
            .Select(pm => MemberFactory.CreateDto(pm.Member)) 
            .ToList();
    }

    public async Task UpdateProjectMembersAsync(int projectId, List<int> memberIds)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null) throw new KeyNotFoundException("Project not found");

        var currentMemberIds = project.ProjectMembers.Select(pm => pm.MemberId).ToList();

        var membersToAdd = memberIds.Except(currentMemberIds).ToList();
        var membersToRemove = currentMemberIds.Except(memberIds).ToList();

        
        foreach (var memberId in membersToAdd)
        {
            project.ProjectMembers.Add(new ProjectMemberEntity { ProjectId = projectId, MemberId = memberId });

            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member != null)
            {
                await _notificationService.NotifyMemberAddedToProjectAsync(
                    projectId,
                    project.Name,
                    $"{member.FirstName} {member.LastName}"
                );
            }
        }

        
        foreach (var memberId in membersToRemove)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member != null)
            {
                await _notificationService.NotifyMemberRemovedFromProjectAsync(
                    projectId,
                    project.Name,
                    $"{member.FirstName} {member.LastName}"
                );
            }
        }

        
        project.ProjectMembers.RemoveAll(pm => membersToRemove.Contains(pm.MemberId));

        await _projectRepository.UpdateAsync(project);
        await _projectRepository.SaveAsync();
    }


}
