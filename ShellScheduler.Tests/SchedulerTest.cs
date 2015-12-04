using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScheduler.Tests.Mocks;
using System;

namespace ShellScheduler.Tests
{
    [TestClass]
    public class SchedulerTest
    {
        [TestMethod]
        public void Scheduler_RunOnce_InvalidAppPath()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            // Set invalid ApplicationPath
            scheduler.ApplicationPath = "invalid";

            // Execute scheduler once with invalid app path
            bool result = scheduler.Execute();

            // Execute should return false and there should be a new log entry
            Assert.IsFalse(result);
            Assert.AreEqual(loggerMock.LogEntries.Count, 1);
        }

        [TestMethod]
        public void Scheduler_Schedule_ZeroExecutionInterval_InvalidAppPath()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            // Set invalid ApplicationPath
            scheduler.ApplicationPath = "invalid";
            // Set ExecutionInterval to 0
            scheduler.ExecutionIntervalInMinutes = 0;

            // Schedule execution with invalid app path
            bool result = scheduler.ScheduldedExecution();

            // Execute should return false and there should be a new log entry
            Assert.IsFalse(result);
            Assert.AreEqual(loggerMock.LogEntries.Count, 1);
        }

        [TestMethod]
        public void Scheduler_Schedule_LessThanExecutionInterval_InvalidAppPath()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            // Set invalid ApplicationPath
            scheduler.ApplicationPath = "invalid";
            // Set ExecutionInterval to less than 0
            scheduler.ExecutionIntervalInMinutes = -1;

            // Schedule execution with invalid app path
            bool result = scheduler.ScheduldedExecution();

            // Execute should return false and there should be a new log entry
            Assert.IsFalse(result);
            Assert.AreEqual(loggerMock.LogEntries.Count, 1);
        }

        [TestMethod]
        public void Scheduler_Schedule_NonZeroExecutionInterval_InvalidAppPath()
        {
            LoggerMock loggerMock = new LoggerMock();
            Scheduler scheduler = new Scheduler(loggerMock);
            // Set invalid ApplicationPath
            scheduler.ApplicationPath = "invalid";
            // Set ExecutionInterval higher than 0
            scheduler.ExecutionIntervalInMinutes = 60;

            // Schedule execution with invalid app path
            bool result = scheduler.ScheduldedExecution();

            // Execute should return false and there should be a new log entry
            Assert.IsTrue(result);
            Assert.AreEqual(loggerMock.LogEntries.Count, 0);
            // Scheduler should be scheduled to execute
            Assert.IsTrue(scheduler.IsScheduled);
        }

    }
}
