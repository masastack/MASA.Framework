// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public class MasaStringLocalizer<TResourceSource> : IMasaStringLocalizer<TResourceSource>
{
    private readonly LocalizationResource? _localizationResource;
    public string this[string name] => GetLocalizedString(name);

    public string this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);

            var value = this[name];
            return string.Format(CultureInfo.CurrentUICulture, value, arguments);
        }
    }

    public string T(string name) => this[name];

    public string T(string name, params object[] arguments) => this[name, arguments];

    public MasaStringLocalizer(IOptions<MasaLocalizationOptions> options)
    {
        _localizationResource = options.Value.Resources.GetOrNull<TResourceSource>();
    }

    private string GetLocalizedString(string name)
    {
        var resourceContributor = _localizationResource?.GetResourceContributor();
        if (resourceContributor != null)
        {
            return resourceContributor.GetOrNull(name) ?? GetLocalizedStringByDefaultCultureName(name);
        }
        throw new NotImplementedException();
    }

    private string GetLocalizedStringByDefaultCultureName(string name)
    {
        if (!LocalizationResourceConfiguration.DefaultCultureName.IsNullOrWhiteSpace())
        {
            var resourceContributor =
                _localizationResource!.GetResourceContributor(new CultureInfo(LocalizationResourceConfiguration.DefaultCultureName));
            return resourceContributor?.GetOrNull(name) ?? name;
        }
        return name;
    }
}
