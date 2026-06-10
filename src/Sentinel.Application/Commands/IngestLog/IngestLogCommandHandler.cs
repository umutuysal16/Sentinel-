using MediatR;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Entities;
using Sentinel.Domain.Enums;

namespace Sentinel.Application.Commands.IngestLog;

public class IngestLogCommandHandler : IRequestHandler<IngestLogCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;

    public IngestLogCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(IngestLogCommand request, CancellationToken cancellationToken)
    {
        var logEntry = LogEntry.Create(
            request.AgentId,
            request.Source,
            (LogLevel)request.Level,
            request.Message,
            request.Timestamp,
            request.StackTrace,
            request.Properties);

        _dbContext.LogEntries.Add(logEntry);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return logEntry.Id;
    }
}
