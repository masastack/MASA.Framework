// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Configuration;

internal static class ConfigurationUtils
{
    /// <summary>
    /// Complete the value of the parameter
    /// the value of the specified parameter will be obtained sequentially from the environment variable and configuration information, if the acquisition fails, the initial value will be returned
    /// </summary>
    /// <param name="initialParameter">parameter initial value</param>
    /// <param name="configuration"></param>
    /// <param name="masaConfiguration"></param>
    /// <returns></returns>
    public static string CompletionParameter(
        string initialParameter,
        IConfiguration? configuration = null,
        IMasaConfiguration? masaConfiguration = null)
    {
        var value = configuration == null
            ? Environment.GetEnvironmentVariable(initialParameter)
            : configuration?.GetSection(initialParameter).Value;
        if (string.IsNullOrWhiteSpace(value))
            value = masaConfiguration?.Local.GetSection(initialParameter).Value;

        if (string.IsNullOrWhiteSpace(value)) return initialParameter;
        return value;
    }
}
