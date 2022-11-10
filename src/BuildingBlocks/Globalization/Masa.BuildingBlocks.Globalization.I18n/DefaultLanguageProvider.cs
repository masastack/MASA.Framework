// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class DefaultLanguageProvider : ILanguageProvider
{
    private readonly II18N<MasaLanguageResource> _i18N;
    private readonly IOptions<CultureSettings> _options;

    private static readonly Dictionary<string, string> _languages = new()
    {
        { "en-US", "English (United States)" },
        { "zh-CN", "中文 (简体)" }
    };

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
            var languageInfo = new LanguageInfo(culture.Culture, culture.DisplayName ?? GetDisplayName(cultureName), culture.Icon)
            {
                UIDisplayName = uiDisplayName
            };

            list.Add(languageInfo);
        });
        return list;
    }

    private static string GetDisplayName(string cultureName)
    {
        if (_languages.TryGetValue(cultureName, out string? displayName))
            return displayName;

        return string.Empty;
    }
}
