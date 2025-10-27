using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Infrastructure.Interfaces.Common;
using SimRMS.Shared.Models;
using SimRMS.Application.Exceptions;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Constants;
using SimRMS.Application.Common;
using ValidationException = SimRMS.Application.Exceptions.ValidationException;
using LB.DAL.Core.Common;
using System.Data;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service provides methods for managing Company information (Read, Update, Authorization)
/// Creation:    28/Aug/2025
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

public class CompanyService : ICompanyService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateCompanyRequest> _updateValidator;
    private readonly IValidator<AuthorizeCompanyRequest> _authorizeValidator;
    private readonly IValidator<GetCompanyWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateCompanyRequest> updateValidator,
        IValidator<AuthorizeCompanyRequest> authorizeValidator,
        IValidator<GetCompanyWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<CompanyService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<CompanyDto>> GetCompanyListAsync(int pageNumber = 1, int pageSize = 10,
        string? searchTerm = null, string? coCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving paged Company list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Validate and sanitize inputs
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        try
        {
            List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("PageNumber", pageNumber),
                new LB_DALParam("PageSize", pageSize),
                new LB_DALParam("SortColumn", "CoDesc"),
                new LB_DALParam("SortDirection", "ASC"),
    
                // Optional/Filter Parameters
                new LB_DALParam("SearchTerm", searchTerm ?? (object)DBNull.Value),
                new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value)
            };

            _logger.LogDebug("Calling LB_SP_GetCompanyList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<CompanyDto>(
                sqlOrSp: "LB_SP_GetCompanyList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Company listretrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Company list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Company list");
            throw new DomainException($"The company list retrieval failed: {ex.Message}");
        }
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(string coCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving Company by ID: {CoCode}", coCode);

        try
        {

            List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameter
                new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value),

                // Output Parameters
                new LB_DALParam("statusCode", 0, ParameterDirection.Output),
                new LB_DALParam("statusMsg", string.Empty, ParameterDirection.Output)
            };

            var result = await _repository.QuerySingleAsync<CompanyDto>(
                sqlOrSp: "LB_SP_GetMstCo_ByCompanyCode",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved company: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Company retrieval: {CoCode}", coCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Company by ID: {CoCode}", coCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Company by ID: {CoCode}", coCode);
            throw new DomainException($"The company retrieval failed: {ex.Message}");
        }
    }

    public async Task<CompanyDto> UpdateCompanyAsync(string coCode, UpdateCompanyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Company: {CoCode}", coCode);

        // Use existing validation method
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
            var existingCompany = await GetCompanyByIdAsync(coCode, cancellationToken);
            if (existingCompany == null)
            {
                throw new DomainException($"The Company was not found: {coCode}");
            }

            List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action/Key Parameters
                new LB_DALParam("Action", (byte)ActionTypeEnum.UPDATE),
                new LB_DALParam("CoCode", coCode), // Input Key
    
                // Data/Update Parameters
                new LB_DALParam("CoDesc", request.CoDesc ?? (object)DBNull.Value),
                new LB_DALParam("EnableExchangeWideSellProceed", request.EnableExchangeWideSellProceed),
    
                // Audit/Control Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
                new LB_DALParam("MakerId", _currentUserService.UserId),
                new LB_DALParam("ActionDt", DateTime.Now),
                new LB_DALParam("TransDt", DateTime.Now.Date),
                new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),
                new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
    
                // Optional Remarks Parameter
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

            _logger.LogDebug("Calling LB_SP_CrudMstCo with Action=2 (UPDATE)");

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstCo",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            _logger.LogDebug("Update operation completed. RowsAffected: {RowsAffected}", rowsAffected);

            if (rowsAffected <= 0)
            {
                throw new DomainException("The company update failed because no rows were affected");
            }

            var updatedCompany = await GetCompanyByIdAsync(coCode, cancellationToken);

            if (updatedCompany == null)
            {
                throw new DomainException($"The Updated company was not found: {coCode}");
            }

            _logger.LogInformation("Successfully updated Company: {CoCode}", coCode);
            return updatedCompany;
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
            _logger.LogError(ex, "An unexpected error occurred while updating Company: {CoCode}", coCode);
            throw new DomainException($"The company update failed: {ex.Message}");
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<CompanyDto>> GetCompanyUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       string? coCode = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetCompanyWorkflowListRequest
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

        _logger.LogInformation("Getting {authAction} Company list for workflow - Page: {PageNumber}, Size: {PageSize}", authAction, pageNumber, pageSize);

        try
        {
            List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination and Sorting Parameters
                new LB_DALParam("PageNumber", pageNumber),
                new LB_DALParam("PageSize", pageSize),
                new LB_DALParam("SortColumn", "CoDesc"),
                new LB_DALParam("SortDirection", "ASC"),

                // Filter Parameters (already includes DBNull handling from the source Dictionary)
                new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value),
                new LB_DALParam("SearchTerm", searchTerm ?? (object)DBNull.Value),

                // Control Parameters
                new LB_DALParam("isAuth", (byte)isAuth),
                new LB_DALParam("MakerId", _currentUserService.UserId)
            };

            _logger.LogDebug("Calling SP {StoredProcedure} with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                "LB_SP_GetCompanyListWF", pageNumber, pageSize, isAuth, _currentUserService.UserId);

            // FIXED: The GenericRepository now properly handles OUTPUT parameters in a single call
            var result = await _repository.QueryPagedAsync<CompanyDto>(
                sqlOrSp: "LB_SP_GetCompanyListWF",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            if (result.TotalCount == 0)
            {
                _logger.LogWarning("SP {StoredProcedure} returned TotalCount=0. Check OUTPUT parameter handling or SP logic.",
                    "LB_SP_GetCompanyListWF");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting "+authAction+" company list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, CoCode={CoCode}, SearchTerm={SearchTerm}",
                pageNumber, pageSize, coCode, searchTerm);
            throw;
        }
    }

    public async Task<bool> AuthorizeCompanyAsync(string coCode, AuthorizeCompanyRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing Company in workflow: companyCode: {CoCode}, AuthType: {authAction}", coCode, authAction);

        // Validate request
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var ipAddress = _currentUserService.GetClientIPAddress();

            List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("Action", request.ActionType), 
                new LB_DALParam("CoCode", coCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("IPAddress", ipAddress ?? (object)DBNull.Value),
                new LB_DALParam("AuthID", _currentUserService.UserId),
                new LB_DALParam("IsAuth", request.IsAuth),
                new LB_DALParam("ActionType", request.ActionType),
    
                // Remarks handling the same logic as the source dictionary
                new LB_DALParam("Remarks", !string.IsNullOrEmpty(request.Remarks) ? (object)request.Remarks : DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthCompany",
                parameters,
                cancellationToken: cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected > 0)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully authorized Company: {CoCode}, RowsAffected: {RowsAffected}",
                    coCode, rowsAffected);
                return true;
            }
            else
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("Authorization failed for Company: {CoCode}", coCode);
                throw new DomainException( $"The company authorization failed: No records were updated");
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
            _logger.LogError(ex, "An error occurred while authorizing Company: {CoCode}", coCode);
            throw new DomainException($"The company authorization failed: {ex.Message}");
        }
    }

    #endregion

    #region Private Validation Methods

    private async Task ValidateUpdateRequestAsync(UpdateCompanyRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Company update validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeCompanyRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Company authorization validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetCompanyWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Company workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}