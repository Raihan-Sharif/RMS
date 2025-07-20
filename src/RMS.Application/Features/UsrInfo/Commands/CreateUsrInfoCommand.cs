using AutoMapper;
using MediatR;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands;

public class CreateUsrInfoCommand : IRequest<UsrInfoDto>
{
    public CreateUsrInfoRequest Request { get; set; } = new();
}

public class CreateUsrInfoCommandHandler : IRequestHandler<CreateUsrInfoCommand, UsrInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateUsrInfoCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<UsrInfoDto> Handle(CreateUsrInfoCommand request, CancellationToken cancellationToken)
    {
        var usrInfo = _mapper.Map<RMS.Domain.Entities.UsrInfo>(request.Request);

        // Manually set the timestamp fields since UsrInfo doesn't inherit from BaseEntity
        usrInfo.UsrCreationDate = DateTime.UtcNow;
        usrInfo.UsrLastUpdatedDate = DateTime.UtcNow;

        await _unitOfWork.UsrInfos.AddAsync(usrInfo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UsrInfoDto>(usrInfo);
    }
}