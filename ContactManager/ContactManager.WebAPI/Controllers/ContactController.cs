using AutoMapper;
using ContactManager.BLL.Interfaces;
using ContactManager.BLL.Models;
using ContactManager.DAL.Entities;
using ContactManager.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebAPI.Controllers;

public class ContactController : Controller
{
    private readonly IContactRepository _repository;
    private readonly ICsvService _csvService;
    private readonly IMapper _mapper;

    public ContactController(IContactRepository repository, ICsvService csvService, IMapper mapper) =>
        (_repository, _csvService, _mapper) = (repository, csvService, mapper);
    
    public async Task<IActionResult> Index()
    {
        var contacts = await _repository.GetAllAsync();
        var contactsResponse = _mapper.Map<List<ContactResponse>>(contacts);

        return View(contactsResponse);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null)
        {
            TempData["Error"] = $"Contact with ID {id} not found.";
            return RedirectToAction("Index");
        }

        var contactEditViewModel = _mapper.Map<ContactEditViewModel>(contact);

        return View(contactEditViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ContactEditViewModel contactEditViewModel)
    {
        try
        {
            var contact = _mapper.Map<Contacts>(contactEditViewModel);

            _repository.Update(contact);

            TempData["Success"] = "Contact updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(contactEditViewModel);
        }
    }

    [HttpGet]
    public IActionResult Upload() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid file.";
                return RedirectToAction("Upload");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "File must be a CSV file.";
                return RedirectToAction("Upload");
            }

            List<ContactCreateViewModel> contactsRequest = await _csvService.ParseContactsAsync(file);

            List<Contacts> contacts = _mapper.Map<List<Contacts>>(contactsRequest);

            await _repository.CreateRangeAsync(contacts);

            TempData["Success"] = $"{contacts.Count} contacts imported successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact is null)
            {
                TempData["Error"] = $"Contact with ID {id} not found.";
                return RedirectToAction("Index");
            }

            _repository.Delete(contact);

            TempData["Success"] = "Contact deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
