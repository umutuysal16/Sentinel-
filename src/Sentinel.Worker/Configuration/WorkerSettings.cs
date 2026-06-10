namespace Sentinel.Worker.Configuration;

public class WorkerSettings
{
    public string AgentName { get; set; } = "Worker-01";
    public string DeviceType { get; set; } = "WebServer";
    public string ApiGrpcAddress { get; set; } = "http://localhost:5053";
    public int LogIntervalMs { get; set; } = 3000;
    public double CriticalLogProbability { get; set; } = 0.08;
    public string LogSource { get; set; } = "Simulated"; // "Simulated" or "WindowsEventLog"
    public string[] EventLogNames { get; set; } = new[] { "Application", "System" };
}
