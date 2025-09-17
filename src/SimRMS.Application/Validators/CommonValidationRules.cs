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

        #region Sorting Validations

        /// <summary>
        /// Validates Sorting direction to be either 'ASC' or 'DESC'
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidSorting<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => string.IsNullOrEmpty(x) || x.ToUpper() == "ASC" || x.ToUpper() == "DESC")
                .WithMessage("Sort direction must be 'ASC' or 'DESC'");
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

        #region Client Validations

        /// <summary>
        /// Validates Client Code format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidClientCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Client code is required")
                .MaximumLength(50).WithMessage("Client code cannot exceed 50 characters")
                .Matches("^[A-Za-z0-9]+$").WithMessage("Client code can only contain alphanumeric characters");
        }

        /// <summary>
        /// Validates Company Branch Code format and length (shared with MstCoBrch)
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidCoBrchCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Company branch code is required")
                .MaximumLength(6).WithMessage("Company branch code cannot exceed 6 characters")
                .Matches("^[A-Za-z0-9]+$").WithMessage("Company branch code can only contain alphanumeric characters");
        }

        /// <summary>
        /// Validates exposure amounts for >= 0 and proper decimal format
        /// </summary>
        public static IRuleBuilderOptions<T, decimal> ValidExposureAmount<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0).WithMessage("Exposure amount must be greater than or equal to 0")
                .PrecisionScale(20, 2, true).WithMessage("Exposure amount can have maximum 20 digits with 2 decimal places");
        }

        /// <summary>
        /// Validates nullable exposure amounts for >= 0 and proper decimal format
        /// </summary>
        public static IRuleBuilderOptions<T, decimal?> ValidExposureAmountNullable<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0).WithMessage("Exposure amount must be greater than or equal to 0")
                .PrecisionScale(20, 2, true).WithMessage("Exposure amount can have maximum 20 digits with 2 decimal places");
        }

        /// <summary>
        /// Validates top-up expiry dates must be in the future
        /// </summary>
        public static IRuleBuilderOptions<T, DateTime?> ValidTopUpExpiry<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder
                .Must(date => !date.HasValue || date.Value > DateTime.Now)
                .WithMessage("Top-up expiry date must be in the future");
        }

        #endregion

        #region User Validations
        /// <summary>
        /// Validates User ID format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidUsrID<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(25).WithMessage("User ID cannot exceed 25 characters")
                .Matches("^[A-Za-z0-9_]+$").WithMessage("User ID can only contain letters, numbers, and underscores");
        }

        /// <summary>
        /// Validates User Email format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidUsrEmail<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(510).WithMessage("User email cannot exceed 510 characters")
                .EmailAddress().WithMessage("Invalid email format");
        }
        /// <summary>
        /// Validates User Mobile format and length
        /// </summary>
        public static IRuleBuilderOptions<T, string?> ValidUsrMobile<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(30).WithMessage("User mobile cannot exceed 30 characters")
                .Matches("^[0-9+()-]+$").WithMessage("User mobile can only contain numbers and special characters like +, -, (, )");
        }
        #endregion
    }
}