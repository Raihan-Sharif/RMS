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

// Configure Serilog with robust directory creation and error handling
try
{
    // Determine log paths based on environment
    var isDevEnvironment = builder.Environment.IsDevelopment();
    var logPaths = isDevEnvironment 
        ? new[] { "logs/dev" }  // Relative path for development
        : new[]  // Absolute paths for production/IIS
        {
            @"C:\Logs\SimRMS\Application",
            @"C:\Logs\SimRMS\Errors", 
            @"C:\Logs\SimRMS\Performance"
        };

    foreach (var logPath in logPaths)
    {
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
            
            // Set permissions for IIS application pool identity
            try
            {
                var directoryInfo = new DirectoryInfo(logPath);
                var directorySecurity = directoryInfo.GetAccessControl();
                
                // Add permissions for IIS_IUSRS and application pool identity
                var iisUsersIdentity = new System.Security.Principal.SecurityIdentifier("S-1-5-32-568"); // IIS_IUSRS
                directorySecurity.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                    iisUsersIdentity, 
                    System.Security.AccessControl.FileSystemRights.FullControl, 
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                    System.Security.AccessControl.PropagationFlags.None, 
                    System.Security.AccessControl.AccessControlType.Allow));
                    
                directoryInfo.SetAccessControl(directorySecurity);
            }
            catch (Exception ex)
            {
                // Log to Windows Event Log if directory permissions fail
                Console.WriteLine($"Warning: Could not set directory permissions for {logPath}: {ex.Message}");
            }
        }
    }

    // Create Serilog logger with configuration + fallback
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "SimRMS")
        .Enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version?.ToString() ?? "Unknown")
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        // Fallback console sink in case file sinks fail
        .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}")
        .CreateLogger();
        
    Log.Information("=== SimRMS Application Starting ===");
    Log.Information("Log directories created and configured successfully");
    Log.Information("Machine: {MachineName}, User: {UserName}, Process: {ProcessId}", 
        Environment.MachineName, 
        Environment.UserName, 
        Environment.ProcessId);
}
catch (Exception ex)
{
    // Fallback to basic console logging if advanced configuration fails
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("rms-fallback-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
        
    Log.Error(ex, "Failed to configure advanced logging, using fallback configuration");
}

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
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

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