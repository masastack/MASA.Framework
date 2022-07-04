// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccSectionOptions
{
    /// <summary>
    /// The environment name.
    /// Get from the environment variable ASPNETCORE_ENVIRONMENT when Environment is null or empty
    /// </summary>
    public string? Environment { get; set; } = null;

    /// <summary>
    /// The cluster name.
    /// </summary>
    public string? Cluster { get; set; }

    /// <summary>
    /// The app id.
    /// </summary>
    public string AppId { get; set; } = default!;

    public List<string> ConfigObjects { get; set; } = default!;

    public string BizId { get; set; } = default!;

    public List<string> BizConfigObjects { get; set; } = default!;

    public string PublicId { get; set; } = default!;

    public List<string> PublicConfigObjects { get; set; } = default!;

    public string? Secret { get; set; }
}
