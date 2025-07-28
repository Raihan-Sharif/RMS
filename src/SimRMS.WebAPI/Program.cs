using SimRMS.Application;
using SimRMS.Infrastructure;
using SimRMS.WebAPI.Extensions;
using SimRMS.WebAPI.Middleware;
using SimRMS.WebAPI.Security;
using SimRMS.Application.Interfaces;
using Serilog;
using AspNetCoreRateLimit;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/rms-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add basic services (keep in Program.cs)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// FIXED: Remove SizeLimit to avoid conflict with AspNetCoreRateLimit
builder.Services.AddMemoryCache(); // Simple configuration without size limit

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

// swagger configuration with production enabled swagger ui if configured
var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:EnableInProduction", false);
var swaggerMode = builder.Configuration.GetValue<string>("Swagger:ProductionMode", "ReadOnly"); // ReadOnly, Limited, Full
var isDevelopment = app.Environment.IsDevelopment();

if (isDevelopment || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMS API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "RMS API Documentation";

        // NEW: Flexible security configurations for production
        if (!isDevelopment)
        {
            switch (swaggerMode.ToLower())
            {
                case "readonly":
                    // Documentation only - no API execution
                    c.SupportedSubmitMethods(); // No methods supported
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelExpandDepth(1);
                    break;

                case "limited":
                    // Only safe methods (GET, HEAD, OPTIONS)
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head, SubmitMethod.Options);
                    c.DocExpansion(DocExpansion.List);
                    c.DefaultModelExpandDepth(2);
                    break;

                case "full":
                    // All methods enabled (use with caution in production)
                    c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put,
                                           SubmitMethod.Delete, SubmitMethod.Head, SubmitMethod.Options,
                                           SubmitMethod.Patch);
                    c.DocExpansion(DocExpansion.List);
                    c.DefaultModelExpandDepth(2);

                    // Add security warning for full mode
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
                    // Default to ReadOnly if invalid mode specified
                    c.SupportedSubmitMethods();
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelExpandDepth(1);
                    break;
            }
        }
        else
        {
            // Development - full functionality
            c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put,
                                   SubmitMethod.Delete, SubmitMethod.Head, SubmitMethod.Options,
                                   SubmitMethod.Patch);
            c.DocExpansion(DocExpansion.List);
            c.DefaultModelExpandDepth(3);
        }

        // Common configurations
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.ShowExtensions();
        c.EnableValidator();
        c.EnableDeepLinking();
        c.EnableFilter();
    });

    // Only redirect to swagger in development
    if (isDevelopment)
    {
        app.MapGet("/", () => Results.Redirect("/swagger"));
    }
}

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");

// FIXED: Middleware pipeline order
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>(); // Move before exception handling
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
    Log.Information("Starting Risk Management System API");
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