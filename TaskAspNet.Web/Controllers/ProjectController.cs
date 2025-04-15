using Microsoft.AspNetCore.Mvc;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Business.Dtos;


namespace TaskAspNet.Web.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProjectController(IProjectService projectService, ILogger<ProjectController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _projectService = projectService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string status = "All")
    {
        var allProjects = (await _projectService.GetAllProjectsAsync()).ToList();

        var filteredProjects = status switch
        {
            "Started" => allProjects.Where(p => p.StatusId == 1).ToList(),
            "Completed" => allProjects.Where(p => p.StatusId == 2).ToList(),
            _ => allProjects
        };


        var predefinedImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "predefined");
        var imageFiles = Directory.GetFiles(predefinedImagesFolder)
                                  .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                                  .Select(Path.GetFileName)
                                  .Where(fileName => fileName != null)
                                  .Select(fileName => fileName!)
                                  .ToList();

        var vm = new ProjectIndexViewModel
        {
            AllProjects = allProjects,
            FilteredProjects = filteredProjects,
            SelectedStatus = status,
            CreateProject = new ProjectDto
            {
                ImageData = new UploadSelectImgDto
                {
                    PredefinedImages = imageFiles
                    
                }
            }
        };

        return View(vm); 
    }
    //[HttpGet]
    //public IActionResult Create()
    //{
    //    var webRootPath = _webHostEnvironment.WebRootPath; // <--- Put breakpoint here
    //    var predefinedImagesFolder = Path.Combine(webRootPath, "images", "predefined"); // <--- And/or here
    //    var files = Directory.GetFiles(predefinedImagesFolder);
    //    _logger.LogInformation("files.Length = {0}", files.Length);

    //    if (!Directory.Exists(predefinedImagesFolder))
    //    {
    //        _logger.LogError("Predefined images folder not found: {Folder}", predefinedImagesFolder);
    //    }

    //    var imageFiles = Directory.GetFiles(predefinedImagesFolder)
    //        .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
    //                    || file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
    //                    || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
    //        .Select(Path.GetFileName)
    //        .ToList();

    //    _logger.LogInformation("Looking in: {Folder}", predefinedImagesFolder);
    //    _logger.LogInformation("Found {Count} predefined images", imageFiles.Count);
    //    _logger.LogInformation("DBG => predefinedImagesFolder: {Folder}", predefinedImagesFolder);
    //    _logger.LogInformation("DBG => imageFiles.Count: {Count}", imageFiles.Count);



    //    var dto = new ProjectDto
    //    {
    //        ImageData = new UploadSelectImgDto
    //        {
    //            PredefinedImages = imageFiles
    //        }
    //    };

    //    return View(dto);
    //}



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("ModelState invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogInformation("Validation error: {Error}", error.ErrorMessage);
            }
            return RedirectToAction("Index");
        }

        _logger.LogInformation("UploadedImage: {FileName}", dto.ImageData?.UploadedImage != null ? dto.ImageData.UploadedImage.FileName : "null");
        _logger.LogInformation("SelectedImage: {SelectedImage}", dto.ImageData?.SelectedImage ?? "null");
        _logger.LogInformation("CurrentImage before upload: {CurrentImage}", dto.ImageData?.CurrentImage ?? "null");

        if (dto.ImageData != null)
        {
            string imagePath = await HandleImageUploadAsync(dto.ImageData, "projects");
            dto.ImageData.CurrentImage = imagePath ?? dto.ImageData.CurrentImage;
            _logger.LogInformation("ImagePath after upload: {ImagePath}", imagePath ?? "null");
            _logger.LogInformation("CurrentImage after upload: {CurrentImage}", dto.ImageData?.CurrentImage ?? "null");
        }

        try
        {
            var createdProject = await _projectService.AddProjectAsync(dto);
            if (createdProject == null)
            {
                ModelState.AddModelError("", "Could not create project.");
                _logger.LogInformation("Project creation failed");
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Project created successfully");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Exception in Create: {Message}", ex.Message);
            return RedirectToAction("Index");
        }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deletedProject = await _projectService.DeleteProjectAsync(id);
        if (deletedProject == null)
        {
            TempData["ErrorMessage"] = "Project not found or already deleted.";
            return RedirectToAction("Index");
        }

        TempData["SuccessMessage"] = $"Deleted project: {deletedProject.Name}";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMembers(UpdateProjectMemberDto dto)
    {
        if (dto == null || dto.ProjectId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid request";
            return RedirectToAction("Index");
        }

        try
        {
            await _projectService.UpdateProjectMembersAsync(dto.ProjectId, dto.MemberIds);
            TempData["SuccessMessage"] = "Project members updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var projectDtos = await _projectService.GetProjectsByIdAsync(id);
        

        var projectDto = projectDtos.FirstOrDefault(); 
        if (projectDto == null)
            return NotFound();

        
        var predefinedImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "predefined");
        var imageFiles = Directory.GetFiles(predefinedImagesFolder)
                                  .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                           || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                           || f.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                                           || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                                  .Select(Path.GetFileName)
                                  .Where(fileName => fileName != null)
                                  .Select(fileName => fileName!)
                                  .ToList();

       
        projectDto.ImageData ??= new UploadSelectImgDto();
        projectDto.ImageData.PredefinedImages = imageFiles;

        return PartialView("~/Views/Shared/Partials/Components/Project/_CreateEditProject.cshtml", projectDto);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View("_CreateOrEditProject", dto);
        }

        
        if (dto.ImageData != null)
        {
            string imagePath = await HandleImageUploadAsync(dto.ImageData, "projects");
            dto.ImageData.CurrentImage = imagePath ?? dto.ImageData.CurrentImage;
        }

        var updated = await _projectService.UpdateProjectAsync(dto.Id, dto);
        if (updated == null)
        {
            ModelState.AddModelError("", "Could not update project.");
            return View("_CreateOrEditProject", dto);
        }

        return RedirectToAction("Index");
    }

    private async Task<string> HandleImageUploadAsync(UploadSelectImgDto imageData, string folderName)
    {
        if (imageData?.UploadedImage != null && imageData.UploadedImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageData.UploadedImage.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageData.UploadedImage.CopyToAsync(stream);
            }

            
            return $"/uploads/{folderName}/{uniqueFileName}";
        }
        else if (!string.IsNullOrWhiteSpace(imageData?.SelectedImage))
        {
            
            return $"/images/predefined/{imageData.SelectedImage}";
        }

        
        return imageData?.CurrentImage ?? "/images/default.png";
    }
}