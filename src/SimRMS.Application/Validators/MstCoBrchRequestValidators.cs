using FluentValidation;
using SimRMS.Application.Models.Requests;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       MstCoBrch Request Validators (Consolidated)
/// Author:      Md. Raihan Sharif
/// Purpose:     All validation rules for Market Stock Company Branch operations
/// Creation:    13/Aug/2025
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
    /// Common validation rules for MstCoBrch fields
    /// </summary>
    public static class MstCoBrchValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidCoCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Company code is required")
                .MaximumLength(5).WithMessage("Company code cannot exceed 5 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Company code can only contain uppercase letters and numbers");
        }

        public static IRuleBuilderOptions<T, string> ValidCoBrchCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Branch code is required")
                .MaximumLength(6).WithMessage("Branch code cannot exceed 6 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Branch code can only contain uppercase letters and numbers");
        }

        public static IRuleBuilderOptions<T, string?> ValidCoBrchDesc<T>(this IRuleBuilder<T, string?> ruleBuilder, bool isRequired = false)
        {
            var rule = ruleBuilder.MaximumLength(80).WithMessage("Branch description cannot exceed 80 characters");
            
            if (isRequired)
            {
                return rule.NotEmpty().WithMessage("Branch description is required");
            }
            
            return rule;
        }

        public static IRuleBuilderOptions<T, string?> ValidCoBrchAddr<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(500).WithMessage("Branch address cannot exceed 500 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidCoBrchPhone<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(60).WithMessage("Branch phone cannot exceed 60 characters");
        }

        public static IRuleBuilderOptions<T, string?> ValidRemarks<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(200).WithMessage("Remarks cannot exceed 200 characters");
        }
    }

    /// <summary>
    /// Validator for creating MstCoBrch
    /// </summary>
    public class CreateMstCoBrchRequestValidator : AbstractValidator<CreateMstCoBrchRequest>
    {
        public CreateMstCoBrchRequestValidator()
        {
            //RuleFor(x => x.CoCode).ValidCoCode();
            //RuleFor(x => x.CoBrchCode).ValidCoBrchCode();
            
            //RuleFor(x => x.CoBrchDesc)
            //    .NotEmpty().WithMessage("Branch description is required")
            //    .MaximumLength(80).WithMessage("Branch description cannot exceed 80 characters");
            
            RuleFor(x => x.CoBrchAddr).ValidCoBrchAddr()
                .When(x => !string.IsNullOrEmpty(x.CoBrchAddr));
            
            RuleFor(x => x.CoBrchPhone).ValidCoBrchPhone()
                .When(x => !string.IsNullOrEmpty(x.CoBrchPhone));
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for updating MstCoBrch
    /// </summary>
    public class UpdateMstCoBrchRequestValidator : AbstractValidator<UpdateMstCoBrchRequest>
    {
        public UpdateMstCoBrchRequestValidator()
        {
            RuleFor(x => x.CoBrchDesc)
                .MaximumLength(80).WithMessage("Branch description cannot exceed 80 characters")
                .When(x => !string.IsNullOrEmpty(x.CoBrchDesc));
            
            RuleFor(x => x.CoBrchAddr).ValidCoBrchAddr()
                .When(x => !string.IsNullOrEmpty(x.CoBrchAddr));
            
            RuleFor(x => x.CoBrchPhone).ValidCoBrchPhone()
                .When(x => !string.IsNullOrEmpty(x.CoBrchPhone));
            
            RuleFor(x => x.Remarks).ValidRemarks()
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private static bool HaveAtLeastOneFieldToUpdate(UpdateMstCoBrchRequest request)
        {
            return !string.IsNullOrEmpty(request.CoBrchDesc) ||
                   !string.IsNullOrEmpty(request.CoBrchAddr) ||
                   !string.IsNullOrEmpty(request.CoBrchPhone) ||
                   !string.IsNullOrEmpty(request.Remarks);
        }
    }

    /// <summary>
    /// Validator for deleting MstCoBrch
    /// </summary>
    public class DeleteMstCoBrchRequestValidator : AbstractValidator<DeleteMstCoBrchRequest>
    {
        public DeleteMstCoBrchRequestValidator()
        {
            RuleFor(x => x.CoCode).ValidCoCode();
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode();
            RuleFor(x => x.Remarks).ValidRemarks().When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }

    /// <summary>
    /// Validator for authorizing MstCoBrch in workflow
    /// </summary>
    public class AuthorizeMstCoBrchRequestValidator : AbstractValidator<AuthorizeMstCoBrchRequest>
    {
        public AuthorizeMstCoBrchRequestValidator()
        {
            RuleFor(x => x.CoCode).ValidCoCode();
            RuleFor(x => x.CoBrchCode).ValidCoBrchCode();
            
            RuleFor(x => x.ActionType)
                .Equal((byte)2).WithMessage("Action type must be 2 for authorization");
            
            RuleFor(x => x.IsAuth)
                .Must(x => x == 1 || x == 2).WithMessage("IsAuth must be 1 (authorized) or 2 (denied)");
            
            RuleFor(x => x.IPAddress)
                .MaximumLength(39).WithMessage("IP Address cannot exceed 39 characters")
                .When(x => !string.IsNullOrEmpty(x.IPAddress));
        }
    }
}