// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public class MasaStringLocalizer<TResourceSource> : IMasaStringLocalizer<TResourceSource>
{
    private readonly LocalizationResource? _localizationResource;
    private readonly IStringLocalizer? _stringLocalizer;
    private readonly string _defaultCultureName;
    public virtual string this[string name] => GetLocalizedString(name);

    public virtual string this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);

            var value = this[name];
            return string.Format(value, arguments);
        }
    }

    public MasaStringLocalizer(IStringLocalizerFactory? stringLocalizerFactory, IOptions<MasaLocalizationOptions> options)
    {
        _stringLocalizer = stringLocalizerFactory?.Create(typeof(TResourceSource));
        _localizationResource = options.Value.Resources.GetOrNull<TResourceSource>();
        _defaultCultureName = options.Value.DefaultCultureName;
    }

    private string GetLocalizedString(string name)
    {
        var resourceContributor = _localizationResource?.GetResourceContributor();
        if (resourceContributor != null)
        {
            return resourceContributor.GetOrNull(name) ?? GetLocalizedStringByDefaultCultureName(name);
        }
        if (_stringLocalizer != null)
        {
            return _stringLocalizer.GetString(name);
        }
        throw new NotImplementedException();
    }

    private string GetLocalizedStringByDefaultCultureName(string name)
    {
        var resourceContributor = _localizationResource!.GetResourceContributor(new CultureInfo(_defaultCultureName));
        if (resourceContributor != null)
        {
            return resourceContributor.GetOrNull(name) ?? name;
        }
        throw new NotImplementedException();
    }
}
