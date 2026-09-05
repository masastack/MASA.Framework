// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Options;

public class DccDaprOptions : DccSectionOptions
{
    public string StoreName { get; set; } = default!;

    /// <summary>
    /// Key for global encryption config object
    /// </summary>
    public string? ConfigObjectSecret { get; set; }

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// public config id
    /// </summary>
    public string? PublicId { get; set; }

    public string? PublicSecret { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; } = new();

    public static implicit operator DccDaprConfigurationOptions(DccDaprOptions options)
    {
        return new DccDaprConfigurationOptions
        {
            StoreName = options.StoreName,
            ConfigObjectSecret = options.ConfigObjectSecret,
            ManageServiceAddress = options.ManageServiceAddress,
            PublicId = options.PublicId,
            PublicSecret = options.PublicSecret,
            DefaultSection = new DccSectionOptions(
                options.AppId,
                options.Environment,
                options.Cluster,
                options.ConfigObjects,
                options.Secret),
            ExpandSections = options.ExpandSections.Select(section => new DccSectionOptions(
                section.AppId,
                section.Environment,
                section.Cluster,
                section.ConfigObjects,
                section.Secret)).ToList()
        };
    }
}
