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
                AND ui.UsrExpiryDate IS NULL
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

    public async Task<IEnumerable<ClientListDto>> GetClientListAsync(string? branchCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting client list {BranchFilter}",
            string.IsNullOrEmpty(branchCode) ? "for all branches" : $"for branch: {branchCode}");

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
            WHERE cm.IsDel = 0 AND cm.IsAuth = 1";

        // Add branch filter if branchCode is provided
        var parameters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(branchCode))
        {
            sql += " AND ca.CoBrchCode = @BranchCode";
            parameters.Add("BranchCode", branchCode);
        }

        sql += " ORDER BY cm.ClntName";

        try
        {
            var result = await _repository.QueryAsync<ClientListDto>(sql, parameters.Count > 0 ? parameters : null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} clients {BranchFilter}", result.Count(),
                string.IsNullOrEmpty(branchCode) ? "for all branches" : $"for branch: {branchCode}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client list for branch: {BranchCode}", branchCode ?? "all");
            throw;
        }
    }

    public async Task<IEnumerable<StockExchangeListDto>> GetStockExchangeListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock exchange list");

        var sql = @"
            SELECT XchgCode, XchgName
            FROM MstStkXchg
            WHERE Currency = 'BDT'
            ORDER BY XchgCode";

        try
        {
            var result = await _repository.QueryAsync<StockExchangeListDto>(sql, null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} stock exchanges", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock exchange list");
            throw;
        }
    }

    public async Task<IEnumerable<StockBoardListDto>> GetStockBoardListAsync(string? exchangeCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock board list with exchangeCode filter: {ExchangeCode}", exchangeCode);

        var sql = @"
            SELECT XchgCode, BrdCode, BrdDesc
            FROM MstStkBrd
            WHERE (XchgCode = @ExchangeCode OR @ExchangeCode IS NULL)
            ORDER BY XchgCode, BrdCode";

        var parameters = new { ExchangeCode = exchangeCode };

        try
        {
            var result = await _repository.QueryAsync<StockBoardListDto>(sql, parameters, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} stock boards", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock board list");
            throw;
        }
    }

    public async Task<IEnumerable<StockBoardMarketListDto>> GetStockBoardMarketListAsync(string? exchangeCode = null, string? boardCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock board market list with exchangeCode: {ExchangeCode}, boardCode: {BoardCode}", exchangeCode, boardCode);

        var sql = @"
            SELECT XchgCode, BrdCode, SectCode, SectDesc
            FROM MstStkSect
            WHERE (XchgCode = @ExchangeCode OR @ExchangeCode IS NULL)
                AND (BrdCode = @BoardCode OR @BoardCode IS NULL)
            ORDER BY XchgCode, BrdCode, SectCode";

        var parameters = new { ExchangeCode = exchangeCode, BoardCode = boardCode };

        try
        {
            var result = await _repository.QueryAsync<StockBoardMarketListDto>(sql, parameters, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} stock board markets", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock board market list");
            throw;
        }
    }

    public async Task<IEnumerable<StockListDto>> GetStockListAsync(string? exchangeCode = null, string? boardCode = null, string? sectorCode = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting stock list with exchangeCode: {ExchangeCode}, boardCode: {BoardCode}, sectorCode: {SectorCode}", exchangeCode, boardCode, sectorCode);

        var sql = @"
            SELECT ms.XchgCode,
                   ms.StkBrdCode,
                   msb.BrdDesc,
                   ms.StkSectCode,
                   mss.SectDesc,
                   ms.StkCode,
                   ms.StkLName,
                   ms.StkSName
            FROM MstStk ms
            INNER JOIN MstStkBrd msb ON ms.StkBrdCode = msb.BrdCode
            INNER JOIN MstStkSect mss ON ms.StkBrdCode = mss.BrdCode AND ms.StkSectCode = mss.SectCode
            WHERE ms.IsAuth = 1 AND ms.IsDel = 0";

        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(exchangeCode))
        {
            sql += " AND ms.XchgCode = @ExchangeCode";
            parameters.Add("ExchangeCode", exchangeCode);
        }

        if (!string.IsNullOrWhiteSpace(boardCode))
        {
            sql += " AND ms.StkBrdCode = @BoardCode";
            parameters.Add("BoardCode", boardCode);
        }

        if (!string.IsNullOrWhiteSpace(sectorCode))
        {
            sql += " AND ms.StkSectCode = @SectorCode";
            parameters.Add("SectorCode", sectorCode);
        }

        sql += " ORDER BY ms.StkCode";

        try
        {
            var result = await _repository.QueryAsync<StockListDto>(sql, parameters.Count > 0 ? parameters : null, false, cancellationToken);
            _logger.LogInformation("Retrieved {Count} stocks", result.Count());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock list");
            throw;
        }
    }
}