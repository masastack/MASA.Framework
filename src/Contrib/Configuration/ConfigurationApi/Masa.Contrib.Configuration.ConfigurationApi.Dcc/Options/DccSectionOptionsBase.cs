// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public abstract class DccSectionOptionsBase
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

    public List<string> ConfigObjects { get; set; } = new();

    public string? Secret { get; set; }
}
