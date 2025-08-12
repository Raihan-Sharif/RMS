using FluentValidation;
using SimRMS.Application.Models.Requests;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Update MstCo Request Validator
/// Author:      Md. Raihan Sharif
/// Purpose:     Validation rules for updating Market Stock Company
/// Creation:    12/Aug/2025
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
    public class UpdateMstCoRequestValidator : AbstractValidator<UpdateMstCoRequest>
    {
        public UpdateMstCoRequestValidator()
        {
            RuleFor(x => x.CoCode)
                .NotEmpty()
                .WithMessage("Company code is required");

            RuleFor(x => x.CoCode)
                .MaximumLength(5)
                .WithMessage("Company code cannot exceed 5 characters")
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoCode)
                .Matches("^[A-Z0-9]+$")
                .WithMessage("Company code can only contain uppercase letters and numbers")
                .When(x => !string.IsNullOrEmpty(x.CoCode));

            RuleFor(x => x.CoDesc)
                .MaximumLength(50)
                .WithMessage("Company description cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.CoDesc));

            //RuleFor(x => x.Remarks)
            //    .MaximumLength(200)
            //    .WithMessage("Remarks cannot exceed 200 characters")
            //    .When(x => !string.IsNullOrEmpty(x.Remarks));

            //RuleFor(x => x.WFName)
            //    .MaximumLength(100)
            //    .WithMessage("Workflow name cannot exceed 100 characters")
            //    .When(x => !string.IsNullOrEmpty(x.WFName));

            // Custom rule to ensure at least one field is being updated
            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least CoDesc must be provided for update");
        }

        private bool HaveAtLeastOneFieldToUpdate(UpdateMstCoRequest request)
        {
            // For now, we always allow updates since EnableExchangeWideSellProceed 
            // is always present and can be updated to true or false
            // Only require that CoDesc is not empty if it's provided
            return true; // Always valid since both fields can be updated
        }
    }
}