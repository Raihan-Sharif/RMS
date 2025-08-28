using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Configuration;
using SimRMS.WebAPI.Security;
using SimRMS.WebAPI.Services;
using SimRMS.WebAPI.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Service Collection Extensions
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Service Collection Extensions for Web API to Register Services in the Dependency Injection Container
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

namespace SimRMS.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cache Configuration
            services.Configure<CacheConfiguration>(configuration.GetSection(CacheConfiguration.SectionName));
            
            // Current User Service
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Enhanced API Versioning - Keep your existing pattern, add versioning
            AddEnhancedVersioning(services, configuration);

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

            // Add Authentication scheme to prevent "No authenticationScheme was specified" error
            services.AddAuthentication("Token")
                .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationSchemeHandler>("Token", options => { });

            // Enhanced Swagger ( working base + versioning support)
            AddEnhancedSwagger(services);

            return services;
        }

        private static void AddEnhancedVersioning(IServiceCollection services, IConfiguration configuration)
        {
            // Dynamic versioning configuration
            var isVersioningEnabled = configuration.GetValue<bool>("ApiVersioning:Enabled", true);
            var defaultVersion = configuration.GetValue<string>("ApiVersioning:DefaultVersion", "1.0");

            if (isVersioningEnabled)
            {
                services.AddApiVersioning(opt =>
                {
                    opt.DefaultApiVersion = ApiVersion.Parse(defaultVersion);
                    opt.AssumeDefaultVersionWhenUnspecified = true;

                    // Multiple version readers - enterprise flexibility
                    opt.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader(AppConstants.Headers.ApiVersion),    // X-API-Version: 1.0
                        new QueryStringApiVersionReader("version"),                      // ?version=1.0
                        new UrlSegmentApiVersionReader()                                 // /v1/api/...
                    );

                    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
                });

                services.AddVersionedApiExplorer(setup =>
                {
                    setup.GroupNameFormat = "'v'VVV";
                    setup.SubstituteApiVersionInUrl = true;
                    setup.AssumeDefaultVersionWhenUnspecified = true;
                });
            }
        }

        private static void AddEnhancedSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // v1 configuration
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Risk Management System API",
                    Version = "v1",
                    Description = "A comprehensive Risk Management System API built with .NET 8"
                });

                // Add v2 for versioning
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Risk Management System API",
                    Version = "v2",
                    Description = "Enhanced Risk Management System API with new features"
                });

                // security definitions
                c.AddSecurityDefinition("X-Handshake-Token", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "X-Handshake-Token",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Handshake token required for all API calls",
                    Scheme = "X-Handshake-Token"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (e.g., 'Bearer {token}')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                // security requirements
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
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

                // operation filter
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // ENHANCED: Better version detection
                // Just replace the DocInclusionPredicate part with this simpler version:

                c.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out var methodInfo))
                        return version == "v1";

                    var controllerType = methodInfo.DeclaringType;
                    if (controllerType == null)
                        return version == "v1";

                    var hasVersioning = controllerType.GetCustomAttributes(typeof(ApiVersionAttribute), true).Any();

                    if (!hasVersioning)
                        return version == "v1";

                    var controllerVersions = controllerType
                        .GetCustomAttributes(typeof(ApiVersionAttribute), true)
                        .Cast<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var methodVersions = methodInfo
                        .GetCustomAttributes(typeof(MapToApiVersionAttribute), true)
                        .Cast<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var applicableVersions = methodVersions.Any() ? methodVersions : controllerVersions;

                    // SIMPLE FIX: Extract major version number and compare
                    foreach (var apiVersion in applicableVersions)
                    {
                        var majorVersion = apiVersion.MajorVersion; // 1 or 2
                        if (version == $"v{majorVersion}")          // Compare with "v1" or "v2"
                        {
                            return true;
                        }
                    }

                    return false;
                });
            });
        }
    }
}