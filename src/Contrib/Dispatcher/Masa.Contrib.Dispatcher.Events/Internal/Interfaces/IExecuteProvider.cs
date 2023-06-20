// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal interface IExecuteProvider
{
    /// <summary>
    /// EventBus execution times
    /// Record the number of times the EventBus publishes an event
    /// </summary>
    int Timer { get; }

    /// <summary>
    /// EventBus execution status
    /// </summary>
    EventExecuteInfo ExecuteResult { get; }

    void ResetTimer();

    /// <summary>
    /// execute event handler
    /// Publishing local events through IEventBus will execute
    /// </summary>
    void ExecuteHandler();

    void Initialize();

    /// <summary>
    /// Set the state and exception information of the execution event handler
    /// </summary>
    void SetExecuteResult(EventExecuteInfo executeInfo);
}
