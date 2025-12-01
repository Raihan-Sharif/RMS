using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Exchange Request Validators (Consolidated)
/// Author:      Raihan
/// Purpose:     All validation rules for Exchange operations
/// Creation:    01/Dec/2025
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
    /// Validator for creating Exchange
    /// </summary>
    public class CreateExchangeRequestValidator : AbstractValidator<CreateExchangeRequest>
    {
        public CreateExchangeRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.BrokerCode).ValidBrokerCode();
            RuleFor(x => x.XchgPrefix).ValidXchgPrefix();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating Exchange
    /// </summary>
    public class UpdateExchangeRequestValidator : AbstractValidator<UpdateExchangeRequest>
    {
        public UpdateExchangeRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.BrokerCode).ValidBrokerCode();
            RuleFor(x => x.XchgPrefix).ValidXchgPrefix();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for deleting Exchange
    /// </summary>
    public class DeleteExchangeRequestValidator : AbstractValidator<DeleteExchangeRequest>
    {
        public DeleteExchangeRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.XchgPrefix).ValidXchgPrefix();
            RuleFor(x => x.BrokerCode).ValidBrokerCode();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing Exchange in workflow
    /// </summary>
    public class AuthorizeExchangeRequestValidator : AbstractValidator<AuthorizeExchangeRequest>
    {
        public AuthorizeExchangeRequestValidator()
        {
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.XchgPrefix).ValidXchgPrefix();
            RuleFor(x => x.BrokerCode).ValidBrokerCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);

        }
    }



    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetExchangeWorkflowListRequestValidator : AbstractValidator<GetExchangeWorkflowListRequest>
    {
        public GetExchangeWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(100);

            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.XchgCode).ValidXchgCode()
                .When(x => !string.IsNullOrEmpty(x.XchgCode));

            RuleFor(x => x.SearchText).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchText));

            RuleFor(x => x.SortDirection).ValidSorting()
                .When(x => !string.IsNullOrEmpty(x.SortDirection));
        }
    }
}
