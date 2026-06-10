using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentinel.Application.Interfaces;
using Sentinel.Domain.Entities;

namespace Sentinel.Application.Commands.RegisterAgent;

public class RegisterAgentCommandHandler : IRequestHandler<RegisterAgentCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;

    public RegisterAgentCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(RegisterAgentCommand request, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Agents
            .FirstOrDefaultAsync(a => a.Name == request.Name, cancellationToken);

        if (existing is not null)
        {
            existing.UpdateHeartbeat();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var agent = Agent.Create(request.Name, request.DeviceType, request.IpAddress);
        _dbContext.Agents.Add(agent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return agent.Id;
    }
}
