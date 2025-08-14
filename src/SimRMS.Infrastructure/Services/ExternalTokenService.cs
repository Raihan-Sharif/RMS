using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimRMS.Application.Interfaces;
using System.Text;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       External Token Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service handles token generation, validation, and revocation by communicating with an external token service.
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


namespace SimRMS.Infrastructure.Services
{
    public class ExternalTokenService : IExternalTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationService _configurationService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ExternalTokenService> _logger;

        public ExternalTokenService(
            HttpClient httpClient,
            IConfigurationService configurationService,
            ICacheService cacheService,
            ILogger<ExternalTokenService> logger)
        {
            _httpClient = httpClient;
            _configurationService = configurationService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> HandshakeAsync()
        {
            // Handshake is now handled by HandshakeService locally
            // This method just checks if the service is reachable
            try
            {
                var generateUrl = _configurationService.GetValue<string>("TokenService:GenerateUrl");
                var response = await _httpClient.GetAsync($"{generateUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reach token service");
                return false;
            }
        }

        public async Task<string> GenerateTokenAsync(string userName, Dictionary<string, object>? additionalClaims = null)
        {
            try
            {
                var tokenApiUrl = _configurationService.GetValue<string>("TokenService:GenerateUrl");
                var loginId = await _configurationService.GetSecretStringAsync("TokenService:LoginId");
                var password = await _configurationService.GetSecretStringAsync("TokenService:Password");

                // Build the payload exactly as the old app does, and use it directly
                string payload = "{ \"HEADER\":{ \"Content-Type\": \"application/json\", \"LBSL_REQ_TYPE\": \"1\", \"LBSL_LGN_ID\": \"" + loginId + "\", \"LBSL_LGN_PWD\": \"" + password + "\" }, \"DATA\":{ \"PID\": \"" + userName + "\" } }";
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                // Optionally clear headers, but do NOT add Content-Type manually (StringContent sets it)
                _httpClient.DefaultRequestHeaders.Clear();

                var response = await _httpClient.PostAsync(tokenApiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenGenerateResponse>(responseContent);

                if (tokenResponse?.LBSL_JWT_TKN != null)
                {
                    // Cache the token with PID
                    var cacheKey = $"TOKEN_{userName}";
                    await _cacheService.SetAsync(cacheKey, tokenResponse.LBSL_JWT_TKN, TimeSpan.FromMinutes(25)); // Cache for 25 minutes

                    _logger.LogInformation("Token generated successfully for user {userName}", userName);
                    return tokenResponse.LBSL_JWT_TKN;
                }

                throw new InvalidOperationException("Token generation failed - no token in response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate token for user {userName}", userName);
                throw;
            }
        }

        //public async Task<bool> ValidateTokenAsync(string token)
        //{
        //    try
        //    {
        //        // Check cache first
        //        var cacheKey = $"TOKEN_VALIDATION_{token.GetHashCode()}";
        //        var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
        //        if (cachedResult.HasValue)
        //        {
        //            _logger.LogDebug("Token validation result from cache: {IsValid}", cachedResult.Value);
        //            return cachedResult.Value;
        //        }

        //        var tokenApiUrl = _configurationService.GetValue<string>("TokenService:ValidateUrl");

        //        // Extract user ID from token instead of using default PID
        //        var userId = ExtractUserIdFromToken(token);
        //        if (string.IsNullOrEmpty(userId))
        //        {
        //            _logger.LogWarning("Could not extract user ID from token");
        //            return false;
        //        }

        //        // Build the payload exactly as the generate method does
        //        string payload = "{ \"HEADER\":{ \"Content-Type\": \"application/json\", \"LBSL_REQ_TYPE\": \"2\" }, \"DATA\":{ \"PID\": \"" + userId + "\", \"TOKEN\": \"" + token + "\" } }";

        //        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        //        // Clear headers (don't add Content-Type manually as StringContent sets it)
        //        _httpClient.DefaultRequestHeaders.Clear();

        //   //     var response = await _httpClient.PostAsync(tokenApiUrl, content);

        //        var response = await _httpClient.PostAsync(tokenApiUrl, content);
        //        response.EnsureSuccessStatusCode();

        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var tokenResponse = JsonConvert.DeserializeObject<TokenGenerateResponse>(responseContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            //var responseContent = await response.Content.ReadAsStringAsync();
        //            var validationResponse = JsonConvert.DeserializeObject<TokenValidateResponse>(responseContent);
        //            var isValid = validationResponse?.IsValid ?? false;

        //            // Cache the validation result for 5 minutes
        //            await _cacheService.SetAsync(cacheKey, isValid, TimeSpan.FromMinutes(5));

        //            _logger.LogDebug("Token validation successful: {IsValid}", isValid);
        //            return isValid;
        //        }

        //        _logger.LogWarning("Token validation failed with status: {StatusCode}", response.StatusCode);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to validate token");
        //        return false;
        //    }
        //}

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var cacheKey = $"TOKEN_VALIDATION_{token.GetHashCode()}";
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
                if (cachedResult.HasValue)
                {
                    _logger.LogDebug("Token validation result from cache: {IsValid}", cachedResult.Value);
                    return cachedResult.Value;
                }

                var tokenApiUrl = _configurationService.GetValue<string>("TokenService:ValidateUrl");
                var userId = ExtractUserIdFromToken(token);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Could not extract user ID from token");
                    return false;
                }

                // Build payload exactly like working code
                string payload = "{ \"HEADER\":{ \"Content-Type\": \"application/json\", \"LBSL_REQ_TYPE\": \"2\" }, \"DATA\":{ \"PID\": \"" + userId + "\", \"TOKEN\": \"" + token + "\" } }";

                _logger.LogDebug("Validation payload: {Payload}", payload);

                var requestContent = new StringContent(payload, Encoding.UTF8, "application/json");

                // Don't clear headers - this might be the issue!
                // _httpClient.DefaultRequestHeaders.Clear(); // Remove this line

                var response = await _httpClient.PostAsync(tokenApiUrl, requestContent);

                _logger.LogDebug("Response Status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("Response Content: {Content}", content);

                    var validateResponse = JsonConvert.DeserializeObject<TokenValidateResponse>(content);
                    var isValid = validateResponse?.LBSL_MSG_TXT == "Success";

                    await _cacheService.SetAsync(cacheKey, isValid, TimeSpan.FromMinutes(5));

                    _logger.LogDebug("Token validation result: {IsValid} for user {UserId}", isValid, userId);
                    return isValid;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Validation failed. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate token");
                return false;
            }
        }

        // Helper method to extract user ID from JWT token
        private string? ExtractUserIdFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length >= 2)
                {
                    var payload = parts[1];
                    // Add padding if needed
                    switch (payload.Length % 4)
                    {
                        case 2: payload += "=="; break;
                        case 3: payload += "="; break;
                    }

                    var payloadBytes = Convert.FromBase64String(payload);
                    var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
                    var payloadData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

                    if (payloadData != null && payloadData.ContainsKey("PID"))
                    {
                        return payloadData["PID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract user ID from token");
            }

            return null;
        }
        public async Task<Dictionary<string, object>?> GetTokenClaimsAsync(string token)
        {
            try
            {
                // Extract claims from JWT token manually or call token service
                // For now, return basic claims
                var claims = new Dictionary<string, object>
            {
                { "token", token },
                { "validated_at", DateTime.UtcNow }
            };

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get token claims");
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                // Remove from cache
                var cacheKey = $"TOKEN_VALIDATION_{token.GetHashCode()}";
                await _cacheService.RemoveAsync(cacheKey);

                // If there's a revoke endpoint, call it here
                _logger.LogInformation("Token revoked from cache");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke token");
                return false;
            }
        }

        private class TokenGenerateResponse
        {
            public string? LBSL_MSG_TXT { get; set; }
            public string? LBSL_PID { get; set; }
            public string? LBSL_JWT_TKN { get; set; }
        }

        private class TokenValidateResponse
        {
            [JsonProperty("LBSL_MSG_TXT")]
            public string? LBSL_MSG_TXT { get; set; }

            [JsonProperty("LBSL_PID")]
            public string? LBSL_PID { get; set; }

            [JsonProperty("LBSL_JWT_TKN")]
            public string? LBSL_JWT_TKN { get; set; }
        }
    }
}
