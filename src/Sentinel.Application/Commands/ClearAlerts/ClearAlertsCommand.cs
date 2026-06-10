using MediatR;

namespace Sentinel.Application.Commands.ClearAlerts;

public record ClearAlertsCommand(bool OnlyAcknowledged = true) : IRequest<int>;
