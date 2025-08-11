using FluentValidation;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       GetUsersQuery
/// Author:      Md. Raihan Sharif
/// Purpose:     This class defines a complex query model for retrieving users with various filtering options.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Application.Models.Requests
{
    /// <summary>
    /// Query model for GET users with complex filtering
    /// Alternative to simple query parameters
    /// </summary>
    public class GetUsersQuery
    {
        /// <summary>
        /// Page number (minimum 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Page size (1-100)
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filter by user status (A=Active, S=Suspend, C=Close)
        /// </summary>
        public string? UsrStatus { get; set; }

        /// <summary>
        /// Filter by company code
        /// </summary>
        public string? CoCode { get; set; }

        /// <summary>
        /// Filter by dealer code
        /// </summary>
        public string? DlrCode { get; set; }

        /// <summary>
        /// Filter by RMS type
        /// </summary>
        public string? RmsType { get; set; }

        /// <summary>
        /// Search term (searches in UsrId, UsrName, UsrEmail)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Sort field
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction (asc/desc)
        /// </summary>
        public string? SortDirection { get; set; } = "asc";

        /// <summary>
        /// Date range filter - From date
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Date range filter - To date
        /// </summary>
        public DateTime? DateTo { get; set; }
    }

    /// <summary>
    /// FluentValidation validator for GetUsersQuery
    /// Shows how to validate complex query objects
    /// </summary>
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        private readonly string[] _validSortFields = { "UsrId", "UsrName", "UsrEmail", "UsrCreationDate", "UsrLastUpdatedDate" };
        private readonly string[] _validSortDirections = { "asc", "desc" };
        private readonly string[] _validStatuses = { "A", "S", "C" };

        public GetUsersQueryValidator()
        {
            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0")
                .LessThanOrEqualTo(1000).WithMessage("Page number cannot exceed 1000");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

            // Status validation
            RuleFor(x => x.UsrStatus)
                .Must(status => status == null || _validStatuses.Contains(status))
                .WithMessage($"User status must be one of: {string.Join(", ", _validStatuses)}")
                .When(x => !string.IsNullOrEmpty(x.UsrStatus));

            // Code validations
            RuleFor(x => x.CoCode)
                .MaximumLength(10).WithMessage("Company code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.DlrCode)
                .MaximumLength(10).WithMessage("Dealer code must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.DlrCode));

            RuleFor(x => x.RmsType)
                .MaximumLength(10).WithMessage("RMS Type must not exceed 10 characters")
                .When(x => !string.IsNullOrEmpty(x.RmsType));

            // Search term validation
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term must not exceed 100 characters")
                .MinimumLength(2).WithMessage("Search term must be at least 2 characters")
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));

            // Sorting validation
            RuleFor(x => x.SortBy)
                .Must(sortBy => sortBy == null || _validSortFields.Contains(sortBy))
                .WithMessage($"Sort field must be one of: {string.Join(", ", _validSortFields)}")
                .When(x => !string.IsNullOrEmpty(x.SortBy));

            RuleFor(x => x.SortDirection)
                .Must(direction => direction == null || _validSortDirections.Contains(direction.ToLower()))
                .WithMessage($"Sort direction must be one of: {string.Join(", ", _validSortDirections)}")
                .When(x => !string.IsNullOrEmpty(x.SortDirection));

            // Date range validation
            RuleFor(x => x.DateFrom)
                .LessThanOrEqualTo(DateTime.Today.AddDays(1)).WithMessage("Date from cannot be in the future")
                .When(x => x.DateFrom.HasValue);

            RuleFor(x => x.DateTo)
                .LessThanOrEqualTo(DateTime.Today.AddDays(1)).WithMessage("Date to cannot be in the future")
                .When(x => x.DateTo.HasValue);

            RuleFor(x => x)
                .Must(x => x.DateFrom == null || x.DateTo == null || x.DateFrom <= x.DateTo)
                .WithMessage("Date from must be less than or equal to date to")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);

            // Business rule: Date range cannot exceed 1 year
            RuleFor(x => x)
                .Must(x => x.DateFrom == null || x.DateTo == null || (x.DateTo.Value - x.DateFrom.Value).TotalDays <= 365)
                .WithMessage("Date range cannot exceed 365 days")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
        }
    }
}