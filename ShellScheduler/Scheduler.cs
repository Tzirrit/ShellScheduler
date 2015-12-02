using System;
using System.Diagnostics;
using System.Timers;

namespace ShellScheduler
{
    public class Scheduler
    {
        /// <summary>
        /// Wether or not the scheduler is currently scheduled to run.
        /// </summary>
        public bool IsScheduled { get; set; }
        /// <summary>
        /// Name and path of the scheduled Application.
        /// </summary>
        public string ApplicationPath { get; set; }
        /// <summary>
        /// Interval in which the Application should be executed.
        /// </summary>
        public int ExecutionIntervalInMinutes { get; set; }

        private DateTime _nextExecutionAt;
        /// <summary>
        /// Date and Time of the next scheduled execution.
        /// </summary>
        public DateTime NextExecutionAt
        {
            get { return _nextExecutionAt; }
            set
            {
                _nextExecutionAt = (value < DateTime.Now) ? DateTime.Now : value;
            }
        }

        private ILoggable _logger;
        private Timer _timer;


        public Scheduler(ILoggable logger)
        {
            _logger = logger;

            IsScheduled = false;
            ApplicationPath = null;
            NextExecutionAt = DateTime.Now;
        }


        /// <summary>
        /// Executes the application defined by ApplicationPath once.
        /// </summary>
        public bool Execute()
        {
            return ExecuteApplication(ApplicationPath);
        }

        /// <summary>
        /// Executes the application defined by ApplicationPath at the next available possibility.
        /// </summary>
        public bool ScheduldedExecution()
        {
            if (ApplicationPath == null)
                return false;

            // If interval is 0 or less, execute only once
            if (ExecutionIntervalInMinutes <= 0)
                return Execute();

            // Calculate delay in milliseconds until next execution
            int initialDelay = (int)(NextExecutionAt - DateTime.Now).TotalMilliseconds;
            Console.WriteLine(initialDelay);

            // Set-up timer
            _timer = new Timer();
            _timer.Interval = (initialDelay > 0) ? initialDelay : 1;
            _timer.AutoReset = false; // run only once
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;
            IsScheduled = true;

            return true;
        }

        public void StopScheduledExecution()
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                IsScheduled = false;

                _logger.AddLogEntry(
                    new LogEntry("Stopping scheduled execution of '" + ApplicationPath + "'.", LogLevels.Message));
            }
            else
            {
                _logger.AddLogEntry(
                    new LogEntry("Scheduled execution was not started. Nothing to stop.", LogLevels.Warning));
            }
        }


        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Timer timer = (Timer)source;

            // Make sure timer only ticks if scheduling is enabled
            if (!IsScheduled)
            {
                timer.Enabled = false;
                timer.AutoReset = false;
                return;
            }

            // If inital run, set Interval to ExecutionIntervalInHours
            if (timer.AutoReset == false)
            {
                timer.AutoReset = true;
                timer.Interval = ExecutionIntervalInMinutes * 1000 * 60;
            }

            // Execute
            _logger.AddLogEntry(
                new LogEntry("Executing '" + ApplicationPath + "' scheduled for " + NextExecutionAt + "...", LogLevels.Message));
            try
            {
                ExecuteApplication(ApplicationPath);
            }
            catch (Exception ex)
            {
                _logger.AddLogEntry(
                    new LogEntry("Exception trying to execute '" + ApplicationPath + "': " + ex.Message, LogLevels.Error));
            }

            // Show next planned execution
            if (ExecutionIntervalInMinutes > 0)
            {
                NextExecutionAt = NextExecutionAt.AddMinutes(ExecutionIntervalInMinutes);
                _logger.AddLogEntry(
                    new LogEntry("Scheduled next execution of '" + ApplicationPath + "' for " + NextExecutionAt + ".", LogLevels.Message));
            }
            else
            {
                _logger.AddLogEntry(new LogEntry("No further executions scheduled.", LogLevels.Message));
                timer.Enabled = false;
            }
        }

        #region Process Handling
        /// <summary>
        /// Executes application defined by appPath with provided parameters.
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="args"></param>
        private bool ExecuteApplication(string appPath, string[] args = null)
        {
            string arguments = (args == null) ? string.Empty : (" " + string.Join(" ", args));

            // Create process
            Process process = new Process();
            process.StartInfo.FileName = appPath;
            process.StartInfo.Arguments = arguments;

            // Set UseShellExecute to false to allow redirection
            process.StartInfo.UseShellExecute = false;

            // Redirect the standard output and error
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            // Events
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(process_Exited);
            process.OutputDataReceived += new DataReceivedEventHandler(process_DataRecieved);

            try
            {
                // Start the process
                process.Start();
                //process.BeginOutputReadLine();

                // Read output
                if (process.StartInfo.RedirectStandardOutput)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    if(output.Trim().Length > 0)
                        _logger.AddLogEntry(new LogEntry(output));
                }
                if (process.StartInfo.RedirectStandardOutput)
                {
                    string error = process.StandardError.ReadToEnd();
                    if (error.Trim().Length > 0)
                        _logger.AddLogEntry(new LogEntry(error, LogLevels.Error));
                }
                process.WaitForExit();
                
            }
            catch (Exception e)
            {
                _logger.AddLogEntry(new LogEntry(
                    string.Format("Exception trying to execute '" + appPath + "':" + e.Message), LogLevels.Error));

                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes process output to console async
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void process_DataRecieved(object sender, DataReceivedEventArgs e)
        {
            // Write data to console, if not null or empty
            if (!String.IsNullOrEmpty(e.Data))
                _logger.AddLogEntry(new LogEntry(e.Data, LogLevels.Message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void process_Exited(object sender, EventArgs e)
        {
            _logger.AddLogEntry(new LogEntry("Process execution completed.", LogLevels.Message));
        }
        #endregion
    }
}
