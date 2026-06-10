using MediatR;
using Sentinel.Domain.Enums;

namespace Sentinel.Application.Commands.CreateAlert;

public record CreateAlertCommand(
    Guid LogEntryId,
    int RiskScore,
    AlertCategory Category,
    string AiExplanation) : IRequest<Guid>;
