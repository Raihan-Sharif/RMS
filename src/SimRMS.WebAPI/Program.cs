using SimRMS.Application;
using SimRMS.Infrastructure;
using SimRMS.WebAPI.Extensions;
using SimRMS.WebAPI.Middleware;
using SimRMS.WebAPI.Security;
using SimRMS.Application.Interfaces;
using Serilog;
using AspNetCoreRateLimit;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Mvc.ApiExplorer;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Programm file (Main entry)
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Full Application Startup and Configuration
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


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/rms-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// Add project-specific services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

// Initialize handshake service
using (var scope = app.Services.CreateScope())
{
    var handshakeService = scope.ServiceProvider.GetRequiredService<IHandshakeService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Application initialized successfully at {Time}", DateTime.UtcNow);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during application initialization");
    }
}

// Enhanced Swagger configuration ( working base + versioning)
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:EnableInProduction", false);
var swaggerMode = builder.Configuration.GetValue<string>("Swagger:ProductionMode", "ReadOnly");
var isDevelopment = app.Environment.IsDevelopment();

if (isDevelopment || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Enhanced: Dynamic version discovery
        try
        {
            var apiVersionDescriptionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();
            if (apiVersionDescriptionProvider != null)
            {
                // Configure endpoints dynamically
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"RMS API {description.GroupName.ToUpperInvariant()}");
                }
            }
            else
            {
                // Fallback to your working configuration
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMS API V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "RMS API V2");
            }
        }
        catch
        {
            // Fallback to your working configuration
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMS API V1");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "RMS API V2");
        }

        c.RoutePrefix = "swagger";
        c.DocumentTitle = "RMS API Documentation";

        // production configuration
        if (!isDevelopment)
        {
            switch (swaggerMode.ToLower())
            {
                case "readonly":
                    c.SupportedSubmitMethods();
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelExpandDepth(1);
                    break;

                case "limited":
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head, SubmitMethod.Options);
                    c.DocExpansion(DocExpansion.List);
                    c.DefaultModelExpandDepth(2);
                    break;

                case "full":
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put,
                                           SubmitMethod.Delete, SubmitMethod.Head, SubmitMethod.Options,
                                           SubmitMethod.Patch);
                    c.DocExpansion(DocExpansion.List);
                    c.DefaultModelExpandDepth(2);

                    c.HeadContent = @"
                        <style>
                            .swagger-ui .topbar { background-color: #ff6b35; }
                            .swagger-ui .topbar .download-url-wrapper { display: none; }
                        </style>
                        <div style='background: #ff6b35; color: white; padding: 10px; text-align: center; font-weight: bold;'>
                            ⚠️ PRODUCTION ENVIRONMENT - USE WITH CAUTION ⚠️
                        </div>";
                    break;

                default:
                    c.SupportedSubmitMethods();
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelExpandDepth(1);
                    break;
            }
        }
        else
        {
            c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put,
                                   SubmitMethod.Delete, SubmitMethod.Head, SubmitMethod.Options,
                                   SubmitMethod.Patch);
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelExpandDepth(3);
        }

        // configuration
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.ShowExtensions();
        c.EnableValidator();
        c.EnableDeepLinking();
        c.EnableFilter();
    });

    if (isDevelopment)
    {
        app.MapGet("/", () => Results.Redirect("/swagger"));
    }
}

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");

// middleware pipeline
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();

app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting Risk Management System API with Enhanced Versioning");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RMS API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}