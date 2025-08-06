// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient;

public class DefaultCallerProvider : ICallerProvider
{
    private readonly IOptionsMonitor<DaprOptions> _daprOptions;
    private readonly IConfiguration? _configuration;
    private readonly IMasaConfiguration? _masaConfiguration;

    public DefaultCallerProvider(IOptionsMonitor<DaprOptions> daprOptions,
        IConfiguration? configuration = null,
        IMasaConfiguration? masaConfiguration = null)
    {
        _daprOptions = daprOptions;
        _configuration = configuration;
        _masaConfiguration = masaConfiguration;
    }

    public string CompletionAppId(string appId)
    {
        var daprOptions = _daprOptions.CurrentValue;
        if (daprOptions.AppPort > 0 && daprOptions.IsIncompleteAppId())
            appId = $"{appId}{daprOptions.AppIdDelimiter}{daprOptions.AppIdSuffix ?? NetworkUtils.GetPhysicalAddress()}";

        var value = _configuration == null ? Environment.GetEnvironmentVariable(appId) : _configuration?.GetSection(appId).Value;
        if (value.IsNullOrWhiteSpace())
            value = _masaConfiguration?.Local.GetSection(appId).Value;

        if (value.IsNullOrWhiteSpace()) return appId;

        return value;
    }
}
