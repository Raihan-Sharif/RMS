using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using System.Reflection;

namespace SimRMS.Application
{
    /// <summary>
    /// Dependency injection configuration for Application layer
    /// ✅ FIXED: Using single validator with working validation context
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // AutoMapper for entity/DTO mapping
            services.AddAutoMapper(assembly);

            // ✅ FIXED: Register single validator that handles both Create and Update
            services.AddScoped<IValidator<UsrInfoRequest>, UsrInfoRequestValidator>();

            // Register other validators from assembly (general approach)
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false, filter: result =>
                result.ValidatorType != typeof(UsrInfoRequestValidator)); // Avoid duplicate registration

            return services;
        }
    }
}