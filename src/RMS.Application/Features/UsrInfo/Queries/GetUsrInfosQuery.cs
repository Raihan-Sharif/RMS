using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Models.DTOs;
using RMS.Domain.Interfaces;
using RMS.Shared.Extensions;
using RMS.Shared.Models;
using System.Linq.Expressions;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Queries
{
    // Keep your existing query class - NO CHANGES NEEDED
    public class GetUsrInfosQuery : IRequest<PagedResult<UsrInfoDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UsrStatus { get; set; }
        public string? CoCode { get; set; }
        public string? DlrCode { get; set; }
        public string? RmsType { get; set; }
        public string? SearchTerm { get; set; }
    }

    // UPDATE THIS HANDLER to use generic repository
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
            _logger.LogInformation("Processing GetUsrInfosQuery - Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);

            try
            {
                // Validate parameters
                if (request.PageNumber < 1) request.PageNumber = 1;
                if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

                // Use the new generic repository for UsrInfo
                var repository = _unitOfWork.Repository<RMS.Domain.Entities.UsrInfo>();

                // Build predicate for filtering
                var predicate = BuildPredicate(request);

                // Get paged data using generic repository
                var pagedResult = await repository.GetPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    predicate,
                    orderBy: null, // Will use default ordering (UsrId)
                    ascending: true,
                    cancellationToken);

                _logger.LogInformation("Retrieved {ItemCount} UsrInfo records out of {TotalCount} total",
                    pagedResult.Data.Count(), pagedResult.TotalCount);

                // Map to DTOs
                var mappedData = _mapper.Map<IEnumerable<UsrInfoDto>>(pagedResult.Data);

                var result = new PagedResult<UsrInfoDto>
                {
                    Data = mappedData,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };

                _logger.LogInformation("Successfully processed GetUsrInfosQuery");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GetUsrInfosQuery");
                throw;
            }
        }

        private Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>? BuildPredicate(GetUsrInfosQuery request)
        {
            Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>? predicate = null;

            // Filter by status
            if (!string.IsNullOrWhiteSpace(request.UsrStatus))
            {
                predicate = x => x.UsrStatus == request.UsrStatus.Trim();
            }

            // Filter by company code
            if (!string.IsNullOrWhiteSpace(request.CoCode))
            {
                var codePredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x => x.CoCode == request.CoCode.Trim());
                predicate = predicate == null ? codePredicate : predicate.And(codePredicate);
            }

            // Filter by dealer code
            if (!string.IsNullOrWhiteSpace(request.DlrCode))
            {
                var dlrPredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x => x.DlrCode == request.DlrCode.Trim());
                predicate = predicate == null ? dlrPredicate : predicate.And(dlrPredicate);
            }

            // Filter by RMS type
            if (!string.IsNullOrWhiteSpace(request.RmsType))
            {
                var rmsTypePredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x => x.RmsType == request.RmsType.Trim());
                predicate = predicate == null ? rmsTypePredicate : predicate.And(rmsTypePredicate);
            }

            // Search term (search in UsrId, UsrName, UsrEmail)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                var searchPredicate = (Expression<Func<RMS.Domain.Entities.UsrInfo, bool>>)(x =>
                    x.UsrId.Contains(searchTerm) ||
                    (x.UsrName != null && x.UsrName.Contains(searchTerm)) ||
                    (x.UsrEmail != null && x.UsrEmail.Contains(searchTerm)));
                predicate = predicate == null ? searchPredicate : predicate.And(searchPredicate);
            }

            return predicate;
        }
    }
}