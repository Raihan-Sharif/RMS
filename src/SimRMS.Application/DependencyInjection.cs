using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Validators;
using System.Reflection;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Dependency Injection for Application Layer
/// Author:      Md. Raihan Sharif
/// Purpose:     This class configures the dependency injection for the application layer.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // FluentValidation
        services.AddScoped<IValidator<UsrInfoRequest>, UsrInfoRequestValidator>();
        services.AddScoped<IValidator<UpdateMstCoRequest>, UpdateMstCoRequestValidator>();
        #region Broker Branch Validators
        services.AddScoped<IValidator<CreateMstCoBrchRequest>, CreateMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<UpdateMstCoBrchRequest>, UpdateMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<DeleteMstCoBrchRequest>, DeleteMstCoBrchRequestValidator>();
        services.AddScoped<IValidator<AuthorizeMstCoBrchRequest>, AuthorizeMstCoBrchRequestValidator>();
        #endregion

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: false);

        return services;
    }
}
