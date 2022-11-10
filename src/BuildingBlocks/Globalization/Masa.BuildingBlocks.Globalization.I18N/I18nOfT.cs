// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class I18N<TResourceSource> : II18N<TResourceSource>
{
    private readonly I18NResource? _resource;
    private readonly List<I18NResource?> _baseResources;

    public string this[string name] => T(name);

    public string? this[string name, bool returnKey] => T(name, returnKey);

    public string this[string name, params object[] arguments] => T(name, arguments);

    public string? this[string name, bool returnKey, params object[] arguments] => T(name, returnKey, arguments);

    public I18N()
    {
        _resource = I18NResourceResourceConfiguration.Resources.GetOrNull<TResourceSource>();

        _baseResources = _resource?.BaseResourceTypes
            .Select(resourceType => I18NResourceResourceConfiguration.Resources.GetOrNull(resourceType))
            .ToList() ?? new List<I18NResource?>();
    }

    public string T(string name) => T(name, true)!;

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <returns></returns>
    public string? T(string name, bool returnKey) => Core(name, returnKey, out _);

    public string T(string name, params object[] arguments)
        => T(name, true, arguments)!;

    public string? T(string name, bool returnKey, params object[] arguments)
    {
        ArgumentNullException.ThrowIfNull(name);

        var value = Core(name, returnKey, out bool isExist);

        if (isExist)
            return string.Format(GetCultureInfo(), value!, arguments);

        return returnKey ? name : null;
    }

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <param name="isExist">does it exist</param>
    /// <returns></returns>
    public string? Core(string name, bool returnKey, out bool isExist)
    {
        isExist = true;
        var value = GetOrNull(name);
        if (value == null)
        {
            foreach (var resource in _baseResources)
            {
                value = GetOrNull(resource, name);
                if (value != null)
                    return value;
            }
            isExist = false;
            return returnKey ? name : null;
        }
        return value;
    }

    public string? GetOrNull(string name) => GetOrNull(_resource, name);

    public string? GetOrNull(I18NResource? i18NResource, string name)
    {
        if (i18NResource == null)
            return null;

        var resourceContributor = i18NResource.GetResourceContributor(GetUiCultureInfo());
        return resourceContributor?.GetOrNull(name);
    }

    public CultureInfo GetCultureInfo() => CultureInfo.CurrentCulture;

    public void SetCulture(string cultureName, bool useUserOverride = true)
        => SetCulture(new CultureInfo(cultureName, useUserOverride));

    public void SetCulture(CultureInfo culture) => CultureInfo.CurrentCulture = culture;

    public CultureInfo GetUiCultureInfo() => CultureInfo.CurrentUICulture;

    public void SetUiCulture(string cultureName, bool useUserOverride = true)
        => SetUiCulture(new CultureInfo(cultureName, useUserOverride));

    public void SetUiCulture(CultureInfo culture) => CultureInfo.CurrentUICulture = culture;
}
