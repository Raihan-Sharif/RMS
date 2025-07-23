using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;

namespace SimRMS.Application.Common.Behaviors
{
    /// <summary>
    /// Validation behavior for MediatR pipeline - validates all commands and queries
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Check if there are any validators for this request type
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // Run all validators in parallel
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // Collect all validation failures
                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    // Create detailed error information
                    var errorDetails = failures.Select(f => new ValidationErrorDetail
                    {
                        PropertyName = f.PropertyName,
                        ErrorMessage = f.ErrorMessage,
                        AttemptedValue = f.AttemptedValue?.ToString()
                    }).ToList();

                    _logger.LogWarning("Validation failed for {RequestType}. Errors: {Errors}",
                        typeof(TRequest).Name,
                        string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

                    // Throw domain validation exception
                    throw new SimRMS.Domain.Exceptions.ValidationException("One or more validation errors occurred")
                    {
                        ValidationErrors = errorDetails
                    };
                }
            }

            // If validation passes, continue with the request
            return await next();
        }
    }
}