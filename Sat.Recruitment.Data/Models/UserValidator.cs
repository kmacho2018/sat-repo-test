using FluentValidation;
using FluentValidation.Validators;

namespace Sat.Recruitment.Data.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Email).EmailAddress(EmailValidationMode.Net4xRegex);
        }
    }
}