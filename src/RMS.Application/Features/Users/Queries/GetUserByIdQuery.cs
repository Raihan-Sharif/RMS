using AutoMapper;
using RMS.Application.Models.DTOs;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using MediatR;
using RMS.Domain.Entities;

namespace RMS.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
                throw new NotFoundException(nameof(User), request.Id);

            return _mapper.Map<UserDto>(user);
        }
    }
}
