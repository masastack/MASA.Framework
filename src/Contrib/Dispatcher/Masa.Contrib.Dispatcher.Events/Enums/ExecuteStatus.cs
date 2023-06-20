// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

public enum ExecuteStatus
{
    /// <summary>
    /// Wait for the event handler to execute
    /// </summary>
    WaitingExecute,

    /// <summary>
    /// in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Event execution succeeded
    /// </summary>
    Succeed,

    /// <summary>
    /// Event execution failed, But Event Rollback Succeeded
    /// </summary>
    RollbackSucceeded,

    /// <summary>
    /// The event execution failed, and there is no compensation handler or an error occurred before the Handler was executed
    /// </summary>
    Failed,

    /// <summary>
    /// Event execution failed and Rollback failed
    /// </summary>
    RollbackFailed
}
