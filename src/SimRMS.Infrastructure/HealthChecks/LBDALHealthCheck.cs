using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;
using LB.DAL.Core.Common;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       LB.DAL Health Check
/// Author:      Md. Raihan Sharif
/// Purpose:     This class implements a health check for the LB.DAL database connection.
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
    /// <summary>
    /// Health check for LB.DAL database connectivity
    /// </summary>
    public class LBDALHealthCheck : IHealthCheck
    {
        private readonly ILB_DAL _dal;

        public LBDALHealthCheck(ILB_DAL dal)
        {
            _dal = dal;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Test database connectivity with a simple query
                await _dal.LB_GetConnectionAsync();

                // Execute a simple scalar query to verify database is responding
                var result = await _dal.LB_ExecuteScalarAsync("SELECT 1",null, CommandType.Text);

                if (result != null && result.ToString() == "1")
                {
                    return HealthCheckResult.Healthy("LB.DAL database connection is healthy");
                }
                else
                {
                    return HealthCheckResult.Degraded("LB.DAL database connection is responding but returned unexpected result");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("LB.DAL database connection failed", ex);
            }
        }
    }
}
