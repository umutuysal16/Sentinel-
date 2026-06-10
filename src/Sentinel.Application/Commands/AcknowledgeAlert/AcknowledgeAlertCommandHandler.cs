using MediatR;
using Sentinel.Application.Common.Exceptions;
using Sentinel.Application.Interfaces;

namespace Sentinel.Application.Commands.AcknowledgeAlert;

public class AcknowledgeAlertCommandHandler : IRequestHandler<AcknowledgeAlertCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;

    public AcknowledgeAlertCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(AcknowledgeAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _dbContext.Alerts.FindAsync(new object[] { request.AlertId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Alert), request.AlertId);

        alert.Acknowledge(request.UserName);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
