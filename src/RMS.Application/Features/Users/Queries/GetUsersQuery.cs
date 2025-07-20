using AutoMapper;
using RMS.Application.Models.DTOs;
using RMS.Domain.Interfaces;
using RMS.Shared.Models;
using MediatR;

namespace RMS.Application.Features.Users.Queries
{
    public class GetUsersQuery : IRequest<PagedResult<UserDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _unitOfWork.Users.GetPagedAsync(
                request.PageNumber, request.PageSize, null, cancellationToken);

            return new PagedResult<UserDto>
            {
                Data = _mapper.Map<IEnumerable<UserDto>>(pagedResult.Data),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
