using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellScheduler
{
    public interface ILoggable
    {
        void AddLogEntry(LogEntry logEntry);
    }
}
