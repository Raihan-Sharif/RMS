using FluentValidation;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Application.Models.Requests;
using SimRMS.Application.Common;
using SimRMS.Application.Exceptions;
using SimRMS.Infrastructure.Interfaces.Common;
using SimRMS.Shared.Constants;
using SimRMS.Shared.Models;
using ValidationException = SimRMS.Application.Exceptions.ValidationException;
using LB.DAL.Core.Common;
using System.Data;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Service
/// Author:      Raihan Sharif
/// Purpose:     This service provides methods for managing Stock information
/// Creation:    23/Sep/2025
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

public class StockService : IStockService
{
    private readonly IGenericRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetStockByKeyRequest> _getByKeyValidator;
    private readonly IValidator<GetStockListRequest> _getListValidator;
    private readonly IValidator<CreateStockRequest> _createValidator;
    private readonly IValidator<UpdateStockRequest> _updateValidator;
    private readonly IValidator<DeleteStockRequest> _deleteValidator;
    private readonly IValidator<AuthorizeStockRequest> _authorizeValidator;
    private readonly IValidator<GetStockWorkflowListRequest> _workflowListValidator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StockService> _logger;

    public StockService(
        IGenericRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<GetStockByKeyRequest> getByKeyValidator,
        IValidator<GetStockListRequest> getListValidator,
        IValidator<CreateStockRequest> createValidator,
        IValidator<UpdateStockRequest> updateValidator,
        IValidator<DeleteStockRequest> deleteValidator,
        IValidator<AuthorizeStockRequest> authorizeValidator,
        IValidator<GetStockWorkflowListRequest> workflowListValidator,
        ICurrentUserService currentUserService,
        ILogger<StockService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _getByKeyValidator = getByKeyValidator;
        _getListValidator = getListValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _deleteValidator = deleteValidator;
        _authorizeValidator = authorizeValidator;
        _workflowListValidator = workflowListValidator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Main CRUD Operations

    public async Task<PagedResult<StockDto>> GetStockListAsync(int pageNumber = 1, int pageSize = 10,
        string? xchgCode = null, string? stkCode = null, string? searchTerm = null,
        string sortColumn = "StkCode", string sortDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving paged Stock list - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // Create request model for validation
        var request = new GetStockListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            XchgCode = xchgCode,
            StkCode = stkCode,
            SearchText = searchTerm,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        // Validate the request
        await ValidateGetListRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Filter Parameters (assuming these can be null/optional)
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value),
	            new LB_DALParam("StkCode", stkCode ?? (object)DBNull.Value),
	            new LB_DALParam("SearchText", searchTerm ?? (object)DBNull.Value),

                // Sorting Parameters (assuming these can be null/optional)
                new LB_DALParam("SortColumn", sortColumn ?? (object)DBNull.Value),
	            new LB_DALParam("SortDirection", sortDirection ?? (object)DBNull.Value)
            };

			_logger.LogDebug("Calling LB_SP_GetMstStkList with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchText={SearchText}",
                pageNumber, pageSize, searchTerm);

