using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Interfaces;

namespace Sentinel.Application.Commands.ClearAlerts;

public class ClearAlertsCommandHandler : IRequestHandler<ClearAlertsCommand, int>
{
    private readonly IApplicationDbContext _dbContext;

    public ClearAlertsCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(ClearAlertsCommand request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Alerts.AsQueryable();

        if (request.OnlyAcknowledged)
            query = query.Where(a => a.IsAcknowledged);

        var alertsToDelete = await query.ToListAsync(cancellationToken);
        _dbContext.Alerts.RemoveRange(alertsToDelete);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return alertsToDelete.Count;
    }
}
