using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       MstTrader Request Validators (Consolidated)
/// Author:      Asif Zaman
/// Purpose:     All validation rules for Mst Trader operations
/// Creation:    25/Aug/2025
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
    /// Validator for creating MstTrader
    /// </summary>
    public class CreateMstTraderRequestValidator : AbstractValidator<CreateMstTraderRequest>
    {
        public CreateMstTraderRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.DlrCode).ValidDlrCode();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating MstTrader
    /// </summary>
    public class UpdateMstTraderRequestValidator : AbstractValidator<UpdateMstTraderRequest>
    {
        public UpdateMstTraderRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.DlrCode).ValidDlrCode();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for deleting MstTrader
    /// </summary>
    public class DeleteMstTraderRequestValidator : AbstractValidator<DeleteMstTraderRequest>
    {
        public DeleteMstTraderRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.DlrCode).ValidDlrCode();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing MstTrader in workflow
    /// </summary>
    public class AuthorizeMstTraderRequestValidator : AbstractValidator<AuthorizeMstTraderRequest>
    {
        public AuthorizeMstTraderRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.DlrCode).ValidDlrCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);

        }
    }



    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetTraderWorkflowListRequestValidator : AbstractValidator<GetTraderWorkflowListRequest>
    {
        public GetTraderWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(100);

            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.XchgCode).ValidXchgCode()
                .When(x => !string.IsNullOrEmpty(x.XchgCode));

            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));

            RuleFor(x => x.SortDirection).ValidSorting()
                .When(x => !string.IsNullOrEmpty(x.SortDirection));
        }
    }
}