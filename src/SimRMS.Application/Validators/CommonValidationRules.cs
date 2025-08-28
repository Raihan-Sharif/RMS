using FluentValidation;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Common Validation Rules
/// Author:      Md. Raihan Sharif
/// Purpose:     Reusable validation rules for common fields across the application
/// Creation:    27/Aug/2025
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
    /// Common validation rules that can be reused across different request models
    /// </summary>
    public static class CommonValidationRules
    {
        #region Auth Type Validations

        /// <summary>
        /// Validates IsAuth field for UnAuthorize or Deny values (int version)
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidIsUnAuthDeny<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x == (int)AuthTypeEnum.UnAuthorize || x == (int)AuthTypeEnum.Deny)
                .WithMessage($"IsAuth must be {(byte)AuthTypeEnum.UnAuthorize} ({AuthTypeEnum.UnAuthorize}) or {(byte)AuthTypeEnum.Deny} ({AuthTypeEnum.Deny})");
        }

        /// <summary>
        /// Validates IsAuth field for UnAuthorize or Deny values (byte version)
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidIsUnAuthDeny<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x == (byte)AuthTypeEnum.UnAuthorize || x == (byte)AuthTypeEnum.Deny)
                .WithMessage($"IsAuth must be {(byte)AuthTypeEnum.UnAuthorize} ({AuthTypeEnum.UnAuthorize}) or {(byte)AuthTypeEnum.Deny} ({AuthTypeEnum.Deny})");
        }

        /// <summary>
        /// Validates IsAuth field for Approve or Deny values (int version)
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidIsApproveDeny<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x == (int)AuthTypeEnum.Approve || x == (int)AuthTypeEnum.Deny)
                .WithMessage($"IsAuth must be {(byte)AuthTypeEnum.Approve} ({AuthTypeEnum.Approve}) or {(byte)AuthTypeEnum.Deny} ({AuthTypeEnum.Deny})");
        }

        /// <summary>
        /// Validates IsAuth field for Approve or Deny values (byte version)
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidIsApproveDeny<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => x == (byte)AuthTypeEnum.Approve || x == (byte)AuthTypeEnum.Deny)
                .WithMessage($"IsAuth must be {(byte)AuthTypeEnum.Approve} ({AuthTypeEnum.Approve}) or {(byte)AuthTypeEnum.Deny} ({AuthTypeEnum.Deny})");
        }

        /// <summary>
        /// Validates IsAuth field for all valid AuthType values (byte version)
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidAuthType<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => Enum.IsDefined(typeof(AuthTypeEnum), x))
                .WithMessage($"IsAuth must be a valid value: {(byte)AuthTypeEnum.UnAuthorize} ({AuthTypeEnum.UnAuthorize}), {(byte)AuthTypeEnum.Approve} ({AuthTypeEnum.Approve}), or {(byte)AuthTypeEnum.Deny} ({AuthTypeEnum.Deny})");
        }

        #endregion

        #region Action Type Validations

        /// <summary>
        /// Validates ActionType field for valid ActionType enum values
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidActionType<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => Enum.IsDefined(typeof(ActionTypeEnum), x))
                .WithMessage($"ActionType must be a valid value: {(byte)ActionTypeEnum.INSERT} ({ActionTypeEnum.INSERT}), {(byte)ActionTypeEnum.UPDATE} ({ActionTypeEnum.UPDATE}), or {(byte)ActionTypeEnum.DELETE} ({ActionTypeEnum.DELETE})");
        }

        /// <summary>
        /// Validates ActionType field for specific UPDATE operation
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidActionTypeUpdate<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Equal((byte)ActionTypeEnum.UPDATE)
                .WithMessage($"ActionType must be {(byte)ActionTypeEnum.UPDATE} ({ActionTypeEnum.UPDATE}) for this operation");
        }

        #endregion

        #region Auth Level Validations

        /// <summary>
        /// Validates AuthLevel field for valid AuthLevel enum values
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidAuthLevel<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => Enum.IsDefined(typeof(AuthLevelEnum), x))
                .WithMessage("AuthLevel must be a valid authorization level");
        }

        #endregion

        #region Delete Status Validations

        /// <summary>
        /// Validates IsDel field for valid DeleteStatus enum values
        /// </summary>
        public static IRuleBuilderOptions<T, byte> ValidDeleteStatus<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => Enum.IsDefined(typeof(DeleteStatusEnum), x))
                .WithMessage($"IsDel must be {(byte)DeleteStatusEnum.Active} ({DeleteStatusEnum.Active}) or {(byte)DeleteStatusEnum.Deleted} ({DeleteStatusEnum.Deleted})");
        }

        #endregion

        #region Common Field Validations

        /// <summary>
        /// Validates Remarks field with standard length limit
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidRemarks<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(200).WithMessage("Remarks cannot exceed 200 characters");
        }

        /// <summary>
        /// Validates IP Address field with standard format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidIPAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(39).WithMessage("IP Address cannot exceed 39 characters");
        }

        /// <summary>
        /// Validates MakerId field for positive values
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidMakerId<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("MakerId must be greater than 0");
        }

        /// <summary>
        /// Validates AuthId field for positive values when provided
        /// </summary>
        public static IRuleBuilderOptions<T, int?> ValidAuthId<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("AuthId must be greater than 0");
        }

        #endregion

        #region Pagination Validations

        /// <summary>
        /// Validates PageNumber for pagination
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidPageNumber<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("Page number must be greater than 0");
        }

        /// <summary>
        /// Validates PageSize for pagination with configurable maximum
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> ruleBuilder, int maxSize = 100)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(maxSize).WithMessage($"Page size cannot exceed {maxSize}");
        }

        /// <summary>
        /// Validates SearchTerm with standard length limit
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidSearchTerm<T>(this IRuleBuilder<T, string?> ruleBuilder, int maxLength = 100)
        {
            return ruleBuilder
                .MaximumLength(maxLength).WithMessage($"Search term cannot exceed {maxLength} characters");
        }

        #endregion

        #region Code Validations

        /// <summary>
        /// Validates Company Code format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidCompanyCode<T>(this IRuleBuilder<T, string> ruleBuilder, bool isRequired = true)
        {
            var rule = ruleBuilder
                .MaximumLength(5).WithMessage("Company code cannot exceed 5 characters")
                .Matches("^[A-Z0-9]*$").WithMessage("Company code can only contain uppercase letters and numbers");

            if (isRequired)
            {
                rule = rule.NotEmpty().WithMessage("Company code is required");
            }

            return rule;
        }

        /// <summary>
        /// Validates Exchange Code format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidXchgCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Exchange code is required")
                .MaximumLength(10).WithMessage("Exchange code cannot exceed 10 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Exchange code can only contain uppercase letters and numbers");
        }

        /// <summary>
        /// Validates Dealer Code format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidDlrCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Dealer code is required")
                .MaximumLength(15).WithMessage("Dealer code cannot exceed 15 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Dealer code can only contain uppercase letters and numbers");
        }

        #endregion
    }
}