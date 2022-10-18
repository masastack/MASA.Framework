// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Localization;

public class LocalizationResource
{
    private readonly Dictionary<string, ILocalizationResourceContributor> _dictionary;

    public Type ResourceType { get; }

    public LocalizationResource(Type resourceType)
    {
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
        ResourceType = resourceType;
    }

    public void AddContributor(string cultureName,ILocalizationResourceContributor localizationResourceContributor)
    {
        if (_dictionary.ContainsKey(cultureName))
            throw new ArgumentException($"The {cultureName} already exists with {ResourceType.FullName}");

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
