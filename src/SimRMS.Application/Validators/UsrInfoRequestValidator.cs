using FluentValidation;
using SimRMS.Application.Models.Requests;

namespace SimRMS.Application.Validators;

/// <summary>
/// Simplified validation using RuleSets instead of complex context management
/// Much easier to understand and maintain
/// </summary>
public class UsrInfoRequestValidator : AbstractValidator<UsrInfoRequest>
{
    public UsrInfoRequestValidator()
    {
        // CREATE-specific rules
        RuleSet("Create", () =>
        {
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("User ID must contain only uppercase letters and numbers");

            RuleFor(x => x.RmsType)
                .NotEmpty().WithMessage("RMS Type is required")
                .MaximumLength(2).WithMessage("RMS Type must not exceed 2 characters");
        });

        // UPDATE-specific rules
        RuleSet("Update", () =>
        {
            RuleFor(x => x.UsrId)
                .Empty().WithMessage("User ID should not be provided in update request body");

            RuleFor(x => x.RmsType)
                .MaximumLength(2).WithMessage("RMS Type must not exceed 2 characters")
                .When(x => !string.IsNullOrEmpty(x.RmsType));
        });

        // Common validation rules (applied to both Create and Update)
        ApplyCommonRules();
    }

    private void ApplyCommonRules()
    {
        RuleFor(x => x.UsrName)
            .MaximumLength(100).WithMessage("User name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.UsrName));

        RuleFor(x => x.UsrEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.UsrEmail));

        RuleFor(x => x.UsrNicno)
            .MaximumLength(20).WithMessage("NIC number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.UsrNicno));

        RuleFor(x => x.UsrPhone)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone number contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.UsrPhone));

        RuleFor(x => x.UsrMobile)
            .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Mobile number contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.UsrMobile));

        RuleFor(x => x.UsrGender)
            .Must(g => g == null || g == "M" || g == "F")
            .WithMessage("Gender must be 'M' for Male or 'F' for Female")
            .When(x => !string.IsNullOrEmpty(x.UsrGender));

        RuleFor(x => x.UsrStatus)
            .Must(s => s == null || s == "A" || s == "S" || s == "C")
            .WithMessage("Status must be 'A' for Active, 'S' for Suspend, or 'C' for Close")
            .When(x => !string.IsNullOrEmpty(x.UsrStatus));

        RuleFor(x => x.UsrDob)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-100)).WithMessage("Date of birth cannot be more than 100 years ago")
            .When(x => x.UsrDob.HasValue);

        RuleFor(x => x.UsrExpiryDate)
            .GreaterThan(DateTime.Today).WithMessage("Expiry date must be in the future")
            .When(x => x.UsrExpiryDate.HasValue);
    }
}
