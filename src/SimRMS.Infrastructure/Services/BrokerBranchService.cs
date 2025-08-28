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
/// Title:       Market Stock Company Branch Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service provides methods for managing Market Stock Company Branch information
/// Creation:    13/Aug/2025
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


public class BrokerBranchService : IBrokerBranchService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateMstCoBrchRequest> _createValidator;
    private readonly IValidator<UpdateMstCoBrchRequest> _updateValidator;
    private readonly IValidator<DeleteMstCoBrchRequest> _deleteValidator;
    private readonly IValidator<AuthorizeMstCoBrchRequest> _authorizeValidator;
    private readonly IValidator<GetBranchWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<BrokerBranchService> _logger;

    public BrokerBranchService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateMstCoBrchRequest> createValidator,
        IValidator<UpdateMstCoBrchRequest> updateValidator,
        IValidator<DeleteMstCoBrchRequest> deleteValidator,
        IValidator<AuthorizeMstCoBrchRequest> authorizeValidator,
        IValidator<GetBranchWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<BrokerBranchService> logger)
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

    public async Task<PagedResult<MstCoBrchDto>> GetMstCoBrchListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, string? coCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged MstCoBrch list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = "CoBrchDesc",
                SortDirection = "ASC",
                CoCode = coCode,
                SearchTerm = searchTerm,
                isAuth = (byte)AuthTypeEnum.Approve,
                TotalCount = 0
            };

            _logger.LogDebug("Calling LB_SP_GetBrokerBranchList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, CoCode={CoCode}, SearchTerm={SearchTerm}, isAuth={IsAuth}",
                pageNumber, pageSize, coCode, searchTerm, 1);

            var result = await _repository.QueryPagedAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetBrokerBranchList",
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
            _logger.LogError(ex, "Invalid arguments for MstCoBrch list retrieval");
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstCoBrch list");
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstCoBrch list");
            throw new DomainException($"Failed to retrieve broker branch list: {ex.Message}");
        }
    }

    public async Task<MstCoBrchDto?> GetMstCoBrchByIdAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        try
        {
            var parameters = new
            {
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                statusCode = 0, // output param
                statusMsg = "" // output param
            };

            var result = await _repository.QuerySingleAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetMstCoBrch_ByBranchCode",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved branch: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for MstCoBrch retrieval: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new ValidationException($"Invalid parameters provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting MstCoBrch by ID: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to retrieve broker branch: {ex.Message}");
        }
    }

    public async Task<MstCoBrchDto> CreateMstCoBrchAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        string defaultCoCode = "073";
        _logger.LogInformation("Creating new MstCoBrch for company: {CoCode}", defaultCoCode);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        // ✅ Generate and capture the branch code
        var generatedBranchCode = await GenerateBranchCodeAsync(defaultCoCode, cancellationToken);

        try
        {
            var parameters = new
            {
                Action = (byte)ActionTypeEnum.INSERT,
                CoCode = defaultCoCode,
                CoBrchCode = generatedBranchCode, // ✅ Use the generated code
                CoBrchDesc = request.CoBrchDesc,
                CoBrchAddr = request.CoBrchAddr,
                CoBrchPhone = request.CoBrchPhone,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks,
                RowsAffected = 0
            };

            _logger.LogDebug("Calling LB_SP_CrudMstCoBrch with Action=1 (INSERT) for branch code: {BranchCode}", generatedBranchCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstCoBrch",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            //var insertedCode = result.GetOutputValue<string>("InsertedCode");

            _logger.LogDebug("Create operation completed. RowsAffected: {RowsAffected}",
                rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to create broker branch - no rows affected");
            }



            //var createdBranch = await GetMstCoBrchByIdAsync(request.CoCode, request.CoBrchCode, cancellationToken);
            //return createdBranch ?? throw new DomainException("Failed to retrieve created MstCoBrch");

            // Return manually constructed DTO since unauthorized records aren't returned by GetById SP
            MstCoBrchDto newBrokerBranch = new MstCoBrchDto
            {
                CoCode = defaultCoCode,
                CoBrchCode = generatedBranchCode,
                CoBrchDesc = request.CoBrchDesc,
                CoBrchAddr = request.CoBrchAddr,
                CoBrchPhone = request.CoBrchPhone,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.INSERT,
                IsAuth = (byte)AuthTypeEnum.UnAuthorize, // Initially unauthorized
                AuthLevel = (byte)AuthLevelEnum.Level1,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks
            };

            return newBrokerBranch;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating MstCoBrch for branch code: {BranchCode}", generatedBranchCode);
            throw new DomainException($"Failed to create broker branch: {ex.Message}");
        }
    }
    public async Task<MstCoBrchDto> UpdateMstCoBrchAsync(string coCode, string coBrchCode, UpdateMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

       // await ValidateBusinessRulesForUpdateAsync(coCode, coBrchCode, request, cancellationToken);

        try
        {
            var existingBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            if (existingBranch == null)
            {
                throw new DomainException($"Broker branch not found: {coCode}-{coBrchCode}");
            }

            var parameters = new
            {
                Action = (byte)ActionTypeEnum.UPDATE,
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                CoBrchDesc = request.CoBrchDesc,
                CoBrchAddr = request.CoBrchAddr,
                CoBrchPhone = request.CoBrchPhone,
                IPAddress =  _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.UPDATE,
                IsDel = (byte)DeleteStatusEnum.Active,
                Remarks = request.Remarks,
                RowsAffected = 0
            };

            _logger.LogDebug("Calling LB_SP_CrudMstCoBrch with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstCoBrch",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("Failed to update broker branch - no rows affected");
            }

            var updatedBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);

            if (updatedBranch == null)
            {
                throw new DomainException($"Updated branch not found: {coCode}-{coBrchCode}");
            }

            _logger.LogInformation("Successfully updated MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            return updatedBranch;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to update broker branch: {ex.Message}");
        }
    }

    public async Task<bool> DeleteMstCoBrchAsync(string coCode, string coBrchCode, DeleteMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        // Use existing validation method
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
            var existingBranch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            if (existingBranch == null)
            {
                throw new DomainException($"Broker branch not found: {coCode}-{coBrchCode}");
            }

            var parameters = new
            {
                Action = (byte)ActionTypeEnum.DELETE,
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                CoBrchDesc = (string?)null,
                CoBrchAddr = (string?)null,
                CoBrchPhone = (string?)null,
                IPAddress = _currentUserService.GetClientIPAddress(),
                MakerId = _currentUserService.UserId,
                ActionDt = DateTime.Now,
                TransDt = DateTime.Now.Date,
                ActionType = (byte)ActionTypeEnum.DELETE,
                IsDel = (byte)DeleteStatusEnum.Deleted,
                Remarks = request.Remarks,
                RowsAffected = 0,
                InsertedCode = string.Empty
            };

            _logger.LogDebug("Calling LB_SP_CrudMstCoBrch with Action=3 (DELETE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstCoBrch",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Delete operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            var success = rowsAffected > 0;

            if (success)
            {
                _logger.LogInformation("Successfully deleted MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            }
            else
            {
                _logger.LogWarning("Delete operation failed - no rows affected: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            }

            return success;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to delete broker branch: {ex.Message}");
        }
    }

    public async Task<bool> MstCoBrchExistsAsync(string coCode, string coBrchCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if MstCoBrch exists: {CoCode}-{CoBrchCode}", coCode, coBrchCode);

        try
        {
            var branch = await GetMstCoBrchByIdAsync(coCode, coBrchCode, cancellationToken);
            return branch != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking MstCoBrch existence: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<MstCoBrchDto>> GetBranchUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       string? coCode = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetBranchWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            CoCode = coCode,
            IsAuth = isAuth
        };

        // Validate the request using FluentValidation
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Branch list for workflow - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize, authAction);

        try
        {
            // FIXED: Proper parameter setup for OUTPUT parameter handling
            var parameters = new Dictionary<string, object>
            {
                ["PageNumber"] = pageNumber,
                ["PageSize"] = pageSize,
                ["SortColumn"] = "CoCode",
                ["SortDirection"] = "ASC",
                ["CoCode"] = coCode ?? (object)DBNull.Value,
                ["SearchTerm"] = searchTerm ?? (object)DBNull.Value,
                ["isAuth"] = (byte)isAuth, // 0: Unauthorized or 2: Denied records
                ["MakerId"] = _currentUserService.UserId,
                ["TotalCount"] = 0 // OUTPUT parameter - will be populated by SP
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetBrokerBranchListWF", pageNumber, pageSize, 0, _currentUserService.UserId);

            // FIXED: The GenericRepository now properly handles OUTPUT parameters in a single call
            var result = await _repository.QueryPagedAsync<MstCoBrchDto>(
                sqlOrSp: "LB_SP_GetBrokerBranchListWF",
                pageNumber: pageNumber,
                pageSize: pageSize,
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            if (result.TotalCount == 0)
            {
                _logger.LogWarning("SP {StoredProcedure} returned TotalCount=0. Check OUTPUT parameter handling or SP logic.",
                    "LB_SP_GetBrokerBranchListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting "+authAction+" broker branch list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, CoCode={CoCode}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, coCode, searchTerm);
            throw;
        }
    }


    public async Task<bool> AuthorizeBranchAsync(string coCode, string coBrchCode, AuthorizeMstCoBrchRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} MstCoBrch in workflow: {CoCode}-{CoBrchCode}", coCode, coBrchCode, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            var parameters = new
            {
                Action = request.ActionType, // Default is 2 for authorization
                CoCode = coCode,
                CoBrchCode = coBrchCode,
                IPAddress = ipAddress,
                AuthID = _currentUserService.UserId,
                IsAuth = request.IsAuth,
                ActionType = request.ActionType,
                Remarks = request.Remarks,
                RowsAffected = 0 // OUTPUT parameter
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthMstCoBrch",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized MstCoBrch: {CoCode}-{CoBrchCode}, RowsAffected: {RowsAffected}",
                    coCode, coBrchCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("No rows affected during {authAction} authorization of MstCoBrch: {CoCode}-{CoBrchCode}", coCode, coBrchCode, authAction);
                throw new DomainException($"Failed to authorize:{authAction} branch: No records were updated");
            }
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error {authAction} authorizing MstCoBrch in workflow: {CoCode}-{CoBrchCode}", coCode, coBrchCode, authAction);
            throw new DomainException($"Failed to authorize:{authAction} branch: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    // FIXED: Using your existing ValidationErrorDetail from Domain.Common
    private async Task ValidateCreateRequestAsync(CreateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Create validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateUpdateRequestAsync(UpdateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);

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

    private async Task ValidateDeleteRequestAsync(DeleteMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _deleteValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Delete validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _authorizeValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Authorization validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetBranchWorkflowListRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _workflowListValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("Workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion

    #region Helper Method
    private async Task ValidateBusinessRulesForUpdateAsync(string coCode, string coBrchCode, UpdateMstCoBrchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.CoBrchDesc))
            {
                var sql = @"SELECT COUNT(1) FROM MstCoBrch 
                           WHERE CoBrchDesc = @CoBrchDesc 
                           AND CoCode = @CoCode 
                           AND CoBrchCode != @CoBrchCode 
                           AND IsDel = 0";
                var count = await _repository.ExecuteScalarAsync<int>(
                    sql,
                    new { CoBrchDesc = request.CoBrchDesc, CoCode = coCode, CoBrchCode = coBrchCode },
                    cancellationToken: cancellationToken);

                if (count > 0)
                    throw new DomainException($"Branch with description '{request.CoBrchDesc}' already exists for company '{coCode}'");
            }
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating business rules for branch update: {CoCode}-{CoBrchCode}", coCode, coBrchCode);
            throw new DomainException($"Failed to validate business rules: {ex.Message}");
        }
    }

    private async Task<string> GenerateBranchCodeAsync(string coCode, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(coCode))
            {
                throw new DomainException("Company code cannot be null or empty");
            }

            var sql = @"DECLARE @CoCode varchar(10) = @InpCoCode
                    DECLARE @NextCode INT
                    ,@GENERATEDCODE VARCHAR(6)
                    SELECT @NextCode = ISNULL(MAX(CAST([CoBrchCode] AS INT)), 0) + 1
                    FROM [dbo].[MstCoBrch] WITH (TABLOCKX)
                    WHERE [CoCode] = @CoCode
                    SET @GENERATEDCODE=RIGHT('000' + CAST(@NextCode AS VARCHAR(3)), 3);
                    SELECT @GENERATEDCODE as NewBranchCode";

            var branchCode = await _repository.ExecuteScalarAsync<string>(
                sql,
                new { InpCoCode = coCode },
                cancellationToken: cancellationToken);

            if (string.IsNullOrEmpty(branchCode))
            {
                throw new DomainException($"Branch code generation failed for company code: '{coCode}'");
            }

            _logger.LogDebug("Generated branch code: {BranchCode} for company: {CoCode}", branchCode, coCode);
            return branchCode; // ✅ RETURN the generated code
        }
        catch (DomainException)
        {
            throw; // Re-throw domain exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate branch code for company: {CoCode}", coCode);
            throw new DomainException($"Failed to generate branch code: {ex.Message}");
        }
    }
    #endregion
}