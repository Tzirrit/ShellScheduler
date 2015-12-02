using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