            var result = await _repository.QueryPagedAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkList",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Stock listretrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Stock list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Stock list");
            throw new DomainException($"The stock list retrieval failed: {ex.Message}");
        }
    }

    public async Task<StockDto?> GetStockByKeyAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Create request model for validation
        var request = new GetStockByKeyRequest
        {
            XchgCode = xchgCode,
            StkCode = stkCode
        };

        // Validate the request
        await ValidateGetByKeyRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Input Parameters
                new LB_DALParam("XchgCode", xchgCode ?? (object)DBNull.Value), // Added null check for safety
                new LB_DALParam("StkCode", stkCode ?? (object)DBNull.Value),   // Added null check for safety

                // Output Parameters
                new LB_DALParam("StatusCode", 0, ParameterDirection.Output),
	            new LB_DALParam("StatusMsg", string.Empty, ParameterDirection.Output)
            };

			var result = await _repository.QuerySingleAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkByKey",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Retrieved stock: {Found}", result != null ? "Found" : "Not Found");
            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments were provided for Stock retrieval: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation encountered an error while retrieving Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving Stock by key: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"The stock retrieval failed: {ex.Message}");
        }
    }

    public async Task<StockDto> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Stock: {XchgCode}-{StkCode}", request.XchgCode, request.StkCode);

        // Validate the request
        await ValidateCreateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.INSERT),
	            new LB_DALParam("XchgCode", request.XchgCode),
	            new LB_DALParam("StkCode", request.StkCode),

                // Data Parameters (includes explicit null checks from source)
                new LB_DALParam("StkBrdCode", request.StkBrdCode ?? (object)DBNull.Value),
	            new LB_DALParam("StkSectCode", request.StkSectCode ?? (object)DBNull.Value),
	            new LB_DALParam("StkLName", request.StkLName),
	            new LB_DALParam("StkSName", request.StkSName),
	            new LB_DALParam("StkLastDonePrice", request.StkLastDonePrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkClosePrice", request.StkClosePrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkRefPrc", request.StkRefPrc ?? (object)DBNull.Value),
	            new LB_DALParam("StkUpperLmtPrice", request.StkUpperLmtPrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkLowerLmtPrice", request.StkLowerLmtPrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkIsSyariah", request.StkIsSyariah ?? (object)DBNull.Value),
	            new LB_DALParam("StkLot", request.StkLot),
	            new LB_DALParam("LastUpdateDate", request.LastUpdateDate ?? (object)DBNull.Value),
	            new LB_DALParam("ISIN", request.ISIN),
	            new LB_DALParam("Currency", request.Currency ?? (object)DBNull.Value),
	            new LB_DALParam("StkParValue", request.StkParValue ?? (object)DBNull.Value),
	            new LB_DALParam("StkVolumeTraded", request.StkVolumeTraded ?? (object)DBNull.Value),
	            new LB_DALParam("YearHigh", request.YearHigh ?? (object)DBNull.Value),
	            new LB_DALParam("YearLow", request.YearLow ?? (object)DBNull.Value),
	            new LB_DALParam("SecurityType", request.SecurityType ?? (object)DBNull.Value),
	            new LB_DALParam("ListingDate", request.ListingDate ?? (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", (object)DBNull.Value),
	            new LB_DALParam("TransDt", (object)DBNull.Value),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.INSERT),

                // Authorization/Workflow Parameters
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Always start unauthorized
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_CrudMstStk for INSERT with XchgCode={XchgCode}, StkCode={StkCode}",
                request.XchgCode, request.StkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to create stock - no rows affected. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock created successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            var createdStock = await GetStockByKeyAsync(request.XchgCode, request.StkCode, cancellationToken);
            if (createdStock == null)
            {
                throw new DomainException($"The Created stock was not found: {request.XchgCode}-{request.StkCode}");
            }

            return createdStock;
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
            _logger.LogError(ex, "An unexpected error occurred while creating Stock: {XchgCode}-{StkCode}", request.XchgCode, request.StkCode);
            throw new DomainException($"The stock creation failed: {ex.Message}");
        }
    }

    public async Task<StockDto> UpdateStockAsync(string xchgCode, string stkCode, UpdateStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Validate the request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateUpdateRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE),
	            new LB_DALParam("XchgCode", xchgCode),
	            new LB_DALParam("StkCode", stkCode),

                new LB_DALParam("StkBrdCode", request.StkBrdCode ?? (object)DBNull.Value),
	            new LB_DALParam("StkSectCode", request.StkSectCode ?? (object)DBNull.Value),
	            new LB_DALParam("StkLName", request.StkLName ?? (object)DBNull.Value),
	            new LB_DALParam("StkSName", request.StkSName ?? (object)DBNull.Value),
	            new LB_DALParam("StkLastDonePrice", request.StkLastDonePrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkClosePrice", request.StkClosePrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkRefPrc", request.StkRefPrc ?? (object)DBNull.Value),
	            new LB_DALParam("StkUpperLmtPrice", request.StkUpperLmtPrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkLowerLmtPrice", request.StkLowerLmtPrice ?? (object)DBNull.Value),
	            new LB_DALParam("StkIsSyariah", request.StkIsSyariah ?? (object)DBNull.Value),
	            new LB_DALParam("StkLot", request.StkLot ?? (object)DBNull.Value),
	            new LB_DALParam("LastUpdateDate", (object)DBNull.Value), // Explicitly DBNull in source
                new LB_DALParam("ISIN", request.ISIN ?? (object)DBNull.Value),
	            new LB_DALParam("Currency", request.Currency ?? (object)DBNull.Value),
	            new LB_DALParam("StkParValue", request.StkParValue ?? (object)DBNull.Value),
	            new LB_DALParam("StkVolumeTraded", request.StkVolumeTraded ?? (object)DBNull.Value),
	            new LB_DALParam("YearHigh", request.YearHigh ?? (object)DBNull.Value),
	            new LB_DALParam("YearLow", request.YearLow ?? (object)DBNull.Value),
	            new LB_DALParam("SecurityType", request.SecurityType ?? (object)DBNull.Value),
	            new LB_DALParam("ListingDate", request.ListingDate ?? (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", (object)DBNull.Value),
	            new LB_DALParam("TransDt", (object)DBNull.Value),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.UPDATE),

                // Authorization/Workflow Parameters
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (byte)AuthTypeEnum.UnAuthorize), // Reset to unauthorized for workflow
                new LB_DALParam("AuthLevel", (byte)AuthLevelEnum.Level1),
	            new LB_DALParam("IsDel", (byte)DeleteStatusEnum.Active),
	            new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_CrudMstStk for UPDATE with XchgCode={XchgCode}, StkCode={StkCode}",
                xchgCode, stkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");


            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to update stock - no rows affected or record not found. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock updated successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            var updatedStock = await GetStockByKeyAsync(xchgCode, stkCode, cancellationToken);
            if (updatedStock == null)
            {
                throw new DomainException($"The Updated stock was not found: {xchgCode}-{stkCode}");
            }

            return updatedStock;
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
            _logger.LogError(ex, "An unexpected error occurred while updating Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"The stock update failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteStockAsync(string xchgCode, string stkCode, DeleteStockRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);

        // Validate the request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateDeleteRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.DELETE),
	            new LB_DALParam("XchgCode", xchgCode),
	            new LB_DALParam("StkCode", stkCode),

                // Data Parameters
                new LB_DALParam("StkBrdCode", (object)DBNull.Value),
	            new LB_DALParam("StkSectCode", (object)DBNull.Value),
	            new LB_DALParam("StkLName", (object)DBNull.Value),
	            new LB_DALParam("StkSName", (object)DBNull.Value),
	            new LB_DALParam("StkLastDonePrice", (object)DBNull.Value),
	            new LB_DALParam("StkClosePrice", (object)DBNull.Value),
	            new LB_DALParam("StkRefPrc", (object)DBNull.Value),
	            new LB_DALParam("StkUpperLmtPrice", (object)DBNull.Value),
	            new LB_DALParam("StkLowerLmtPrice", (object)DBNull.Value),
	            new LB_DALParam("StkIsSyariah", (object)DBNull.Value),
	            new LB_DALParam("StkLot", (object)DBNull.Value),
	            new LB_DALParam("LastUpdateDate", (object)DBNull.Value),
	            new LB_DALParam("ISIN", (object)DBNull.Value),
	            new LB_DALParam("Currency", (object)DBNull.Value),
	            new LB_DALParam("StkParValue", (object)DBNull.Value),
	            new LB_DALParam("StkVolumeTraded", (object)DBNull.Value),
	            new LB_DALParam("YearHigh", (object)DBNull.Value),
	            new LB_DALParam("YearLow", (object)DBNull.Value),
	            new LB_DALParam("SecurityType", (object)DBNull.Value),
	            new LB_DALParam("ListingDate", (object)DBNull.Value),

                // Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("MakerId", _currentUserService.UserId),
	            new LB_DALParam("ActionDt", (object)DBNull.Value),
	            new LB_DALParam("TransDt", (object)DBNull.Value),
	            new LB_DALParam("ActionType", (byte)ActionTypeEnum.DELETE),

                // Authorization/Workflow Parameters (Explicitly DBNull in source)
                new LB_DALParam("AuthId", (object)DBNull.Value),
	            new LB_DALParam("AuthDt", (object)DBNull.Value),
	            new LB_DALParam("AuthTransDt", (object)DBNull.Value),
	            new LB_DALParam("IsAuth", (object)DBNull.Value),
	            new LB_DALParam("AuthLevel", (object)DBNull.Value),
	            new LB_DALParam("IsDel", (object)DBNull.Value),

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_CrudMstStk for DELETE with XchgCode={XchgCode}, StkCode={StkCode}",
                xchgCode, stkCode);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_CrudMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");
            var errorMessage = result.GetOutputValue<string>("ErrorMessage");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to delete stock - no rows affected or record not found. Error Message: {errorMessage}");
            }

            _logger.LogDebug("Stock deleted successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}, Error Message: {errorMessage}",
                request.XchgCode, request.StkCode, rowsAffected, errorMessage);

            return true;
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
            _logger.LogError(ex, "An unexpected error occurred while deleting Stock: {XchgCode}-{StkCode}", xchgCode, stkCode);
            throw new DomainException($"The stock deletion failed: {ex.Message}");
        }
    }

    public async Task<bool> StockExistsAsync(string xchgCode, string stkCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if Stock exists: {XchgCode}-{StkCode}", xchgCode, stkCode);

        try
        {
            var stock = await GetStockByKeyAsync(xchgCode, stkCode, cancellationToken);
            return stock != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking Stock existence: {XchgCode}-{StkCode}", xchgCode, stkCode);
            return false;
        }
    }

    #endregion

    #region Workflow Operations

    public async Task<PagedResult<StockDto>> GetStockUnAuthDeniedListAsync(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchTerm = null,
       int isAuth = (byte)AuthTypeEnum.UnAuthorize,
       string sortColumn = "StkCode",
       string sortDirection = "ASC",
       CancellationToken cancellationToken = default)
    {
        // Create request model for validation
        var request = new GetStockWorkflowListRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchText = searchTerm,
            IsAuth = (byte)isAuth,
            MakerId = _currentUserService.UserId,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        // Validate the request using FluentValidation
        await ValidateWorkflowListRequestAsync(request, cancellationToken);

        string authAction = string.Empty;
        if (isAuth == (byte)AuthTypeEnum.UnAuthorize)
            authAction = AuthTypeEnum.UnAuthorize.ToString();
        else if (isAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Getting {authAction} Stock list for workflow - Page: {PageNumber}, Size: {PageSize}", authAction, pageNumber, pageSize);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Pagination Parameters
                new LB_DALParam("PageNumber", pageNumber),
	            new LB_DALParam("PageSize", pageSize),

                // Filter Parameters (explicitly DBNull in source for no initial filtering)
                new LB_DALParam("XchgCode", (object)DBNull.Value),
	            new LB_DALParam("StkCode", (object)DBNull.Value),

                // Control and Audit Parameters
                new LB_DALParam("isAuth", (byte)isAuth),
	            new LB_DALParam("MakerId", _currentUserService.UserId),

                // Search and Sorting Parameters (assuming they can be null/optional)
                new LB_DALParam("SearchText", searchTerm ?? (object)DBNull.Value),
	            new LB_DALParam("SortColumn", sortColumn ?? (object)DBNull.Value),
	            new LB_DALParam("SortDirection", sortDirection ?? (object)DBNull.Value)
            };

			_logger.LogDebug("Calling LB_SP_GetMstStkListWF with parameters: PageNumber={PageNumber}, PageSize={PageSize}, isAuth={IsAuth}, MakerId={MakerId}",
                pageNumber, pageSize, isAuth, _currentUserService.UserId);

            var result = await _repository.QueryPagedAsync<StockDto>(
                sqlOrSp: "LB_SP_GetMstStkListWF",
                parameters: parameters,
                isStoredProcedure: true,
                cancellationToken: cancellationToken);

            _logger.LogDebug("SP returned TotalCount: {TotalCount}, Data count: {DataCount}",
                result.TotalCount, result.Data.Count());

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid arguments for workflow Stock list retrieval");
            throw new ValidationException($"Invalid parameters were provided: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database operation error getting workflow Stock list");
            throw new DomainException($"The database operation failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting {authAction} stock list with parameters: PageNumber={PageNumber}, PageSize={PageSize}, SearchText={SearchText}",
                authAction, pageNumber, pageSize, searchTerm);
            throw new DomainException($"The workflow stock list retrieval failed: {ex.Message}");
        }
    }

    public async Task<bool> AuthorizeStockAsync(string xchgCode, string stkCode, AuthorizeStockRequest request, CancellationToken cancellationToken = default)
    {
        string authAction = string.Empty;
        if (request.IsAuth == (byte)AuthTypeEnum.Approve)
            authAction = AuthTypeEnum.Approve.ToString();
        else if (request.IsAuth == (byte)AuthTypeEnum.Deny)
            authAction = AuthTypeEnum.Deny.ToString();

        _logger.LogInformation("Authorizing {authAction} Stock in workflow: {XchgCode}-{StkCode}", authAction, xchgCode, stkCode);

        // Validate request
        request.XchgCode = xchgCode;
        request.StkCode = stkCode;
        await ValidateAuthorizeRequestAsync(request, cancellationToken);

        try
        {
			List<LB_DALParam> parameters = new List<LB_DALParam>()
            {
                // Primary Action and Key Parameters
                new LB_DALParam("Action", (int)ActionTypeEnum.UPDATE), // Input: UPDATE for Authorization action
                new LB_DALParam("XchgCode", xchgCode),
	            new LB_DALParam("StkCode", stkCode),

                // Authorization/Audit Parameters
                new LB_DALParam("IPAddress", _currentUserService.GetClientIPAddress()),
	            new LB_DALParam("AuthId", _currentUserService.UserId), // The user performing the authorization
                new LB_DALParam("AuthDt", (object)DBNull.Value),       // Placeholder, likely set by SP
                new LB_DALParam("IsAuth", request.IsAuth),             // Authorization status (e.g., Authorized, Denied)
                new LB_DALParam("ActionType", (int)ActionTypeEnum.UPDATE),

                // Remarks
                new LB_DALParam("Remarks", request.Remarks ?? (object)DBNull.Value),

                // Output Parameter
                new LB_DALParam("RowsAffected", 0, ParameterDirection.Output)
            };

			_logger.LogDebug("Calling LB_SP_AuthMstStk for {authAction} with XchgCode={XchgCode}, StkCode={StkCode}, IsAuth={IsAuth}",
                authAction, xchgCode, stkCode, request.IsAuth);

            var result = await _repository.ExecuteWithOutputAsync(
                "LB_SP_AuthMstStk",
                parameters,
                cancellationToken);

            var rowsAffected = result.GetOutputValue<int>("RowsAffected");

            if (rowsAffected <= 0)
            {
                throw new DomainException($"Failed to {authAction.ToLower()} stock - no rows affected or record not found");
            }

            _logger.LogDebug("Stock {authAction} successfully: {XchgCode}-{StkCode}, RowsAffected={RowsAffected}",
                authAction, xchgCode, stkCode, rowsAffected);

            return true;
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
            _logger.LogError(ex, "Error {authAction} authorizing Stock in workflow: {XchgCode}-{StkCode}", authAction, xchgCode, stkCode);
            throw new DomainException($"Failed to {authAction.ToLower()} stock: {ex.Message}");
        }
    }
    #endregion

    #region Private Validation Methods

    private async Task ValidateGetByKeyRequestAsync(GetStockByKeyRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _getByKeyValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("The Stock retrieval by key validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateGetListRequestAsync(GetStockListRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _getListValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationErrorDetail
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue?.ToString()
            }).ToList();

            throw new ValidationException("The Stock retrieval list validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateCreateRequestAsync(CreateStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Stock creation validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateUpdateRequestAsync(UpdateStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Stock update validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateDeleteRequestAsync(DeleteStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Stock deletion validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateAuthorizeRequestAsync(AuthorizeStockRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Stock authorization validation failed") { ValidationErrors = errors };
        }
    }

    private async Task ValidateWorkflowListRequestAsync(GetStockWorkflowListRequest request, CancellationToken cancellationToken)
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

            throw new ValidationException("The Stock workflow list validation failed") { ValidationErrors = errors };
        }
    }

    #endregion
}