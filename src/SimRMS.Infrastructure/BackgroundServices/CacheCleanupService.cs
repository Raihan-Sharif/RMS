using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Cache Cleanup Service
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage periodic cleanup of cache entries to prevent memory leaks
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

namespace SimRMS.Infrastructure.BackgroundServices
{
    public class CacheCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CacheCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30);

        public CacheCleanupService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<CacheCleanupService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a scope to resolve scoped services
                    using var scope = _serviceScopeFactory.CreateScope();
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                    await CleanupExpiredCaches(cacheService);
                    _logger.LogDebug("Cache cleanup completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during cache cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }

        private static async Task CleanupExpiredCaches(ICacheService cacheService)
        {
            // Clean up token validation cache
            await cacheService.RemovePatternAsync("TOKEN_VALIDATION_");

            // Clean up old user sessions
            await cacheService.RemovePatternAsync($"{AppConstants.UserSessionCacheKey}");

            // Clean up handshake tokens (older than 25 hours)
            await cacheService.RemovePatternAsync("HANDSHAKE_TOKEN_");
        }
    }
}