// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class ConfigurationApiBase
{
    private readonly DccConfigurationOptions _dccConfigurationOptions;

    protected ConfigurationApiBase(DccConfigurationOptions dccConfigurationOptions)
    {
        _dccConfigurationOptions = dccConfigurationOptions;
    }

    protected string GetSecret(string appId)
    {
        return _dccConfigurationOptions
            .GetAllAvailabilitySections()
            .Where(options => options.AppId == appId)
            .Select(options => options.Secret)
            .FirstOrDefault() ?? string.Empty;
    }

    protected string GetEnvironment(string? environment = null)
        => !string.IsNullOrEmpty(environment) ? environment : _dccConfigurationOptions.DefaultSection.Environment!;

    protected string GetCluster(string? cluster = null)
        => !string.IsNullOrEmpty(cluster) ? cluster : _dccConfigurationOptions.DefaultSection.Cluster!;

    protected string GetAppId(string? appId = null)
        => !string.IsNullOrEmpty(appId) ? appId : _dccConfigurationOptions.DefaultSection.AppId!;

    protected string GetConfigObject(string configObject)
        => !string.IsNullOrEmpty(configObject) ? configObject : throw new ArgumentNullException(nameof(configObject));
}
