// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccSectionOptions : DccSectionOptionsBase
{
    /// <summary>
    /// The app id.
    /// </summary>
    public string AppId { get; set; } = string.Empty;

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
}
