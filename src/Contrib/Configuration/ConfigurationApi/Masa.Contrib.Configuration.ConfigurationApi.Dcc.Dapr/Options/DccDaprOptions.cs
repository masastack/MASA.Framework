// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Options;

public class DccDaprOptions : DccSectionOptions
{
    public string StoreName { get; set; }

    /// <summary>
    /// Key for global encryption config object
    /// </summary>
    public string? ConfigObjectSecret { get; set; }

    public string ManageServiceAddress { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; }

    public IEnumerable<DccSectionOptions> GetAllSections() => new List<DccSectionOptions>()
    {
        this
    }.Concat(ExpandSections);
}
