// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DefaultExecuteProvider : IExecuteProvider
{
    private int _timer;
    public int Timer => _timer;

    public EventExecuteInfo ExecuteResult { get; private set; }

    public DefaultExecuteProvider()
    {
        _timer = 0;
    }

    public void ExecuteHandler() => _timer++;

    public void Initialize()
    {
        ExecuteResult = new EventExecuteInfo()
        {
            Exception = null,
            Status = ExecuteStatus.InProgress
        };
        _timer = 0;
    }

    public void SetExecuteResult(EventExecuteInfo executeInfo)
        => ExecuteResult = executeInfo;
}
