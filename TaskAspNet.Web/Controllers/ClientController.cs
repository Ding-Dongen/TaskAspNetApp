using Microsoft.AspNetCore.Mvc;
using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Business.ViewModel;


namespace TaskAspNet.Web.Controllers;

public class ClientController : Controller
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _clientService.GetAllClientsAsync();

        var viewModel = new ClientIndexViewModel
        {
            AllMembers = clients,
            CreateClient = new ClientDto()
        };

        return View(viewModel);
    }



    // GET: /Client/Create
    public IActionResult Create()
    {
        return View(new ClientDto());
    }

    // POST: /Client/CreateClient
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClient(ClientDto model)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", model);
        }

        await _clientService.CreateClientAsync(model);

        return RedirectToAction("Index", "Client"); // or wherever you list clients
    }

    // GET: /Client/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        return View("Create", client); // reuse the same form view
    }

    // POST: /Client/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ClientDto model)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", model);
        }

        await _clientService.UpdateClientAsync(model);

        return RedirectToAction("Index", "Client");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _clientService.DeleteClientAsync(id);
            return RedirectToAction("Index", "Client"); // or wherever your list is
        }
        catch (Exception ex)
        {
            // Optionally: log or return a message
            ModelState.AddModelError(string.Empty, $"Could not delete client: {ex.Message}");
            return RedirectToAction("Index", "Client");
        }
    }

    [HttpGet("Dropdown")]                   
    public async Task<IActionResult> Dropdown()
    {
        var clients = await _clientService.GetAllClientsAsync();
        return PartialView("~/Views/Shared/Partials/Components/Client/_ClientSelect.cshtml", clients);
    }



}
