using ContactManager.BLL.Interfaces;
using ContactManager.BLL.Models;
using CsvHelper;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace ContactManager.BLL.Services;

public class CsvService : ICsvService
{
    private readonly IValidator<ContactCreateViewModel> _validator;

    public CsvService(IValidator<ContactCreateViewModel> validator)
    {
        _validator = validator;
    }

    public async Task<Result<List<ContactCreateViewModel>>> ParseContactsAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Fail("File is empty.");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("File must be a CSV file.");
        }

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var contacts = new List<ContactCreateViewModel>();

        try
        {
            var records = csv.GetRecords<ContactCreateViewModel>();
            int rowNumber = 2;

            foreach (var record in records)
            {
                var validationResult = await _validator.ValidateAsync(record);

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                    return Result.Fail($"Row {rowNumber}: {errors}");
                }

                contacts.Add(record);
                rowNumber++;
            }

            return Result.Ok(contacts);
        }
        catch (CsvHelperException ex)
        {
            return Result.Fail($"CSV parsing error at line {csv.Parser.Row}: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error processing CSV: {ex.Message}");
        }
    }
}
