using FluentValidation;
using SimRMS.Application.Features.UsrInfo.Queries;

namespace SimRMS.Application.Validators
{
    public class GetUsrInfosQueryValidator : AbstractValidator<GetUsrInfosQuery>
    {
        public GetUsrInfosQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

            RuleFor(x => x.UsrStatus)
                .Must(status => status == null || new[] { "A", "S", "C" }.Contains(status))
                .WithMessage("User status must be A (Active), S (Suspend), or C (Close)");
        }
    }
}