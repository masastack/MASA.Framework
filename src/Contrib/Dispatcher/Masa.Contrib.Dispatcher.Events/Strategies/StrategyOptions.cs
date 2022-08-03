// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Strategies;

public class StrategyOptions
{
    /// <summary>
    /// The maximum number of retry attempts.
    /// </summary>
    public int MaxRetryCount { get; set; }

    public FailureLevels FailureLevels { get; set; }

    public bool IsRetry(int retryTimes) => retryTimes <= MaxRetryCount;

    public void SetStrategy(EventHandlerAttribute dispatchHandler)
    {
        MaxRetryCount = dispatchHandler.ActualRetryTimes; ;
        FailureLevels = dispatchHandler.FailureLevels;
    }
}
