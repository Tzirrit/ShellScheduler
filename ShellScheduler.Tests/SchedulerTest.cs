using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScheduler.Tests.Mocks;
using System;

namespace ShellScheduler.Tests
{
    [TestClass]
    public class SchedulerTest
    {
        [TestMethod]
        public void Scheduler_Initialize()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);

            // Make sure that scheduler is properly initialized
            Assert.IsFalse(scheduler.IsScheduled);
            Assert.IsNull(scheduler.ApplicationPath);
            Assert.AreEqual(DateTime.Now, scheduler.NextExecutionAt);
        }

        [TestMethod]
        public void Scheduler_Execute_InvalidAppPath()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            scheduler.ApplicationPath = "invalid";

            // Execute scheduler once with invalid app path
            bool result = scheduler.Execute();

            // Make sure that execute returns false and that there is a new log entry
            Assert.IsFalse(result);
            Assert.AreEqual(loggerMock.LogEntries.Count, 1);
        }

        /// <summary>
        /// This test is expected to fail, because we currently do not check if the app path is valid when scheduling the execution.
        /// ToDo: only allow to schedule executions with a valid app path!
        /// </summary>
        [TestMethod]
        public void Scheduler_Schedule_InvalidAppPath_FAIL()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            scheduler.ApplicationPath = "invalid";

            // Schedule execution with invalid app path
            bool result = scheduler.ScheduldedExecution();

            // Make sure that execute returns false and that there is a new log entry
            Assert.IsFalse(result); // this will fail
            Assert.AreEqual(loggerMock.LogEntries.Count, 1);
        }
    }
}
