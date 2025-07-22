using FluentValidation;
using RMS.Application.Features.UsrInfo.Commands;

namespace RMS.Application.Validators
{
    public class CreateUsrInfoCommandValidator : AbstractValidator<CreateUsrInfoCommand>
    {
        public CreateUsrInfoCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request is required")
                .SetValidator(new CreateUsrInfoRequestValidator());
        }
    }
}