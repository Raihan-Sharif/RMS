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
            
            // Exposure amounts are required and must be >= 0
            RuleFor(x => x.UsrExpsBuyAmt).ValidExposureAmount();
            RuleFor(x => x.UsrExpsSellAmt).ValidExposureAmount();
            RuleFor(x => x.UsrExpsTotalAmt).ValidExposureAmount();
            
            // Business rule: If buy and sell are both true, total must be false
            RuleFor(x => x)
                .Must(NotHaveBothBuySellAndTotal)
                .WithMessage("Cannot have both Buy/Sell exposure and Total exposure enabled at the same time");
            
            // Business rule: At least one exposure type must be enabled
            RuleFor(x => x)
                .Must(HaveAtLeastOneExposureType)
                .WithMessage("At least one exposure type (Buy, Sell, or Total) must be enabled");
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }

        private static bool NotHaveBothBuySellAndTotal(CreateUserExposureRequest request)
        {
            // If both buy and sell are checked, total should not be checked
            if (request.UsrExpsCheckBuy && request.UsrExpsCheckSell)
            {
                return !request.UsrExpsCheckTotal;
            }
            return true;
        }

        private static bool HaveAtLeastOneExposureType(CreateUserExposureRequest request)
        {
            return request.UsrExpsCheckBuy || request.UsrExpsCheckSell || request.UsrExpsCheckTotal;
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
            
            RuleFor(x => x.UsrExpsBuyAmt).ValidExposureAmountNullable()
                .When(x => x.UsrExpsBuyAmt.HasValue);
            
            RuleFor(x => x.UsrExpsSellAmt).ValidExposureAmountNullable()
                .When(x => x.UsrExpsSellAmt.HasValue);
            
            RuleFor(x => x.UsrExpsTotalAmt).ValidExposureAmountNullable()
                .When(x => x.UsrExpsTotalAmt.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateUserExposureRequest request)
        {
            return request.UsrExpsCheckBuy.HasValue ||
                   request.UsrExpsBuyAmt.HasValue ||
                   request.UsrExpsCheckSell.HasValue ||
                   request.UsrExpsSellAmt.HasValue ||
                   request.UsrExpsCheckTotal.HasValue ||
                   request.UsrExpsTotalAmt.HasValue ||
                   request.UsrExpsWithShrLimit.HasValue ||
                   !string.IsNullOrEmpty(request.Remarks);
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