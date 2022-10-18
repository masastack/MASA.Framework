// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Extensions;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccSectionOptions
{
    /// <summary>
    /// The environment name.
    /// Get from the environment variable ASPNETCORE_ENVIRONMENT when Environment is null or empty
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// The cluster name.
    /// </summary>
    public string Cluster { get; set; } = string.Empty;

    /// <summary>
    /// The app id.
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    public List<string> ConfigObjects { get; set; } = new();

    public string? Secret { get; set; }

    public DccSectionOptions()
    {

    }

    public DccSectionOptions(string appId, string environment, string cluster, List<string> configObjects, string? secret) : this()
    {
        AppId = appId;
        Environment = environment;
        Cluster = cluster;
        ConfigObjects = configObjects;
        Secret = secret ?? string.Empty;
    }

    public void ComplementConfigObjects(IDistributedCacheClient distributedCacheClient)
    {
        if (!ConfigObjects.Any())
        {
            ConfigObjects = distributedCacheClient.GetAllConfigObjects(AppId, Environment, Cluster);
        }
    }

    public void ComplementAndCheckAppId(string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(AppId)) AppId = defaultValue;
    }

    public void ComplementAndCheckEnvironment(string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(Environment)) Environment = defaultValue;
    }

    public void ComplementAndCheckCluster(string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(Cluster)) Cluster = defaultValue;
    }
}
