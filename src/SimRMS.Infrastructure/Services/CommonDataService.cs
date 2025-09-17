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

    public async Task<IEnumerable<CountryListDto>> GetCountryListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting country list");

        var sql = @"
            SELECT CountryCode,
                   CountryName
            FROM MstCountry
            ORDER BY CountryName";

        try
        {
            var result = await _repository.QueryAsync<CountryListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} countries", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country list");
            throw;
        }
    }

    public async Task<IEnumerable<ClientTypeListDto>> GetClientTypeListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting client type list");

        var sql = @"
            SELECT ClntType as ClientType,
                   clntTypeDesc as ClientTypeName
            FROM MstClntType
            ORDER BY ClientTypeName";

        try
        {
            var result = await _repository.QueryAsync<ClientTypeListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} client types", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client type list");
            throw;
        }
    }

    public async Task<IEnumerable<ClientListDto>> GetClientListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting client list");

        var sql = @"
            SELECT
                cm.GCIF,
                cm.ClntName,
                cm.ClntNICNo,
                cm.ClntAddr,
                cm.ClntPhone,
                cm.ClntMobile,
                cm.Gender,
                cm.Nationality,
                cm.ClntEmail,
                cm.CountryCode,
                mc.CountryName,
                ca.ClntCode,
                ca.CoBrchCode,
                mcb.CoBrchDesc as BranchName,
                ca.ClntCDSNo
            FROM dbo.ClntMaster cm
            INNER JOIN MstCountry mc ON cm.CountryCode = mc.CountryCode
            LEFT JOIN dbo.ClntAcct ca ON cm.GCIF = ca.GCIF AND ca.IsDel = 0 AND ca.IsAuth = 1
            LEFT JOIN MstCoBrch mcb ON ca.CoBrchCode = mcb.CoBrchCode
            WHERE cm.IsDel = 0 AND cm.IsAuth = 1
            ORDER BY cm.ClntName";

        try
        {
            var result = await _repository.QueryAsync<ClientListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} clients", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client list");
            throw;
        }
    }
}