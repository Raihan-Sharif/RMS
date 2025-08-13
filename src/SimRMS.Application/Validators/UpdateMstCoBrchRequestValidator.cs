using FluentValidation;
using SimRMS.Application.Models.Requests;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Update MstCoBrch Request Validator
/// Author:      Md. Raihan Sharif
/// Purpose:     Validation rules for updating Market Stock Company Branch
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
    public class UpdateMstCoBrchRequestValidator : AbstractValidator<UpdateMstCoBrchRequest>
    {
        public UpdateMstCoBrchRequestValidator()
        {
            RuleFor(x => x.CoCode)
                .NotEmpty()
                .WithMessage("Company code is required")
                .MaximumLength(5)
                .WithMessage("Company code cannot exceed 5 characters")
                .Matches("^[A-Z0-9]+$")
                .WithMessage("Company code can only contain uppercase letters and numbers");

            RuleFor(x => x.CoBrchCode)
                .NotEmpty()
                .WithMessage("Branch code is required")
                .MaximumLength(6)
                .WithMessage("Branch code cannot exceed 6 characters")
                .Matches("^[A-Z0-9]+$")
                .WithMessage("Branch code can only contain uppercase letters and numbers");

            RuleFor(x => x.CoBrchDesc)
                .MaximumLength(80)
                .WithMessage("Branch description cannot exceed 80 characters")
                .When(x => !string.IsNullOrEmpty(x.CoBrchDesc));

            RuleFor(x => x.CoBrchAddr)
                .MaximumLength(500)
                .WithMessage("Branch address cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.CoBrchAddr));

            RuleFor(x => x.CoBrchPhone)
                .MaximumLength(60)
                .WithMessage("Branch phone cannot exceed 60 characters")
                .When(x => !string.IsNullOrEmpty(x.CoBrchPhone));

            RuleFor(x => x.Remarks)
                .MaximumLength(200)
                .WithMessage("Remarks cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Remarks));

            RuleFor(x => x)
                .Must(HaveAtLeastOneFieldToUpdate)
                .WithMessage("At least one field must be provided for update");
        }

        private bool HaveAtLeastOneFieldToUpdate(UpdateMstCoBrchRequest request)
        {
            return !string.IsNullOrEmpty(request.CoBrchDesc) ||
                   !string.IsNullOrEmpty(request.CoBrchAddr) ||
                   !string.IsNullOrEmpty(request.CoBrchPhone) ||
                   !string.IsNullOrEmpty(request.Remarks);
        }
    }
}