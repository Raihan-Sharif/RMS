using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Interfaces;

namespace SimRMS.Application.Features.UsrInfo.Queries
{
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
}