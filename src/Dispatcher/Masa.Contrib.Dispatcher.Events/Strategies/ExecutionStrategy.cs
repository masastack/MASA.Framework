// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Strategies;

public class ExecutionStrategy : IExecutionStrategy
{
    private readonly IStrategyExceptionProvider _strategyExceptionProvider;
    private readonly ILogger<ExecutionStrategy>? _logger;

    public ExecutionStrategy(IStrategyExceptionProvider strategyExceptionProvider, ILogger<ExecutionStrategy>? logger = null)
    {
        _strategyExceptionProvider = strategyExceptionProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync<TEvent>(StrategyOptions strategyOptions, TEvent @event, Func<TEvent, Task> func,
        Func<TEvent, Exception, FailureLevels, Task> cancel)
        where TEvent : IEvent
    {
        int retryTimes = 0;

        Exception? exception = null!;
        while (strategyOptions.IsRetry(retryTimes))
        {
            try
            {
                if (retryTimes > 0)
                {
                    _strategyExceptionProvider.LogWrite(LogLevel.Warning,
                        null,
                        "----- Error Publishing event {@Event} start: The {retries}th retrying consume a message failed. message id: {messageId} -----",
                        @event, retryTimes, @event.GetEventId());
                }
                await func.Invoke(@event);
                return;
            }
            catch (Exception? ex)
            {
                if (retryTimes > 0)
                {
                    _strategyExceptionProvider.LogWrite(LogLevel.Error,
                        ex,
                        "----- Error Publishing event {@Event} finish: The {retries}th retrying consume a message failed. message id: {messageId} -----",
                        @event, retryTimes, @event.GetEventId());
                }
                else
                {
                    _strategyExceptionProvider.LogWrite(LogLevel.Error,
                        ex,
                        "----- Error Publishing event {@Event}: after {maxRetries}th executions and we will stop retrying. message id: {messageId} -----",
                        @event, strategyOptions.MaxRetryCount, @event.GetEventId());
                }
                exception = ex;
                if (_strategyExceptionProvider.SupportRetry(exception))
                {
                    retryTimes++;
                }
                else
                {
                    retryTimes = strategyOptions.MaxRetryCount + 1;
                }
            }
        }

        //perform the cancel handler

        await cancel(@event, exception, strategyOptions.FailureLevels);
    }
}
