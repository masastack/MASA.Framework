// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DefaultExecuteProvider : IExecuteProvider
{
    public int Timer { get; private set; }

    public EventExecuteInfo ExecuteResult { get; private set; }

    public DefaultExecuteProvider()
        => ResetTimer();

    public void ResetTimer()
        => Timer = 0;

    public void ExecuteHandler() => Timer++;

    public void Initialize()
    {
        ExecuteResult = new EventExecuteInfo()
        {
            Exception = null,
            Status = ExecuteStatus.InProgress
        };
    }

    public void SetExecuteResult(EventExecuteInfo executeInfo)
        => ExecuteResult = executeInfo;
}
