using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Factories;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Business.Services;


public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;
    private readonly IClientRepository _clients;
    private readonly IProjectStatusRepository _statuses;
    private readonly INotificationService _notifications;
    private readonly IMemberRepository _members;
    private readonly IClientService _clientService;

    public ProjectService(
        IProjectRepository projectRepository,
        IClientRepository clientRepository,
        IProjectStatusRepository statusRepository,
        INotificationService notificationService,
        IMemberRepository memberRepository,
        IClientService clientService)
    {
        _projects = projectRepository;
        _clients = clientRepository;
        _statuses = statusRepository;
        _notifications = notificationService;
        _members = memberRepository;
        _clientService = clientService;
    }

    


    public async Task<ProjectDto> AddProjectAsync(ProjectDto dto)
    {
        if (!await _statuses.ExistsAsync(dto.StatusId))
            throw new Exception($"Status Id {dto.StatusId} is invalid.");

        dto.ClientId = await _clientService.EnsureClientAsync(dto.ClientId, dto.Client?.ClientName);


        var entity = ProjectFactory.CreateEntity(dto);

        await _projects.AddAsync(entity);
        await _projects.SaveAsync();

        await _notifications.NotifyProjectCreatedAsync(
            entity.Id, entity.Name, /* createdBy */ string.Empty);

        return ProjectFactory.CreateDto(entity);
    }

    public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto dto)
    {
        var project = await _projects.GetByIdAsync(id)
                      ?? throw new KeyNotFoundException("Project not found.");

        project.ClientId = await _clientService.EnsureClientAsync(dto.ClientId, dto.Client?.ClientName);

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.StatusId = dto.StatusId;
        project.Budget = dto.Budget;
        project.ImageUrl = ProjectServiceExtensions.NormaliseImagePath(
                                  dto.ImageData?.CurrentImage ?? dto.ImageData?.SelectedImage);

        if (dto.SelectedMemberIds?.Any() == true)
        {
            project.ProjectMembers.Clear();
            project.ProjectMembers = dto.SelectedMemberIds
                .Select(i => new ProjectMemberEntity { ProjectId = id, MemberId = i })
                .ToList();
        }

        await _projects.SaveAsync();

        await _notifications.NotifyProjectUpdatedAsync(project.Id, project.Name);

        return ProjectFactory.CreateDto(project);
    }


    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projects.GetProjectsWithClientsAsync();
        return projects.Select(ProjectFactory.CreateDto).ToList();
    }

    public async Task<ProjectDto> DeleteProjectAsync(int id)
    {
        var entity = await _projects.GetByIdAsync(id)
                     ?? throw new Exception("Project does not exist.");

        var dto = ProjectFactory.CreateDto(entity);

        await _projects.DeleteAsync(entity);
        await _projects.SaveAsync();

        return dto;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByIdAsync(int id)
    {
        var entity = await _projects.GetByIdAsync(id);
        return entity != null
            ? new[] { ProjectFactory.CreateDto(entity) }
            : Enumerable.Empty<ProjectDto>();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        var entity = await _projects.GetProjectByIdAsync(id);
        return entity != null ? ProjectFactory.CreateDto(entity) : null;
    }

    public async Task<List<MemberDto>> GetProjectMembersAsync(int projectId)
    {
        var project = await _projects.GetProjectByIdAsync(projectId);
        return project == null
            ? new List<MemberDto>()
            : project.ProjectMembers.Select(pm => MemberFactory.CreateDto(pm.Member)).ToList();
    }

    public async Task UpdateProjectMembersAsync(int projectId, List<int> memberIds)
    {
        var project = await _projects.GetProjectByIdAsync(projectId)
                      ?? throw new KeyNotFoundException("Project not found.");

        var current = project.ProjectMembers.Select(pm => pm.MemberId).ToHashSet();
        var target = memberIds.ToHashSet();

        var toAdd = target.Except(current);
        var toRemove = current.Except(target);

        foreach (var id in toAdd)
        {
            project.ProjectMembers.Add(new ProjectMemberEntity { ProjectId = projectId, MemberId = id });

            if (await _members.GetByIdAsync(id) is { } m)
                await _notifications.NotifyMemberAddedToProjectAsync(
                    projectId, project.Name, $"{m.FirstName} {m.LastName}");
        }

        foreach (var id in toRemove)
        {
            project.ProjectMembers.RemoveAll(pm => pm.MemberId == id);

            if (await _members.GetByIdAsync(id) is { } m)
                await _notifications.NotifyMemberRemovedFromProjectAsync(
                    projectId, project.Name, $"{m.FirstName} {m.LastName}");
        }

        await _projects.SaveAsync();
    }
}

// Created with  chatGPT 4o
// Helper for images
// returns null if there is no image
// keeps data‑URIs unchanged
// turns any absolute URL to relative path
// leaves every other string untouched

