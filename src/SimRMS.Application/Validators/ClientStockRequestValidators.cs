using FluentValidation;
using SimRMS.Application.Models.Requests;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Stock Request Validators (Consolidated)
/// Author:      Raihan Sharif
/// Purpose:     All validation rules for Client Stock operations
/// Creation:    29/Sep/2025
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
    /// Common validation rules for Client Stock operations
    /// </summary>
    public static class ClientStockValidationRules
    {
        /// <summary>
        /// Validates Open Free Balance (must be positive for create operations)
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidOpenFreeBalance<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(0).WithMessage("Open Free Balance must be greater than 0");
        }

        /// <summary>
        /// Validates Pending Free Balance (can be negative for adjustments)
        /// </summary>
        public static IRuleBuilderOptions<T, int> ValidPendingFreeBalance<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .NotEqual(0).WithMessage("Pending Free Balance cannot be zero - provide adjustment amount");
        }
    }

    /// <summary>
    /// Validator for getting client stock list with pagination and filtering
    /// </summary>
    public class GetClientStockListRequestValidator : AbstractValidator<GetClientStockListRequest>
    {
        public GetClientStockListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.SearchText).ValidSearchTerm(100).When(x => !string.IsNullOrEmpty(x.SearchText));
            RuleFor(x => x.SortDirection).ValidSorting().When(x => !string.IsNullOrEmpty(x.SortDirection));

            // Optional filters
            RuleFor(x => x.BranchCode).ValidCoBrchCode().When(x => !string.IsNullOrEmpty(x.BranchCode));
            RuleFor(x => x.ClientCode).ValidClientCode().When(x => !string.IsNullOrEmpty(x.ClientCode));
            RuleFor(x => x.StockCode).ValidStkCode().When(x => !string.IsNullOrEmpty(x.StockCode));
            RuleFor(x => x.XchgCode).ValidXchgCode().When(x => !string.IsNullOrEmpty(x.XchgCode));
        }
    }

    /// <summary>
    /// Validator for getting client stock by key (BranchCode, ClientCode, StockCode)
    /// </summary>
    public class GetClientStockByKeyRequestValidator : AbstractValidator<GetClientStockByKeyRequest>
    {
        public GetClientStockByKeyRequestValidator()
        {
            RuleFor(x => x.BranchCode).ValidCoBrchCode();
            RuleFor(x => x.ClientCode).ValidClientCode();
            RuleFor(x => x.StockCode).ValidStkCode();
        }
    }

    /// <summary>
    /// Validator for creating new client stock record
    /// </summary>
    public class CreateClientStockRequestValidator : AbstractValidator<CreateClientStockRequest>
    {
        public CreateClientStockRequestValidator()
        {
            RuleFor(x => x.BranchCode).ValidCoBrchCode();
            RuleFor(x => x.ClientCode).ValidClientCode();
            RuleFor(x => x.StockCode).ValidStkCode();
            RuleFor(x => x.XchgCode).ValidXchgCode();
            RuleFor(x => x.OpenFreeBalance).ValidOpenFreeBalance();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating existing client stock record
    /// </summary>
    public class UpdateClientStockRequestValidator : AbstractValidator<UpdateClientStockRequest>
    {
        public UpdateClientStockRequestValidator()
        {
            RuleFor(x => x.BranchCode).ValidCoBrchCode();
            RuleFor(x => x.ClientCode).ValidClientCode();
            RuleFor(x => x.StockCode).ValidStkCode();
            RuleFor(x => x.PendingFreeBalance).ValidPendingFreeBalance();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for deleting client stock record
    /// </summary>
    public class DeleteClientStockRequestValidator : AbstractValidator<DeleteClientStockRequest>
    {
        public DeleteClientStockRequestValidator()
        {
            RuleFor(x => x.BranchCode).ValidCoBrchCode();
            RuleFor(x => x.ClientCode).ValidClientCode();
            RuleFor(x => x.StockCode).ValidStkCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for getting client stock workflow list (unauthorized/pending records)
    /// </summary>
    public class GetClientStockWorkflowListRequestValidator : AbstractValidator<GetClientStockWorkflowListRequest>
    {
        public GetClientStockWorkflowListRequestValidator()
        {
            RuleFor(x => x.PageNumber).ValidPageNumber();
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.SearchText).ValidSearchTerm(100).When(x => !string.IsNullOrEmpty(x.SearchText));
            RuleFor(x => x.SortDirection).ValidSorting().When(x => !string.IsNullOrEmpty(x.SortDirection));
            RuleFor(x => x.IsAuth)
                .Must(x => x == AuthTypeEnum.UnAuthorize || x == AuthTypeEnum.Deny)
                .WithMessage($"IsAuth must be {AuthTypeEnum.UnAuthorize} or {AuthTypeEnum.Deny}");

            // Optional filters
            RuleFor(x => x.BranchCode).ValidCoBrchCode().When(x => !string.IsNullOrEmpty(x.BranchCode));
            RuleFor(x => x.ClientCode).ValidClientCode().When(x => !string.IsNullOrEmpty(x.ClientCode));
            RuleFor(x => x.StockCode).ValidStkCode().When(x => !string.IsNullOrEmpty(x.StockCode));
            RuleFor(x => x.XchgCode).ValidXchgCode().When(x => !string.IsNullOrEmpty(x.XchgCode));
        }
    }

    /// <summary>
    /// Validator for authorizing client stock records
    /// </summary>
    public class AuthorizeClientStockRequestValidator : AbstractValidator<AuthorizeClientStockRequest>
    {
        public AuthorizeClientStockRequestValidator()
        {
            RuleFor(x => x.BranchCode).ValidCoBrchCode();
            RuleFor(x => x.ClientCode).ValidClientCode();
            RuleFor(x => x.StockCode).ValidStkCode();
            RuleFor(x => x.AuthAction)
                .Must(x => x == AuthTypeEnum.Approve || x == AuthTypeEnum.Deny)
                .WithMessage($"Auth action must be {AuthTypeEnum.Approve} or {AuthTypeEnum.Deny}");
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }
}