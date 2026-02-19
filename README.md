# Contact Manager

An ASP.NET Core MVC application for managing contacts — upload from CSV, view, inline-edit, filter and sort records entirely on the client side without page reloads.

Built as a test assignment with a focus on clean architecture, separation of concerns, and production-ready patterns rather than just "making it work."

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| Database | MS SQL Server |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| Result Handling | FluentResults |
| CSV Parsing | CsvHelper |

---

## Architecture

The solution is split into **3 independent projects**, each with a single responsibility:

```
ContactManager.sln
├── ContactManager.WebAPI      # Presentation — Controllers, Views, DI configuration
├── ContactManager.BLL         # Business Logic — Services, Validators, DTOs, AutoMapper profiles
└── ContactManager.DAL         # Data Access — EF Core, Entities, Repository, Fluent API config
```

Dependencies flow strictly downward: `WebAPI → BLL → DAL`. The BLL has no reference to ASP.NET Core infrastructure — it only depends on abstractions.

---

## Key Design Decisions

### Repository Pattern
Data access is fully abstracted behind `IContactRepository`. The BLL and controllers never touch `DbContext` directly, which keeps layers decoupled and makes the data layer swappable without touching business logic.

### FluentResults for Predictable Error Handling
Service methods return `Result<T>` instead of throwing exceptions for expected failures. This makes error handling explicit at the call site and avoids using exceptions as control flow:

```csharp
var result = await _csvService.ParseContactsAsync(file);
if (result.IsFailed)
{
    TempData["FileError"] = string.Join("; ", result.Errors.Select(e => e.Message));
    return RedirectToAction(nameof(Upload));
}
```

### FluentValidation with Per-Row CSV Validation
Validation logic is separated from models entirely. The `CsvService` reuses the same `IValidator<ContactCreateViewModel>` that the MVC pipeline uses for form validation — no duplicated rules:

```csharp
foreach (var record in records)
{
    var validationResult = await _validator.ValidateAsync(record);
    if (!validationResult.IsValid)
    {
        var errors = string.Join(" | ", validationResult.Errors
            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        return Result.Fail($"Row {rowNumber}: {errors}");
    }
    contacts.Add(record);
    rowNumber++;
}
```

If a row fails validation the import stops immediately with the exact row number and field that caused the error.

### EF Core Fluent API Configuration
Entity configuration is kept in a dedicated `IEntityTypeConfiguration<T>` class rather than data annotations on the entity, keeping the domain model clean:

```csharp
builder.Property(c => c.Salary)
    .IsRequired()
    .HasPrecision(18, 2);
```

### Clean DI Registration
All service registration is extracted into `ServiceCollectionExtensions` and all middleware configuration into `ApplicationBuilderExtensions`, leaving `Program.cs` with a single responsibility:

```csharp
builder.Services.AddCoreConfiguration(builder.Configuration);
app.UseCoreConfiguration();
```

### Client-Side Sorting and Filtering
Sorting and filtering happen entirely in the browser with no server roundtrips. Sorting uses `localeCompare` with `{ numeric: true }` so numeric columns sort correctly as numbers rather than strings:

```javascript
rows.sort((rowA, rowB) => {
    const a = rowA.children[columnIndex].textContent.trim();
    const b = rowB.children[columnIndex].textContent.trim();
    return sortDirection === 'asc'
        ? a.localeCompare(b, undefined, { numeric: true })
        : b.localeCompare(a, undefined, { numeric: true });
});
```

Toggle direction is tracked per column — clicking the same header reverses the sort, clicking a different column resets direction to ascending with a visual indicator.

---

## Features

- Upload contacts from a CSV file with per-row validation and meaningful error messages
- View all contacts in a table with client-side search filtering across all columns
- Sort by any column (toggle asc/desc) with visual indicators
- Edit contacts via a dedicated form with both server-side and client-side validation
- Delete contacts via POST to prevent accidental deletion through GET requests
- Global error handling via `UseExceptionHandler` middleware
- Notification system (success/error) via TempData rendered in shared layout partial

---

## CSV Format

```
Name,DateOfBirth,Married,Phone,Salary
John Doe,1990-05-15,true,+1234567890,5000.00
```

| Field | Type | Format |
|---|---|---|
| Name | string | — |
| DateOfBirth | date | YYYY-MM-DD |
| Married | bool | true / false |
| Phone | string | — |
| Salary | decimal | 0.00 |

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- MS SQL Server

### Setup

1. Clone the repository:
```bash
git clone https://github.com/your-username/ContactManager.git
cd ContactManager
```

2. Update the connection string in `ContactManager.WebAPI/appsettings.json`:
```json
"ConnectionStrings": {
  "DataBaseConnection": "Server=YOUR_SERVER;Database=ContactManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Apply migrations:
```bash
dotnet ef database update --project ContactManager.DAL --startup-project ContactManager.WebAPI
```

4. Run the application:
```bash
dotnet run --project ContactManager.WebAPI
```

---

## Project Structure

```
ContactManager.BLL
├── Interfaces/
│   └── ICsvService.cs
├── Mapping/
│   └── ContactProfile.cs
├── Models/
│   ├── ContactCreateViewModel.cs
│   ├── ContactEditViewModel.cs
│   ├── ContactResponse.cs
│   └── ErrorViewModel.cs
├── Services/
│   └── CsvService.cs
└── Validators/
    ├── BaseContactValidator.cs
    ├── ContactCreateViewModelValidator.cs
    └── ContactEditViewModelValidator.cs

ContactManager.DAL
├── Entities/
│   └── Contacts.cs
├── Migrations/
├── Repositories/
│   ├── Interfaces/IContactRepository.cs
│   └── Realizations/ContactRepository.cs
├── TypeConfigurations/
│   └── ContactConfiguration.cs
└── ContactManagerDbContext.cs

ContactManager.WebAPI
├── Configuration/
│   ├── ApplicationBuilderExtensions.cs
│   └── ServiceCollectionExtensions.cs
├── Controllers/
│   └── ContactController.cs
└── Views/
    ├── Contact/
    │   ├── Index.cshtml
    │   ├── Edit.cshtml
    │   └── Upload.cshtml
    └── Shared/
        ├── _Layout.cshtml
        ├── _Notifications.cshtml
        └── Error.cshtml
```
