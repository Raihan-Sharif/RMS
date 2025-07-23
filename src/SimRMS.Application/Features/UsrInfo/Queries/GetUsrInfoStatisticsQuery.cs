using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimRMS.Application.Features.UsrInfo.Queries
{
    // GET STATISTICS QUERY
    public class GetUsrInfoStatisticsQuery : IRequest<Dictionary<string, object>>
    {
    }

    public class GetUsrInfoStatisticsQueryHandler : IRequestHandler<GetUsrInfoStatisticsQuery, Dictionary<string, object>>
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

        public async Task<Dictionary<string, object>> Handle(GetUsrInfoStatisticsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting UsrInfo statistics");

            try
            {
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Get statistics through domain repository
                var statistics = await _unitOfWork.UsrInfoRepository.GetUserStatisticsAsync(cancellationToken);

                _logger.LogInformation("Successfully retrieved UsrInfo statistics");
                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting UsrInfo statistics");
                throw;
            }
        }
    }
}
