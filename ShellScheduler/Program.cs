using System;
using System.Collections.Generic;

namespace ShellScheduler
{
    public class Program : ILoggable
    {
        public bool IsVerboseLoggingEnabled { get { return true; } }

        private List<LogEntry> _logEntries;
        private static Program _instance;
        private Scheduler _scheduler;

        /// <summary>
        /// Arguments: AppPath, ExecutionIntervalInHours, NextExecutionAt
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            _instance = new Program();
            _instance.Initialize(args);

            string keyIn = string.Empty;

            // UI Loop
            do
            {
                keyIn = Console.ReadLine();

                switch (keyIn)
                {
                    case "help":
                        _instance.ShowHelp();
                        break;

                    case "once":
                        _instance.ValidateAndExecute();
                        break;

                    case "start":
                        _instance.ValidateAndExecute(runonce: false);
                        break;

                    case "stop":
                        _instance.StopExecution();
                        break;

                    case "list":
                        _instance.ShowCurrentParameters();
                        break;

                    case "set":
                        _instance.SetParameters();
                        break;

                    case "time":
                        _instance.WriteToConsole("Current date and time is: " + DateTime.Now.ToString() + ".", ConsoleColor.White);
                        break;

                    case "exit":
                        break;

                    default:
                        _instance.WriteToConsole(string.Format("Invalid command '{0}'\n Try 'help' for a list of all available commands.", keyIn), ConsoleColor.Yellow);
                        break;
                }

            } while (keyIn != "exit");
        }

        /// <summary>
        /// Initializes the Scheduler.
        /// </summary>
        /// <param name="args"></param>
        private void Initialize(string[] args)
        {
            WriteToConsole("-= Welcome to the Shell Scheduler =-", ConsoleColor.White);
            _logEntries = new List<LogEntry>();
            _scheduler = new Scheduler(this);

            // Check provided parameters
            switch (args.Length)
            {
                // AppPath, SetExecutionIntervalInMinutes, NextExecutionAt
                case 3:
                    SetApplicationPath(args[0]);
                    SetExecutionIntervalInMinutes(args[1]);
                    SetNextExecutionAt(args[2]);
                    break;

                // AppPath, SetExecutionIntervalInMinutes
                case 2:
                    SetApplicationPath(args[0]);
                    SetExecutionIntervalInMinutes(args[1]);
                    SetNextExecutionAt(DateTime.Now.ToString());
                    break;

                // AppPath
                case 1:
                    SetApplicationPath(args[0]);
                    SetExecutionIntervalInMinutes(null);
                    SetNextExecutionAt(DateTime.Now.ToString());
                    break;

                // none or more
                default:
                    if (IsVerboseLoggingEnabled)
                        WriteToConsole("No or invalid number of arguments passed... Nothing to initialize.");
                    break;
            }
        }

        /// <summary>
        /// Shows all available commands.
        /// </summary>
        private void ShowHelp()
        {
            WriteToConsole("Available commands");
            WriteToConsole(" set   - Edit current command, execution interval, and next execution time.");
            WriteToConsole(" list  - Show current command, execution interval, and next execution time.");
            WriteToConsole(" once  - Execute specified command once.");
            WriteToConsole(" start - Start scheduled execution of specified command.");
            WriteToConsole(" stop  - Stop scheduled execution of specified command.");
            WriteToConsole(" time  - Shows current date and time.");
            WriteToConsole(" exit  - Close this application.");
        }

        /// <summary>
        /// Shows current parameters of scheduler.
        /// </summary>
        private void ShowCurrentParameters()
        {
            WriteToConsole("Current command is '" + _scheduler.ApplicationPath + "'.", ConsoleColor.White);
            WriteToConsole("Current execution interval is " + _scheduler.ExecutionIntervalInMinutes + " (minutes).", ConsoleColor.White);
            WriteToConsole("Next scheduled execution at " + _scheduler.NextExecutionAt + ".", ConsoleColor.White);
            WriteToConsole("Execution ", color: ConsoleColor.White, linebreak: false);
            if (_scheduler.IsScheduled)
                _instance.WriteToConsole("is currently", color: ConsoleColor.DarkGreen, linebreak: false);
            else
                _instance.WriteToConsole("is not", color: ConsoleColor.Yellow, linebreak: false);
            _instance.WriteToConsole(" scheduled.", color: ConsoleColor.White);
        }


