// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public class ProcessProvider : IProcessProvider
{
    private readonly ILogger<ProcessProvider>? _logger;

    public ProcessProvider(ILogger<ProcessProvider>? logger = null)
    {
        _logger = logger;
    }

    public IProcess GetProcess(int pId)
        => new SystemProcess(Process.GetProcessById(pId));

    /// <summary>
    /// Is the port available
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public bool IsAvailablePorts(ushort port)
        => !GetPortsByUsed().Contains(port);

    public List<int> GetPidByPort(ushort port)
    {
        List<int> pIdList = new();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            pIdList = GetPidByPortByWindows(port);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            pIdList = GetPidByPortByLinux(port);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            pIdList = GetPidByPortByOsx(port);
        else
        {
            _logger?.LogError("unsupported operating system");
        }

        return pIdList.Where(pid => pid > 0).ToList();
    }

    private static List<int> GetPidByPortByWindows(ushort port)
    {
        List<int> pIdList = new();
        List<string> output = GetResponse("netstat", $"-a -n -o", "\r\n");
        foreach (var line in output)
        {
            if (line.Trim().StartsWith("Proto") || line.Trim().StartsWith("协议"))
                continue;

            var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var len = parts.Length;
            if (len > 2)
            {
                var pId = int.Parse(parts[len - 1].Split('/')[0]);
                if (int.Parse(parts[1].Split(':').Last()) == port && !pIdList.Contains(pId))
                {
                    pIdList.Add(pId);
                }
            }
        }
        return pIdList;
    }

    private List<int> GetPidByPortByLinux(ushort port)
    {
        List<int> pIdList = new();
        List<string> output = GetResponse("netstat", $"-tunlp", "\n");

        _logger?.LogDebug("{Result} by netstat on linux", System.Text.Json.JsonSerializer.Serialize(output));
        var index = 0;
        foreach (var line in output)
        {
            index++;
            _logger?.LogDebug("the {Index}nth record: {Result} by netstat on linux", index, line);
            if (!line.Trim().StartsWith("tcp", StringComparison.OrdinalIgnoreCase) &&
                !line.Trim().StartsWith("udp", StringComparison.OrdinalIgnoreCase))
                continue;

            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var len = parts.Length;
            if (len > 2)
            {
                var pId = int.Parse(parts[GetIndex(parts, "LISTEN") + 1].Split('/')[0]);
                if (int.Parse(parts[3].Split(':').Last()) == port && !pIdList.Contains(pId))
                {
                    pIdList.Add(pId);
                }
            }
        }
        return pIdList;
    }

    private static int GetIndex(string[] array, string content)
    {
        for (var index = 0; index < array.Length; index++)
        {
            if (array[index].Equals(content, StringComparison.OrdinalIgnoreCase))
                return index;
        }
        return 0;
    }

    private List<int> GetPidByPortByOsx(ushort port)
    {
        List<int> pIdList = new();
        List<string> output = GetResponse("lsof", $"-nP -iTCP -sTCP:LISTEN", "\n");

        _logger?.LogDebug("{Result} by netstat on OSX", System.Text.Json.JsonSerializer.Serialize(output));
        var index = 0;
        foreach (var line in output)
        {
            index++;
            _logger?.LogDebug("the {Index}nth record: {Result} by netstat on OSX", index, line);
            if (line.Trim().StartsWith("COMMAND", StringComparison.OrdinalIgnoreCase))
                continue;

            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var len = parts.Length;
            if (len > 2)
            {
                var pId = int.Parse(parts[1]);
                if (int.Parse(parts[parts.Length - 2].Split(':').Last()) == port && !pIdList.Contains(pId))
                {
                    pIdList.Add(pId);
                }
            }
        }
        return pIdList;
    }

#pragma warning disable S6444
    private static List<string> GetResponse(string fileName, string arguments, string pattern)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        return Regex.Split(output, pattern).ToList();
    }
#pragma warning restore S6444

    /// <summary>
    /// get the currently used port
    /// </summary>
    /// <returns>Port set that has been used</returns>
    private static IEnumerable<int> GetPortsByUsed()
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var connectionEndPoints = ipGlobalProperties.GetActiveTcpConnections().Select(information => information.LocalEndPoint);
        var tcpListenerEndPoints = ipGlobalProperties.GetActiveTcpListeners();
        var udpListenerEndPoints = ipGlobalProperties.GetActiveUdpListeners();
        return connectionEndPoints
            .Concat(tcpListenerEndPoints)
            .Concat(udpListenerEndPoints)
            .Select(endPoint => endPoint.Port);
    }
}
