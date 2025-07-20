using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Interfaces;
using RMS.Application.Models.Auth;
using RMS.Shared.Models;
using System.Security.Claims;

namespace RMS.WebAPI.Controllers
{


    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly ISecurityService _securityService;
        private readonly IExternalTokenService _tokenService;
        private readonly ICacheService _cacheService;

        public AuthController(
            ISecurityService securityService,
            IExternalTokenService tokenService,
            ICacheService cacheService,
            IConfigurationService configurationService)
            : base(configurationService)
        {
            _securityService = securityService;
            _tokenService = tokenService;
            _cacheService = cacheService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            // Validate handshake token
            var handshakeToken = Request.Headers["X-Handshake-Token"].FirstOrDefault();
            if (string.IsNullOrEmpty(handshakeToken))
            {
                return BadRequest<LoginResponse>("Handshake token is required");
            }

            var handshakeCacheKey = $"HANDSHAKE_TOKEN_{handshakeToken}";
            var handshakeInfo = await _cacheService.GetAsync<HandshakeTokenInfo>(handshakeCacheKey);

            if (handshakeInfo == null || !handshakeInfo.IsActive || handshakeInfo.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized<LoginResponse>("Invalid or expired handshake token");
            }

            // Authenticate user
            var userSession = await _securityService.AuthenticateAsync(request.Username, request.Password);

            if (userSession == null)
            {
                return Unauthorized<LoginResponse>("Invalid credentials");
            }

            // Generate user token through external service
            var userToken = await _tokenService.GenerateTokenAsync(userSession.UserId, new Dictionary<string, object>
        {
            { "email", userSession.Email },
            { "fullName", userSession.FullName },
            { "roles", userSession.Roles }
        });

            var response = new LoginResponse
            {
                UserToken = userToken,
                HandshakeToken = handshakeToken, // Return the same handshake token
                User = new UserInfo
                {
                    Id = userSession.UserId,
                    Username = userSession.UserName,
                    Email = userSession.Email,
                    FullName = userSession.FullName,
                    Roles = userSession.Roles
                }
            };

            return Ok(response, "Login successful");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await _securityService.LogoutAsync(userId);

                // Revoke user token
                var userToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(userToken))
                {
                    await _tokenService.RevokeTokenAsync(userToken);
                }
            }

            return Ok("Logout successful", "Logout successful");
        }
    }
}
