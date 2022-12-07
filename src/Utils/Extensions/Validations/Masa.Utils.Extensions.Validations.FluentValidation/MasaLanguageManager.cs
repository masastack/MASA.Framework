// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

public class MasaLanguageManager : LanguageManager
{
    private static readonly string[] SupportLanguages = new[] { "en", "en-US", "en-GB", "zh-CN" };

    public MasaLanguageManager()
    {
        foreach (var language in SupportLanguages)
        {
            var languageProvider = GetLanguageProvider(language);
            if (languageProvider == null)
                continue;

            AddTranslation(language, nameof(ChineseValidator<string>),languageProvider.GetTranslation(nameof(ChineseValidator<string>)));
            AddTranslation(language, nameof(NumberValidator<string>),languageProvider.GetTranslation(nameof(NumberValidator<string>)));
        }
    }

    private ILanguageProvider? GetLanguageProvider(string language) => language switch
    {
        "en" => new EnglishLanguage(),
        "en-US" => new EnglishLanguage(),
        "en-GB" => new EnglishLanguage(),
        "zh-CN" => new ChineseSimplifiedLanguage(),
        _ => null,
    };
}
