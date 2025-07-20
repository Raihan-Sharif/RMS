using Microsoft.Extensions.Diagnostics.HealthChecks;
using RMS.Infrastructure.Data;

namespace RMS.Infrastructure.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly DbEfbtxLbslContext _context;
        //private readonly ApplicationDbContext _context;

        public DatabaseHealthCheck(DbEfbtxLbslContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.CanConnectAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection failed", ex);
            }
        }
    }
}
