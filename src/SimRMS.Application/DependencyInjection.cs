using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using SimRMS.Application.Common.Behaviors;

namespace SimRMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Add AutoMapper
            services.AddAutoMapper(assembly);

            // Add FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Add MediatR with correct configuration for version 12.x
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                // Add behaviors - correct syntax for MediatR 12.x
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}