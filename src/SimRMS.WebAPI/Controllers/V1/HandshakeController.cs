using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.Auth;
using SimRMS.Shared.Models;
using System.Security.Cryptography;
using System.Text;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Handshake Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage Handshake Operations for API Clients
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

namespace SimRMS.WebAPI.Controllers.V1
{
    [Route("api/v{version:apiVersion}/handshake")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HandshakeController : BaseController
    {
        private readonly IConfigurationService _configurationService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<HandshakeController> _logger;

        public HandshakeController(
            IConfigurationService configurationService,
            ICacheService cacheService,
            ILogger<HandshakeController> logger,
            IConfigurationService baseConfigurationService)
            : base(baseConfigurationService)
        {
            _configurationService = configurationService;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<ApiResponse<HandshakeResponse>>> Handshake([FromBody] HandshakeRequest request)
        {
            try
            {
                // Validate app credentials
                var validAppId = _configurationService.GetValue("AppCredentials:AppId", "RMS_APP_2025");
                var validAppSecret = await _configurationService.GetSecretStringAsync("AppCredentials:AppSecret");

                if (request.AppId != validAppId || request.AppSecret != validAppSecret)
                {
                    _logger.LogWarning("Invalid app credentials provided. AppId: {AppId}", request.AppId);
                    return Unauthorized<HandshakeResponse>("Invalid app credentials");
                }

                // Generate handshake token
                var handshakeToken = GenerateHandshakeToken(request.AppId);

                // Cache the handshake token for 24 hours
                var cacheKey = $"HANDSHAKE_TOKEN_{handshakeToken}";
                await _cacheService.SetAsync(cacheKey, new HandshakeTokenInfo
                {
                    AppId = request.AppId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    IsActive = true
                }, TimeSpan.FromHours(24));

                _logger.LogInformation("Handshake successful for AppId: {AppId}", request.AppId);

                var response = new HandshakeResponse
                {
                    HandshakeToken = handshakeToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    TokenType = "Handshake",
                    Message = "Handshake successful"
                };

                return Ok(response, "Handshake completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during handshake process");
                return BadRequest<HandshakeResponse>("Handshake failed due to internal error");
            }
        }

        [HttpPost("validate")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<ApiResponse<HandshakeValidationResponse>>> ValidateHandshake([FromBody] HandshakeValidationRequest request)
        {
            try
            {
                var cacheKey = $"HANDSHAKE_TOKEN_{request.HandshakeToken}";
                var tokenInfo = await _cacheService.GetAsync<HandshakeTokenInfo>(cacheKey);

                if (tokenInfo == null || !tokenInfo.IsActive || tokenInfo.ExpiresAt < DateTime.UtcNow)
                {
                    return Ok(new HandshakeValidationResponse
                    {
                        IsValid = false,
                        Message = "Invalid or expired handshake token"
                    }, "Handshake validation completed");
                }

                return Ok(new HandshakeValidationResponse
                {
                    IsValid = true,
                    AppId = tokenInfo.AppId,
                    ExpiresAt = tokenInfo.ExpiresAt,
                    Message = "Valid handshake token"
                }, "Handshake validation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during handshake validation");
                return BadRequest<HandshakeValidationResponse>("Handshake validation failed");
            }
        }

        private string GenerateHandshakeToken(string appId)
        {
            var tokenData = $"{appId}:{DateTime.UtcNow:yyyyMMddHHmmss}:{Guid.NewGuid()}";
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
            return Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
}
