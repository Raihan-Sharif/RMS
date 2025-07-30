using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using System.Reflection;

namespace SimRMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // FluentValidation
        services.AddScoped<IValidator<UsrInfoRequest>, UsrInfoRequestValidator>();
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false);

        return services;
    }
}
