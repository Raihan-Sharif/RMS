using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces.Common;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Common Data Service
/// Author:      Raihan Sharif
/// Purpose:     this is for common data end point service
/// Creation:    4/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 
namespace SimRMS.Infrastructure.Services;

public class CommonDataService : ICommonDataService
{
    private readonly IGenericRepository _repository;
    private readonly ILogger<CommonDataService> _logger;

    public CommonDataService(
        IGenericRepository repository,
        ILogger<CommonDataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<BranchListDto>> GetBranchListAsync(string? companyCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting branch list with companyCode filter: {CompanyCode}", companyCode);

        var sql = @"
            SELECT CoCode as CompanyCode, 
                   CoBrchCode as BranchCode, 
                   CoBrchDesc as BranchName
            FROM MstCoBrch
            WHERE IsAuth = 1 AND IsDel = 0";

        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(companyCode))
        {
            sql += " AND CoCode = @CompanyCode";
            parameters = new { CompanyCode = companyCode };
        }

        sql += " ORDER BY CoCode, CoBrchCode";

        try
        {
            var result = await _repository.QueryAsync<BranchListDto>(sql, parameters, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} branches", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branch list");
            throw;
        }
    }

    public async Task<IEnumerable<CompanyListDto>> GetCompanyListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting company list");

        var sql = @"
            SELECT CoCode as CompanyCode, 
                   CoDesc as CompanyName
            FROM MstCo
            WHERE IsAuth = 1 AND IsDel = 0
            ORDER BY CoCode";

        try
        {
            var result = await _repository.QueryAsync<CompanyListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} companies", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company list");
            throw;
        }
    }

    public async Task<IEnumerable<TraderListDto>> GetTraderListAsync(string? exchangeCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting trader list with exchangeCode filter: {ExchangeCode}", exchangeCode);

        var sql = @"
            SELECT XchgCode as ExchangeCode, 
                   DlrCode as TraderCode
            FROM LBTrader
            WHERE IsAuth = 1 AND IsDel = 0";

        object? parameters = null;

        if (!string.IsNullOrWhiteSpace(exchangeCode))
        {
            sql += " AND XchgCode = @ExchangeCode";
            parameters = new { ExchangeCode = exchangeCode };
        }

        sql += " ORDER BY XchgCode, DlrCode";

        try
        {
            var result = await _repository.QueryAsync<TraderListDto>(sql, parameters, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} traders", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trader list");
            throw;
        }
    }

    public async Task<IEnumerable<UserListDto>> GetUserListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user list for dropdown/lookup");

        var sql = @"
            SELECT 
                ui.UsrID, ui.DlrCode, ui.CoBrchCode, ui.UsrName, ui.UsrType, 
                ui.UsrEmail, ui.UsrMobile, ui.UsrStatus, ui.UsrExpiryDate, ui.CSEDlrCode,
                ui.Remarks, ui.CoCode,
                mc.CoDesc AS CompanyName,
                mcb.CoBrchDesc AS BranchName
            FROM dbo.UsrInfo ui
            INNER JOIN MstCo mc ON ui.CoCode = mc.CoCode
            INNER JOIN MstCoBrch mcb ON ui.CoBrchCode = mcb.CoBrchCode
            WHERE ui.IsAuth = 1
                AND ui.IsDel = 0
                AND ui.UsrExpiryDate IS NOT NULL
            ORDER BY ui.UsrID ASC";

        try
        {
            var result = await _repository.QueryAsync<UserListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} users", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user list");
            throw;
        }
    }
}