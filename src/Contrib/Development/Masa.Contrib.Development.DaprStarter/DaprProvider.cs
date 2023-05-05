// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public class DaprProcessProvider : IDaprProcessProvider
{
    private readonly ILogger<DaprProcessProvider>? _logger;

    public DaprProcessProvider(ILogger<DaprProcessProvider>? logger = null)
    {
        _logger = logger;
    }

    public List<DaprRuntimeOptions> GetDaprList(string fileName, string appId, out bool isException)
    {
        isException = false;
        var processUtils = new ProcessUtils(_logger);
        processUtils.Exit += delegate
        {
            _logger?.LogDebug("{Name} process has exited, appid: {AppId}", DaprStarterConstant.DEFAULT_PROCESS_NAME, appId);
        };
        processUtils.Run(fileName, "list -o json", out string response, true, true);
        List<DaprRuntimeOptions> daprList = new();
        try
        {
            if (response.StartsWith("["))
            {
                daprList = System.Text.Json.JsonSerializer.Deserialize<List<DaprRuntimeOptions>>(response) ?? new();
            }
            else if (response.StartsWith("{"))
            {
                var option = System.Text.Json.JsonSerializer.Deserialize<DaprRuntimeOptions>(response);
                if (option != null)
                {
                    daprList.Add(option);
                }
            }
            else
            {
                _logger?.LogWarning("----- Failed to get currently running dapr");
            }
        }
        catch (Exception exception)
        {
            isException = true;
            _logger?.LogWarning(exception, "----- Error getting list of running dapr, response message is {Response}", response);
            return new List<DaprRuntimeOptions>();
        }

        return daprList.Where(dapr => dapr.AppId == appId).ToList();
    }

    public Process DaprStart(string fileName,
        string arguments,
        bool createNoWindow,
        Action<object?, DataReceivedEventArgs> outputDataReceivedAction,
        Action exitAction)
    {
        var processUtils = new ProcessUtils(_logger);

        processUtils.OutputDataReceived += delegate(object? sender, DataReceivedEventArgs args)
        {
            outputDataReceivedAction.Invoke(sender, args);
            DaprProcess_OutputDataReceived(args);
        };
        processUtils.ErrorDataReceived += DaprProcess_ErrorDataReceived;
        processUtils.Exit += delegate
        {
            exitAction.Invoke();
            _logger?.LogDebug("{Name} process has exited", DaprStarterConstant.DEFAULT_PROCESS_NAME);
        };
        return processUtils.Run(fileName, $" {arguments}", createNoWindow);
    }

    private static void DaprProcess_OutputDataReceived(DataReceivedEventArgs e)
    {
        if (e.Data == null) return;

        var dataSpan = e.Data.AsSpan();
        var levelStartIndex = e.Data.IndexOf("level=", StringComparison.Ordinal) + 6;
        var level = "information";
        if (levelStartIndex > 5)
        {
            var levelLength = dataSpan.Slice(levelStartIndex).IndexOf(' ');
            level = dataSpan.Slice(levelStartIndex, levelLength).ToString();
        }

        var color = Console.ForegroundColor;
        switch (level)
        {
            case "warning":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "error":
            case "critical":
            case "fatal":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
        }

        Console.WriteLine(e.Data);
        Console.ForegroundColor = color;
    }

    private static void DaprProcess_ErrorDataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data == null) return;

        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Data);
        Console.ForegroundColor = color;
    }

    public void DaprStop(string fileName, string appId)
    {
        var daprList = GetDaprList(fileName, appId, out _);

        var pidList = daprList.Select(dapr => dapr.PId).ToList();
        foreach (var pid in pidList)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error stopping dapr sidecar, appid is {AppId}, pid is {PId}", appId, pid);
            }
        }
    }
}
