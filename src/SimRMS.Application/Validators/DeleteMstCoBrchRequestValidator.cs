using FluentValidation;
using SimRMS.Application.Models.Requests;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Delete MstCoBrch Request Validator
/// Author:      Md. Raihan Sharif
/// Purpose:     Validation rules for deleting Market Stock Company Branch
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
    public class DeleteMstCoBrchRequestValidator : AbstractValidator<DeleteMstCoBrchRequest>
    {
        public DeleteMstCoBrchRequestValidator()
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

            RuleFor(x => x.Remarks)
                .MaximumLength(200)
                .WithMessage("Remarks cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }
}