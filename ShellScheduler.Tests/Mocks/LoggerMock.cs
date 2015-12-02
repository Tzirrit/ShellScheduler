using System.Collections.Generic;

namespace ShellScheduler.Tests.Mocks
{
    public class LoggerMock : ILoggable
    {
        public List<LogEntry> LogEntries = new List<LogEntry>();

        public void AddLogEntry(LogEntry logEntry)
        {
            LogEntries.Add(logEntry);
        }
    }
}
