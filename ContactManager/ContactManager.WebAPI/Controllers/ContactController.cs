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
        try
        {
            var contacts = await _repository.GetAllAsync();

            return View(_mapper.Map<List<ContactResponse>>(contacts));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;

            return View(new List<ContactResponse>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null)
        {
            TempData["Error"] = $"Contact with ID {id} not found.";

            return RedirectToAction(nameof(Index));
        }

        return View(_mapper.Map<ContactEditViewModel>(contact));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ContactEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }    

        try
        {
            _repository.Update(_mapper.Map<Contacts>(model));
            TempData["Success"] = "Contact updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Upload() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var result = await _csvService.ParseContactsAsync(file);
            if (result.IsFailed)
            {
                ViewBag.Error = string.Join("; ", result.Errors.Select(e => e.Message));
                return View();
            }

            await _repository.CreateRangeAsync(_mapper.Map<List<Contacts>>(result.Value));
            TempData["Success"] = $"{result.Value.Count} contacts imported successfully!";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(nameof(file), ex.Message);

            return View();
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact is null)
            {
                TempData["Error"] = $"Contact with ID {id} not found.";

                return RedirectToAction(nameof(Index));
            }

            _repository.Delete(contact);
            TempData["Success"] = "Contact deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
