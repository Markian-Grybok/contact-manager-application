using ContactManager.BLL.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace ContactManager.BLL.Interfaces;

public interface ICsvService
{
    Task<Result<List<ContactCreateViewModel>>> ParseContactsAsync(IFormFile file);
}
