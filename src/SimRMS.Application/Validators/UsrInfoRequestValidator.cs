using FluentValidation;
using SimRMS.Application.Models.Requests;

namespace SimRMS.Application.Validators
{
    /// <summary>
    /// Single validator for UsrInfoRequest that handles both Create and Update scenarios
    /// FIXED: Working validation context detection with shared common rules
    /// </summary>
    public class UsrInfoRequestValidator : AbstractValidator<UsrInfoRequest>
    {
        public UsrInfoRequestValidator()
        {
            // ✅ FIXED: Create/Update specific validations
            ApplyCreateOnlyRules();
            ApplyUpdateOnlyRules();

            // ✅ SHARED: Common validations for both CREATE and UPDATE
            ApplyCommonValidationRules();
        }

        /// <summary>
        /// Rules that apply only to CREATE operations
        /// </summary>
        private void ApplyCreateOnlyRules()
        {
            // UsrId validation - Required only for CREATE
            RuleFor(x => x.UsrId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(50).WithMessage("User ID must not exceed 50 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("User ID must contain only uppercase letters and numbers")
                .When(x => IsCreateOperation());

            // RmsType validation - Required only for CREATE
            RuleFor(x => x.RmsType)
                .NotEmpty().WithMessage("RMS Type is required")
                .MaximumLength(2).WithMessage("RMS Type must not exceed 2 characters")
                .When(x => IsCreateOperation());
        }

        /// <summary>
        /// Rules that apply only to UPDATE operations
        /// </summary>
        private void ApplyUpdateOnlyRules()
        {
            // UsrId should not be provided in UPDATE request body
            RuleFor(x => x.UsrId)
                .Empty().WithMessage("User ID should not be provided in update request body (use route parameter)")
                .When(x => IsUpdateOperation());

            // RmsType is optional for UPDATE but has length limit
            RuleFor(x => x.RmsType)
                .MaximumLength(2).WithMessage("RMS Type must not exceed 2 characters")
                .When(x => IsUpdateOperation() && !string.IsNullOrEmpty(x.RmsType));
        }

        /// <summary>
        /// Common validation rules shared between CREATE and UPDATE
        /// ✅ NO REPETITIVE CODE - Single definition used by both operations
        /// </summary>
        private void ApplyCommonValidationRules()
        {
            // String length validations
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

            RuleFor(x => x.DlrCode)
                .MaximumLength(10).WithMessage("Dealer code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.DlrCode));

            RuleFor(x => x.CoCode)
                .MaximumLength(10).WithMessage("Company code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoBrchCode)
                .MaximumLength(10).WithMessage("Branch code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.UsrAddr)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrAddr));

            RuleFor(x => x.UsrRace)
                .MaximumLength(50).WithMessage("Race must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrRace));

            RuleFor(x => x.UsrPassNo)
                .MaximumLength(20).WithMessage("Passport number must not exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrPassNo));

            RuleFor(x => x.ClntCode)
                .MaximumLength(10).WithMessage("Client code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.UsrLicenseNo)
                .MaximumLength(50).WithMessage("License number must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrLicenseNo));

            RuleFor(x => x.Category)
                .MaximumLength(20).WithMessage("Category must not exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.UsrChannel)
                .MaximumLength(20).WithMessage("Channel must not exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.UsrChannel));

            RuleFor(x => x.Pid)
                .MaximumLength(50).WithMessage("PID must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.Pid));

            RuleFor(x => x.PidRms)
                .MaximumLength(50).WithMessage("PID RMS must not exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.PidRms));

            // Enum-like validations
            RuleFor(x => x.UsrGender)
                .Must(g => g == null || g == "M" || g == "F")
                .WithMessage("Gender must be 'M' for Male or 'F' for Female")
                .When(x => !string.IsNullOrEmpty(x.UsrGender));

            RuleFor(x => x.UsrStatus)
                .Must(s => s == null || s == "A" || s == "S" || s == "C")
                .WithMessage("Status must be 'A' for Active, 'S' for Suspend, or 'C' for Close")
                .When(x => !string.IsNullOrEmpty(x.UsrStatus));

            // Date validations
            RuleFor(x => x.UsrDob)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
                .GreaterThan(DateTime.Today.AddYears(-100)).WithMessage("Date of birth cannot be more than 100 years ago")
                .When(x => x.UsrDob.HasValue);

            RuleFor(x => x.UsrExpiryDate)
                .GreaterThan(DateTime.Today).WithMessage("Expiry date must be in the future")
                .When(x => x.UsrExpiryDate.HasValue);

            RuleFor(x => x.UsrRegisterDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Registration date cannot be in the future")
                .When(x => x.UsrRegisterDate.HasValue);

            RuleFor(x => x.UsrTdrdate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("TDR date cannot be in the future")
                .When(x => x.UsrTdrdate.HasValue);

            RuleFor(x => x.UsrResignDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Resignation date cannot be in the future")
                .When(x => x.UsrResignDate.HasValue);

            // Numeric validations
            RuleFor(x => x.UsrType)
                .GreaterThanOrEqualTo(0).WithMessage("User type must be non-negative")
                .When(x => x.UsrType.HasValue);

            RuleFor(x => x.UsrSuperiorId)
                .GreaterThan(0).WithMessage("Superior ID must be positive")
                .When(x => x.UsrSuperiorId.HasValue);

            RuleFor(x => x.UsrNotifierId)
                .GreaterThan(0).WithMessage("Notifier ID must be positive")
                .When(x => x.UsrNotifierId.HasValue);
        }

        /// <summary>
        /// ✅ FIXED: Working method to determine if this is a CREATE operation
        /// </summary>
        private bool IsCreateOperation()
        {
            return ValidationContextManager.Current?.IsUpdate == false;
        }

        /// <summary>
        /// ✅ FIXED: Working method to determine if this is an UPDATE operation
        /// </summary>
        private bool IsUpdateOperation()
        {
            return ValidationContextManager.Current?.IsUpdate == true;
        }
    }

    /// <summary>
    /// ✅ FIXED: Working validation context manager
    /// Properly manages validation context for create/update differentiation
    /// </summary>
    public static class ValidationContextManager
    {
        private static readonly AsyncLocal<ValidationOperationContext> _current = new();

        public static ValidationOperationContext? Current
        {
            get => _current.Value;
            set => _current.Value = value!;
        }

        /// <summary>
        /// Execute validation with CREATE context
        /// </summary>
        public static async Task<FluentValidation.Results.ValidationResult> ValidateForCreateAsync<T>(
            this IValidator<T> validator, T instance)
        {
            using var context = new ValidationOperationContext { IsUpdate = false };
            _current.Value = context;
            return await validator.ValidateAsync(instance);
        }

        /// <summary>
        /// Execute validation with UPDATE context
        /// </summary>
        public static async Task<FluentValidation.Results.ValidationResult> ValidateForUpdateAsync<T>(
            this IValidator<T> validator, T instance)
        {
            using var context = new ValidationOperationContext { IsUpdate = true };
            _current.Value = context;
            return await validator.ValidateAsync(instance);
        }
    }

    /// <summary>
    /// Simple context class to track validation operation type
    /// </summary>
    public class ValidationOperationContext : IDisposable
    {
        public bool IsUpdate { get; set; }

        public void Dispose()
        {
            // Clean up context when validation is complete
            ValidationContextManager.Current = null;
        }
    }
}