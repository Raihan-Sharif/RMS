using FluentValidation;
using SimRMS.Application.Models.DTOs;

namespace SimRMS.Application.Validators;

public class FileUploadRequestValidator : AbstractValidator<FileUploadRequestDto>
{
    public FileUploadRequestValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .MaximumLength(255)
            .WithMessage("File name cannot exceed 255 characters");

        RuleFor(x => x.ServerName)
            .MaximumLength(50)
            .WithMessage("Server name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.ServerName));

        RuleFor(x => x.FolderPath)
            .MaximumLength(500)
            .WithMessage("Folder path cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.FolderPath));

        RuleFor(x => x.OldFileName)
            .MaximumLength(255)
            .WithMessage("Old file name cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.OldFileName));
    }
}