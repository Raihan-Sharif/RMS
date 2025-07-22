using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Infrastructure.Services
{
    public class HandshakeService : IHandshakeService
    {
        private readonly IExternalTokenService _tokenService;
        private readonly ILogger<HandshakeService> _logger;
        private readonly ICacheService _cacheService;

        private bool _isHandshakeCompleted = false;
        private DateTime? _lastHandshakeTime = null;

        public HandshakeService(
            IExternalTokenService tokenService,
            ILogger<HandshakeService> logger,
            ICacheService cacheService)
        {
            _tokenService = tokenService;
            _logger = logger;
            _cacheService = cacheService;
        }

        public bool IsHandshakeCompleted => _isHandshakeCompleted;
        public DateTime? LastHandshakeTime => _lastHandshakeTime;

        public async Task<bool> PerformHandshakeAsync()
        {
            try
            {
                _logger.LogInformation("Starting handshake process with token service");

                // Test token generation to verify connection
                var testResult = await IsTokenServiceAvailableAsync();

                if (testResult)
                {
                    _isHandshakeCompleted = true;
                    _lastHandshakeTime = DateTime.UtcNow;

                    // Cache handshake status
                    await _cacheService.SetAsync("HANDSHAKE_STATUS", true, TimeSpan.FromHours(1));
                    await _cacheService.SetAsync("HANDSHAKE_TIME", _lastHandshakeTime, TimeSpan.FromHours(1));

                    _logger.LogInformation("Handshake completed successfully at {Time}", _lastHandshakeTime);
                    return true;
                }

                _logger.LogWarning("Handshake failed - token service not available");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handshake process failed");
                return false;
            }
        }

        public async Task<bool> IsTokenServiceAvailableAsync()
        {
            try
            {
                // Try to generate a test token to check if service is available
                var testToken = await _tokenService.GenerateTokenAsync("HANDSHAKE_TEST", null);
                return !string.IsNullOrEmpty(testToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token service availability check failed");
                return false;
            }
        }
    }
}
