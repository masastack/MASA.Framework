// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Process;

public interface IProcessProvider
{
    /// <summary>
    /// Get process collection based on process name
    /// </summary>
    /// <param name="processName"></param>
    /// <returns>Process collection</returns>
    IEnumerable<IProcess> GetProcesses(string processName);

    IProcess GetProcess(int pId);

    /// <summary>
    /// get available ports
    /// </summary>
    /// <param name="minPort">Minimum port (includes minimum port), default: 0</param>
    /// <param name="maxPort">Maximum ports (including maximum ports), default: 65535</param>
    /// <returns></returns>
    int GetAvailablePorts(ushort? minPort = null, ushort? maxPort = null);

    /// <summary>
    /// Is the port available
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    bool IsAvailablePorts(ushort port);

    List<int> GetPidByPort(ushort port);
}
