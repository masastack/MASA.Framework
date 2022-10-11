// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr;

public class DaprProvider : IDaprProvider
{
    private readonly ILogger<DaprProvider>? _logger;
    private readonly ProcessUtils _processUtils;

    public DaprProvider(ILoggerFactory? loggerFactory)
    {
        _logger = loggerFactory?.CreateLogger<DaprProvider>();
        _processUtils = new ProcessUtils(loggerFactory);
    }

    public List<DaprRuntimeOptions> GetDaprList(string appId)
    {
        _processUtils.Exit += delegate
        {
            _logger?.LogDebug("{Name} process has exited", Const.DEFAULT_FILE_NAME);
        };
        _processUtils.Run(Const.DEFAULT_FILE_NAME, "list -o json", out string response, true, true);
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

    public void DaprStop(string appId)
    {
        var process = System.Diagnostics.Process.Start(@$"{Const.DEFAULT_FILE_NAME}", $"stop {appId}");
        process.WaitForExit();
    }

    public bool IsExist(string appId)
    {
        return GetDaprList(appId).Any();
    }
}
