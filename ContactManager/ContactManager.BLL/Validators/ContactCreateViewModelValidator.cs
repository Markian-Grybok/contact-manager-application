using ContactManager.BLL.Models;

namespace ContactManager.BLL.Validators;

public class ContactCreateViewModelValidator : BaseContactValidator<ContactCreateViewModel>
{
    public ContactCreateViewModelValidator()
    {
        AddCommonRules();
    }
}