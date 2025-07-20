using AutoMapper;
using MediatR;
using RMS.Application.Interfaces;
using RMS.Application.Models.DTOs;
using RMS.Application.Models.Requests;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands;

public class UpdateUsrInfoCommand : IRequest<UsrInfoDto>
{
    public string UsrId { get; set; } = null!;
    public UpdateUsrInfoRequest Request { get; set; } = new();
}

public class UpdateUsrInfoCommandHandler : IRequestHandler<UpdateUsrInfoCommand, UsrInfoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUsrInfoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UsrInfoDto> Handle(UpdateUsrInfoCommand request, CancellationToken cancellationToken)
    {
        // Use the generic repository with string key
        var usrInfo = await _unitOfWork.UsrInfos.FirstOrDefaultAsync(x => x.UsrId == request.UsrId, cancellationToken);

        if (usrInfo == null)
            throw new NotFoundException(nameof(UsrInfo), request.UsrId);

        // Map the update request to the existing entity
        _mapper.Map(request.Request, usrInfo);

        // Manually update the timestamp since UsrInfo doesn't inherit from BaseEntity
        usrInfo.UsrLastUpdatedDate = DateTime.UtcNow;

        await _unitOfWork.UsrInfos.UpdateAsync(usrInfo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UsrInfoDto>(usrInfo);
    }
}