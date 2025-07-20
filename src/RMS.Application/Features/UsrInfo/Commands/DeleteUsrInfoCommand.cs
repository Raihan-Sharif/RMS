using MediatR;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands;

public class DeleteUsrInfoCommand : IRequest<bool>
{
    public string UsrId { get; set; } = null!;
}

public class DeleteUsrInfoCommandHandler : IRequestHandler<DeleteUsrInfoCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUsrInfoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteUsrInfoCommand request, CancellationToken cancellationToken)
    {
        var usrInfo = await _unitOfWork.UsrInfos.FirstOrDefaultAsync(x => x.UsrId == request.UsrId, cancellationToken);

        if (usrInfo == null)
            throw new NotFoundException(nameof(UsrInfo), request.UsrId);

        await _unitOfWork.UsrInfos.DeleteAsync(usrInfo, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}