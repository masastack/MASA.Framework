// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

internal class DefaultIntegrationEventDaprProvider : IIntegrationEventDaprProvider
{
    private readonly IOptions<MasaAppConfigureOptions>? _options;
    private readonly IConfiguration? _configuration;
    private readonly IMasaConfiguration? _masaConfiguration;

    public DefaultIntegrationEventDaprProvider(
        IOptions<MasaAppConfigureOptions>? options = null,
        IConfiguration? configuration = null,
        IMasaConfiguration? masaConfiguration = null)
    {
        _options = options;
        _configuration = configuration;
        _masaConfiguration = masaConfiguration;
    }

    public string? GetDaprAppId(string? appId)
    {
        string? actualAppId = null;
        if (!appId.IsNullOrWhiteSpace())
            actualAppId = ConfigurationUtils.CompletionParameter(appId, _configuration, _masaConfiguration);
        if (!actualAppId.IsNullOrWhiteSpace())
            return actualAppId;

        actualAppId = Environment.GetEnvironmentVariable(DaprStarterConstant.DEFAULT_DAPR_APPID);
        if (!actualAppId.IsNullOrWhiteSpace())
            return actualAppId;

        if (_options != null && !_options.Value.AppId.IsNullOrWhiteSpace())
            actualAppId = ConfigurationUtils.CompletionParameter(_options.Value.AppId, _configuration, _masaConfiguration);

        return actualAppId;
    }
}
