// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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

    public void ComplementConfigObjects(IDistributedCacheClient distributedCacheClient)
    {
        if (!ConfigObjects.Any())
        {
            ConfigObjects = distributedCacheClient.GetAllConfigObjects(AppId, Environment, Cluster);
        }
    }

    public void ComplementAndCheckAppId(string defaultValue, bool isCheck = true)
    {
        if (IsSetValue(isCheck, AppId)) AppId = defaultValue;
    }

    public void ComplementAndCheckEnvironment(string defaultValue, bool isCheck = true)
    {
        if (IsSetValue(isCheck, Environment)) Environment = defaultValue;
    }

    public void ComplementAndCheckCluster(string defaultValue, bool isCheck = true)
    {
        if (IsSetValue(isCheck, Cluster)) Cluster = defaultValue;
    }

    private static bool IsSetValue(bool isCheck, string? value) => !isCheck || string.IsNullOrWhiteSpace(value);
}
