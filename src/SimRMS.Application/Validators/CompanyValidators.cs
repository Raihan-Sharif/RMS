using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Request Validators (Consolidated)
/// Author:      Md. Raihan Sharif
/// Purpose:     All validation rules for Company operations (Read, Update, Authorization)
/// Creation:    28/Aug/2025
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
    /// Company specific validation rules
    /// </summary>
    public static class CompanyValidationRules
    {
        public static IRuleBuilderOptions<T, string?> ValidCoDesc<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(50).WithMessage("Company description cannot exceed 50 characters");
        }
    }

    /// <summary>
    /// Validator for updating Company
    /// </summary>
    public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyRequestValidator()
        {
            RuleFor(x => x.CoCode).ValidCompanyCode();

            RuleFor(x => x.CoDesc).ValidCoDesc()
                .When(x => !string.IsNullOrEmpty(x.CoDesc));

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing Company
    /// </summary>
    public class AuthorizeCompanyRequestValidator : AbstractValidator<AuthorizeCompanyRequest>
    {
        public AuthorizeCompanyRequestValidator()
        {
            RuleFor(x => x.CoCode).ValidCompanyCode();
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    /// <summary>
    /// Validator for getting Company workflow list
    /// </summary>
    public class GetCompanyWorkflowListRequestValidator : AbstractValidator<GetCompanyWorkflowListRequest>
    {
        public GetCompanyWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.SearchTerm).ValidSearchTerm()
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
            RuleFor(x => x.CoCode).ValidCompanyCode(isRequired: false)
                .When(x => !string.IsNullOrEmpty(x.CoCode));

        }
    }
}