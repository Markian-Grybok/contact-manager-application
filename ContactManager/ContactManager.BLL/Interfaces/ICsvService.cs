using ContactManager.BLL.Models;
using Microsoft.AspNetCore.Http;

namespace ContactManager.BLL.Interfaces;

public interface ICsvService
{
    Task<List<ContactCreateViewModel>> ParseContactsAsync(IFormFile file);
}
