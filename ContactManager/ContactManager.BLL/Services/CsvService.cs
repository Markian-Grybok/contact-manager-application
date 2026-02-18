using ContactManager.BLL.Interfaces;
using ContactManager.BLL.Models;
using CsvHelper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace ContactManager.BLL.Services;

public class CsvService : ICsvService
{
    private readonly IValidator<ContactCreateViewModel> _validator;

    public CsvService(IValidator<ContactCreateViewModel> validator)
    {
        _validator = validator;
    }

    public async Task<List<ContactCreateViewModel>> ParseContactsAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var contacts = new List<ContactCreateViewModel>();
        var invalidRecords = new List<(int rowNumber, ContactCreateViewModel record, List<string> errors)>();

        try
        {
            var records = csv.GetRecords<ContactCreateViewModel>();
            int rowNumber = 2;

            foreach (var record in records)
            {
                var validationResult = await _validator.ValidateAsync(record);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();
                    invalidRecords.Add((rowNumber, record, errors));
                }
                else
                {
                    contacts.Add(record);
                }

                rowNumber++;
            }

            if (invalidRecords.Count > 0)
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine($"CSV validation failed. {invalidRecords.Count} record(s) have errors:");
                errorBuilder.AppendLine();

                foreach (var (invalidRowNumber, record, errors) in invalidRecords)
                {
                    errorBuilder.AppendLine($"❌ Row {invalidRowNumber}:");
                    foreach (var error in errors)
                    {
                        errorBuilder.AppendLine($"   • {error}");
                    }
                }

                throw new InvalidOperationException(errorBuilder.ToString());
            }
        }
        catch (CsvHelperException ex)
        {
            throw new InvalidOperationException($"CSV parsing error at line {csv.Parser.Row}: {ex.Message}", ex);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing CSV: {ex.Message}", ex);
        }

        return contacts;
    }
}
