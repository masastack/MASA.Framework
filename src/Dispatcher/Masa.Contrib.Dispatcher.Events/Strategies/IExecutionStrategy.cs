// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Strategies;

public interface IExecutionStrategy
{
    Task ExecuteAsync<TEvent>(StrategyOptions strategyOptions, TEvent @event, Func<TEvent, Task> func, Func<TEvent, Exception, FailureLevels, Task> cancel)
        where TEvent : IEvent;
}
