using MediatR;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Entities;

namespace Sentinel.Application.Commands.CreateAlert;

public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateAlertCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = Alert.Create(
            request.LogEntryId,
            request.RiskScore,
            request.Category,
            request.AiExplanation);

        _dbContext.Alerts.Add(alert);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return alert.Id;
    }
}
