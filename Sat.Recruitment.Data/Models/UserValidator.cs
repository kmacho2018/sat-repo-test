using FluentValidation;
using FluentValidation.Validators;

namespace Sat.Recruitment.Data.Models
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name).NotEmpty();
            RuleFor(user => user.Phone).NotEmpty();
            RuleFor(user => user.Address).NotEmpty();
            RuleFor(user => user.Email).NotEmpty();
            RuleFor(user => user.UserType).NotEmpty();
            RuleFor(user => user.Email).EmailAddress(EmailValidationMode.Net4xRegex);
        }
    }
}