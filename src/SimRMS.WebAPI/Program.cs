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
var isDevelopment = app.Environment.IsDevelopment();

if (isDevelopment || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMS API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "RMS API Documentation";

        // Security configurations for production
        if (!isDevelopment)
        {
            c.SupportedSubmitMethods(SubmitMethod.Get); // Read-only in production
            c.DocExpansion(DocExpansion.None); // Collapse by default
            c.DefaultModelExpandDepth(1);
        }
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