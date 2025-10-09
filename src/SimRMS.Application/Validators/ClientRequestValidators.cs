using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Client operations
/// Creation:    16/Sep/2025
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
    /// Client specific validation rules
    /// </summary>
    public static class ClientValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidGCIF<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("GCIF is required")
                .MaximumLength(20).WithMessage("GCIF cannot exceed 20 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("GCIF can only contain uppercase letters and numbers");
        }

        public static IRuleBuilderOptions<T, string?> ValidGCIFNullable<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(20).WithMessage("GCIF cannot exceed 20 characters")
                .Matches("^[A-Z0-9]*$").WithMessage("GCIF can only contain uppercase letters and numbers");
        }

        public static IRuleBuilderOptions<T, string> ValidClientName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Client name is required")
                .MaximumLength(510).WithMessage("Client name cannot exceed 510 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidClientName<T>(this IRuleBuilder<T, string?> ruleBuilder, bool isRequired = false)
        {
            var rule = ruleBuilder.MaximumLength(510).WithMessage("Client name cannot exceed 510 characters");

            if (isRequired)
            {
                return rule.NotEmpty().WithMessage("Client name is required");
            }

            return rule;
        }

        public static IRuleBuilderOptions<T, string?> ValidNICNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(30).WithMessage("NIC number cannot exceed 30 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidPhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidMobile<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(50).WithMessage("Mobile number cannot exceed 50 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidGender<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(10).WithMessage("Gender cannot exceed 10 characters")
                .Must(x => string.IsNullOrEmpty(x) || x.ToUpper() == "M" || x.ToUpper() == "F")
                .WithMessage("Gender must be F or M");
        }

        public static IRuleBuilderOptions<T, string?> ValidOffice<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(100).WithMessage("Office cannot exceed 100 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidFax<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(50).WithMessage("Fax number cannot exceed 50 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidClientEmail<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(510).WithMessage("Email cannot exceed 510 characters")
                .EmailAddress().WithMessage("Invalid email format");
        }

        public static IRuleBuilderOptions<T, string?> ValidCountryCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(10).WithMessage("Country code cannot exceed 10 characters")
                .Must(x => x.ToUpper() == "BD")
                .WithMessage("Country must be BD"); 
        }

        public static IRuleBuilderOptions<T, string> ValidClientCoBrchCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Company branch code is required")
                .MaximumLength(6).WithMessage("Company branch code cannot exceed 6 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Company branch code can only contain uppercase letters and numbers");
        }

        public static IRuleBuilderOptions<T, string?> ValidClientStatus<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(1).WithMessage("Client status must be a single character")
                .Must(x => string.IsNullOrEmpty(x) || x == "1" || x == "2" || x == "3")
                .WithMessage("Client status must be 1 (Active), 2 (Cancel), or 3 (Suspended)");
        }

        public static IRuleBuilderOptions<T, string?> ValidTradingStatus<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(1).WithMessage("Trading status must be a single character")
                .Must(x => string.IsNullOrEmpty(x) || x == "S" || x == "B" || x == "N" || x == "Y")
                .WithMessage("Client status must be S, B, N or Y");
        }

        public static IRuleBuilderOptions<T, string?> ValidAccountType<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(5).WithMessage("Account type cannot exceed 5 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidCDSNo<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(20).WithMessage("CDS number cannot exceed 20 characters");
        }

        //public static IRuleBuilderOptions<T, string?> ValidClientDealerCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
        //{
        //    return ruleBuilder
        //        .MaximumLength(25).WithMessage("Dealer code cannot exceed 25 characters")
        //        .Matches("^[A-Z0-9]*$").WithMessage("Dealer code can only contain uppercase letters and numbers");
        //}

        public static IRuleBuilderOptions<T, decimal> ValidCommission<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0).WithMessage("Commission must be greater than or equal to 0")
                .PrecisionScale(20, 6, true).WithMessage("Commission can have maximum 20 digits with 6 decimal places");
        }

        public static IRuleBuilderOptions<T, decimal?> ValidCommissionNullable<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0).WithMessage("Commission must be greater than or equal to 0")
                .PrecisionScale(20, 6, true).WithMessage("Commission can have maximum 20 digits with 6 decimal places");
        }
    }

    /// <summary>
    /// Validator for creating Client
    /// </summary>
    public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
    {
        public CreateClientRequestValidator()
        {
            RuleFor(x => x.ClntCode).ValidClientCode();
            RuleFor(x => x.ClntName).ValidClientName();
            RuleFor(x => x.CoBrchCode).ValidClientCoBrchCode();

            RuleFor(x => x.ClntNICNo).ValidNICNo()
                .When(x => !string.IsNullOrEmpty(x.ClntNICNo));

            RuleFor(x => x.ClntAddr).ValidAddress()
                .When(x => !string.IsNullOrEmpty(x.ClntAddr));

            RuleFor(x => x.ClntPhone).ValidPhone()
                .When(x => !string.IsNullOrEmpty(x.ClntPhone));

            RuleFor(x => x.ClntMobile).ValidMobile()
                .When(x => !string.IsNullOrEmpty(x.ClntMobile));

            RuleFor(x => x.Gender).ValidGender()
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.ClntOffice).ValidOffice()
                .When(x => !string.IsNullOrEmpty(x.ClntOffice));

            RuleFor(x => x.ClntFax).ValidFax()
                .When(x => !string.IsNullOrEmpty(x.ClntFax));

            RuleFor(x => x.ClntEmail).ValidClientEmail()
                .When(x => !string.IsNullOrEmpty(x.ClntEmail));

            RuleFor(x => x.CountryCode).ValidCountryCode()
                .When(x => !string.IsNullOrEmpty(x.CountryCode));

            RuleFor(x => x.ClntStat).ValidClientStatus()
                .When(x => !string.IsNullOrEmpty(x.ClntStat));

            RuleFor(x => x.ClntTrdgStat).ValidTradingStatus()
                .When(x => !string.IsNullOrEmpty(x.ClntTrdgStat));

            RuleFor(x => x.ClntAcctType).ValidAccountType()
                .When(x => !string.IsNullOrEmpty(x.ClntAcctType));

            RuleFor(x => x.ClntCDSNo).ValidCDSNo()
                .When(x => !string.IsNullOrEmpty(x.ClntCDSNo));

            RuleFor(x => x.ClntDlrCode).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.ClntDlrCode));

            RuleFor(x => x.ClntReassignDlrCode).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.ClntReassignDlrCode));

            RuleFor(x => x.ClientCommission).ValidCommission();

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating Client
    /// </summary>
    public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
    {
        public UpdateClientRequestValidator()
        {
            RuleFor(x => x.GCIF).ValidGCIF();

            RuleFor(x => x.ClntName).ValidClientName(false)
                .When(x => !string.IsNullOrEmpty(x.ClntName));

            RuleFor(x => x.ClntNICNo).ValidNICNo()
                .When(x => !string.IsNullOrEmpty(x.ClntNICNo));

            RuleFor(x => x.ClntAddr).ValidAddress()
                .When(x => !string.IsNullOrEmpty(x.ClntAddr));

            RuleFor(x => x.ClntPhone).ValidPhone()
                .When(x => !string.IsNullOrEmpty(x.ClntPhone));

            RuleFor(x => x.ClntMobile).ValidMobile()
                .When(x => !string.IsNullOrEmpty(x.ClntMobile));

            RuleFor(x => x.Gender).ValidGender()
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.ClntOffice).ValidOffice()
                .When(x => !string.IsNullOrEmpty(x.ClntOffice));

            RuleFor(x => x.ClntFax).ValidFax()
                .When(x => !string.IsNullOrEmpty(x.ClntFax));

            RuleFor(x => x.ClntEmail).ValidClientEmail()
                .When(x => !string.IsNullOrEmpty(x.ClntEmail));

            RuleFor(x => x.CountryCode).ValidCountryCode()
                .When(x => !string.IsNullOrEmpty(x.CountryCode));

            RuleFor(x => x.ClntStat).ValidClientStatus()
                .When(x => !string.IsNullOrEmpty(x.ClntStat));

            RuleFor(x => x.ClntTrdgStat).ValidTradingStatus()
                .When(x => !string.IsNullOrEmpty(x.ClntTrdgStat));

            RuleFor(x => x.ClntAcctType).ValidAccountType()
                .When(x => !string.IsNullOrEmpty(x.ClntAcctType));

            RuleFor(x => x.ClntCDSNo).ValidCDSNo()
                .When(x => !string.IsNullOrEmpty(x.ClntCDSNo));

            RuleFor(x => x.ClntDlrCode).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.ClntDlrCode));

            RuleFor(x => x.ClntReassignDlrCode).ValidUsrID()
                .When(x => !string.IsNullOrEmpty(x.ClntReassignDlrCode));

            RuleFor(x => x.ClientCommission).ValidCommissionNullable()
                .When(x => x.ClientCommission.HasValue);

            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateClientRequest request)
        {
            return !string.IsNullOrEmpty(request.ClntName) ||
                   !string.IsNullOrEmpty(request.ClntNICNo) ||
                   !string.IsNullOrEmpty(request.ClntAddr) ||
                   !string.IsNullOrEmpty(request.ClntPhone) ||
                   !string.IsNullOrEmpty(request.ClntMobile) ||
                   !string.IsNullOrEmpty(request.Gender) ||
                   request.Nationality.HasValue ||
                   !string.IsNullOrEmpty(request.ClntOffice) ||
                   !string.IsNullOrEmpty(request.ClntFax) ||
                   !string.IsNullOrEmpty(request.ClntEmail) ||
                   !string.IsNullOrEmpty(request.CountryCode) ||
                   !string.IsNullOrEmpty(request.ClntStat) ||
                   !string.IsNullOrEmpty(request.ClntTrdgStat) ||
                   !string.IsNullOrEmpty(request.ClntAcctType) ||
                   !string.IsNullOrEmpty(request.ClntCDSNo) ||
                   !string.IsNullOrEmpty(request.ClntDlrCode) ||
                   request.ClntAllowAssociate.HasValue ||
                   request.ClntDlrReassign.HasValue ||
                   !string.IsNullOrEmpty(request.ClntReassignDlrCode) ||
                   request.ClientCommission.HasValue ||
                   request.AllowSME.HasValue ||
                   !string.IsNullOrEmpty(request.Remarks);
        }
    }

    /// <summary>
    /// Validator for deleting Client
    /// </summary>
    public class DeleteClientRequestValidator : AbstractValidator<DeleteClientRequest>
    {
        public DeleteClientRequestValidator()
        {
            RuleFor(x => x.GCIF).ValidGCIF();
            RuleFor(x => x.ClntCode).ValidClientCode();
            RuleFor(x => x.CoBrchCode).ValidClientCoBrchCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing Client in workflow
    /// </summary>
    public class AuthorizeClientRequestValidator : AbstractValidator<AuthorizeClientRequest>
    {
        public AuthorizeClientRequestValidator()
        {
            RuleFor(x => x.GCIF).ValidGCIF();
            RuleFor(x => x.ClntCode).ValidClientCode();

            RuleFor(x => x.ActionType).ValidActionTypeUpdate();

            RuleFor(x => x.IsAuth).ValidIsApproveDeny();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
            RuleFor(x => x.Remarks).NotEmpty().WithMessage("Remarks are required for denial").When(x => x.IsAuth == (byte)AuthTypeEnum.Deny);
        }
    }

    /// <summary>
    /// Validator for getting workflow list with IsAuth parameter
    /// </summary>
    public class GetClientWorkflowListRequestValidator : AbstractValidator<GetClientWorkflowListRequest>
    {
        public GetClientWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();

            RuleFor(x => x.PageSize).ValidPageSize(100);

            RuleFor(x => x.IsAuth).ValidIsUnAuthDeny();

            RuleFor(x => x.GCIF).ValidGCIFNullable()
                .When(x => !string.IsNullOrEmpty(x.GCIF));

            RuleFor(x => x.SearchTerm).ValidSearchTerm(100)
                .When(x => !string.IsNullOrEmpty(x.SearchTerm));
        }
    }
}