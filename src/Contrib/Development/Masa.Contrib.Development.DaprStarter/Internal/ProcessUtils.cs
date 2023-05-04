// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
internal sealed class ProcessUtils
{
    private readonly ILogger? _logger;

    public ProcessUtils(ILogger? logger)
    {
        _logger = logger;
    }

    public Process Run(
        string fileName,
        string arguments,
        bool createNoWindow = true,
        bool isWait = false)
        => Run(fileName, arguments, out string _, createNoWindow, isWait);

    public Process Run(
        string fileName,
        string arguments,
        out string response,
        bool createNoWindow = true,
        bool isWait = false)
    {
        _logger?.LogDebug("FileName: {FileName}, Arguments: {Arguments}", fileName, arguments);
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = createNoWindow
        };
        var daprProcess = new Process()
        {
            StartInfo = processStartInfo,
        };
        if (createNoWindow)
        {
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;

            if (!isWait)
            {
                daprProcess.OutputDataReceived += (_, args) => OnOutputDataReceived(args);
                daprProcess.ErrorDataReceived += (_, args) => OnErrorDataReceived(args);
            }
        }

        daprProcess.Start();
        if (createNoWindow && !isWait)
        {
            daprProcess.BeginOutputReadLine();
            daprProcess.BeginErrorReadLine();
        }

        daprProcess.Exited += (_, _) => OnExited();
        string command = $"{fileName} {arguments}";
        _logger?.LogDebug("Process: {ProcessName}, Command: {Command}, PID: {ProcessId} executed successfully", fileName,
            command, daprProcess.Id);

        if (isWait)
        {
            response = daprProcess.StandardOutput.ReadToEnd();
            daprProcess.WaitForExit();
        }
        else
        {
            response = string.Empty;
        }

        return daprProcess;
    }

    public event EventHandler<DataReceivedEventArgs> OutputDataReceived = default!;

    public event EventHandler<DataReceivedEventArgs>? ErrorDataReceived;

    public event EventHandler Exit = default!;

    private void OnOutputDataReceived(DataReceivedEventArgs args)
    {
        try
        {
            OutputDataReceived(this, args);
        }
        catch (Exception ex)
        {
            _logger?.LogError("ProcessUtils: error in output information ", ex);
        }
    }

    private void OnErrorDataReceived(DataReceivedEventArgs args)
    {
        try
        {
            ErrorDataReceived?.Invoke(this, args);
        }
        catch (Exception ex)
        {
            _logger?.LogError("execution error", ex);
        }
    }

    private void OnExited() => Exit(this, EventArgs.Empty);
}
