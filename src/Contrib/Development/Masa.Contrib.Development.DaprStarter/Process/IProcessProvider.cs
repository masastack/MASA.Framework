// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

public interface IProcessProvider
{
    IProcess GetProcess(int pId);

    /// <summary>
    /// Is the port available
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    bool IsAvailablePorts(ushort port);

    List<int> GetPidByPort(ushort port);
}
