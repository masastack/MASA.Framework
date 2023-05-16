// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

public class CustomConfigurationApi : ConfigurationApiBase
{
    public CustomConfigurationApi(DccSectionOptions defaultSectionOption, List<DccSectionOptions>? expandSectionOptions) : base(
        defaultSectionOption, expandSectionOptions)
    {
    }

    public string GetSecretByTest(string appId) => base.GetSecret(appId);

    public string GetEnvironmentByTest(string environment) => base.GetEnvironment(environment);

    public string GetClusterByTest(string cluster) => base.GetCluster(cluster);

    public string GetAppIdByTest(string appId) => base.GetAppId(appId);

    public string GetConfigObjectByTest(string configObject) => base.GetConfigObject(configObject);
}
