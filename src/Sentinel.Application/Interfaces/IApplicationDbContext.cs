using Microsoft.EntityFrameworkCore;
using Sentinel.Domain.Entities;

namespace Sentinel.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Agent> Agents { get; }
    DbSet<LogEntry> LogEntries { get; }
    DbSet<Alert> Alerts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
