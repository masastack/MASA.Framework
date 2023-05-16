// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

/// <summary>
/// Configuration information required for using DCC
/// </summary>
public class DccOptions : DccSectionOptions
{
    public RedisConfigurationOptions RedisOptions { get; set; } = new();

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }

    /// <summary>
    /// public config id
    /// </summary>
    public string? PublicId { get; set; } = default!;

    public string? PublicSecret { get; set; }

    /// <summary>
    /// Key for global encryption config object
    /// </summary>
    public string? ConfigObjectSecret { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; } = new();
}
