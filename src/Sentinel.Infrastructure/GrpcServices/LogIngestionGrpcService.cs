using System.Text.Json;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Sentinel.Application.Commands.IngestLog;
using Sentinel.Application.Commands.RegisterAgent;
using Sentinel.Application.Commands.UpdateAgentStatus;
using Sentinel.Infrastructure.Protos;

namespace Sentinel.Infrastructure.GrpcServices;

public class LogIngestionGrpcService : LogIngestion.LogIngestionBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LogIngestionGrpcService> _logger;

    public LogIngestionGrpcService(IMediator mediator, ILogger<LogIngestionGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<SendLogResponse> SendLog(SendLogRequest request, ServerCallContext context)
    {
        var properties = string.IsNullOrEmpty(request.PropertiesJson)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, object>>(request.PropertiesJson);

        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(request.TimestampUnixMs).UtcDateTime;

        var logEntryId = await _mediator.Send(new IngestLogCommand(
            Guid.Parse(request.AgentId),
            request.Level,
            request.Source,
            request.Message,
            string.IsNullOrEmpty(request.StackTrace) ? null : request.StackTrace,
            properties,
            timestamp));

        return new SendLogResponse
        {
            Success = true,
            LogEntryId = logEntryId.ToString()
        };
    }

    public override async Task<SendLogResponse> SendLogStream(
        IAsyncStreamReader<SendLogRequest> requestStream,
        ServerCallContext context)
    {
        var count = 0;
        await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
        {
            try
            {
                await SendLog(request, context);
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing streamed log entry.");
            }
        }

        return new SendLogResponse { Success = true, LogEntryId = $"batch:{count}" };
    }

    public override async Task<HeartbeatResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
    {
        await _mediator.Send(new UpdateAgentStatusCommand(
            Guid.Parse(request.AgentId),
            request.Hostname,
            request.DeviceType));

        return new HeartbeatResponse { Acknowledged = true };
    }

    public override async Task<RegisterAgentResponse> RegisterAgent(RegisterAgentRequest request, ServerCallContext context)
    {
        var agentId = await _mediator.Send(new RegisterAgentCommand(
            request.Name,
            request.DeviceType,
            string.IsNullOrEmpty(request.IpAddress) ? null : request.IpAddress));

        return new RegisterAgentResponse { AgentId = agentId.ToString() };
    }
}
