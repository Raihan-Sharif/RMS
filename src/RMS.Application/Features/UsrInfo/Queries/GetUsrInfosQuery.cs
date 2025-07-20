using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Models.DTOs;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;
using RMS.Shared.Extensions;
using RMS.Shared.Models;

namespace RMS.Application.Features.UsrInfo.Queries;

public class GetUsrInfosQuery : IRequest<PagedResult<UsrInfoDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? UsrStatus { get; set; }
    public string? CoCode { get; set; }
    public string? DlrCode { get; set; }
}

public class GetUsrInfosQueryHandler : IRequestHandler<GetUsrInfosQuery, PagedResult<UsrInfoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsrInfosQueryHandler> _logger;

    public GetUsrInfosQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetUsrInfosQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<UsrInfoDto>> Handle(GetUsrInfosQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("=== GetUsrInfosQueryHandler.Handle START ===");
        _logger.LogInformation("Request: PageNumber={PageNumber}, PageSize={PageSize}, UsrStatus={UsrStatus}, CoCode={CoCode}, DlrCode={DlrCode}",
            request.PageNumber, request.PageSize, request.UsrStatus, request.CoCode, request.DlrCode);

        try
        {
            // Log repository info
            var pkName = _unitOfWork.UsrInfos.GetPrimaryKeyName();
            var pkType = _unitOfWork.UsrInfos.GetPrimaryKeyType();
            _logger.LogInformation("UsrInfo Primary Key: {PrimaryKeyName} ({PrimaryKeyType})", pkName, pkType.Name);

            var predicate = BuildPredicate(request);
            _logger.LogInformation("Predicate built successfully");

            _logger.LogInformation("Calling GetPagedAsync...");
            var pagedResult = await _unitOfWork.UsrInfos.GetPagedAsync(
                request.PageNumber, request.PageSize, predicate, cancellationToken);

            _logger.LogInformation("GetPagedAsync completed. Total items: {TotalCount}, Retrieved: {ItemCount}",
                pagedResult.TotalCount, pagedResult.Data.Count());

            var mappedData = _mapper.Map<IEnumerable<UsrInfoDto>>(pagedResult.Data);
            _logger.LogInformation("Mapping completed. Mapped items count: {MappedCount}", mappedData.Count());

            var result = new PagedResult<UsrInfoDto>
            {
                Data = mappedData,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

            _logger.LogInformation("=== GetUsrInfosQueryHandler.Handle END SUCCESS ===");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsrInfosQueryHandler.Handle");
            throw;
        }
    }

    private static Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>? BuildPredicate(GetUsrInfosQuery request)
    {
        Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(request.UsrStatus))
        {
            predicate = x => x.UsrStatus == request.UsrStatus;
        }

        if (!string.IsNullOrEmpty(request.CoCode))
        {
            var codePredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x => x.CoCode == request.CoCode);
            predicate = predicate == null ? codePredicate : predicate.And(codePredicate);
        }

        if (!string.IsNullOrEmpty(request.DlrCode))
        {
            var dlrPredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x => x.DlrCode == request.DlrCode);
            predicate = predicate == null ? dlrPredicate : predicate.And(dlrPredicate);
        }

        return predicate;
    }
}