// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Process;

public class SystemProcess : IProcess
{
    private readonly System.Diagnostics.Process _process;

    public SystemProcess(System.Diagnostics.Process process)
    {
        _process = process;
    }

    public int PId => IsRun ? _process.Id : 0;

    public string Name => IsRun ? _process.ProcessName : string.Empty;

    public bool IsRun => !_process.HasExited;

    public void Kill()
    {
        if (IsRun) _process.Kill();
    }

    public bool Start() => _process.Start();

    public void WaitForExit(int? milliseconds = null)
    {
        if (milliseconds is > 0)
        {
            _process.WaitForExit(milliseconds.Value);
        }
        else if (IsRun)
        {
            _process.Kill();
        }
    }
}
