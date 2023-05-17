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
    /// Key for global encryption config object
    /// </summary>
    public string? ConfigObjectSecret { get; set; }

    /// <summary>
    /// Whether to enable public configuration
    /// default: true
    /// </summary>
    public bool EnablePublicConfig { get; set; } = true;

    /// <summary>
    /// Public configuration information
    /// default: null
    /// </summary>
    public PublicConfigOptions? PublicConfig { get; set; }

    public DccSectionOptions DefaultSection { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; }

    public DccConfigurationOptions() => ExpandSections = new();

    private List<DccSectionOptions>? _allAvailabilitySections;

    public List<DccSectionOptions> GetAllAvailabilitySections()
    {
        if (_allAvailabilitySections != null)
            return _allAvailabilitySections;

        var sections = new List<DccSectionOptions>(ExpandSections)
        {
            DefaultSection
        };
        if (PublicConfig != null) sections.Add(PublicConfig);

        _allAvailabilitySections = sections
            .Where(section =>
                !section.AppId.IsNullOrWhiteSpace() &&
                !section.Environment.IsNullOrWhiteSpace() &&
                !section.Cluster.IsNullOrWhiteSpace())
            .ToList();
        return _allAvailabilitySections;
    }
}
