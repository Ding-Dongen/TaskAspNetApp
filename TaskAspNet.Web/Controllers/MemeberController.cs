using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using TaskAspNet.Data.Models;
using TaskAspNet.Web.Interfaces;

namespace TaskAspNet.Web.Controllers;

[Route("Member")]
public class MemberController : Controller
{
    private readonly IApplicationService _memberAppService;  
    private readonly IMemberService _memberService;        
    private readonly IProjectService _projectService;
    private readonly IWebHostEnvironment _webHostEnvironment; 
    private readonly UserManager<AppUser> _userManager;
    private readonly ILoggerService _logger;

    public MemberController(
        IApplicationService memberAppService,
        IMemberService memberService,
        IProjectService projectService,
        IWebHostEnvironment webHostEnvironment,
        UserManager<AppUser> userManager,
        ILoggerService logger)
    {
        _memberAppService = memberAppService;
        _memberService = memberService;
        _projectService = projectService;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
        _logger = logger;
    }

    
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var allMembers = (await _memberService.GetAllMembersAsync()).ToList();

        var jobTitles = await _memberService.GetAllJobTitlesAsync();

        var predefinedImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "membericon");
        var imageFiles = Directory.GetFiles(predefinedImagesFolder)
                                  .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                                  .Select(Path.GetFileName)
                                  .Where(fileName => fileName != null)
                                  .Select(fileName => fileName!)
                                  .ToList();

        var model = new MemberIndexViewModel
        {
            AllMembers = allMembers,
            CreateMember = new MemberDto
            {
                ImageData = new UploadSelectImgDto
                {
                    PredefinedImages = imageFiles
                },
                AvailableJobTitles = jobTitles.Select(jt => new SelectListItem
                {
                    Value = jt.Id.ToString(),
                    Text = jt.Title
                }).ToList()
            }
        };

        return View(model);
    }

    [Authorize]
    [HttpGet("CreateMember")]
    public async Task<IActionResult> CreateMember(string fullName, string email, string userId)
    {
        var jobTitles = await _memberService.GetAllJobTitlesAsync();

        var predefinedImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "membericon");
        var imageFiles = Directory.GetFiles(predefinedImagesFolder)
                                  .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                                  .Select(Path.GetFileName)
                                  .Where(fileName => fileName != null)
                                  .Select(fileName => fileName!)
                                  .ToList();

        var names = !string.IsNullOrEmpty(fullName)
            ? fullName.Split(' ', 2)
            : new string[0];

        var firstName = names.Length > 0 ? names[0] : "";
        var lastName = names.Length > 1 ? names[1] : "";

        var model = new MemberDto
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserId = userId,

            ImageData = new UploadSelectImgDto
            {
                PredefinedImages = imageFiles
            },
            AvailableJobTitles = jobTitles.Select(jt => new SelectListItem
            {
                Value = jt.Id.ToString(),
                Text = jt.Title
            }).ToList()
        };

        return View("~/Views/Shared/Partials/Components/Member/_CreateMemberModal.cshtml", model);
    }

    [Authorize]
    [HttpPost("CreateMember")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMember(MemberDto memberDto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateJobTitlesAsync(memberDto);
            await PopulatePredefinedImagesAsync(memberDto);

            return PartialView(
                "~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", memberDto);
        }

        var (success, createdMember, errorMessage) = await _memberAppService.CreateMemberAsync(memberDto);
        if (!success)
        {
            ModelState.AddModelError("", errorMessage ?? "An error occurred while creating the member.");
            await PopulateJobTitlesAsync(memberDto);
            await PopulatePredefinedImagesAsync(memberDto);

            return PartialView(
                "~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", memberDto);
        }

        return RedirectToAction("Index","Project");
    }

    private async Task PopulateJobTitlesAsync(MemberDto memberDto)
    {
        var jobTitles = await _memberService.GetAllJobTitlesAsync();
        memberDto.AvailableJobTitles = jobTitles.Select(jt => new SelectListItem
        {
            Value = jt.Id.ToString(),
            Text = jt.Title
        }).ToList();
    }

    private async Task PopulatePredefinedImagesAsync(MemberDto memberDto)
    {
        var predefinedImagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "membericon");
        var imageFiles = Directory.GetFiles(predefinedImagesFolder)
                                  .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                                              || file.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                                  .Select(Path.GetFileName)
                                  .Where(fileName => fileName != null)
                                  .Select(fileName => fileName!)
                                  .ToList();

        if (memberDto.ImageData == null)
            memberDto.ImageData = new UploadSelectImgDto();

        memberDto.ImageData.PredefinedImages = imageFiles;
    }

  

    [Authorize(Roles = "Admin, SuperAdmin")]
    [HttpGet("Search")]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Ok(new List<MemberDto>());
        var matches = await _memberService.SearchMembersAsync(term);
        return Ok(matches);
    }

    [HttpGet("GetMembers")]
    public async Task<IActionResult> GetMembers([FromQuery] int projectId)
    {
        if (projectId <= 0)
            return BadRequest(new { error = "Invalid project ID" });

        var members = await _projectService.GetProjectMembersAsync(projectId);
        if (members == null || !members.Any())
            return NotFound(new { error = "No members found for this project" });

        return Ok(members);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var members = await _memberService.GetAllMembersAsync();
        if (members == null || !members.Any())
            return NotFound(new { error = "No members found." });
        return Ok(members);
    }

    [Authorize(Roles = "Admin, SuperAdmin")]
    [HttpPost("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var member = (await _memberService.GetMembersByIdAsync(id)).FirstOrDefault();
        if (member == null)
        {
            TempData["ErrorMessage"] = "Member not found or already deleted.";
            return RedirectToAction("Index");
        }

        try
        {
            await _memberService.DeleteMemberAsync(member.Id);

            var user = await _userManager.FindByIdAsync(member.UserId);
            if (user != null)
                await _userManager.DeleteAsync(user);
            else
                await _logger.LogErrorAsync($"[DeleteMember] Could not find user with ID {member.UserId}");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"[DeleteMember] Exception: {ex.Message}\n{ex.StackTrace}");
            TempData["ErrorMessage"] = "Something went wrong while deleting the member and user.";
        }


        TempData["SuccessMessage"] = $"Deleted member and related user: {member.FirstName} {member.LastName}";
        return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();

            if (string.IsNullOrEmpty(member.UserId) && !string.IsNullOrEmpty(member.Email))
            {
                var user = await _userManager.FindByEmailAsync(member.Email);
                if (user != null)
                {
                    member.UserId = user.Id;
                    Console.WriteLine($"[GET Edit] Fallback UserId assigned: {user.Id}");
                }
            }

            await PopulateJobTitlesAsync(member);
            await PopulatePredefinedImagesAsync(member);

            return PartialView("~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", member);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Edit failed for member ID {id}: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, "Something went wrong.");
        }
    }






    //[Authorize(Roles = "Admin, SuperAdmin")]
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MemberDto member)
    {
        Console.WriteLine($"[POST Edit] Incoming UserId = {member.UserId}");

        if (string.IsNullOrEmpty(member.UserId))
        {
            var fallbackUser = await _userManager.FindByEmailAsync(member.Email);
            if (fallbackUser != null)
            {
                member.UserId = fallbackUser.Id;
                Console.WriteLine($"[POST Edit] UserId set via fallback: {member.UserId}");
            }
            else
            {
                ModelState.AddModelError("UserId", "UserId is required. Cannot update member.");
                await PopulateJobTitlesAsync(member);
                return PartialView("~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", member);
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateJobTitlesAsync(member);
            return PartialView("~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", member);
        }

        var (success, updatedMember, errorMessage) = await _memberAppService.UpdateMemberAsync(member.Id, member);

        if (!success)
        {
            ModelState.AddModelError("", errorMessage ?? "Error updating the member.");
            await PopulateJobTitlesAsync(member);
            return PartialView("~/Views/Shared/Partials/Components/Member/_CreateEditMember.cshtml", member);
        }

        return RedirectToAction("Index", "Project");

    }





    [HttpGet("CompleteProfile")]
    public async Task<IActionResult> CompleteProfile(string fullName, string email, string userId)
    {
        var parts = (fullName ?? "").Split(' ', 2);
        var firstName = parts.Length > 0 ? parts[0] : "";
        var lastName = parts.Length > 1 ? parts[1] : "";

        var model = new MemberDto
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserId = userId
            // Day/Month/Year, addresses, phones, etc. if you want defaults
        };

        await PopulateJobTitlesAsync(model);

        await PopulatePredefinedImagesAsync(model);

        return View("~/Views/Shared/Partials/Components/Member/_CreateMemberModal.cshtml", model);
    }



}