internal static class ProjectServiceExtensions
{
    public static string? NormaliseImagePath(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        if (raw.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return raw;

        var idx = raw.IndexOf("/images/", StringComparison.OrdinalIgnoreCase);
        return idx >= 0 ? raw[idx..] : raw;
    }
}


//using taskaspnet.business.factories;
//using taskaspnet.business.interfaces;
//using taskaspnet.data.interfaces;
//using taskaspnet.data.entities;
//using taskaspnet.business.dtos;

//namespace taskaspnet.business.services;

//public class projectservice(iprojectrepository projectrepository, iclientrepository clientrepository, iprojectstatusrepository statusrepository, inotificationservice notificationservice, imemberrepository memberrepository) : iprojectservice
//{
//    private readonly iprojectrepository _projectrepository = projectrepository;
//    private readonly iclientrepository _clientrepository = clientrepository;
//    private readonly iprojectstatusrepository _statusrepository = statusrepository;
//    private readonly inotificationservice _notificationservice = notificationservice;
//    private readonly imemberrepository _memberrepository = memberrepository;

//    public async task<projectdto> addprojectasync(projectdto projectdto)
//    {
//        if (string.isnullorwhitespace(projectdto.client.clientname))
//            throw new exception("client name is required to create a project.");

//        var statusexists = await _statusrepository.existsasync(projectdto.statusid);
//        if (!statusexists)
//            throw new exception($"status id {projectdto.statusid} is invalid.");

//        var client = await _clientrepository.getbynameasync(projectdto.client.clientname);
//        if (client == null)
//        {
//            client = new cliententity { clientname = projectdto.client.clientname };
//            client = await _clientrepository.addasync(client);
//            await _clientrepository.saveasync();
//        }
//        projectdto.clientid = client.id;

//        var projectentity = projectfactory.createentity(projectdto);

//        await _projectrepository.addasync(projectentity);
//        await _projectrepository.saveasync();

//        var fullname = $"{projectdto.name}";
//        var createdbyuserid = "[the user id of who created this member]";

//        await _notificationservice.notifyprojectcreatedasync(projectentity.id, fullname, createdbyuserid);

//        return projectfactory.createdto(projectentity);
//    }



//    public async task<ienumerable<projectdto>> getallprojectsasync()
//    {
//        var projects = await _projectrepository.getprojectswithclientsasync();
//        return projects.select(projectfactory.createdto).tolist();
//    }

//    public async task<projectdto> updateprojectasync(int id, projectdto projectdto)
//    {
//        var existingproject = await _projectrepository.getbyidasync(id);
//        if (existingproject is null) return null!;


//        if (!string.isnullorwhitespace(projectdto.client?.clientname))
//        {
//            var client = await _clientrepository.getbynameasync(projectdto.client.clientname);
//            if (client == null)
//            {
//                client = new cliententity { clientname = projectdto.client.clientname };
//                client = await _clientrepository.addasync(client);
//                await _clientrepository.saveasync();
//            }
//            projectdto.clientid = client.id;
//        }


//        var updatedproject = projectfactory.createentity(projectdto);
//        updatedproject.id = id;


//        existingproject.name = updatedproject.name;
//        existingproject.description = updatedproject.description;
//        existingproject.startdate = updatedproject.startdate;
//        existingproject.enddate = updatedproject.enddate;
//        existingproject.statusid = updatedproject.statusid;
//        existingproject.budget = updatedproject.budget;
//        existingproject.clientid = updatedproject.clientid;
//        existingproject.imageurl = updatedproject.imageurl;


//        if (projectdto.selectedmemberids?.any() == true)
//        {
//            existingproject.projectmembers.clear();
//            existingproject.projectmembers = updatedproject.projectmembers;
//        }

//        await _projectrepository.saveasync();

//        await _notificationservice.notifyprojectupdatedasync(
//        existingproject.id,
//        existingproject.name
//    );


//        return projectfactory.createdto(existingproject);
//    }
//    public async task<projectdto> deleteprojectasync(int id)
//    {

//        var projectentity = await _projectrepository.getbyidasync(id);
//        if (projectentity == null)
//        {
//            throw new exception("project does not exist");
//        }


//        var deletedprojectdto = projectfactory.createdto(projectentity);


//        await _projectrepository.deleteasync(projectentity);


//        await _projectrepository.saveasync();


//        return deletedprojectdto;
//    }


//    public async task<ienumerable<projectdto>> getprojectsbyidasync(int id)
//    {
//        var project = await _projectrepository.getbyidasync(id);
//        return project is not null ? [projectfactory.createdto(project)] : enumerable.empty<projectdto>();
//    }

//    public async task<projectdto?> getprojectbyidasync(int id)
//    {
//        var projectentity = await _projectrepository.getprojectbyidasync(id);
//        return projectentity != null ? projectfactory.createdto(projectentity) : null;
//    }


//    public async task<list<memberdto>> getprojectmembersasync(int projectid)
//    {
//        var project = await _projectrepository.getprojectbyidasync(projectid);
//        if (project == null) return new list<memberdto>();

//        return project.projectmembers
//            .select(pm => memberfactory.createdto(pm.member))
//            .tolist();
//    }

//    public async task updateprojectmembersasync(int projectid, list<int> memberids)
//    {
//        var project = await _projectrepository.getprojectbyidasync(projectid);
//        if (project == null) throw new keynotfoundexception("project not found");

//        var currentmemberids = project.projectmembers.select(pm => pm.memberid).tolist();

//        var memberstoadd = memberids.except(currentmemberids).tolist();
//        var memberstoremove = currentmemberids.except(memberids).tolist();


//        foreach (var memberid in memberstoadd)
//        {
//            project.projectmembers.add(new projectmemberentity { projectid = projectid, memberid = memberid });

//            var member = await _memberrepository.getbyidasync(memberid);
//            if (member != null)
//            {
//                await _notificationservice.notifymemberaddedtoprojectasync(
//                    projectid,
//                    project.name,
//                    $"{member.firstname} {member.lastname}"
//                );
//            }
//        }


//        foreach (var memberid in memberstoremove)
//        {
//            var member = await _memberrepository.getbyidasync(memberid);
//            if (member != null)
//            {
//                await _notificationservice.notifymemberremovedfromprojectasync(
//                    projectid,
//                    project.name,
//                    $"{member.firstname} {member.lastname}"
//                );
//            }
//        }


//        project.projectmembers.removeall(pm => memberstoremove.contains(pm.memberid));

//        await _projectrepository.updateasync(project);
//        await _projectrepository.saveasync();
//    }


//}
