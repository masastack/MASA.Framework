// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Process;

public class ProcessProvider : IProcessProvider
{
    private readonly ILogger<ProcessProvider>? _logger;

    public ProcessProvider(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ProcessProvider>();
    }

    /// <summary>
    /// Get process collection based on process name
    /// </summary>
    /// <param name="processName"></param>
    /// <returns></returns>
    public IEnumerable<IProcess> GetProcesses(string processName)
        => System.Diagnostics.Process.GetProcessesByName(processName).Select(process => new SystemProcess(process));

    public IProcess GetProcess(int pId)
        => new SystemProcess(System.Diagnostics.Process.GetProcessById(pId));

    /// <summary>
    /// get available ports
    /// </summary>
    /// <param name="minPort">Minimum port (includes minimum port), default: 0</param>
    /// <param name="maxPort">Maximum ports (including maximum ports), default: 65535</param>
    /// <returns></returns>
    public int GetAvailablePorts(ushort? minPort = null, ushort? maxPort = null)
    {
        minPort ??= ushort.MinValue;
        maxPort ??= ushort.MaxValue;
        var usePorts = GetPortsByUsed();

        var effectivePorts = Enumerable.Range(minPort.Value, maxPort.Value).Except(usePorts).ToList();
        if (effectivePorts.Count == 0)
            throw new MasaException("... No port available exception");

        return effectivePorts.FirstOrDefault();
    }

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
        {
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
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            List<string> output = GetResponse("netstat", $"-tunlp", "\n");

            Console.WriteLine("result: " + output.Count);
            Console.WriteLine("result2: " + System.Text.Json.JsonSerializer.Serialize(output));
            foreach (var line in output)
            {
                Console.WriteLine("line: " + line);
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
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            List<string> output = GetResponse("lsof", $"-nP -iTCP -sTCP:LISTEN", "\n");

            Console.WriteLine("result: " + output.Count);
            Console.WriteLine("result2: " + System.Text.Json.JsonSerializer.Serialize(output));
            foreach (var line in output)
            {
                Console.WriteLine("line: " + line);
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
        }
        else
        {
            _logger?.LogError("unsupported operating system");
        }
        return pIdList.Where(pid => pid > 0).ToList();
    }

    private int GetIndex(string[] array, string content)
    {
        for (var index = 0; index < array.Length; index++)
        {
            if (array[index].Equals(content, StringComparison.OrdinalIgnoreCase))
                return index;
        }
        return 0;
    }

    private List<string> GetResponse(string fileName, string arguments, string pattern)
    {
        var process = new System.Diagnostics.Process()
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

    /// <summary>
    /// get the currently used port
    /// </summary>
    /// <returns>Port set that has been used</returns>
    private IEnumerable<int> GetPortsByUsed()
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
