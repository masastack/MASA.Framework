// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class LocalizationResource
{
    private List<ILocalizationResourceContributor> _dictionary { get; }

    public Type ResourceType { get; }

    public string? DefaultCultureName { get; }

    public LocalizationResource(Type resourceType, string? defaultCultureName)
    {
        _dictionary = new();
        ResourceType = resourceType;
        DefaultCultureName = defaultCultureName;
    }

    public void AddContributor(ILocalizationResourceContributor localizationResourceContributor)
    {
        if (_dictionary.Any(d => d.CultureName.Equals(localizationResourceContributor.CultureName, StringComparison.OrdinalIgnoreCase)))
            throw new Exception($"The {localizationResourceContributor.CultureName} already exists with {ResourceType.FullName}");

        _dictionary.Add(localizationResourceContributor);
    }
}
