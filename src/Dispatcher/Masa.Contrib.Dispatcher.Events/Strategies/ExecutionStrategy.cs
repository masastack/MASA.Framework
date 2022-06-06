// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Strategies;

public class ExecutionStrategy : IExecutionStrategy
{
    private readonly ILogger<ExecutionStrategy>? _logger;

    public ExecutionStrategy(ILogger<ExecutionStrategy>? logger = null) => _logger = logger;

    public async Task ExecuteAsync<TEvent>(StrategyOptions strategyOptions, TEvent @event, Func<TEvent, Task> func, Func<TEvent, Exception, FailureLevels, Task> cancel)
        where TEvent : IEvent
    {
        int retryTimes = 0;

        Exception exception = null!;
        while (strategyOptions.IsRetry(retryTimes))
        {
            try
            {
                if (retryTimes > 0)
                {
                    _logger?.LogWarning("----- Error Publishing event {@Event} start: The {retries}th retrying consume a message failed. message id: {messageId} -----", @event, retryTimes, @event.GetEventId());
                }
                await func.Invoke(@event);
                return;
            }
            catch (Exception ex)
            {
                if (retryTimes > 0)
                {
                    _logger?.LogError(ex, "----- Error Publishing event {@Event} finish: The {retries}th retrying consume a message failed. message id: {messageId} -----", @event, retryTimes, @event.GetEventId());
                }
                else
                {
                    _logger?.LogError(ex, "----- Error Publishing event {@Event}: after {maxRetries}th executions and we will stop retrying. message id: {messageId} -----", @event, strategyOptions.MaxRetryCount, @event.GetEventId());
                }
                exception = ex;
                retryTimes++;
            }
        }

        //perform the cancel handler

        await cancel(@event, exception, strategyOptions.FailureLevels);
    }
}
