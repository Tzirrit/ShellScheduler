using System;

namespace ShellScheduler
{
    public enum LogLevels
    {
        Error = 0,
        Warning = 1,
        Message = 2
    }

    public class LogEntry
    {
        public string Message { get; private set; }
        public LogLevels Level { get; private set; }
        public DateTime Time { get; private set; }

        public LogEntry(string message, LogLevels level = LogLevels.Message, DateTime? time = null)
        {
            Message = message;
            Level = level;
            Time = (time == null) ? DateTime.Now : (DateTime)time;
        }
    }

  
}
