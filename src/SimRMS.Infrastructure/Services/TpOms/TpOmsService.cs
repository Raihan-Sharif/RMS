using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using System.Text.Json;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Cache Service
/// Author:      Asif Zaman
/// Purpose:     This service provides TpOms API service integration for OMS cache invalidation.
/// Creation:    11/Nov/2025
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
    public class TpOmsService : ITpOmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TpOmsService> _logger;
        private readonly string _baseUrl;
        private readonly int _timeoutSeconds;

        public TpOmsService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TpOmsService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = _configuration.GetValue<string>("ExternalApi:TpOms:BaseUrl")
                ?? throw new InvalidOperationException("TpOms Base URL not configured");
            _timeoutSeconds = _configuration.GetValue<int>("ExternalApi:TpOms:TimeoutSeconds", 30);

            // Configure HttpClient timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        }

        public async Task<TpOmsDto> UpdateClientAsync(TpOmsUpdateClientRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling TpOms API to update client: {ClientOrUserId}, Branch: {BranchId}",
                    request.ClientOrUserId, request.branchId);

                var response = await CallTpOmsApiAsync("/update-client", request, "UpdateClient", cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TpOms update-client API for client: {ClientOrUserId}", request.ClientOrUserId);
                return new TpOmsDto { success = false, message = $"Error calling TpOms API: {ex.Message}" };
            }
        }

        public async Task<TpOmsDto> UpdateUserClientAsync(TpOmsUpdateUserClientRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling TpOms API to update user client: {UserId}", request.userId);

                var response = await CallTpOmsApiAsync("/update-user-client", request, "UpdateUserClient", cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TpOms update-user-client API for user: {UserId}", request.userId);
                return new TpOmsDto { success = false, message = $"Error calling TpOms API: {ex.Message}" };
            }
        }

        public async Task<TpOmsDto> UpdateShareHoldingAsync(TpOmsUpdateShareHoldingRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling TpOms API to update share holding: Client: {ClientId}, Branch: {BranchId}, Stock: {StockCode}, Exchange: {ExchangeCode}",
                    request.clientId, request.branchId, request.stockCode, request.exchangeCode);

                var response = await CallTpOmsApiAsync("/update-share-holding", request, "UpdateShareHolding", cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TpOms update-share-holding API for: {ClientId}-{BranchId}-{StockCode}",
                    request.clientId, request.branchId, request.stockCode);
                return new TpOmsDto { success = false, message = $"Error calling TpOms API: {ex.Message}" };
            }
        }

        private async Task<TpOmsDto> CallTpOmsApiAsync<T>(string endpoint, T request, string operationName, CancellationToken cancellationToken) where T : class
        {
            try
            {
                _logger.LogDebug("TpOms {OperationName} request: {Request}", operationName, JsonSerializer.Serialize(request));

                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content, cancellationToken);
                var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("TpOms {OperationName} response: Status={StatusCode}, Content={Content}",
                    operationName, httpResponse.StatusCode, responseContent);

                if (httpResponse.IsSuccessStatusCode)
                {
                    // Deserialize response to check success flag
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        try
                        {
                            var apiResponse = JsonSerializer.Deserialize<TpOmsDto>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });

                            if (apiResponse != null)
                            {
                                if (apiResponse.success)
                                {
                                    _logger.LogInformation("TpOms {OperationName} successful: {Message}", operationName, apiResponse.message);
                                }
                                else
                                {
                                    _logger.LogWarning("TpOms {OperationName} returned success=false: {Message}", operationName, apiResponse.message);
                                }
                                return apiResponse;
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            _logger.LogWarning(jsonEx, "Failed to parse TpOms {OperationName} response JSON, treating as success due to HTTP status", operationName);
                            return new TpOmsDto { success = true, message = "Operation completed but response format was unexpected" };
                        }
                    }

                    // If we reach here, HTTP was successful but no valid JSON response
                    _logger.LogInformation("TpOms {OperationName} HTTP successful (no response body)", operationName);
                    return new TpOmsDto { success = true, message = "Operation completed successfully" };
                }
                else
                {
                    // Try to parse error response for better error message
                    string errorMessage = $"HTTP {httpResponse.StatusCode}";
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(responseContent))
                        {
                            var errorResponse = JsonSerializer.Deserialize<TpOmsDto>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                PropertyNameCaseInsensitive = true
                            });

                            if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.message))
                            {
                                errorMessage = errorResponse.message;
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        errorMessage = responseContent.Length > 200 ? responseContent.Substring(0, 200) + "..." : responseContent;
                    }

                    _logger.LogWarning("TpOms {OperationName} failed - Status: {StatusCode}, Error: {ErrorMessage}",
                        operationName, httpResponse.StatusCode, errorMessage);

                    return new TpOmsDto { success = false, message = errorMessage };
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                var message = $"TpOms API call timed out after {_timeoutSeconds} seconds";
                _logger.LogWarning("TpOms {OperationName} timed out after {TimeoutSeconds} seconds", operationName, _timeoutSeconds);
                return new TpOmsDto { success = false, message = message };
            }
            catch (HttpRequestException ex)
            {
                var message = $"Network error when calling TpOms API: {ex.Message}";
                _logger.LogError(ex, "TpOms {OperationName} network error", operationName);
                return new TpOmsDto { success = false, message = message };
            }
            catch (Exception ex)
            {
                var message = $"Unexpected error when calling TpOms API: {ex.Message}";
                _logger.LogError(ex, "TpOms {OperationName} unexpected error", operationName);
                return new TpOmsDto { success = false, message = message };
            }
        }
    }
}