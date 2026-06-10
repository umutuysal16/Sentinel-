using MediatR;
using Sentinel.Application.Common.Exceptions;
using Sentinel.Application.Interfaces;

namespace Sentinel.Application.Commands.UpdateAgentStatus;

public class UpdateAgentStatusCommandHandler : IRequestHandler<UpdateAgentStatusCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateAgentStatusCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpdateAgentStatusCommand request, CancellationToken cancellationToken)
    {
        var agent = await _dbContext.Agents.FindAsync(new object[] { request.AgentId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Agent), request.AgentId);

        agent.UpdateHeartbeat();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
