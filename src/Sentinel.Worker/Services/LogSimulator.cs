using LogLevel = Sentinel.Domain.Enums.LogLevel;

namespace Sentinel.Worker.Services;

public class LogSimulator
{
    private readonly Random _random = new();
    private readonly double _criticalProbability;
    private double _cpuUsage = 25.0;
    private double _memoryUsage = 40.0;

    public LogSimulator(double criticalProbability = 0.08)
    {
        _criticalProbability = criticalProbability;
    }

    public (LogLevel Level, string Source, string Message, Dictionary<string, object> Properties) GenerateLog()
    {
        var level = PickLevel();
        var (source, message) = GenerateMessage(level);
        var properties = GenerateMetrics();

        return (level, source, message, properties);
    }

    private LogLevel PickLevel()
    {
        var roll = _random.NextDouble();

        if (roll < 0.02) return LogLevel.Debug;
        if (roll < 0.52) return LogLevel.Information;
        if (roll < 0.77) return LogLevel.Warning;
        if (roll < 0.92) return LogLevel.Error;
        return LogLevel.Critical; // ~8%
    }

    private (string Source, string Message) GenerateMessage(LogLevel level) => level switch
    {
        LogLevel.Debug => PickRandom(DebugMessages),
        LogLevel.Information => PickRandom(InfoMessages),
        LogLevel.Warning => PickRandom(WarningMessages),
        LogLevel.Error => PickRandom(ErrorMessages),
        LogLevel.Critical => PickRandom(CriticalMessages),
        _ => ("system", "Unknown log event")
    };

    private Dictionary<string, object> GenerateMetrics()
    {
        // Simulate realistic fluctuation
        _cpuUsage = Math.Clamp(_cpuUsage + (_random.NextDouble() - 0.48) * 10, 5, 99);
        _memoryUsage = Math.Clamp(_memoryUsage + (_random.NextDouble() - 0.48) * 5, 20, 95);

        return new Dictionary<string, object>
        {
            ["cpu_percent"] = Math.Round(_cpuUsage, 1),
            ["memory_percent"] = Math.Round(_memoryUsage, 1),
            ["disk_io_mbps"] = Math.Round(_random.NextDouble() * 100, 1),
            ["network_in_mbps"] = Math.Round(_random.NextDouble() * 50, 1),
            ["network_out_mbps"] = Math.Round(_random.NextDouble() * 30, 1),
            ["active_connections"] = _random.Next(10, 500)
        };
    }

    private (string, string) PickRandom((string Source, string Message)[] pool)
    {
        var item = pool[_random.Next(pool.Length)];
        return item;
    }

    private static readonly (string Source, string Message)[] DebugMessages =
    {
        ("http-server", "Request pipeline initialized with 12 middleware components"),
        ("auth-service", "Token validation cache hit ratio: 94.2%"),
        ("db-connector", "Connection pool stats: active=12, idle=38, total=50"),
        ("scheduler", "Background job scheduler tick: 3 jobs queued"),
    };

    private static readonly (string Source, string Message)[] InfoMessages =
    {
        ("http-server", $"GET /api/health responded 200 in 12ms"),
        ("http-server", $"POST /api/data responded 201 in 89ms"),
        ("auth-service", "User login successful for user admin@sentinel.io"),
        ("auth-service", "JWT token refreshed for session 8a3f2c"),
        ("system-monitor", "Health check passed. All services operational."),
        ("system-monitor", "Scheduled backup completed successfully."),
        ("db-connector", "Query executed: SELECT count(*) FROM logs — 4ms"),
        ("task-runner", "Background job DataSync completed in 2.3s"),
    };

    private static readonly (string Source, string Message)[] WarningMessages =
    {
        ("system-monitor", "High memory usage detected: 87% utilized"),
        ("db-connector", "Slow query detected: SELECT * FROM log_entries ORDER BY timestamp — 3200ms"),
        ("http-server", "Request queue depth exceeding threshold: 150 pending"),
        ("system-monitor", "Disk space running low: 12% remaining on /dev/sda1"),
        ("db-connector", "Connection pool near capacity: 45/50 connections in use"),
        ("auth-service", "Certificate expiring in 7 days for api.sentinel.io"),
        ("system-monitor", "CPU temperature elevated: 78°C on core 2"),
    };

    private static readonly (string Source, string Message)[] ErrorMessages =
    {
        ("db-connector", "Connection refused to database server at 10.0.1.5:5432"),
        ("http-server", "OutOfMemoryException in ImageProcessingService.Resize()"),
        ("system-monitor", "Disk write failure on /dev/sda1: I/O error"),
        ("http-server", "HTTP 503 Service Unavailable from upstream payment-service"),
        ("auth-service", "LDAP connection timeout after 30000ms"),
        ("task-runner", "Background job EmailSender failed: SMTP connection refused"),
    };

    private static readonly (string Source, string Message)[] CriticalMessages =
    {
        ("auth-service", "Brute force detected: 150 failed login attempts from 192.168.1.100 in 60 seconds"),
        ("auth-service", "Unauthorized root access attempt detected from unknown IP 10.0.0.55"),
        ("system-monitor", "CPU usage at 99% for 10 consecutive minutes on production server"),
        ("system-monitor", "Service auth-service unresponsive for 5 minutes — possible service down"),
        ("auth-service", "Privilege escalation attempt: user guest tried to access /admin/config"),
        ("system-monitor", "Memory leak detected: RSS growing 50MB/hour in api-gateway process"),
        ("db-connector", "Database replication lag exceeding 30 seconds — data consistency at risk"),
        ("http-server", "Suspected DDoS: 10,000 requests/sec from subnet 203.0.113.0/24"),
    };
}
