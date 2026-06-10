using Microsoft.Extensions.Options;
using Sentinel.Worker.Configuration;
using Sentinel.Worker.Services;
using LogLevel = Sentinel.Domain.Enums.LogLevel;

namespace Sentinel.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly WorkerSettings _settings;
    private GrpcLogClient? _grpcClient;
    private string? _agentId;

    public Worker(ILogger<Worker> logger, IOptions<WorkerSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sentinel Worker starting. Agent: {AgentName}, Type: {DeviceType}, Source: {LogSource}, Target: {ApiAddress}",
            _settings.AgentName, _settings.DeviceType, _settings.LogSource, _settings.ApiGrpcAddress);

        await Task.Delay(3000, stoppingToken);

        _grpcClient = new GrpcLogClient(_settings.ApiGrpcAddress);
        var simulator = new LogSimulator(_settings.CriticalLogProbability);
        var eventLogReader = _settings.LogSource == "WindowsEventLog"
            ? new WindowsEventLogReader(_settings.EventLogNames)
            : null;

        var heartbeatInterval = TimeSpan.FromSeconds(30);
        var lastHeartbeat = DateTime.MinValue;

        // Register agent
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _agentId = await _grpcClient.RegisterAgentAsync(
                    _settings.AgentName,
                    _settings.DeviceType);
                _logger.LogInformation("Agent registered. AgentId: {AgentId}", _agentId);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to register agent. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }

        // Main loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                LogLevel level;
                string source;
                string message;
                Dictionary<string, object> properties;

                if (eventLogReader is not null)
                {
                    // Try to read a real Windows Event Log entry
                    var realLog = eventLogReader.ReadNextLog();
                    if (realLog.HasValue)
                    {
                        (level, source, message, properties) = realLog.Value;
                    }
                    else
                    {
                        // No new events, skip this cycle
                        await Task.Delay(_settings.LogIntervalMs, stoppingToken);
                        continue;
                    }
                }
                else
                {
                    // Simulated log
                    (level, source, message, properties) = simulator.GenerateLog();
                }

                var success = await _grpcClient.SendLogAsync(_agentId!, level, source, message, properties);

                if (success)
                {
                    _logger.LogDebug("[{Level}] {Source}: {Message}", level, source, message);
                }

                // Periodic heartbeat
                if (DateTime.UtcNow - lastHeartbeat > heartbeatInterval)
                {
                    await _grpcClient.HeartbeatAsync(_agentId!, Environment.MachineName, _settings.DeviceType);
                    lastHeartbeat = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending log. Retrying...");
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            await Task.Delay(_settings.LogIntervalMs, stoppingToken);
        }

        _grpcClient?.Dispose();
    }
}
