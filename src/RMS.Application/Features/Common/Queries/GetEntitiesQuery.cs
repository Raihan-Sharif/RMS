using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Domain.Interfaces;
using RMS.Shared.Models;
using System.Linq.Expressions;

namespace RMS.Application.Features.Common.Queries
{
    public class GetEntitiesQuery<TEntity, TDto> : IRequest<PagedResult<TDto>>
        where TEntity : class
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Expression<Func<TEntity, bool>>? Filter { get; set; }
        public Expression<Func<TEntity, object>>? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
    }

    public class GetEntitiesQueryHandler<TEntity, TDto> : IRequestHandler<GetEntitiesQuery<TEntity, TDto>, PagedResult<TDto>>
        where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetEntitiesQueryHandler<TEntity, TDto>> _logger;

        public GetEntitiesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetEntitiesQueryHandler<TEntity, TDto>> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<TDto>> Handle(GetEntitiesQuery<TEntity, TDto> request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting paged {EntityType} - Page: {PageNumber}, Size: {PageSize}",
                typeof(TEntity).Name, request.PageNumber, request.PageSize);

            try
            {
                var repository = _unitOfWork.Repository<TEntity>();

                var pagedResult = await repository.GetPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Filter,
                    request.OrderBy,
                    request.Ascending,
                    cancellationToken);

                var mappedData = _mapper.Map<IEnumerable<TDto>>(pagedResult.Data);

                return new PagedResult<TDto>
                {
                    Data = mappedData,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged {EntityType}", typeof(TEntity).Name);
                throw;
            }
        }
    }
}