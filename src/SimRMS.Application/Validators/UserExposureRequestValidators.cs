using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       UserExposure Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for User Exposure operations
/// Creation:    04/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Validators
{

    /// <summary>
    /// Validator for creating UserExposure
    /// </summary>
    public class CreateUserExposureRequestValidator : AbstractValidator<CreateUserExposureRequest>
    {
        public CreateUserExposureRequestValidator()
        {
            RuleFor(x => x.UsrId).ValidUsrID();

            // Business rule: Cannot have both Buy/Sell and Total enabled at the same time
            RuleFor(x => x)
                .Must(NotHaveBothBuySellAndTotal)
                .WithMessage("Cannot have both Buy/Sell exposure and Total exposure enabled at the same time");

            // Business rule: At least one exposure type must be enabled
            RuleFor(x => x)
                .Must(HaveAtLeastOneExposureType)
                .WithMessage("At least one exposure type (Buy/Sell, or Total) must be enabled");

            // Business rule: When Buy/Sell are enabled, amounts must be > 0 and Total must be 0
            RuleFor(x => x)
                .Must(ValidateBuySellExposureAmounts)
                .WithMessage("When Buy/Sell exposure is enabled, Buy and Sell amounts must be greater than 0 and Total amount must be 0");

            // Business rule: When Total is enabled, amount must be > 0 and Buy/Sell must be 0
            RuleFor(x => x)
                .Must(ValidateTotalExposureAmount)
                .WithMessage("When Total exposure is enabled, Total amount must be greater than 0 and Buy/Sell amounts must be 0");

            // Basic exposure amount validations (>= 0)
            RuleFor(x => x.UsrExpsBuyAmt).ValidExposureAmount();
            RuleFor(x => x.UsrExpsSellAmt).ValidExposureAmount();
            RuleFor(x => x.UsrExpsTotalAmt).ValidExposureAmount();

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }

        private static bool NotHaveBothBuySellAndTotal(CreateUserExposureRequest request)
        {
            // Cannot have both Buy/Sell exposure and Total exposure enabled
            var buySellEnabled = request.UsrExpsCheckBuy || request.UsrExpsCheckSell;
            var totalEnabled = request.UsrExpsCheckTotal;

            return !(buySellEnabled && totalEnabled);
        }

        private static bool HaveAtLeastOneExposureType(CreateUserExposureRequest request)
        {
            return request.UsrExpsCheckBuy || request.UsrExpsCheckSell || request.UsrExpsCheckTotal;
        }

        private static bool ValidateBuySellExposureAmounts(CreateUserExposureRequest request)
        {
            // If any Buy/Sell exposure is enabled
            if (request.UsrExpsCheckBuy || request.UsrExpsCheckSell)
            {
                // When Buy is enabled, amount must be > 0
                if (request.UsrExpsCheckBuy && request.UsrExpsBuyAmt <= 0)
                    return false;

                // When Sell is enabled, amount must be > 0
                if (request.UsrExpsCheckSell && request.UsrExpsSellAmt <= 0)
                    return false;

                // Total amount must be 0 when Buy/Sell is enabled
                if (request.UsrExpsTotalAmt != 0)
                    return false;
            }
            return true;
        }

        private static bool ValidateTotalExposureAmount(CreateUserExposureRequest request)
        {
            // If Total exposure is enabled
            if (request.UsrExpsCheckTotal)
            {
                // Total amount must be > 0
                if (request.UsrExpsTotalAmt <= 0)
                    return false;

                // Buy and Sell amounts must be 0 when Total is enabled
                if (request.UsrExpsBuyAmt != 0 || request.UsrExpsSellAmt != 0)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Validator for updating UserExposure
    /// </summary>
    public class UpdateUserExposureRequestValidator : AbstractValidator<UpdateUserExposureRequest>
    {
        public UpdateUserExposureRequestValidator()
        {
            RuleFor(x => x.UsrId).ValidUsrID();

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");

            // Business rule: Cannot have both Buy/Sell and Total enabled at the same time
            RuleFor(x => x)
                .Must(NotHaveBothBuySellAndTotalUpdate)
                .WithMessage("Cannot have both Buy/Sell exposure and Total exposure enabled at the same time")
                .When(x => HasExposureCheckboxUpdates(x));

            // Business rule: At least one exposure type must be enabled when updating checkboxes
            RuleFor(x => x)
                .Must(HaveAtLeastOneExposureTypeUpdate)
                .WithMessage("At least one exposure type (Buy/Sell, or Total) must be enabled")
                .When(x => HasExposureCheckboxUpdates(x));

            // Business rule: When Buy/Sell are enabled, amounts must be > 0 and Total must be 0
            RuleFor(x => x)
                .Must(ValidateBuySellExposureAmountsUpdate)
                .WithMessage("When Buy/Sell exposure is enabled, Buy and Sell amounts must be greater than 0 and Total amount must be 0")
                .When(x => HasExposureUpdates(x));

            // Business rule: When Total is enabled, amount must be > 0 and Buy/Sell must be 0
            RuleFor(x => x)
                .Must(ValidateTotalExposureAmountUpdate)
                .WithMessage("When Total exposure is enabled, Total amount must be greater than 0 and Buy/Sell amounts must be 0")
                .When(x => HasExposureUpdates(x));

            // Basic exposure amount validations (>= 0)
            RuleFor(x => x.PendingUsrExpsBuyAmt).ValidExposureAmountNullable()
                .When(x => x.PendingUsrExpsBuyAmt.HasValue);

            RuleFor(x => x.PendingUsrExpsSellAmt).ValidExposureAmountNullable()
                .When(x => x.PendingUsrExpsSellAmt.HasValue);

            RuleFor(x => x.PendingUsrExpsTotalAmt).ValidExposureAmountNullable()
                .When(x => x.PendingUsrExpsTotalAmt.HasValue);

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateUserExposureRequest request)
        {
            return request.PendingUsrExpsCheckBuy.HasValue ||
                   request.PendingUsrExpsBuyAmt.HasValue ||
                   request.PendingUsrExpsCheckSell.HasValue ||
                   request.PendingUsrExpsSellAmt.HasValue ||
                   request.PendingUsrExpsCheckTotal.HasValue ||
                   request.PendingUsrExpsTotalAmt.HasValue ||
                   !string.IsNullOrEmpty(request.Remarks);
        }

        private static bool HasExposureCheckboxUpdates(UpdateUserExposureRequest request)
        {
            return request.PendingUsrExpsCheckBuy.HasValue ||
                   request.PendingUsrExpsCheckSell.HasValue ||
                   request.PendingUsrExpsCheckTotal.HasValue;
        }

        private static bool HasExposureUpdates(UpdateUserExposureRequest request)
        {
            return request.PendingUsrExpsCheckBuy.HasValue ||
                   request.PendingUsrExpsBuyAmt.HasValue ||
                   request.PendingUsrExpsCheckSell.HasValue ||
                   request.PendingUsrExpsSellAmt.HasValue ||
                   request.PendingUsrExpsCheckTotal.HasValue ||
                   request.PendingUsrExpsTotalAmt.HasValue;
        }

        private static bool NotHaveBothBuySellAndTotalUpdate(UpdateUserExposureRequest request)
        {
            // For updates, we only check if checkbox values are being updated
            var buyEnabled = request.PendingUsrExpsCheckBuy == true;
            var sellEnabled = request.PendingUsrExpsCheckSell == true;
            var totalEnabled = request.PendingUsrExpsCheckTotal == true;

            var buySellEnabled = buyEnabled || sellEnabled;
            return !(buySellEnabled && totalEnabled);
        }

        private static bool HaveAtLeastOneExposureTypeUpdate(UpdateUserExposureRequest request)
        {
            // For updates, check if at least one checkbox is being set to true
            return (request.PendingUsrExpsCheckBuy == true) ||
                   (request.PendingUsrExpsCheckSell == true) ||
                   (request.PendingUsrExpsCheckTotal == true);
        }

        private static bool ValidateBuySellExposureAmountsUpdate(UpdateUserExposureRequest request)
        {
            // If any Buy/Sell exposure is being enabled
            if ((request.PendingUsrExpsCheckBuy == true) || (request.PendingUsrExpsCheckSell == true))
            {
                // When Buy is enabled, amount must be > 0 (if amount is being updated)
                if (request.PendingUsrExpsCheckBuy == true && request.PendingUsrExpsBuyAmt.HasValue && request.PendingUsrExpsBuyAmt <= 0)
                    return false;

                // When Sell is enabled, amount must be > 0 (if amount is being updated)
                if (request.PendingUsrExpsCheckSell == true && request.PendingUsrExpsSellAmt.HasValue && request.PendingUsrExpsSellAmt <= 0)
                    return false;

                // Total amount must be 0 when Buy/Sell is enabled (if total amount is being updated)
                if (request.PendingUsrExpsTotalAmt.HasValue && request.PendingUsrExpsTotalAmt != 0)
                    return false;
            }
            return true;
        }

        private static bool ValidateTotalExposureAmountUpdate(UpdateUserExposureRequest request)
        {
            // If Total exposure is being enabled
            if (request.PendingUsrExpsCheckTotal == true)
            {
                // Total amount must be > 0 (if amount is being updated)
                if (request.PendingUsrExpsTotalAmt.HasValue && request.PendingUsrExpsTotalAmt <= 0)
                    return false;

                // Buy and Sell amounts must be 0 when Total is enabled (if amounts are being updated)
                if ((request.PendingUsrExpsBuyAmt.HasValue && request.PendingUsrExpsBuyAmt != 0) ||
                    (request.PendingUsrExpsSellAmt.HasValue && request.PendingUsrExpsSellAmt != 0))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Validator for deleting UserExposure
    /// </summary>
    public class DeleteUserExposureRequestValidator : AbstractValidator<DeleteUserExposureRequest>
    {
        public DeleteUserExposureRequestValidator()
        {
            RuleFor(x => x.UsrId).ValidUsrID();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing UserExposure in workflow
    /// </summary>
    public class AuthorizeUserExposureRequestValidator : AbstractValidator<AuthorizeUserExposureRequest>
    {
        public AuthorizeUserExposureRequestValidator()
        {
            RuleFor(x => x.UsrId).ValidUsrID();
            
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetUserExposureWorkflowListRequestValidator : AbstractValidator<GetUserExposureWorkflowListRequest>
    {
        public GetUserExposureWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            
            RuleFor(x => x.PageSize).ValidPageSize(100);
            
            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();
            
            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}