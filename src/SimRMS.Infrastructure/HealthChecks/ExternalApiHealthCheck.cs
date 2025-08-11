using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRMS.Application.Interfaces;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       External API Health Check
/// Author:      Md. Raihan Sharif
/// Purpose:     The health check for external API services, specifically for token validation.
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

namespace SimRMS.Infrastructure.HealthChecks
{
    public class ExternalApiHealthCheck : IHealthCheck
    {
        private readonly IExternalTokenService _tokenService;

        public ExternalApiHealthCheck(IExternalTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _tokenService.HandshakeAsync();
                return result
                    ? HealthCheckResult.Healthy("External token service is healthy")
                    : HealthCheckResult.Degraded("External token service handshake failed");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("External token service is unhealthy", ex);
            }
        }
    }
}
