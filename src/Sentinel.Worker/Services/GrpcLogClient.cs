using System.Text.Json;
using Grpc.Net.Client;
using Sentinel.Infrastructure.Protos;
using LogLevel = Sentinel.Domain.Enums.LogLevel;

namespace Sentinel.Worker.Services;

public class GrpcLogClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly LogIngestion.LogIngestionClient _client;

    public GrpcLogClient(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _client = new LogIngestion.LogIngestionClient(_channel);
    }

    public async Task<string> RegisterAgentAsync(string name, string deviceType, string? ipAddress = null)
    {
        var response = await _client.RegisterAgentAsync(new RegisterAgentRequest
        {
            Name = name,
            DeviceType = deviceType,
            IpAddress = ipAddress ?? string.Empty
        });

        return response.AgentId;
    }

    public async Task<bool> SendLogAsync(string agentId, LogLevel level, string source, string message,
        Dictionary<string, object>? properties = null)
    {
        var request = new SendLogRequest
        {
            AgentId = agentId,
            Level = (int)level,
            Source = source,
            Message = message,
            StackTrace = string.Empty,
            PropertiesJson = properties is not null ? JsonSerializer.Serialize(properties) : string.Empty,
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var response = await _client.SendLogAsync(request);
        return response.Success;
    }

    public async Task<bool> HeartbeatAsync(string agentId, string hostname, string deviceType)
    {
        var response = await _client.HeartbeatAsync(new HeartbeatRequest
        {
            AgentId = agentId,
            Hostname = hostname,
            DeviceType = deviceType
        });

        return response.Acknowledged;
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
