using MediatR;

namespace Sentinel.Application.Commands.AcknowledgeAlert;

public record AcknowledgeAlertCommand(
    Guid AlertId,
    string UserName) : IRequest<Unit>;
