using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Common.Exceptions;
using Sentinel.Application.DTOs;
using Sentinel.Application.Interfaces;
using Sentinel.Application.Mappings;

namespace Sentinel.Application.Queries.GetLogById;

public class GetLogByIdQueryHandler : IRequestHandler<GetLogByIdQuery, LogEntryDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetLogByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LogEntryDto> Handle(GetLogByIdQuery request, CancellationToken cancellationToken)
    {
        var logEntry = await _dbContext.LogEntries
            .Include(l => l.Agent)
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.LogEntry), request.Id);

        return logEntry.ToDto();
    }
}
