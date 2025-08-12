using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Common;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Shared.Constants;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service provides methods for managing Market Stock Company information
/// Creation:    12/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

/// <summary>
/// Market Stock Company service with business logic and validation
/// </summary>
public class MstCoService : IMstCoService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMstCoRequest> _validator;
    private readonly ILogger<MstCoService> _logger;

    public MstCoService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMstCoRequest> validator,
        ILogger<MstCoService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PagedResult<MstCoDto>> GetMstCoListAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? coCode = null, string? coDesc = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged MstCo list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Input validation
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var parameters = new
        {
            SearchTerm = searchTerm,
            CoCode = coCode,
            CoDesc = coDesc
        };

        // Use stored procedure for pagination with total count
        var result = await _repository.QueryPagedAsync<MstCoDto>(
            sqlOrSp: "LBS_SP_GetMstCoList",
            pageNumber: pageNumber,
            pageSize: pageSize,
            parameters: parameters,
            isStoredProcedure: true,
            cancellationToken: cancellationToken);

        return result;
    }

    public async Task<MstCoDto?> GetMstCoByIdAsync(string coCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            throw new ArgumentException("Company code cannot be null or empty", nameof(coCode));

        _logger.LogInformation("Getting MstCo by ID: {CoCode}", coCode);

        var parameters = new { CoCode = coCode };
        
        return await _repository.QuerySingleAsync<MstCoDto>(
            sqlOrSp: "LBS_SP_GetMstCoById",
            parameters: parameters,
            isStoredProcedure: true,
            cancellationToken: cancellationToken);
    }

    public async Task<MstCoDto> UpdateMstCoAsync(string coCode, UpdateMstCoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            throw new ArgumentException("Company code cannot be null or empty", nameof(coCode));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Updating MstCo: {CoCode}", coCode);

        // Validation
        await ValidateUpdateRequestAsync(request, cancellationToken);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Check if company exists
            var existingCompany = await GetMstCoByIdAsync(coCode, cancellationToken);
            if (existingCompany == null)
                throw new NotFoundException("MstCo", coCode);

            // Business validation
            await ValidateBusinessRulesAsync(coCode, request, cancellationToken);

            // Update using stored procedure
            var parameters = new
            {
                CoCode = coCode,
                CoDesc = request.CoDesc,
                EnableExchangeWideSellProceed = request.EnableExchangeWideSellProceed,
                IPAddress = GetClientIPAddress(),
                MakerId = GetCurrentUserId(),
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                WFName = WorkflowEnum.MSTCO
            };

            var rowsAffected = await _repository.ExecuteAsync(
                sqlOrSp: "LBS_SP_UpdateMstCo",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            if (rowsAffected == 0)
                throw new DomainException($"Failed to update MstCo with code '{coCode}'");

            // Return updated company
            var updatedCompany = await GetMstCoByIdAsync(coCode, cancellationToken);
            return updatedCompany ?? throw new DomainException("Failed to retrieve updated MstCo");
        }, cancellationToken);
    }

    public async Task<bool> MstCoExistsAsync(string coCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(coCode))
            return false;

        var sql = "SELECT COUNT(1) FROM MstCo WHERE CoCode = @CoCode AND IsDel = 0";
        var count = await _repository.ExecuteScalarAsync<int>(sql, new { CoCode = coCode }, cancellationToken: cancellationToken);
        return count > 0;
    }

    #region Private Helper Methods

    private async Task ValidateUpdateRequestAsync(UpdateMstCoRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();
            
            throw new ValidationException("Update validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateBusinessRulesAsync(string coCode, UpdateMstCoRequest request, CancellationToken cancellationToken)
    {
        // Add any business-specific validation rules here

        //check if CoDesc is unique if it's being updated
        if (!string.IsNullOrEmpty(request.CoDesc))
        {
            var sql = @"SELECT COUNT(1) FROM MstCo 
                       WHERE CoDesc = @CoDesc 
                       AND CoCode != @CoCode 
                       AND IsDel = 0";
            var count = await _repository.ExecuteScalarAsync<int>(
                sql, 
                new { CoDesc = request.CoDesc, CoCode = coCode }, 
                cancellationToken: cancellationToken);
            
            if (count > 0)
                throw new DomainException($"Company with description '{request.CoDesc}' already exists");
        }
    }

    private string GetClientIPAddress()
    {
        // This should be injected from current user service or HTTP context
        // For now, returning a placeholder
        return "127.0.0.1";
    }

    private int GetCurrentUserId()
    {
        // This should be injected from current user service
        // For now, returning a placeholder
        return 1;
    }

    #endregion
}