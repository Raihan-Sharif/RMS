using FluentValidation;
using SimRMS.Application.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Validators
{
    public class UpdateUsrInfoRequestValidator : AbstractValidator<UpdateUsrInfoRequest>
    {
        public UpdateUsrInfoRequestValidator()
        {
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
