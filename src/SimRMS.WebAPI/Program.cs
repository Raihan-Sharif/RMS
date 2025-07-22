using SimRMS.Application;
using SimRMS.Infrastructure;
using SimRMS.WebAPI.Extensions;
using SimRMS.WebAPI.Middleware;
using SimRMS.WebAPI.Security;
using SimRMS.Application.Interfaces;
using Serilog;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/rms-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add custom services
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
        //var handshakeResult = await handshakeService.PerformHandshakeAsync();
        //if (handshakeResult)
        //{
        //    logger.LogInformation("Handshake completed successfully at {Time}", DateTime.UtcNow);
        //}
        //else
        //{
        //    logger.LogWarning("Handshake failed - continuing with degraded service");
        //}
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during handshake process");
    }
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMS API V1");
        c.RoutePrefix = "swagger"; // Swagger available at /swagger
        c.DocumentTitle = "RMS API Documentation";
    });

    // Redirect root to swagger for convenience
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<TokenAuthenticationMiddleware>(); // This handles token validation on every request

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