using MediatR;
using Microsoft.Extensions.Logging;
using RMS.Domain.Exceptions;
using RMS.Domain.Interfaces;
using RMS.Domain.Entities;

namespace RMS.Application.Features.UsrInfo.Commands
{
    // Keep existing command class - NO CHANGES NEEDED
    public class DeleteUsrInfoCommand : IRequest<bool>
    {
        public string UsrId { get; set; } = null!;
    }

    // UPDATE THIS HANDLER to use generic repository
    public class DeleteUsrInfoCommandHandler : IRequestHandler<DeleteUsrInfoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUsrInfoCommandHandler> _logger;

        public DeleteUsrInfoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteUsrInfoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUsrInfoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting UsrInfo: {UsrId}", request.UsrId);

            try
            {
                // Use the new generic repository for UsrInfo
                var repository = _unitOfWork.Repository<RMS.Domain.Entities.UsrInfo>();
                var usrInfo = await repository.GetByIdAsync<string>(request.UsrId, cancellationToken);

                if (usrInfo == null)
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);

                // Delete using generic repository
                await repository.DeleteAsync(usrInfo, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted UsrInfo: {UsrId}", request.UsrId);
                return true;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error deleting UsrInfo: {UsrId}", request.UsrId);
                throw;
            }
        }
    }
}