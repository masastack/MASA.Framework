// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccConfigurationOptions
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
    public string? PublicId { get; set; }

    public string? PublicSecret { get; set; }

    public DccSectionOptions DefaultSection { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; }

    public DccConfigurationOptions()
    {
        ExpandSections = new();
    }

    public IEnumerable<DccSectionOptions> GetAllSections() => new List<DccSectionOptions>()
    {
        DefaultSection
    }.Concat(ExpandSections);
}
