// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Process;

public interface IProcess
{
    int PId { get; }

    public string Name { get; }

    void Kill();

    bool Start();

    void WaitForExit(int? milliseconds = null);
}