        /// <summary>
        /// Shows current parameters of scheduler and lets user change or keep them.
        /// </summary>
        private void SetParameters()
        {
            WriteToConsole("New command (<Enter> to keep current command '" + _scheduler.ApplicationPath + "'):", ConsoleColor.White);
            string appPathIn = Console.ReadLine();
            if (appPathIn != string.Empty)
                SetApplicationPath(appPathIn);
            else
                WriteToConsole("Keeping old command '" + _scheduler.ApplicationPath + "'.", ConsoleColor.Yellow);

            WriteToConsole("New execution interval  (<Enter> to keep current interval of " + _scheduler.ExecutionIntervalInMinutes + " minutes):", ConsoleColor.White);
            string minutesIn = Console.ReadLine();
            if (minutesIn != string.Empty)
                SetExecutionIntervalInMinutes(minutesIn);
            else
                WriteToConsole("Keeping old execution interval of " + _scheduler.ExecutionIntervalInMinutes + " minutes.", ConsoleColor.Yellow);

            WriteToConsole("New next scheduled execution (<Enter> to keep current next scheduled execution at " + _scheduler.NextExecutionAt + "):", ConsoleColor.White);
            string nextExecutionIn = Console.ReadLine();
            if (nextExecutionIn != string.Empty)
                SetNextExecutionAt(nextExecutionIn);
            else
                WriteToConsole("Keeping old scheduled execution at " + _scheduler.NextExecutionAt + ".", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Set application of scheduler.
        /// </summary>
        /// <param name="arg"></param>
        private void SetApplicationPath(string arg)
        {
            WriteToConsole("Setting AppPath to '" + arg + "'.", ConsoleColor.DarkGreen);
            _scheduler.ApplicationPath = arg;
        }

        /// <summary>
        /// Sets execution interval of scheduler.
        /// </summary>
        /// <param name="arg"></param>
        private void SetExecutionIntervalInMinutes(string arg)
        {
            int interval = 0;
            int.TryParse(arg, out interval);

            if (interval > 0)
                WriteToConsole("Setting execution interval to " + interval + " hours.", ConsoleColor.DarkGreen);
            else
                WriteToConsole("Defaulting to execute only once.", ConsoleColor.Yellow);

            _scheduler.ExecutionIntervalInMinutes = interval;
        }
        
        /// <summary>
        /// Sets date and time for next scheduled execution.
        /// </summary>
        /// <param name="arg"></param>
        private void SetNextExecutionAt(string arg)
        {
            DateTime nextExecution = DateTime.Now;

            try
            {
                nextExecution = DateTime.Parse(arg);
                WriteToConsole("Next execution at " + nextExecution + ".", ConsoleColor.DarkGreen);
            }
            catch (Exception e)
            {
                WriteToConsole("Could not convert given execution time '" + arg + "': " + e.Message, ConsoleColor.DarkRed);
                WriteToConsole("Defaulting to execute now.", ConsoleColor.Yellow);
            }

            _scheduler.NextExecutionAt = nextExecution;
        }

        /// <summary>
        /// Validates required parameters and then executes scheduler.
        /// </summary>
        /// <param name="runonce"></param>
        private void ValidateAndExecute(bool runonce = true)
        {
            if (_scheduler.ApplicationPath == null)
            {
                WriteToConsole("No command specified.", ConsoleColor.Yellow);
                WriteToConsole("Please enter command to be executed: ", linebreak: false);
                _scheduler.ApplicationPath = Console.ReadLine();
            }

            // Run once
            if (runonce)
            {
                WriteToConsole("Executing '" + _scheduler.ApplicationPath + "' once.", color: ConsoleColor.White, showDateTime: true);
                _scheduler.Execute();
            }
            // Start scheduled execution
            else
            {
                if (_scheduler.NextExecutionAt == null)
                {
                    WriteToConsole("No next execution set!", color: ConsoleColor.DarkRed, showDateTime: true);
                }

                if (_scheduler.ExecutionIntervalInMinutes <= 0)
                {
                    //WriteToConsole("No execution interval set!", ConsoleColor.DarkRed);
                    WriteToConsole("Current execution interval is set to 0.", ConsoleColor.Yellow);
                    WriteToConsole("Please enter execution interval: ", linebreak: false);
                    SetExecutionIntervalInMinutes(Console.ReadLine());
                }
                WriteToConsole("Scheduling execution of '" + _scheduler.ApplicationPath + "' every " + _scheduler.ExecutionIntervalInMinutes + " minutes.\nNext execution at " + _scheduler.NextExecutionAt + ".",
                    color: ConsoleColor.White, showDateTime: true);
                _scheduler.ScheduldedExecution();
            }
        }

        /// <summary>
        /// Stops execution of scheduler.
        /// </summary>
        private void StopExecution()
        {
            _scheduler.StopScheduledExecution();
        }

        #region Logging
        public void AddLogEntry(LogEntry logEntry)
        {
            switch (logEntry.Level)
            {
                case LogLevels.Error:
                    WriteToConsole(logEntry.Message, ConsoleColor.Red, showDateTime: true);
                    break;

                case LogLevels.Warning:
                    WriteToConsole(logEntry.Message, ConsoleColor.Yellow, showDateTime: true);
                    break;

                default:
                    WriteToConsole(logEntry.Message, ConsoleColor.White, showDateTime: true);
                    break;
            }
        }

        /// <summary>
        /// Writes given string to the console
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="linebreak"></param>
        private void WriteToConsole(string text, ConsoleColor color = ConsoleColor.Gray, bool linebreak = true, bool showDateTime = false)
        {
            Console.ForegroundColor = color;

            if (showDateTime)
            {
                text = string.Format("[{0}]: {1}", DateTime.Now, text);
            }

            if (linebreak)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }

            Console.ResetColor();
        }
        #endregion

    }
}
