using FluentValidation;
using IntScience.Repository.Models;

namespace IntScience.Services.Validators;

public class ApplicationLogValidator : AbstractValidator<ApplicationLog>
{
    public ApplicationLogValidator()
    {
        RuleFor(a => a.Severity)
            .NotEmpty()
            .WithMessage("The severity must be specified.")
            .MaximumLength(50)
            .WithMessage("The severity cannot be greater than 50 characters.");
        RuleFor(a => a.Message)
            .NotEmpty()
            .WithMessage("The log message cannot be blank.")
            .MaximumLength(500)
            .WithMessage("The log message cannot be greater than 500 characters.");
    }
}
