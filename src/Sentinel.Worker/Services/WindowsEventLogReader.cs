using System.Diagnostics;
using LogLevel = Sentinel.Domain.Enums.LogLevel;

namespace Sentinel.Worker.Services;

public class WindowsEventLogReader
{
    private readonly Dictionary<string, int> _lastIndexes = new();
    private readonly string[] _logNames;

    public WindowsEventLogReader(string[]? logNames = null)
    {
        _logNames = logNames ?? new[] { "Application", "System" };

        // Initialize last indexes to current position (don't read historical logs)
        foreach (var logName in _logNames)
        {
            try
            {
                using var eventLog = new EventLog(logName);
                _lastIndexes[logName] = eventLog.Entries.Count;
            }
            catch
            {
                _lastIndexes[logName] = 0;
            }
        }
    }

    public (LogLevel Level, string Source, string Message, Dictionary<string, object> Properties)? ReadNextLog()
    {
        foreach (var logName in _logNames)
        {
            try
            {
                using var eventLog = new EventLog(logName);
                var currentCount = eventLog.Entries.Count;
                var lastIndex = _lastIndexes.GetValueOrDefault(logName, 0);

                if (currentCount <= lastIndex) continue;

                // Read the next unread entry
                var entry = eventLog.Entries[lastIndex];
                _lastIndexes[logName] = lastIndex + 1;

                var level = MapEntryType(entry.EntryType);
                var source = entry.Source ?? logName;
                var message = entry.Message?.Length > 2000
                    ? entry.Message[..2000]
                    : entry.Message ?? "No message";

                var properties = new Dictionary<string, object>
                {
                    ["event_id"] = entry.InstanceId,
                    ["category"] = logName,
                    ["machine_name"] = entry.MachineName ?? Environment.MachineName,
                    ["time_generated"] = entry.TimeGenerated.ToString("o"),
                    ["entry_type"] = entry.EntryType.ToString(),
                    ["log_source"] = "WindowsEventLog"
                };

                return (level, source, message, properties);
            }
            catch
            {
                // Skip problematic log sources
            }
        }

        return null;
    }

    private static LogLevel MapEntryType(EventLogEntryType entryType) => entryType switch
    {
        EventLogEntryType.FailureAudit => LogLevel.Critical,
        EventLogEntryType.Error => LogLevel.Error,
        EventLogEntryType.Warning => LogLevel.Warning,
        EventLogEntryType.Information => LogLevel.Information,
        EventLogEntryType.SuccessAudit => LogLevel.Information,
        _ => LogLevel.Information
    };
}
