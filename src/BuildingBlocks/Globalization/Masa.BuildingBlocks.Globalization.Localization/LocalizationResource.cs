// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class LocalizationResource
{
    private Dictionary<string, ILocalizationResourceContributor> _dictionary { get; }

    public Type ResourceType { get; }

    public string? DefaultCultureName { get; internal set; }

    public LocalizationResource(Type resourceType, string? defaultCultureName)
    {
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
        ResourceType = resourceType;
        DefaultCultureName = defaultCultureName;
    }

    public void AddContributor(ILocalizationResourceContributor localizationResourceContributor)
    {
        if (_dictionary.Any(d => d.Key.Equals(localizationResourceContributor.CultureName, StringComparison.OrdinalIgnoreCase)))
            throw new Exception($"The {localizationResourceContributor.CultureName} already exists with {ResourceType.FullName}");

        _dictionary.Add(localizationResourceContributor.CultureName, localizationResourceContributor);
    }

    public ILocalizationResourceContributor? GetResourceContributor(CultureInfo? cultureInfo = null)
    {
        var cultureName = cultureInfo?.Name ?? CultureInfo.CurrentUICulture.Name;

        if (_dictionary.ContainsKey(cultureName))
            return _dictionary[cultureName];

        return null;
    }
}
