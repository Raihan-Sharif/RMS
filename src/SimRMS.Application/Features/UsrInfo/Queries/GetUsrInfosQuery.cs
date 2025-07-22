using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces;
using SimRMS.Shared.Models;
using SimRMS.Domain.Entities;

namespace SimRMS.Application.Features.UsrInfo.Queries
{
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

                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Use domain interface method for optimized paging with filtering
                var pagedResult = await _unitOfWork.UsrInfoRepository.GetUsersPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.UsrStatus,
                    request.CoCode,
                    request.DlrCode,
                    request.RmsType,
                    request.SearchTerm,
                    cancellationToken);

                _logger.LogInformation("Retrieved {ItemCount} UsrInfo records out of {TotalCount} total",
                    pagedResult.Data.Count(), pagedResult.TotalCount);

                // Map to DTOs
                var mappedData = _mapper.Map<IEnumerable<UsrInfoDto>>(pagedResult.Data);

                return new PagedResult<UsrInfoDto>
                {
                    Data = mappedData,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GetUsrInfosQuery");
                throw;
            }
        }
    }
}