using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       ClientExposure Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Client Exposure operations
/// Creation:    17/Sep/2025
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
    /// Validator for updating ClientExposure
    /// </summary>
    public class UpdateClientExposureRequestValidator : AbstractValidator<UpdateClientExposureRequest>
    {
        public UpdateClientExposureRequestValidator()
        {
            RuleFor(x => x.ClntCode).ValidClientCode();
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode();


            RuleFor(x => x.ClntExpsBuyAmtTopUp).ValidExposureAmountNullable()
                .When(x => x.ClntExpsBuyAmtTopUp.HasValue);

            // Top-up expiry validation
            //RuleFor(x => x.ClntExpsBuyAmtTopUpExpiry).ValidTopUpExpiry();

            //RuleFor(x => x.Remarks).ValidRemarks()
            //    .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateClientExposureRequest request)
        {
            return
                   request.ClntExpsBuyAmtTopUp.HasValue;
        }
    }

    /// <summary>
    /// Validator for deleting ClientExposure
    /// </summary>
    public class DeleteClientExposureRequestValidator : AbstractValidator<DeleteClientExposureRequest>
    {
        public DeleteClientExposureRequestValidator()
        {
            RuleFor(x => x.ClntCode).ValidClientCode();
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing ClientExposure in workflow
    /// </summary>
    public class AuthorizeClientExposureRequestValidator : AbstractValidator<AuthorizeClientExposureRequest>
    {
        public AuthorizeClientExposureRequestValidator()
        {
            RuleFor(x => x.ClntCode).ValidClientCode();
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetClientExposureWorkflowListRequestValidator : AbstractValidator<GetClientExposureWorkflowListRequest>
    {
        public GetClientExposureWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(100);

            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.ClntCode).ValidClientCode()
                .When(x => !string.IsNullOrEmpty(x.ClntCode));

            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));

            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}