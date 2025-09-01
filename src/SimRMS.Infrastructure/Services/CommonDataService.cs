using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces.Common;

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
}