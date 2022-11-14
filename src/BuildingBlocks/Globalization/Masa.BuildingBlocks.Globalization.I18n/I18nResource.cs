// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public class I18nResource
{
    private readonly Dictionary<string, II18nResourceContributor> _dictionary;

    public Type ResourceType { get; }

    public List<Type> BaseResourceTypes { get; private set; }

    public IEnumerable<Assembly> Assemblies { get; set; } = new List<Assembly>();

    public I18nResource(Type resourceType, IEnumerable<Type> baseResourceTypes)
    {
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
        ResourceType = resourceType;
        BaseResourceTypes = baseResourceTypes.ToList();
    }

    public void AddContributor(string cultureName, II18nResourceContributor localizationResourceContributor)
    {
        if (_dictionary.ContainsKey(cultureName))
            throw new ArgumentException($"The {cultureName} already exists with {ResourceType.FullName}");

        _dictionary.Add(localizationResourceContributor.CultureName, localizationResourceContributor);
    }

    public II18nResourceContributor? GetResourceContributor(CultureInfo? cultureInfo = null)
    {
        var cultureName = cultureInfo?.Name ?? CultureInfo.CurrentUICulture.Name;

        if (_dictionary.ContainsKey(cultureName))
            return _dictionary[cultureName];

        return null;
    }

    public void TryAddBaseResourceTypes<TBaseResourceType>() where TBaseResourceType : class
        => TryAddBaseResourceTypes(typeof(TBaseResourceType));

    public void TryAddBaseResourceTypes(params Type[] resourceTypes)
    {
        foreach (var type in resourceTypes)
        {
            if (!BaseResourceTypes.Contains(type))
                BaseResourceTypes.Add(type);
        }
    }
}
