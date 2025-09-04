using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Request Validators (Consolidated)
/// Author:      Asif Zaman
/// Purpose:     All validation rules for User operations
/// Creation:    03/Sep/2025
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

    #region local validation rules
    public static class UserValidationRules
    {
        public static IRuleBuilderOptions<T, string?> ValidUsrName<T>(this IRuleBuilder<T, string?> ruleBuilder, bool isRequired = false)
        {
            var rule = ruleBuilder.MaximumLength(510).WithMessage("User name cannot exceed 510 characters");
            if (isRequired)
            {
                return rule.NotEmpty().WithMessage("User name is required");
            }
            return rule;
        }

        /// <summary>
        /// Validates User Status format and length
        /// </summary>
        
    }
    #endregion


    /// <summary>
    /// Validator for creating User
    /// </summary>
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.UsrName).ValidUsrName(true);
            RuleFor(x => x.DlrCode).ValidDlrCode()
                .When(x => !string.IsNullOrEmpty(x.DlrCode));
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));
            RuleFor(x => x.UsrEmail).ValidUsrEmail()
                .When(x => !string.IsNullOrEmpty(x.UsrEmail));
            RuleFor(x => x.UsrMobile).ValidUsrMobile()
                .When(x => !string.IsNullOrEmpty(x.UsrMobile));
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating User
    /// </summary>
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.UsrName).ValidUsrName()
                .When(x => !string.IsNullOrEmpty(x.UsrName));
            RuleFor(x => x.DlrCode).ValidDlrCode()
                .When(x => !string.IsNullOrEmpty(x.DlrCode));
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));
            RuleFor(x => x.UsrEmail).ValidUsrEmail()
                .When(x => !string.IsNullOrEmpty(x.UsrEmail));
            RuleFor(x => x.UsrMobile).ValidUsrMobile()
                .When(x => !string.IsNullOrEmpty(x.UsrMobile));
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for deleting User
    /// </summary>
    public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserRequestValidator()
        {
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for getting user workflow list
    /// </summary>
    public class GetUserWorkflowListRequestValidator : AbstractValidator<GetUserWorkflowListRequest>
    {
        public GetUserWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.SearchText).ValidSearchTerm()
                .When(x => !string.IsNullOrEmpty(x.SearchText));
            //RuleFor(x => x.UsrStatus).ValidUsrStatus()
            //    .When(x => !string.IsNullOrEmpty(x.UsrStatus));
            RuleFor(x => x.DlrCode).ValidDlrCode()
                .When(x => !string.IsNullOrEmpty(x.DlrCode));
            RuleFor(x => x.CoCode).ValidCompanyCode(false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode()
                .When(x => !string.IsNullOrEmpty(x.CoBrchCode));
            //RuleFor(x => x.SortColumn).ValidSortColumn()
            //    .When(x => !string.IsNullOrEmpty(x.SortColumn));
            RuleFor(x => x.SortDirection).ValidSorting();
            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();
        }
    }

    /// <summary>
    /// Validator for authorizing User in workflow
    /// </summary>
    public class AuthorizeUserRequestValidator : AbstractValidator<AuthorizeUserRequest>
    {
        public AuthorizeUserRequestValidator()
        {
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);

        }
    }

}