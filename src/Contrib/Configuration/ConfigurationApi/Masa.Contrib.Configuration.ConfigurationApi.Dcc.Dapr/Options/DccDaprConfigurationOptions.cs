// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Options;

public class DccDaprConfigurationOptions
{
    public string StoreName { get; set; } = default!;

    public string? ConfigObjectSecret { get; set; }

    public string ManageServiceAddress { get; set; } = default!;

    public string? PublicId { get; set; }

    public string? PublicSecret { get; set; }

    public DccSectionOptions DefaultSection { get; set; } = new();

    public List<DccSectionOptions> ExpandSections { get; set; } = new();

    public IEnumerable<DccSectionOptions> GetAllSections() => new List<DccSectionOptions>
    {
        DefaultSection
    }.Concat(ExpandSections);
}
