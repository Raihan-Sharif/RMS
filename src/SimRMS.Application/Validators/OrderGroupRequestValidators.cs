using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       OrderGroup Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Order Group master-details workflow operations
/// Creation:    11/Sep/2025
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


        public static bool HaveAtLeastOnePermission(OrderGroupUserRequest user)
        {
            return user.ViewOrder == true || user.PlaceOrder == true || 
                   user.ViewClient == true || user.ModifyOrder == true;
        }
    }

    /// <summary>
    /// Validator for nested OrderGroupUserRequest
    /// </summary>
    public class OrderGroupUserRequestValidator : AbstractValidator<OrderGroupUserRequest>
    {
        public OrderGroupUserRequestValidator()
        {
            RuleFor(x => x.UsrID).ValidUsrID();
            
            // Business rule: At least one permission must be granted
            RuleFor(x => x)
                .Must(OrderGroupValidationRules.HaveAtLeastOnePermission)
                .WithMessage("At least one permission (ViewOrder, PlaceOrder, ViewClient, or ModifyOrder) must be granted for user {PropertyValue.UsrID}");
        }
    }

    #region Master Group Validators

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
            RuleFor(x => x.UsrId).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.UsrId));
            RuleFor(x => x.SortDirection).ValidSorting();
        }
    }

    /// <summary>
    /// Validator for getting master group workflow list
    /// </summary>
    public class GetMasterGroupWorkflowListRequestValidator : AbstractValidator<GetMasterGroupWorkflowListRequest>
    {
        public GetMasterGroupWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize(1000);
            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
            RuleFor(x => x.SortDirection).ValidSorting();
            RuleFor(x => x.IsAuth).Must(x => x >= 0 && x <= 2).WithMessage("IsAuth must be 0 (Unauthorized), 1 (Authorized), or 2 (All)");
            RuleFor(x => x.MakerId).GreaterThan(0).WithMessage("MakerId must be greater than 0");
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
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFrom)
                .WithMessage("DateTo must be after DateFrom, or leave DateTo null for no expiration")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Validate nested Users list
            RuleForEach(x => x.Users).SetValidator(new OrderGroupUserRequestValidator())
                .When(x => x.Users != null && x.Users.Any());
            
            // Business rule: Users list cannot have duplicate User IDs
            RuleFor(x => x.Users)
                .Must(HaveUniqueUserIds)
                .WithMessage("Users list cannot contain duplicate User IDs")
                .When(x => x.Users != null && x.Users.Any());
        }

        private static bool DateToAfterDateFrom(CreateOrderGroupRequest request)
        {
            if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
                return true;
            
            return request.DateTo.Value > request.DateFrom.Value;
        }

        private static bool HaveUniqueUserIds(List<OrderGroupUserRequest> users)
        {
            if (users == null || !users.Any()) return true;
            
            var userIds = users.Select(u => u.UsrID).Where(id => !string.IsNullOrEmpty(id));
            return userIds.Count() == userIds.Distinct().Count();
        }
    }

    /// <summary>
    /// Validator for updating OrderGroup master information
    /// </summary>
    public class UpdateOrderGroupMasterRequestValidator : AbstractValidator<UpdateOrderGroupMasterRequest>
    {
        public UpdateOrderGroupMasterRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            
            RuleFor(x => x.GroupDesc).ValidGroupDesc()
                .When(x => !string.IsNullOrEmpty(x.GroupDesc));
            
            RuleFor(x => x.GroupType).ValidGroupType()
                .When(x => !string.IsNullOrEmpty(x.GroupType));
            
            RuleFor(x => x.GroupValue).ValidGroupValue()
                .When(x => !string.IsNullOrEmpty(x.GroupValue));
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFrom)
                .WithMessage("DateTo must be after DateFrom, or leave DateTo null for no expiration")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool DateToAfterDateFrom(UpdateOrderGroupMasterRequest request)
        {
            if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
                return true;
            
            return request.DateTo.Value > request.DateFrom.Value;
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateOrderGroupMasterRequest request)
        {
            return !string.IsNullOrEmpty(request.GroupDesc) ||
                   !string.IsNullOrEmpty(request.GroupType) ||
                   !string.IsNullOrEmpty(request.GroupValue) ||
                   request.DateFrom.HasValue ||
                   request.DateTo.HasValue ||
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
    /// Validator for authorizing OrderGroup master
    /// </summary>
    public class AuthorizeOrderGroupMasterRequestValidator : AbstractValidator<AuthorizeOrderGroupMasterRequest>
    {
        public AuthorizeOrderGroupMasterRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    #endregion

    #region Detail/User Validators

    /// <summary>
    /// Validator for getting detail group workflow list
    /// </summary>
    public class GetDetailGroupWorkflowListRequestValidator : AbstractValidator<GetDetailGroupWorkflowListRequest>
    {
        public GetDetailGroupWorkflowListRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize(1000);
            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
            RuleFor(x => x.SortDirection).ValidSorting();
            RuleFor(x => x.IsAuth).Must(x => x >= 0 && x <= 2).WithMessage("IsAuth must be 0 (Unauthorized), 1 (Authorized), or 2 (All)");
            RuleFor(x => x.MakerId).GreaterThan(0).WithMessage("MakerId must be greater than 0");
        }
    }

    /// <summary>
    /// Validator for updating OrderGroup user
    /// </summary>
    public class UpdateOrderGroupUserRequestValidator : AbstractValidator<UpdateOrderGroupUserRequest>
    {
        public UpdateOrderGroupUserRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            
            // Business rule: At least one permission must be granted
            RuleFor(x => x)
                .Must(HaveAtLeastOnePermission)
                .WithMessage("At least one permission (ViewOrder, PlaceOrder, ViewClient, or ModifyOrder) must be granted");
        }

        private static bool HaveAtLeastOnePermission(UpdateOrderGroupUserRequest request)
        {
            return request.ViewOrder == true || request.PlaceOrder == true || 
                   request.ViewClient == true || request.ModifyOrder == true;
        }
    }

    /// <summary>
    /// Validator for removing user from OrderGroup
    /// </summary>
    public class RemoveUserFromGroupRequestValidator : AbstractValidator<RemoveUserFromGroupRequest>
    {
        public RemoveUserFromGroupRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing OrderGroup detail/user
    /// </summary>
    public class AuthorizeOrderGroupDetailRequestValidator : AbstractValidator<AuthorizeOrderGroupDetailRequest>
    {
        public AuthorizeOrderGroupDetailRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.ActionType).ValidActionTypeUpdate();
            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    #endregion

    #region Master Group Specific Validators

    /// <summary>
    /// Validator for creating OrderGroup master only
    /// </summary>
    public class CreateOrderGroupMasterRequestValidator : AbstractValidator<CreateOrderGroupMasterRequest>
    {
        public CreateOrderGroupMasterRequestValidator()
        {
            RuleFor(x => x.GroupDesc).ValidGroupDesc();
            RuleFor(x => x.GroupType).ValidGroupType()
                .When(x => !string.IsNullOrEmpty(x.GroupType));
            RuleFor(x => x.GroupValue).ValidGroupValue()
                .When(x => !string.IsNullOrEmpty(x.GroupValue));
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFromMaster)
                .WithMessage("DateTo must be after DateFrom, or leave DateTo null for no expiration")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }

        private static bool DateToAfterDateFromMaster(CreateOrderGroupMasterRequest request)
        {
            if (!request.DateFrom.HasValue || !request.DateTo.HasValue)
                return true;
            
            return request.DateTo.Value > request.DateFrom.Value;
        }
    }

    /// <summary>
    /// Validator for deleting OrderGroup master
    /// </summary>
    public class DeleteOrderGroupMasterRequestValidator : AbstractValidator<DeleteOrderGroupMasterRequest>
    {
        public DeleteOrderGroupMasterRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    #endregion

    #region Detail/User Specific Validators

    /// <summary>
    /// Validator for adding user to OrderGroup
    /// </summary>
    public class AddUserToOrderGroupRequestValidator : AbstractValidator<AddUserToOrderGroupRequest>
    {
        public AddUserToOrderGroupRequestValidator()
        {
            RuleFor(x => x.GroupCode).ValidGroupCode();
            RuleFor(x => x.UsrID).ValidUsrID();
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
            
            // Business rule: At least one permission must be granted
            RuleFor(x => x)
                .Must(HaveAtLeastOnePermissionAdd)
                .WithMessage("At least one permission (ViewOrder, PlaceOrder, ViewClient, or ModifyOrder) must be granted");
        }

        private static bool HaveAtLeastOnePermissionAdd(AddUserToOrderGroupRequest request)
        {
            return request.ViewOrder == true || request.PlaceOrder == true || 
                   request.ViewClient == true || request.ModifyOrder == true;
        }
    }

    #endregion

    #region Legacy Support Validators (Required for Service)

    /// <summary>
    /// Validator for updating OrderGroup (unified CRUD operation)
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
            
            // Business rule: DateTo must be after DateFrom if both are provided
            RuleFor(x => x)
                .Must(DateToAfterDateFrom)
                .WithMessage("DateTo must be after DateFrom, or leave DateTo null for no expiration")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            // Validate nested Users list
            RuleForEach(x => x.Users).SetValidator(new OrderGroupUserRequestValidator())
                .When(x => x.Users != null && x.Users.Any());
            
            // Business rule: Users list cannot have duplicate User IDs
            RuleFor(x => x.Users)
                .Must(HaveUniqueUserIds)
                .WithMessage("Users list cannot contain duplicate User IDs")
                .When(x => x.Users != null && x.Users.Any());

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

        private static bool HaveUniqueUserIds(List<OrderGroupUserRequest> users)
        {
            if (users == null || !users.Any()) return true;
            
            var userIds = users.Select(u => u.UsrID).Where(id => !string.IsNullOrEmpty(id));
            return userIds.Count() == userIds.Distinct().Count();
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateOrderGroupRequest request)
        {
            return !string.IsNullOrEmpty(request.GroupDesc) ||
                   !string.IsNullOrEmpty(request.GroupType) ||
                   !string.IsNullOrEmpty(request.GroupValue) ||
                   request.DateFrom.HasValue ||
                   request.DateTo.HasValue ||
                   (request.Users != null && request.Users.Any()) ||
                   !string.IsNullOrEmpty(request.Remarks);
        }
    }

    /// <summary>
    /// Validator for authorizing OrderGroup in workflow (legacy)
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

    #endregion

}