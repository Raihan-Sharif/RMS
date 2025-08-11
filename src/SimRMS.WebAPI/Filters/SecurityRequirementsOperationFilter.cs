using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Security Requirements Operation Filter
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Security Requirements for API Endpoints
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

namespace SimRMS.WebAPI.Filters
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Skip security for handshake and health endpoints
            var path = context.ApiDescription.RelativePath?.ToLower();

            if (path != null && (path.Contains("handshake") || path.Contains("health")))
            {
                // Remove all security requirements for these endpoints
                operation.Security?.Clear();
                return;
            }

            // For login endpoint, only require handshake token
            if (path != null && path.Contains("auth/login"))
            {
                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
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
                    }
                }
            };
                return;
            }

            // For all other endpoints, require both tokens
            operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
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
            }
        };
        }
    }
}
