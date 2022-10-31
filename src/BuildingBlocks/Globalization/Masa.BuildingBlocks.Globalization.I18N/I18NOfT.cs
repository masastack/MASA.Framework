// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class I18N<TResourceSource> : II18N<TResourceSource>
{
    private readonly I18NResource? _resource;

    public string this[string name] => T(name);

    public string? this[string name, bool returnKey] => T(name, returnKey);

    public string this[string name, params object[] arguments] => T(name, arguments);

    public string? this[string name, bool returnKey, params object[] arguments] => T(name, returnKey, arguments);

    public I18N()
    {
        _resource = I18NResourceResourceConfiguration.Resources.GetOrNull<TResourceSource>();
    }

    public virtual string T(string name)
        => T(name, true)!;

    /// <summary>
    /// Gets the string resource with the given name.
    /// </summary>
    /// <param name="name">The name of the string resource.</param>
    /// <param name="returnKey">Return Key when key does not exist, default: true</param>
    /// <returns></returns>
    public virtual string? T(string name, bool returnKey)
    {
        var resourceContributor = _resource?.GetResourceContributor(GetCultureInfo());
        if (resourceContributor != null)
        {
            return resourceContributor.GetOrNull(name) ?? (returnKey ? name : null);
        }

        return returnKey ? name : null;
    }

    public virtual string T(string name, params object[] arguments)
        => T(name, true, arguments)!;

    public virtual string? T(string name, bool returnKey, params object[] arguments)
    {
        ArgumentNullException.ThrowIfNull(name);

        var value = this.T(name, returnKey);
        if (value != null)
            return string.Format(GetCultureInfo(), value, arguments);

        return null;
    }

    public virtual CultureInfo GetCultureInfo() => CultureInfo.CurrentUICulture;

    public virtual void SetCulture(string cultureName, bool useUserOverride = true)
        => SetCulture(new CultureInfo(cultureName, useUserOverride));

    public virtual void SetCulture(CultureInfo culture)
    {
        CultureInfo.CurrentUICulture = culture;
    }
}
