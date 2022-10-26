// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters;

public class DaprProvider : IDaprProvider
{
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger<DaprProvider>? _logger;

    public DaprProvider(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<DaprProvider>();
    }

    public List<DaprRuntimeOptions> GetDaprList(string appId)
    {
        var processUtils = new ProcessUtils(_loggerFactory);
        processUtils.Exit += delegate
        {
            _logger?.LogDebug("{Name} process has exited", Constant.DEFAULT_FILE_NAME);
        };
        processUtils.Run(Constant.DEFAULT_FILE_NAME, "list -o json", out string response, true, true);
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
            _logger?.LogWarning(exception, "----- Error getting list of running dapr, response message is {response}", response);
            return new List<DaprRuntimeOptions>();
        }
        return daprList.Where(dapr => dapr.AppId == appId).ToList();
    }

    public Process DaprStart(string arguments,
        bool createNoWindow,
        Action<object?, DataReceivedEventArgs> outputDataReceivedAction,
        Action exitAction)
    {
        var processUtils = new ProcessUtils(_loggerFactory);

        processUtils.OutputDataReceived += delegate(object? sender, DataReceivedEventArgs args)
        {
            outputDataReceivedAction.Invoke(sender, args);
            DaprProcess_OutputDataReceived(sender, args);
        };
        processUtils.ErrorDataReceived += DaprProcess_ErrorDataReceived;
        processUtils.Exit += delegate
        {
            exitAction.Invoke();
            _logger?.LogDebug("{Name} process has exited", Constant.DEFAULT_FILE_NAME);
        };
        return processUtils.Run(Constant.DEFAULT_FILE_NAME, $"run {arguments}", createNoWindow);
    }

    private static void DaprProcess_OutputDataReceived(object? sender, DataReceivedEventArgs e)
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
            default:
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

    public void DaprStop(string appId)
    {
        var process = Process.Start(@$"{Constant.DEFAULT_FILE_NAME}", $"stop {appId}");
        process.WaitForExit();
    }

    public bool IsExist(string appId)
    {
        return GetDaprList(appId).Any();
    }
}
