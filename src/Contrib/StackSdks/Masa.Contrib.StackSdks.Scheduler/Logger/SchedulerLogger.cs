// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler.Logger
{
    public class SchedulerLogger<T>
    {
        private readonly ILogger<T> _logger;

        private readonly Guid _taskId;

        private readonly Guid _jobId;

        public const string LOGGER_BODY = "{Message}, LogType: {LogType}, LogWriter: {LogWriter}, TaskId: {TaskId}, JobId: {JobId}";

        public SchedulerLogger(ILoggerFactory loggerFactory, Guid jobId, Guid taskId)
        {
            _logger = loggerFactory.CreateLogger<T>();
            _jobId = jobId;
            _taskId = taskId;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(LOGGER_BODY, message, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.LogError(exception, LOGGER_BODY, message, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId);
        }

        public void LogError(string message)
        {
            _logger.LogError(LOGGER_BODY, message, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(LOGGER_BODY, message, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(LOGGER_BODY, message, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId);
        }
    }
}
