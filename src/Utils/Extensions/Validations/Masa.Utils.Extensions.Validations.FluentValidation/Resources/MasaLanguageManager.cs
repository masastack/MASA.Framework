// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

            AddTranslation(language, nameof(ChineseLetterNumberUnderlineValidator<string>),
                languageProvider.GetTranslation(nameof(ChineseLetterNumberUnderlineValidator<string>)));
            AddTranslation(language, nameof(ChineseLetterUnderlineValidator<string>),
                languageProvider.GetTranslation(nameof(ChineseLetterUnderlineValidator<string>)));
            AddTranslation(language, nameof(ChineseValidator<string>), languageProvider.GetTranslation(nameof(ChineseValidator<string>)));
            AddTranslation(language, nameof(IdCardValidator<string>), languageProvider.GetTranslation(nameof(IdCardValidator<string>)));
            AddTranslation(language, nameof(LetterNumberValidator<string>),
                languageProvider.GetTranslation(nameof(LetterNumberValidator<string>)));
            AddTranslation(language, nameof(LetterValidator<string>), languageProvider.GetTranslation(nameof(LetterValidator<string>)));
            AddTranslation(language, nameof(LowerLetterValidator<string>),
                languageProvider.GetTranslation(nameof(LowerLetterValidator<string>)));
            AddTranslation(language, nameof(NumberValidator<string>), languageProvider.GetTranslation(nameof(NumberValidator<string>)));
            AddTranslation(language, nameof(PhoneValidator<string>), languageProvider.GetTranslation(nameof(PhoneValidator<string>)));
            AddTranslation(language, nameof(PortValidator<string>), languageProvider.GetTranslation(nameof(PortValidator<string>)));
            AddTranslation(language, nameof(RequiredValidator<string, string>), languageProvider.GetTranslation(nameof(RequiredValidator<string, string>)));
            AddTranslation(language, nameof(UpperLetterValidator<string>), languageProvider.GetTranslation(nameof(UpperLetterValidator<string>)));
            AddTranslation(language, nameof(UrlValidator<string>), languageProvider.GetTranslation(nameof(UrlValidator<string>)));
        }
    }

    private static ILanguageProvider? GetLanguageProvider(string language)
    {
        switch (language)
        {
            case { } l when l.Equals("en", StringComparison.OrdinalIgnoreCase):
                return new EnglishLanguage();
            case { } l when l.Equals("en-US", StringComparison.OrdinalIgnoreCase):
                return new EnglishLanguage();
            case { } l when l.Equals("en-GB", StringComparison.OrdinalIgnoreCase):
                return new EnglishLanguage();
            case { } l when l.Equals("zh-CN", StringComparison.OrdinalIgnoreCase):
                return new ChineseSimplifiedLanguage();
            default:
                return null;
        }
    }
}
