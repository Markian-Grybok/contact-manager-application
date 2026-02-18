using ContactManager.BLL.Models;
using FluentValidation;

namespace ContactManager.BLL.Validators;

public class ContactEditViewModelValidator : BaseContactValidator<ContactEditViewModel>
{
    public ContactEditViewModelValidator()
    {
        AddCommonRules();

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Contact ID must be valid");
    }
}
