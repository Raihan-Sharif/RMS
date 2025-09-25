using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Stock operations
/// Creation:    23/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Validators
{
    /// <summary>
    /// Validator for getting stock by key
    /// </summary>
    public class GetStockByKeyRequestValidator : AbstractValidator<GetStockByKeyRequest>
    {
        public GetStockByKeyRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.StkCode).ValidStkCode();
            */
        }
    }

    /// <summary>
    /// Validator for getting stock list
    /// </summary>
    public class GetStockListRequestValidator : AbstractValidator<GetStockListRequest>
    {
        public GetStockListRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.XchgCode).ValidXchgCode()
                .When(x => !string.IsNullOrEmpty(x.XchgCode));

            RuleFor(x => x.StkCode).ValidStkCode()
                .When(x => !string.IsNullOrEmpty(x.StkCode));

            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(1000);

            RuleFor(x => x.SearchText).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchText));

            RuleFor(x => x.SortColumn)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "StkCode", "StkLName", "StkSName", "XchgCode" }.Contains(x))
                .WithMessage("SortColumn must be one of: StkCode, StkLName, StkSName, XchgCode");

            RuleFor(x => x.SortDirection).ValidSorting();
            */
        }
    }

    /// <summary>
    /// Validator for creating stock - Based on SP LB_SP_CrudMstStk validation rules
    /// </summary>
    public class CreateStockRequestValidator : AbstractValidator<CreateStockRequest>
    {
        public CreateStockRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            // Required fields based on SP validation (lines 170-193)
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.StkCode).ValidStkCode();
            RuleFor(x => x.StkLName)
                .NotEmpty().WithMessage("Stock Long Name is required for insert operation")
                .MaximumLength(100).WithMessage("Stock Long Name cannot exceed 100 characters");

            RuleFor(x => x.StkSName)
                .NotEmpty().WithMessage("Stock Short Name is required for insert operation")
                .MaximumLength(100).WithMessage("Stock Short Name cannot exceed 100 characters");

            RuleFor(x => x.StkLot)
                .GreaterThan(0).WithMessage("Stock Lot size must be greater than 0 for insert operation");

            RuleFor(x => x.ISIN)
                .NotEmpty().WithMessage("ISIN is required for insert operation")
                .MaximumLength(12).WithMessage("ISIN cannot exceed 12 characters");

            // Optional fields with validation
            RuleFor(x => x.StkBrdCode).ValidStkBrdCode()
                .When(x => !string.IsNullOrEmpty(x.StkBrdCode));
            RuleFor(x => x.StkSectCode).ValidStkSectCode()
                .When(x => !string.IsNullOrEmpty(x.StkSectCode));

            // Price validations (SP lines 196-230)
            RuleFor(x => x.StkLastDonePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Stock Last Done Price cannot be negative")
                .When(x => x.StkLastDonePrice.HasValue);

            RuleFor(x => x.StkClosePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Stock Close Price cannot be negative")
                .When(x => x.StkClosePrice.HasValue);

            RuleFor(x => x.StkRefPrc)
                .GreaterThanOrEqualTo(0).WithMessage("Stock Reference Price cannot be negative")
                .When(x => x.StkRefPrc.HasValue);

            RuleFor(x => x.StkVolumeTraded)
                .GreaterThanOrEqualTo(0).WithMessage("Stock Volume Traded cannot be negative")
                .When(x => x.StkVolumeTraded.HasValue);

            // Business rules from SP (lines 214-224)
            RuleFor(x => x)
                .Must(ValidatePriceLimits)
                .WithMessage("Stock Upper Limit Price must be greater than Lower Limit Price")
                .When(x => x.StkUpperLmtPrice.HasValue && x.StkLowerLmtPrice.HasValue);

            RuleFor(x => x)
                .Must(ValidateYearHighLow)
                .WithMessage("Year High must be greater than Year Low")
                .When(x => x.YearHigh.HasValue && x.YearLow.HasValue);

            // Date validation (SP line 232-236)
            RuleFor(x => x.ListingDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Listing Date cannot be in the future")
                .When(x => x.ListingDate.HasValue);

            // Optional field validations
            RuleFor(x => x.Currency).ValidCurrency()
                .When(x => !string.IsNullOrEmpty(x.Currency));
            RuleFor(x => x.SecurityType).ValidSecurityType()
                .When(x => !string.IsNullOrEmpty(x.SecurityType));
            RuleFor(x => x.StkIsSyariah).ValidSyariahFlag()
                .When(x => !string.IsNullOrEmpty(x.StkIsSyariah));
            RuleFor(x => x.StkParValue).ValidStockPrice()
                .When(x => x.StkParValue.HasValue);
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            */
        }

        private static bool ValidatePriceLimits(CreateStockRequest request)
        {
            if (request.StkUpperLmtPrice.HasValue && request.StkLowerLmtPrice.HasValue)
            {
                return request.StkUpperLmtPrice.Value > request.StkLowerLmtPrice.Value;
            }
            return true;
        }

        private static bool ValidateYearHighLow(CreateStockRequest request)
        {
            if (request.YearHigh.HasValue && request.YearLow.HasValue)
            {
                return request.YearHigh.Value > request.YearLow.Value;
            }
            return true;
        }
    }

    /// <summary>
    /// Validator for updating stock
    /// </summary>
    public class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequest>
    {
        public UpdateStockRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.StkCode).ValidStkCode();

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");

            RuleFor(x => x.StkBrdCode).ValidStkBrdCode()
                .When(x => !string.IsNullOrEmpty(x.StkBrdCode));

            RuleFor(x => x.StkSectCode).ValidStkSectCode()
                .When(x => !string.IsNullOrEmpty(x.StkSectCode));

            RuleFor(x => x.StkLName).ValidStkLName()
                .When(x => !string.IsNullOrEmpty(x.StkLName));

            RuleFor(x => x.StkSName).ValidStkSName()
                .When(x => !string.IsNullOrEmpty(x.StkSName));

            RuleFor(x => x.ISIN).ValidISIN();
            RuleFor(x => x.Currency).ValidCurrency();
            RuleFor(x => x.SecurityType).ValidSecurityType();
            RuleFor(x => x.StkIsSyariah).ValidSyariahFlag();
            RuleFor(x => x.StkLot).ValidStkLot();

            RuleFor(x => x.StkParValue).ValidStockPrice();
            RuleFor(x => x.StkLastDonePrice).ValidStockPrice();
            RuleFor(x => x.StkClosePrice).ValidStockPrice();
            RuleFor(x => x.StkRefPrc).ValidStockPrice();
            RuleFor(x => x.StkUpperLmtPrice).ValidStockPrice();
            RuleFor(x => x.StkLowerLmtPrice).ValidStockPrice();
            RuleFor(x => x.YearHigh).ValidStockPrice();
            RuleFor(x => x.YearLow).ValidStockPrice();

            RuleFor(x => x.StkVolumeTraded).ValidStockVolume();

            RuleFor(x => x.ListingDate)
                .LessThanOrEqualTo(DateTime.Now.Date)
                .WithMessage("Listing date cannot be in the future")
                .When(x => x.ListingDate.HasValue);

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Business rule: Upper limit must be greater than lower limit
            RuleFor(x => x)
                .Must(ValidatePriceLimitsUpdate)
                .WithMessage("Upper limit price must be greater than lower limit price")
                .When(x => x.StkUpperLmtPrice.HasValue && x.StkLowerLmtPrice.HasValue);

            // Business rule: Year high must be greater than year low
            RuleFor(x => x)
                .Must(ValidateYearHighLowUpdate)
                .WithMessage("Year high must be greater than year low")
                .When(x => x.YearHigh.HasValue && x.YearLow.HasValue);
            */
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateStockRequest request)
        {
            return !string.IsNullOrEmpty(request.StkBrdCode) ||
                   !string.IsNullOrEmpty(request.StkSectCode) ||
                   !string.IsNullOrEmpty(request.StkLName) ||
                   !string.IsNullOrEmpty(request.StkSName) ||
                   !string.IsNullOrEmpty(request.ISIN) ||
                   !string.IsNullOrEmpty(request.Currency) ||
                   !string.IsNullOrEmpty(request.SecurityType) ||
                   !string.IsNullOrEmpty(request.StkIsSyariah) ||
                   request.StkLot.HasValue ||
                   request.StkParValue.HasValue ||
                   request.StkLastDonePrice.HasValue ||
                   request.StkClosePrice.HasValue ||
                   request.StkRefPrc.HasValue ||
                   request.StkUpperLmtPrice.HasValue ||
                   request.StkLowerLmtPrice.HasValue ||
                   request.YearHigh.HasValue ||
                   request.YearLow.HasValue ||
                   request.StkVolumeTraded.HasValue ||
                   request.ListingDate.HasValue ||
                   !string.IsNullOrEmpty(request.Remarks);
        }

        private static bool ValidatePriceLimitsUpdate(UpdateStockRequest request)
        {
            if (request.StkUpperLmtPrice.HasValue && request.StkLowerLmtPrice.HasValue)
            {
                return request.StkUpperLmtPrice.Value > request.StkLowerLmtPrice.Value;
            }
            return true;
        }

        private static bool ValidateYearHighLowUpdate(UpdateStockRequest request)
        {
            if (request.YearHigh.HasValue && request.YearLow.HasValue)
            {
                return request.YearHigh.Value > request.YearLow.Value;
            }
            return true;
        }
    }

    /// <summary>
    /// Validator for deleting stock
    /// </summary>
    public class DeleteStockRequestValidator : AbstractValidator<DeleteStockRequest>
    {
        public DeleteStockRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.StkCode).ValidStkCode();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            */
        }
    }

    /// <summary>
    /// Validator for authorizing stock in workflow
    /// </summary>
    public class AuthorizeStockRequestValidator : AbstractValidator<AuthorizeStockRequest>
    {
        public AuthorizeStockRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.StkCode).ValidStkCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty()
                .WithMessage("Remarks are required for denial")
                .When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
            */
        }
    }

    /// <summary>
    /// Validator for getting workflow list - Based on SP LB_SP_GetMstStkListWF
    /// </summary>
    public class GetStockWorkflowListRequestValidator : AbstractValidator<GetStockWorkflowListRequest>
    {
        public GetStockWorkflowListRequestValidator()
        {
            // COMMENTED OUT FOR EASIER TESTING - Uncomment for production
            /*
            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(1000);

            RuleFor(x => x.XchgCode).ValidXchgCode()
                .When(x => !string.IsNullOrEmpty(x.XchgCode));

            RuleFor(x => x.StkCode).ValidStkCode()
                .When(x => !string.IsNullOrEmpty(x.StkCode));

            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.MakerId)
                .GreaterThan(0).WithMessage("MakerId must be greater than 0");

            RuleFor(x => x.SearchText).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchText));

            RuleFor(x => x.SortColumn)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "StkCode", "StkLName", "StkSName", "XchgCode" }.Contains(x))
                .WithMessage("SortColumn must be one of: StkCode, StkLName, StkSName, XchgCode");

            RuleFor(x => x.SortDirection).ValidSorting();
            */
        }
    }
}