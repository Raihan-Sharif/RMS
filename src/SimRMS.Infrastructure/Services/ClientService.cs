using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Domain.Exceptions;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Domain.Common;
using ValidationException = SimRMS.Domain.Exceptions.ValidationException;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Client information
/// Creation:    16/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateClientRequest> _createValidator;
    private readonly IValidator<UpdateClientRequest> _updateValidator;
    private readonly IValidator<DeleteClientRequest> _deleteValidator;
    private readonly IValidator<AuthorizeClientRequest> _authorizeValidator;
    private readonly IValidator<GetClientWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ClientService> _logger;

    public ClientService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateClientRequest> createValidator,
        IValidator<UpdateClientRequest> updateValidator,
        IValidator<DeleteClientRequest> deleteValidator,
        IValidator<AuthorizeClientRequest> authorizeValidator,
        IValidator<GetClientWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<ClientService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<ClientDto>> GetClientListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged Client list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                GCIF = gcif,
                ClntName = clntName,
                ClntCode = clntCode,
                SortBy = "GCIF",
                SortOrder = "ASC",
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetClientMstAccList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, GCIF={GCIF}, ClntName={ClntName}, ClntCode={ClntCode}",
                pageNumber, pageSize, gcif, clntName, clntCode);

            var result = await _repository.QueryPagedAsync<ClientDto>(
                sqlOrSp: "LB_SP_GetClientMstAccList",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Client list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Client list");
            throw new DomainException($"Failed to retrieve client list: {ex.Message}");
        }
    }

    public async Task<ClientDto?> GetClientByIdAsync(string gcif, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Client by GCIF: {GCIF}", gcif);

        try
        {
            var parameters = new
            {
                GCIF = gcif,
                StatusCode = 0, // output param
                StatusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<ClientDto>(
                sqlOrSp: "LB_SP_GetClientMstAccByClientID",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved client: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client retrieval: {GCIF}", gcif);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Client: {GCIF}", gcif);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Client by GCIF: {GCIF}", gcif);
            throw new DomainException($"Failed to retrieve client: {ex.Message}");
        }
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new Client with ClntCode: {ClntCode}", request.ClntCode);

        // Validate the request
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for creating Client: {Errors}", string.Join(", ", validationResult.Errors));
            throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var parameters = new
            {
                Action = (int)ActionTypeEnum.INSERT,
                // ClntMaster parameters (GCIF will be auto-generated by SP)
                GCIF = (string?)null,
                ClntCode = request.ClntCode,
                ClntName = request.ClntName,
                ClntNICNo = request.ClntNICNo,
                ClntAddr = request.ClntAddr,
                ClntPhone = request.ClntPhone,
                ClntMobile = request.ClntMobile,
                Gender = request.Gender,
                Nationality = request.Nationality,
                ClntOffice = request.ClntOffice,
                ClntFax = request.ClntFax,
                ClntEmail = request.ClntEmail,
                CountryCode = request.CountryCode,
                // ClntAcct parameters
                CoBrchCode = request.CoBrchCode,
                ClntStat = request.ClntStat,
                ClntTrdgStat = request.ClntTrdgStat,
                ClntAcctType = request.ClntAcctType,
                ClntCDSNo = request.ClntCDSNo,
                ClntDlrCode = request.ClntDlrCode,
                ClntAllowAssociate = request.ClntAllowAssociate,
                ClntDlrReassign = request.ClntDlrReassign,
                ClntReassignDlrCode = request.ClntReassignDlrCode,
                ClientCommission = request.ClientCommission,
                AllowSME = request.AllowSME,
                // Common parameters
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.INSERT,
                AuthId = _currentUserService.UserId,
                AuthDt = DateTime.Now,
                AuthTransDt = DateTime.Today,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize,
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks,
                // Output parameters
                RowsAffected = 0,
                AffectedRowsClntMaster = 0,
                AffectedRowsClntAcct = 0,
                TotalAffectedRows = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudClientMstAcc",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            if (rowsAffected <= 0)
            {
                _logger.LogWarning("Client creation failed - No rows affected for ClntCode: {ClntCode}", request.ClntCode);
                throw new DomainException("Failed to create client - no rows affected");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Successfully created Client with ClntCode: {ClntCode}", request.ClntCode);

            string gcifId = await GetGCIFByClientCodeAsync(request.CoBrchCode,request.ClntCode);

            var createdClient = await GetClientByIdAsync(gcifId);

            return createdClient ?? throw new DomainException("Failed to retrieve created client");
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client creation");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error creating Client");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating Client");
            throw new DomainException($"Failed to create client: {ex.Message}");
        }
    }

    public async Task<ClientDto> UpdateClientAsync(string gcif, UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Client with GCIF: {GCIF}", gcif);

        // Validate the request
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for updating Client: {Errors}", string.Join(", ", validationResult.Errors));
            throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Check if client exists
        var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
        if (existingClient == null)
        {
            _logger.LogWarning("Client with GCIF {GCIF} not found for update", gcif);
            throw new NotFoundException("Client", gcif);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                // ClntMaster parameters
                GCIF = gcif,
                ClntCode = request.ClntCode,
                ClntName = request.ClntName,
                ClntNICNo = request.ClntNICNo,
                ClntAddr = request.ClntAddr,
                ClntPhone = request.ClntPhone,
                ClntMobile = request.ClntMobile,
                Gender = request.Gender,
                Nationality = request.Nationality,
                ClntOffice = request.ClntOffice,
                ClntFax = request.ClntFax,
                ClntEmail = request.ClntEmail,
                CountryCode = request.CountryCode,
                // ClntAcct parameters
                CoBrchCode = request.CoBrchCode,
                ClntStat = request.ClntStat,
                ClntTrdgStat = request.ClntTrdgStat,
                ClntAcctType = request.ClntAcctType,
                ClntCDSNo = request.ClntCDSNo,
                ClntDlrCode = request.ClntDlrCode,
                ClntAllowAssociate = request.ClntAllowAssociate,
                ClntDlrReassign = request.ClntDlrReassign,
                ClntReassignDlrCode = request.ClntReassignDlrCode,
                ClientCommission = request.ClientCommission,
                AllowSME = request.AllowSME,
                // Common parameters
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                AuthId = _currentUserService.UserId,
                AuthDt = DateTime.Now,
                AuthTransDt = DateTime.Today,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize,
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks,
                // Output parameters
                RowsAffected = 0,
                AffectedRowsClntMaster = 0,
                AffectedRowsClntAcct = 0,
                TotalAffectedRows = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudClientMstAcc",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            if (rowsAffected <= 0)
            {
                _logger.LogWarning("Client update failed - No rows affected for GCIF: {GCIF}", gcif);
                throw new DomainException("Failed to update client - no rows affected");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Successfully updated Client with GCIF: {GCIF}", gcif);

            // Return the updated client
            var updatedClient = await GetClientByIdAsync(gcif, cancellationToken);
            return updatedClient ?? throw new DomainException("Failed to retrieve updated client");
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client update");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error updating Client");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating Client");
            throw new DomainException($"Failed to update client: {ex.Message}");
        }
    }

    public async Task<bool> DeleteClientAsync(string gcif, DeleteClientRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Client with GCIF: {GCIF}", gcif);

        // Validate the request
        var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for deleting Client: {Errors}", string.Join(", ", validationResult.Errors));
            throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Check if client exists
        var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
        if (existingClient == null)
        {
            _logger.LogWarning("Client with GCIF {GCIF} not found for deletion", gcif);
            throw new NotFoundException("Client", gcif);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var parameters = new
            {
                Action = (int)ActionTypeEnum.DELETE,
                GCIF = gcif,
                ClntCode = request.ClntCode,
                CoBrchCode = request.CoBrchCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Today,
                ActionType = (byte)ActionTypeEnum.DELETE,
                Remarks = request.Remarks ?? "Soft deleted",
                // Output parameters
                RowsAffected = 0,
                AffectedRowsClntMaster = 0,
                AffectedRowsClntAcct = 0,
                TotalAffectedRows = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudClientMstAcc",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            if (rowsAffected <= 0)
            {
                _logger.LogWarning("Client deletion failed - No rows affected for GCIF: {GCIF}", gcif);
                throw new DomainException("Failed to delete client - no rows affected");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted Client with GCIF: {GCIF}", gcif);
            return true;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client deletion");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error deleting Client");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting Client");
            throw new DomainException($"Failed to delete client: {ex.Message}");
        }
    }

    public async Task<bool> ClientExistsAsync(string gcif, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if Client exists with GCIF: {GCIF}", gcif);

        var client = await GetClientByIdAsync(gcif, cancellationToken);
        return client != null;
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<ClientDto>> GetClientUnAuthDeniedListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, string? gcif = null, string? clntName = null, string? clntCode = null, int isAuth = 0, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged Client workflow list - Page: {PageNumber}, Size: {PageSize}, IsAuth: {IsAuth}", pageNumber, pageSize, isAuth);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                GCIF = gcif,
                ClntName = clntName,
                ClntCode = clntCode,
                MakerId = _currentUserService.UserId,
                isAuth = (byte)isAuth,
                SortBy = "GCIF",
                SortOrder = "ASC",
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetClientMstAccListWF with parameters: PageNumber={PageNumber}, PageSize={PageSize}, IsAuth={IsAuth}",
                pageNumber, pageSize, isAuth);

            var result = await _repository.QueryPagedAsync<ClientDto>(
                sqlOrSp: "LB_SP_GetClientMstAccListWF",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Workflow SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client workflow list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting Client workflow list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting Client workflow list");
            throw new DomainException($"Failed to retrieve client workflow list: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeClientAsync(string gcif, AuthorizeClientRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Authorizing Client with GCIF: {GCIF}, IsAuth: {IsAuth}", gcif, request.IsAuth);

        // Validate the request
        var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for authorizing Client: {Errors}", string.Join(", ", validationResult.Errors));
            throw new ValidationException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Check if client exists
        var existingClient = await GetClientByIdAsync(gcif, cancellationToken);
        if (existingClient == null)
        {
            _logger.LogWarning("Client with GCIF {GCIF} not found for authorization", gcif);
            throw new NotFoundException("Client", gcif);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var parameters = new
            {
                Action = (int)ActionTypeEnum.UPDATE,
                GCIF = gcif,
                ClntCode = request.ClntCode,
                CoBrchCode = request.CoBrchCode,
                IPAddress = _currentUserService.GetClientIPAddress(),
                AuthID = _currentUserService.UserId,
                IsAuth = request.IsAuth,
                Remarks = request.Remarks,
                ActionType = request.ActionType,
                RowsAffected = 0
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthClientMstAcc",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            if (rowsAffected <= 0)
            {
                _logger.LogWarning("Client authorization failed - No rows affected for GCIF: {GCIF}", gcif);
                throw new DomainException("Failed to authorize client - no rows affected");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Successfully authorized Client with GCIF: {GCIF}", gcif);
            return true;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for Client authorization");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error authorizing Client");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error authorizing Client");
            throw new DomainException($"Failed to authorize client: {ex.Message}");
        }
    }

    #endregion

    #region Private Method
    public async Task<string> GetGCIFByClientCodeAsync(string branchCode, string clientCode, CancellationToken cancellationToken = default)
    {
        var sql = @"
        SELECT CM.GCIF
        FROM CLNTMASTER CM
        INNER JOIN CLNTACCT CA ON CM.GCIF = CA.GCIF
        WHERE 1=1";

        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(branchCode))
        {
            sql += " AND CA.COBRCHCODE = @BranchCode";
            parameters.Add("BranchCode", branchCode);
        }

        if (!string.IsNullOrWhiteSpace(clientCode))
        {
            sql += " AND CA.CLNTCODE = @ClientCode";
            parameters.Add("ClientCode", clientCode);
        }

        try
        {
            var gcifResult = await _repository.QuerySingleAsync<string>(sql, parameters, false, cancellationToken);

            return gcifResult ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GCIF by client code. BranchCode: {BranchCode}, ClientCode: {ClientCode}", branchCode, clientCode);
            throw;
        }
    }
    #endregion
}