using FluentValidation;
using SimRMS.Application.Models.Requests;

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

            RuleFor(x => x.Action)
                .Equal(2).WithMessage("Action must be 2 for authorization");

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();

            RuleFor(x => x.ActionType)
                .Equal((byte)2).WithMessage("Action type must be 2 for authorization");

            RuleFor(x => x.IPAddress).ValidIPAddress()
                .When(x => !string.IsNullOrEmpty(x.IPAddress));
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

            RuleFor(x => x.SortDirection)
                .Must(x => string.IsNullOrEmpty(x) || x.ToUpper() == "ASC" || x.ToUpper() == "DESC")
                .WithMessage("Sort direction must be 'ASC' or 'DESC'")
                .When(x => !string.IsNullOrEmpty(x.SortDirection));
        }
    }
}