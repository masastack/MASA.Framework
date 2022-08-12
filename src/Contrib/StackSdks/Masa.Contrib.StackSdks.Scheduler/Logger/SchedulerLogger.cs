// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler.Logger
{
    public class SchedulerLogger<T>
    {
        private readonly ILogger<T> _logger;

        private readonly Guid _taskId;

        private readonly Guid _jobId;

        public const string LOGGER_BODY = "LogType: {LogType}, Writer: {Writer}, TaskId: {TaskId}, JobId: {JobId}, Message: {Message}";

        public SchedulerLogger(ILoggerFactory loggerFactory, Guid jobId, Guid taskId)
        {
            _logger = loggerFactory.CreateLogger<T>();
            _jobId = jobId;
            _taskId = taskId;
        }

        public void LogInfomation(string message)
        {
            _logger.LogInformation(LOGGER_BODY, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId, message);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.LogError(exception, LOGGER_BODY, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId, message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(LOGGER_BODY, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId, message);
        }

        public void LogError(string message)
        {
            _logger.LogError(LOGGER_BODY, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId, message);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(LOGGER_BODY, LoggerTypes.JobLog.ToString(), WriterTypes.Job.ToString(), _taskId, _jobId, message);
        }
    }
}
