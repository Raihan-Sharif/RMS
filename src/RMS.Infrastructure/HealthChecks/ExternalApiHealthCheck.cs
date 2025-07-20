using Microsoft.Extensions.Diagnostics.HealthChecks;
using RMS.Application.Interfaces;

namespace RMS.Infrastructure.HealthChecks
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
