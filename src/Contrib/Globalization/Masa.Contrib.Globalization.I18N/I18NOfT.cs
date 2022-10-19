// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N;

public class I18N<TResourceSource> : II18N<TResourceSource>
{
    private readonly I18NResource? _resource;
    public string this[string name] => T(name);

    public string this[string name, params object[] arguments] => T(name, arguments);

    public I18N(IOptions<MasaI18NOptions> options)
    {
        _resource = options.Value.Resources.GetOrNull<TResourceSource>();
    }

    public string T(string name)
    {
        var resourceContributor = _resource?.GetResourceContributor(CultureInfo.CurrentUICulture);
        if (resourceContributor != null)
        {
            return resourceContributor.GetOrNull(name) ?? GetLocalizedStringByDefaultCultureName(name);
        }

        return name;
    }

    public string T(string name, params object[] arguments)
    {
        ArgumentNullException.ThrowIfNull(name);

        var value = this[name];
        return string.Format(CultureInfo.CurrentUICulture, value, arguments);
    }

    public void SetCulture(string cultureName, bool useUserOverride = true)
        => SetCulture(new CultureInfo(cultureName, useUserOverride));

    public virtual void SetCulture(CultureInfo culture)
    {
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private string GetLocalizedStringByDefaultCultureName(string name)
    {
        if (!I18NResourceResourceConfiguration.DefaultCultureName.IsNullOrWhiteSpace())
        {
            var resourceContributor =
                _resource!.GetResourceContributor(new CultureInfo(I18NResourceResourceConfiguration.DefaultCultureName));
            return resourceContributor?.GetOrNull(name) ?? name;
        }
        return name;
    }
}
