using FluentValidation;
using SimRMS.Application.Features.UsrInfo.Commands;

namespace SimRMS.Application.Validators
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