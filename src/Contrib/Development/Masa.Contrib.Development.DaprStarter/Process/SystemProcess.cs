// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public class SystemProcess : IProcess
{
    private readonly Process _process;

    public SystemProcess(Process process)
    {
        _process = process;
    }

    public string Name => IsRun ? _process.ProcessName : string.Empty;

    public bool IsRun => !_process.HasExited;

    public void Kill()
    {
        if (IsRun) _process.Kill();
    }
}
