// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class DefaultLanguageProvider : ILanguageProvider
{
    private readonly II18N<MasaLanguageResource> _i18N;
    private readonly IOptions<CultureSettings> _options;

    public DefaultLanguageProvider(II18N<MasaLanguageResource> i18N, IOptions<CultureSettings> options)
    {
        _i18N = i18N;
        _options = options;
    }

    public IReadOnlyList<LanguageInfo> GetLanguages()
    {
        var list = new List<LanguageInfo>();
        string cultureName = _i18N.GetCultureInfo().Name;
        _options.Value.SupportedCultures.ForEach(culture =>
        {
            string key = culture.Culture.Equals(cultureName, StringComparison.OrdinalIgnoreCase) ?
                nameof(LanguageInfo.UIDisplayName) :
                $"{culture.Culture}\\.{nameof(LanguageInfo.UIDisplayName)}";
            var uiDisplayName = _i18N.T(key);
            var languageInfo = new LanguageInfo(culture.Culture, culture.DisplayName, culture.Icon)
            {
                UIDisplayName = uiDisplayName
            };

            list.Add(languageInfo);
        });
        return list;
    }
}
