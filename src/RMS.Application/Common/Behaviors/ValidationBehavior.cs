using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Domain.Common;
using RMS.Domain.Exceptions;

namespace RMS.Application.Common.Behaviors
{
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
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    var errorDetails = failures.Select(f => new ValidationErrorDetail
                    {
                        PropertyName = f.PropertyName,
                        ErrorMessage = f.ErrorMessage,
                        AttemptedValue = f.AttemptedValue?.ToString()
                    }).ToList();

                    _logger.LogWarning("Validation failed for {RequestType}. Errors: {Errors}",
                        typeof(TRequest).Name,
                        string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

                    throw new Domain.Exceptions.ValidationException("Validation failed")
                    {
                        ValidationErrors = errorDetails
                    };
                }
            }

            // Fixed: Pass the cancellationToken to next()
            return await next();
        }
    }
}