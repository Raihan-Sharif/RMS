using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       OrderGroup Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Order Group operations
/// Creation:    08/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Validators
{
    /// <summary>
    /// OrderGroup specific validation rules
    /// </summary>
    public static class OrderGroupValidationRules
    {
        public static IRuleBuilderOptions<T, int> ValidGroupCode<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("Group Code must be greater than 0");
        }

        public static IRuleBuilderOptions<T, string> ValidGroupDesc<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Group Description is required")
                .MaximumLength(100).WithMessage("Group Description cannot exceed 100 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidGroupType<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(50).WithMessage("Group Type cannot exceed 50 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidGroupValue<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(100).WithMessage("Group Value cannot exceed 100 characters");
        }

        public static IRuleBuilderOptions<T, DateTime?> ValidDateRange<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Date must be today or in the future");
        }
    }

    /// <summary>
    /// Validator for getting OrderGroup by code
    /// </summary>
    public class GetOrderGroupByCodeRequestValidator : AbstractValidator<GetOrderGroupByCodeRequest>
    {
        public GetOrderGroupByCodeRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
        }
    }

    /// <summary>
    /// Validator for getting OrderGroup list
    /// </summary>
    public class GetOrderGroupListRequestValidator : AbstractValidator<GetOrderGroupListRequest>
    {
        public GetOrderGroupListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize(1000);
            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));
            RuleFor(x => x.SortDirection).ValidSorting();
        }
    }

    /// <summary>
    /// Validator for creating OrderGroup
    /// </summary>
    public class CreateOrderGroupRequestValidator : AbstractValidator<CreateOrderGroupRequest>
    {
        public CreateOrderGroupRequestValidator()
        {
            RuleFor(x => x.GroupDesc).ValidGroupDesc();
            RuleFor(x => x.GroupType).ValidGroupType()
                .When(x => !string.IsNullOrEmpty(x.GroupType));
            RuleFor(x => x.GroupValue).ValidGroupValue()
                .When(x => !string.IsNullOrEmpty(x.GroupValue));
            
            RuleFor(x => x.DateFrom).ValidDateRange()
                .When(x => x.DateFrom.HasValue);
            
            RuleFor(x => x.DateTo).ValidDateRange()
                .When(x => x.DateTo.HasValue);
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFrom)
                .WithMessage("Date To must be after Date From")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            
            // Order Group Detail validations
            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));
            
            // Business rule: At least one permission must be granted
            RuleFor(x => x)
                .Must(HaveAtLeastOnePermission)
                .WithMessage("At least one permission (ViewOrder, PlaceOrder, ViewClient, or ModifyOrder) must be granted");
        }

        private static bool DateToAfterDateFrom(CreateOrderGroupRequest request)
        {
            if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
                return true;
            
            return request.DateTo.Value > request.DateFrom.Value;
        }

        private static bool HaveAtLeastOnePermission(CreateOrderGroupRequest request)
        {
            return request.ViewOrder || request.PlaceOrder || request.ViewClient || request.ModifyOrder;
        }
    }

    /// <summary>
    /// Validator for updating OrderGroup
    /// </summary>
    public class UpdateOrderGroupRequestValidator : AbstractValidator<UpdateOrderGroupRequest>
    {
        public UpdateOrderGroupRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            
            RuleFor(x => x.GroupDesc).ValidGroupDesc()
                .When(x => !string.IsNullOrEmpty(x.GroupDesc));
            
            RuleFor(x => x.GroupType).ValidGroupType()
                .When(x => !string.IsNullOrEmpty(x.GroupType));
            
            RuleFor(x => x.GroupValue).ValidGroupValue()
                .When(x => !string.IsNullOrEmpty(x.GroupValue));
            
            RuleFor(x => x.DateFrom).ValidDateRange()
                .When(x => x.DateFrom.HasValue);
            
            RuleFor(x => x.DateTo).ValidDateRange()
                .When(x => x.DateTo.HasValue);
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFrom)
                .WithMessage("Date To must be after Date From")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            
            RuleFor(x => x.UsrID).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrID));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool DateToAfterDateFrom(UpdateOrderGroupRequest request)
        {
            if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
                return true;
            
            return request.DateTo.Value > request.DateFrom.Value;
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateOrderGroupRequest request)
        {
            return !string.IsNullOrEmpty(request.GroupDesc) ||
                   !string.IsNullOrEmpty(request.GroupType) ||
                   !string.IsNullOrEmpty(request.GroupValue) ||
                   request.DateFrom.HasValue ||
                   request.DateTo.HasValue ||
                   !string.IsNullOrEmpty(request.UsrID) ||
                   request.ViewOrder.HasValue ||
                   request.PlaceOrder.HasValue ||
                   request.ViewClient.HasValue ||
                   request.ModifyOrder.HasValue ||
                   !string.IsNullOrEmpty(request.Remarks);
        }
    }

    /// <summary>
    /// Validator for deleting OrderGroup
    /// </summary>
    public class DeleteOrderGroupRequestValidator : AbstractValidator<DeleteOrderGroupRequest>
    {
        public DeleteOrderGroupRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing OrderGroup in workflow
    /// </summary>
    public class AuthorizeOrderGroupRequestValidator : AbstractValidator<AuthorizeOrderGroupRequest>
    {
        public AuthorizeOrderGroupRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetOrderGroupWorkflowListRequestValidator : AbstractValidator<GetOrderGroupWorkflowListRequest>
    {
        public GetOrderGroupWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            
            RuleFor(x => x.PageSize).ValidPageSize(100);
            
            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();
            
            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}