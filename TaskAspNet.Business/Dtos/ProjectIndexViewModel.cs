using TaskAspNet.Business.Dtos;

public class ProjectIndexViewModel
{
    public List<ProjectDto> AllProjects { get; set; } = new();
    public ProjectDto CreateProject { get; set; } = new();
    public string SelectedStatus { get; set; } = "All";
    public List<ProjectDto> FilteredProjects { get; set; } = new();


}
