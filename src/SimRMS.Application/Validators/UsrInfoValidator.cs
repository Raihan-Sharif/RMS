using FluentValidation;
using SimRMS.Application.Features.UsrInfo;
using SimRMS.Application.Models.Requests;

namespace SimRMS.Application.Validators
{
    #region Command Validator
    // CREATE COMMAND VALIDATOR
    public class CreateUsrInfoCommandValidator : AbstractValidator<CreateUsrInfoCommand>
    {
        public CreateUsrInfoCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request is required")
                .SetValidator(new CreateUsrInfoRequestValidator());
        }
    }

    public class CreateUsrInfoRequestValidator : AbstractValidator<CreateUsrInfoRequest>
    {
        public CreateUsrInfoRequestValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("User ID must contain only uppercase letters and numbers");

            //RuleFor(x => x.RmsType)
            //    .NotEmpty().WithMessage("RMS Type is required")
            //    .MaximumLength(10).WithMessage("RMS Type must not exceed 10 characters");

            //RuleFor(x => x.UsrName)
            //    .MaximumLength(100).WithMessage("User name must not exceed 100 characters");

            //RuleFor(x => x.UsrEmail)
            //    .EmailAddress().WithMessage("Invalid email format")
            //    .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            //    .When(x => !string.IsNullOrEmpty(x.UsrEmail));

            //RuleFor(x => x.UsrGender)
            //    .Must(g => g == null || g == "M" || g == "F")
            //    .WithMessage("Gender must be 'M' for Male or 'F' for Female");

            //RuleFor(x => x.UsrStatus)
            //    .Must(s => s == null || s == "A" || s == "S" || s == "C")
            //    .WithMessage("Status must be 'A' for Active, 'S' for Suspend, or 'C' for Close");

            //RuleFor(x => x.UsrNicno)
            //    .MaximumLength(20).WithMessage("NIC number must not exceed 20 characters");

            //RuleFor(x => x.UsrPhone)
            //    .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

            //RuleFor(x => x.UsrMobile)
            //    .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters");

            //RuleFor(x => x.DlrCode)
            //    .MaximumLength(10).WithMessage("Dealer code must not exceed 10 characters");

            //RuleFor(x => x.CoCode)
            //    .MaximumLength(10).WithMessage("Company code must not exceed 10 characters");
        }
    }

    // UPDATE COMMAND VALIDATOR
    public class UpdateUsrInfoCommandValidator : AbstractValidator<UpdateUsrInfoCommand>
    {
        public UpdateUsrInfoCommandValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request is required")
                .SetValidator(new UpdateUsrInfoRequestValidator());
        }
    }

    public class UpdateUsrInfoRequestValidator : AbstractValidator<UpdateUsrInfoRequest>
    {
        public UpdateUsrInfoRequestValidator()
        {
            RuleFor(x => x.UsrName)
                .MaximumLength(100).WithMessage("User name must not exceed 100 characters");

            //RuleFor(x => x.UsrEmail)
            //    .EmailAddress().WithMessage("Invalid email format")
            //    .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            //    .When(x => !string.IsNullOrEmpty(x.UsrEmail));

            //RuleFor(x => x.UsrGender)
            //    .Must(g => g == null || g == "M" || g == "F")
            //    .WithMessage("Gender must be 'M' for Male or 'F' for Female");

            //RuleFor(x => x.UsrStatus)
            //    .Must(s => s == null || s == "A" || s == "S" || s == "C")
            //    .WithMessage("Status must be 'A' for Active, 'S' for Suspend, or 'C' for Close");

            //RuleFor(x => x.UsrNicno)
            //    .MaximumLength(20).WithMessage("NIC number must not exceed 20 characters");

            //RuleFor(x => x.UsrPhone)
            //    .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

            //RuleFor(x => x.UsrMobile)
            //    .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters");

            //RuleFor(x => x.RmsType)
            //    .MaximumLength(10).WithMessage("RMS Type must not exceed 10 characters")
            //    .When(x => !string.IsNullOrEmpty(x.RmsType));
        }
    }

    // DELETE COMMAND VALIDATOR
    public class DeleteUsrInfoCommandValidator : AbstractValidator<DeleteUsrInfoCommand>
    {
        public DeleteUsrInfoCommandValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters");
        }
    }

    #endregion

    #region Query Validator
    // GET BY ID QUERY VALIDATOR
    public class GetUsrInfoByIdQueryValidator : AbstractValidator<GetUsrInfoByIdQuery>
    {
        public GetUsrInfoByIdQueryValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters");
        }
    }

    // GET PAGED QUERY VALIDATOR
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

            RuleFor(x => x.CoCode)
                .MaximumLength(10).WithMessage("Company code must not exceed 10 characters");

            RuleFor(x => x.DlrCode)
                .MaximumLength(10).WithMessage("Dealer code must not exceed 10 characters");

            RuleFor(x => x.RmsType)
                .MaximumLength(10).WithMessage("RMS Type must not exceed 10 characters");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term must not exceed 100 characters");
        }
    }

    #endregion
}