using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Application.Models.DTOs;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Queries;

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
        _logger.LogInformation("Looking for UsrInfo with UsrId: {UsrId}", request.UsrId);

        // Use the generic repository with string key
        var usrInfo = await _unitOfWork.UsrInfos.GetByIdAsync<string>(request.UsrId, cancellationToken);

        if (usrInfo == null)
        {
            _logger.LogWarning("UsrInfo not found with UsrId: {UsrId}", request.UsrId);
            throw new NotFoundException(nameof(UsrInfo), request.UsrId);
        }

        return _mapper.Map<UsrInfoDto>(usrInfo);
    }
}