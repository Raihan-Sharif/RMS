using FluentValidation;
using RMS.Application.Features.UsrInfo.Commands;
using RMS.Application.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Validators
{
    public class CreateUsrInfoRequestValidator : AbstractValidator<CreateUsrInfoRequest>
    {
        public CreateUsrInfoRequestValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters");

            RuleFor(x => x.RmsType)
                .NotEmpty().WithMessage("RMS Type is required")
                .MaximumLength(10).WithMessage("RMS Type must not exceed 10 characters");

            RuleFor(x => x.UsrName)
                .MaximumLength(100).WithMessage("User name must not exceed 100 characters");

            RuleFor(x => x.UsrEmail)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrEmail));

            RuleFor(x => x.UsrGender)
                .Must(g => g == null || g == "M" || g == "F")
                .WithMessage("Gender must be 'M' for Male or 'F' for Female");

            RuleFor(x => x.UsrStatus)
                .Must(s => s == null || s == "A" || s == "S" || s == "C")
                .WithMessage("Status must be 'A' for Active, 'S' for Suspend, or 'C' for Close");

            RuleFor(x => x.UsrNicno)
                .MaximumLength(20).WithMessage("NIC number must not exceed 20 characters");

            RuleFor(x => x.UsrPhone)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.UsrMobile)
                .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters");
        }
    }
}
