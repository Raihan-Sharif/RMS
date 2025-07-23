using MediatR;
using Microsoft.Extensions.Logging;
using SimRMS.Domain.Exceptions;
using SimRMS.Domain.Interfaces;
using SimRMS.Domain.Entities;

namespace SimRMS.Application.Features.UsrInfo.Commands
{
    // DELETE COMMAND
    public class DeleteUsrInfoCommand : IRequest<bool>
    {
        public string UsrId { get; set; } = null!;
    }

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
                await _unitOfWork.EnsureConnectionAsync(cancellationToken);

                // Business validation - Check if user exists
                var existingUser = await _unitOfWork.UsrInfoRepository.GetByUserIdAsync(request.UsrId, cancellationToken);
                if (existingUser == null)
                    throw new NotFoundException(nameof(UsrInfo), request.UsrId);

                // Use transaction for data integrity
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Delete through domain repository
                    var deleted = await _unitOfWork.UsrInfoRepository.DeleteUserAsync(request.UsrId, cancellationToken);

                    if (!deleted)
                    {
                        throw new DomainException($"Failed to delete user with ID '{request.UsrId}'");
                    }

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    _logger.LogInformation("Successfully deleted UsrInfo: {UsrId}", request.UsrId);
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Error deleting UsrInfo: {UsrId}", request.UsrId);
                throw;
            }
        }
    }
}