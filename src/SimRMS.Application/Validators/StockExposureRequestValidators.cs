using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Exposure Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Stock Exposure operations
/// Creation:    08/Oct/2025
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
    /// Stock Exposure specific validation rules
    /// </summary>
    public static class StockExposureValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidDataType<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Data type is required")
                .MaximumLength(1).WithMessage("Data type cannot exceed 1 character")
                .Must(x => "RBCTU".Contains(x))
                .WithMessage("Data type must be R (Broker), B (Branch), C (Client), T (Client Type), or U (User)");
        }

        public static IRuleBuilderOptions<T, string> ValidCtrlType<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Control type is required")
                .MaximumLength(1).WithMessage("Control type cannot exceed 1 character")
                .Must(x => x == "T")
                .WithMessage("Control type must be T (Stock Control)");
        }

        public static IRuleBuilderOptions<T, string> ValidCtrlStatus<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Control status is required")
                .MaximumLength(1).WithMessage("Control status cannot exceed 1 character")
                .Must(x => "BSNY".Contains(x))
                .WithMessage("Control status must be B (Buy Suspend), S (Sell Suspend), N (Both Suspended), or Y (Both Allowed)");
        }

        public static IRuleBuilderOptions<T, string?> ValidClntType<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(5).WithMessage("Client type cannot exceed 5 characters");
        }
    }

    /// <summary>
    /// Validator for creating Stock Exposure
    /// </summary>
    public class CreateStockExposureRequestValidator : AbstractValidator<CreateStockExposureRequest>
    {
        public CreateStockExposureRequestValidator()
        {
            RuleFor(x => x.DataType).ValidDataType();
            RuleFor(x => x.CtrlType).ValidCtrlType();
            RuleFor(x => x.StkCode).ValidStkCode();
            RuleFor(x => x.CtrlStatus).ValidCtrlStatus();

            // Optional fields based on DataType
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.ClntType).ValidClntType()
                .When(x => !string.IsNullOrEmpty(x.ClntType));

            RuleFor(x => x.XchgCode).ValidXchgCode()
                .When(x => !string.IsNullOrEmpty(x.XchgCode));

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Business rule: Must provide exactly one scope field
            RuleFor(x => x)
                .Must(HaveExactlyOneScopeField)
                .WithMessage("Exactly one scope field must be provided (CoCode, CoBrchCode, UsrID, ClntCode, or ClntType)");
        }

        private static bool HaveExactlyOneScopeField(CreateStockExposureRequest request)
        {
            int scopeFieldCount = 0;
            if (!string.IsNullOrEmpty(request.CoCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.CoBrchCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.UsrID)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntType)) scopeFieldCount++;

            return scopeFieldCount == 1;
        }
    }

    /// <summary>
    /// Validator for updating Stock Exposure
    /// </summary>
    public class UpdateStockExposureRequestValidator : AbstractValidator<UpdateStockExposureRequest>
    {
        public UpdateStockExposureRequestValidator()
        {
            RuleFor(x => x.DataType).ValidDataType();
            RuleFor(x => x.CtrlType).ValidCtrlType();
            RuleFor(x => x.StkCode).ValidStkCode();

            RuleFor(x => x.CtrlStatus).ValidCtrlStatus()
                .When(x => !string.IsNullOrEmpty(x.CtrlStatus));

            // Optional fields based on DataType
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.ClntType).ValidClntType()
                .When(x => !string.IsNullOrEmpty(x.ClntType));

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Business rule: Must provide exactly one scope field for identification
            RuleFor(x => x)
                .Must(HaveExactlyOneScopeField)
                .WithMessage("Exactly one scope field must be provided (CoCode, CoBrchCode, UsrID, ClntCode, or ClntType)");

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update (CtrlStatus or Remarks)");
        }

        private static bool HaveExactlyOneScopeField(UpdateStockExposureRequest request)
        {
            int scopeFieldCount = 0;
            if (!string.IsNullOrEmpty(request.CoCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.CoBrchCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.UsrID)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntType)) scopeFieldCount++;

            return scopeFieldCount == 1;
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateStockExposureRequest request)
        {
            return !string.IsNullOrEmpty(request.CtrlStatus) || !string.IsNullOrEmpty(request.Remarks);
        }
    }

    /// <summary>
    /// Validator for deleting Stock Exposure
    /// </summary>
    public class DeleteStockExposureRequestValidator : AbstractValidator<DeleteStockExposureRequest>
    {
        public DeleteStockExposureRequestValidator()
        {
            RuleFor(x => x.DataType).ValidDataType();
            RuleFor(x => x.CtrlType).ValidCtrlType();
            RuleFor(x => x.StkCode).ValidStkCode();

            // Optional fields based on DataType
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.ClntType).ValidClntType()
                .When(x => !string.IsNullOrEmpty(x.ClntType));

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Business rule: Must provide exactly one scope field
            RuleFor(x => x)
                .Must(HaveExactlyOneScopeField)
                .WithMessage("Exactly one scope field must be provided (CoCode, CoBrchCode, UsrID, ClntCode, or ClntType)");
        }

        private static bool HaveExactlyOneScopeField(DeleteStockExposureRequest request)
        {
            int scopeFieldCount = 0;
            if (!string.IsNullOrEmpty(request.CoCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.CoBrchCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.UsrID)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntType)) scopeFieldCount++;

            return scopeFieldCount == 1;
        }
    }

    /// <summary>
    /// Validator for authorizing Stock Exposure in workflow
    /// </summary>
    public class AuthorizeStockExposureRequestValidator : AbstractValidator<AuthorizeStockExposureRequest>
    {
        public AuthorizeStockExposureRequestValidator()
        {
            RuleFor(x => x.DataType).ValidDataType();
            RuleFor(x => x.CtrlType).ValidCtrlType();
            RuleFor(x => x.StkCode).ValidStkCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();

            // Optional fields based on DataType
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.ClntType).ValidClntType()
                .When(x => !string.IsNullOrEmpty(x.ClntType));

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x.Remarks).NotEmpty()
                .WithMessage("Remarks are required for denial")
                .When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);

            // Business rule: Must provide exactly one scope field
            RuleFor(x => x)
                .Must(HaveExactlyOneScopeField)
                .WithMessage("Exactly one scope field must be provided (CoCode, CoBrchCode, UsrID, ClntCode, or ClntType)");
        }

        private static bool HaveExactlyOneScopeField(AuthorizeStockExposureRequest request)
        {
            int scopeFieldCount = 0;
            if (!string.IsNullOrEmpty(request.CoCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.CoBrchCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.UsrID)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntCode)) scopeFieldCount++;
            if (!string.IsNullOrEmpty(request.ClntType)) scopeFieldCount++;

            return scopeFieldCount == 1;
        }
    }

    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetStockExposureWorkflowListRequestValidator : AbstractValidator<GetStockExposureWorkflowListRequest>
    {
        public GetStockExposureWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize(100);
            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.StkCode).ValidStkCode()
                .When(x => !string.IsNullOrEmpty(x.StkCode));

            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}
