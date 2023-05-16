// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal class DccConfigurationOptions
{
    public RedisConfigurationOptions RedisOptions { get; set; }

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }

    /// <summary>
    /// public config id
    /// </summary>
    internal string? PublicId { get; set; }

    internal string? PublicSecret { get; set; }

    /// <summary>
    /// Key for global encryption config object
    /// </summary>
    public string? ConfigObjectSecret { get; set; }

    public DccSectionOptions DefaultSection { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; }

    public DccConfigurationOptions() => ExpandSections = new();

    public IEnumerable<DccSectionOptions> GetAllAvailabilitySections()
    {
        var sections = new List<DccSectionOptions>(ExpandSections)
        {
            DefaultSection
        };
        return sections.Where(section =>
            !section.AppId.IsNullOrWhiteSpace() &&
            !section.Environment.IsNullOrWhiteSpace() &&
            !section.Cluster.IsNullOrWhiteSpace());
    }
}
