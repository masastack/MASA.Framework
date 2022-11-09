// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class I18NResource
{
    private readonly Dictionary<string, II18NResourceContributor> _dictionary;

    public Type ResourceType { get; }

    public IEnumerable<Type> BaseResourceTypes { get; }

    public IEnumerable<Assembly> Assemblies { get; set; } = new List<Assembly>();

    public I18NResource(Type resourceType, IEnumerable<Type> baseResourceTypes)
    {
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
        ResourceType = resourceType;
        BaseResourceTypes = baseResourceTypes;
    }

    public void AddContributor(string cultureName, II18NResourceContributor localizationResourceContributor)
    {
        if (_dictionary.ContainsKey(cultureName))
            throw new ArgumentException($"The {cultureName} already exists with {ResourceType.FullName}");

        _dictionary.Add(localizationResourceContributor.CultureName, localizationResourceContributor);
    }

    public II18NResourceContributor? GetResourceContributor(CultureInfo? cultureInfo = null)
    {
        var cultureName = cultureInfo?.Name ?? CultureInfo.CurrentUICulture.Name;

        if (_dictionary.ContainsKey(cultureName))
            return _dictionary[cultureName];

        return null;
    }
}
