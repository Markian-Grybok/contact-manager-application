using ContactManager.BLL.Models;
using FluentValidation;

namespace ContactManager.BLL.Validators;

public abstract class BaseContactValidator<T> : AbstractValidator<T> where T : ContactCreateViewModel
{
    protected void AddCommonRules()
    {
        RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required")
                    .MinimumLength(2).WithMessage("Name must be at least 2 characters")
                    .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Date of birth cannot be in the future")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-150))).WithMessage("Date of birth must be valid (not older than 150 years)");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+?[\d\s\-()]+$").WithMessage("Phone must contain only digits, spaces, hyphens, parentheses, or plus sign")
            .MinimumLength(7).WithMessage("Phone must be at least 7 characters")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters");

        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Salary is required")
            .GreaterThan(0).WithMessage("Salary must be greater than 0")
            .LessThan(9999999.99m).WithMessage("Salary is too high (maximum 9,999,999.99)");
    }
}
