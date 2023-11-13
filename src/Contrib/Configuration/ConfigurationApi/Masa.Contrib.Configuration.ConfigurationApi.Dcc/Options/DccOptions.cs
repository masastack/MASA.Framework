// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccOptions : DccSectionOptions
{
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

    public static implicit operator DccConfigurationOptions(DccOptions options)
    {
        var dccConfigurationOptions = new DccConfigurationOptions()
        {
            ManageServiceAddress = options.ManageServiceAddress,
            SubscribeKeyPrefix = options.SubscribeKeyPrefix,
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
        return dccConfigurationOptions;
    }
}
