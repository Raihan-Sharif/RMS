using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces;
using SimRMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Features.UsrInfo
{
    #region Get all usr info queries
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
                // Input validation
                if (request.PageNumber < 1) request.PageNumber = 1;
                if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Get paged data through domain repository
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

                // Map to DTOs for response
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

    #endregion

    #region Get all usr info by Id
    public class GetUsrInfoByIdQuery : IRequest<UsrInfoDto>
    {
        public string UsrId { get; set; } = null!;
    }

    public class GetUsrInfoByIdQueryHandler : IRequestHandler<GetUsrInfoByIdQuery, UsrInfoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsrInfoByIdQueryHandler> _logger;

        public GetUsrInfoByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetUsrInfoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UsrInfoDto> Handle(GetUsrInfoByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting UsrInfo by UsrId: {UsrId}", request.UsrId);

            try
            {
                if (string.IsNullOrWhiteSpace(request.UsrId))
                {
                    _logger.LogWarning("GetUsrInfoByIdQuery called with null or empty UsrId");
                    throw new ArgumentException("UsrId cannot be null or empty", nameof(request.UsrId));
                }

                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Get user through domain repository
                var usrInfo = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.UsrId.Trim(), cancellationToken);

                if (usrInfo == null)
                {
                    _logger.LogWarning("UsrInfo not found for UsrId: {UsrId}", request.UsrId);
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);
                }

                // Map to DTO for response
                var result = _mapper.Map<UsrInfoDto>(usrInfo);
                _logger.LogInformation("Successfully retrieved UsrInfo for UsrId: {UsrId}", request.UsrId);

                return result;
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ArgumentException))
            {
                _logger.LogError(ex, "Error getting UsrInfo by UsrId: {UsrId}", request.UsrId);
                throw;
            }
        }
    }

    #endregion

    #region get uss statistics query
    public class GetUsrInfoStatisticsQuery : IRequest<UserStatisticsDto>
    {
    }

    public class GetUsrInfoStatisticsQueryHandler : IRequestHandler<GetUsrInfoStatisticsQuery, UserStatisticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetUsrInfoStatisticsQueryHandler> _logger;

        public GetUsrInfoStatisticsQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetUsrInfoStatisticsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserStatisticsDto> Handle(GetUsrInfoStatisticsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting UsrInfo statistics");

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Get basic statistics from repository (database-level aggregation)
                var basicStats = await _unitOfWork.UsrInfoRepository.GetUserStatisticsAsync(cancellationToken);

                // Get additional statistics that require more complex queries
                var additionalStats = await GetAdditionalStatisticsAsync(cancellationToken);

                var result = new UserStatisticsDto
                {
                    TotalUsers = Convert.ToInt32(basicStats["TotalUsers"]),
                    ActiveUsers = Convert.ToInt32(basicStats["ActiveUsers"]),
                    SuspendedUsers = Convert.ToInt32(basicStats["SuspendedUsers"]),
                    ClosedUsers = Convert.ToInt32(basicStats["ClosedUsers"]),
                    RmsTypes = additionalStats.RmsTypes,
                    CompanyCodes = additionalStats.CompanyCodes
                };

                _logger.LogInformation("Successfully retrieved UsrInfo statistics");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrInfo statistics");
                throw;
            }
        }

        private async Task<(List<RmsTypeStatistic> RmsTypes, List<CompanyCodeStatistic> CompanyCodes)> GetAdditionalStatisticsAsync(CancellationToken cancellationToken)
        {
            // Get first page to check if we need pagination
            var sampleData = await _unitOfWork.UsrInfoRepository.GetUsersPagedAsync(1, 100, cancellationToken: cancellationToken);

            List<RmsTypeStatistic> rmsTypes = new();
            List<CompanyCodeStatistic> companyCodes = new();

            if (sampleData.TotalCount <= 1000) // For small datasets, load all
            {
                var allData = await _unitOfWork.UsrInfoRepository.GetUsersPagedAsync(1, sampleData.TotalCount, cancellationToken: cancellationToken);

                rmsTypes = allData.Data
                    .GroupBy(u => u.RmsType)
                    .Select(g => new RmsTypeStatistic { RmsType = g.Key, Count = g.Count() })
                    .ToList();

                companyCodes = allData.Data
                    .Where(u => !string.IsNullOrEmpty(u.CoCode))
                    .GroupBy(u => u.CoCode!)
                    .Select(g => new CompanyCodeStatistic { CoCode = g.Key, Count = g.Count() })
                    .ToList();
            }
            else
            {
                // For large datasets, you might want to add dedicated repository methods
                // or use database-level grouping queries
                _logger.LogWarning("Large dataset detected ({Count} users). Consider implementing database-level aggregation for RmsTypes and CompanyCodes", sampleData.TotalCount);

                // For now, return empty lists or implement database-level aggregation
                rmsTypes = new List<RmsTypeStatistic>();
                companyCodes = new List<CompanyCodeStatistic>();
            }

            return (rmsTypes, companyCodes);
        }
    }
    #endregion
}
