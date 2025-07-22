using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.WebAPI.Security;
using SimRMS.WebAPI.Services;
using SimRMS.WebAPI.Filters;

namespace SimRMS.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Current User Service
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // API Versioning - Dynamic configuration
            var isVersioningEnabled = configuration.GetValue<bool>("ApiVersioning:Enabled", true);
            var defaultVersion = configuration.GetValue<string>("ApiVersioning:DefaultVersion") ?? "1.0";

            if (isVersioningEnabled)
            {
                services.AddApiVersioning(opt =>
                {
                    opt.DefaultApiVersion = ApiVersion.Parse(defaultVersion);
                    opt.AssumeDefaultVersionWhenUnspecified = true;
                    opt.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader(AppConstants.Headers.ApiVersion),
                        new QueryStringApiVersionReader("version"));
                });

                services.AddVersionedApiExplorer(setup =>
                {
                    setup.GroupNameFormat = "'v'VVV";
                    setup.SubstituteApiVersionInUrl = true;
                });
            }

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Rate Limiting
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            // Custom Authorization
            services.AddAuthorization(options =>
            {
                // Define policies based on permissions
                options.AddPolicy("ViewUsers", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.ViewUsers }));

                options.AddPolicy("ManageUsers", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.ManageUsers }));

                options.AddPolicy("ViewRisks", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.ViewRisks }));

                options.AddPolicy("CreateRisks", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.CreateRisks }));

                options.AddPolicy("UpdateRisks", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.UpdateRisks }));

                options.AddPolicy("DeleteRisks", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.DeleteRisks }));

                options.AddPolicy("ViewReports", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.ViewReports }));

                options.AddPolicy("ManageSystem", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Permission = AppConstants.Permissions.ManageSystem }));

                // Role-based policies
                options.AddPolicy("AdminOnly", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Role = AppConstants.Roles.Admin }));

                options.AddPolicy("SuperAdminOnly", policy =>
                    policy.Requirements.Add(new CustomAuthorizationRequirement { Role = AppConstants.Roles.SuperAdmin }));
            });

            services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Risk Management System API",
                    Version = "v1",
                    Description = "A comprehensive Risk Management System API built with .NET 8"
                });

                // X-Handshake-Token header
                c.AddSecurityDefinition("X-Handshake-Token", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "X-Handshake-Token",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Handshake token required for all API calls",
                    Scheme = "X-Handshake-Token"
                });

                // JWT Bearer token header
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (e.g., 'Bearer {token}')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                // Add security requirements for both tokens
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    // Handshake Token Requirement
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-Handshake-Token"
                            }
                        },
                        new string[] {}
                    },
                    // Bearer Token Requirement
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Add operation filter to customize per-endpoint security
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            return services;
        }
    }
}