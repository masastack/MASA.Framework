// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class EnglishLanguage : ILanguageProvider
{
    public string GetTranslation(string key) => key switch
    {
        "ChineseLetterNumberUnderlineValidator" => "'{PropertyName}' must be Chinese, numbers, letters or underscores.",
        "LetterNumberUnderlineValidator" => "'{PropertyName}' must be numbers, letters or underscores.",
        "ChineseLetterNumberValidator" => "'{PropertyName}' must be Chinese, numbers, letters.",
        "ChineseLetterUnderlineValidator" => "'{PropertyName}' must be Chinese, letters or underscores.",
        "ChineseLetterValidator" => "'{PropertyName}' must be Chinese, letters.",
        "ChineseValidator" => "'{PropertyName}' must be Chinese.",
        "IdCardValidator" => "'{PropertyName}' is not a valid ID.",
        "LetterNumberValidator" => "'{PropertyName}' must be a letter or underscore.",
        "LetterValidator" => "'{PropertyName}' must be a letter.",
        "LowerLetterValidator" => "'{PropertyName}' must be lowercase.",
        "NumberValidator" => "'{PropertyName}' must be Number.",
        "PhoneValidator" => "'{PropertyName}' must be a valid mobile phone number.",
        "PortValidator" => "'{PropertyName}' must be a legal port, it needs to be between [0 - 65535].",
        "RequiredValidator" => "'{PropertyName}' is required.",
        "UpperLetterValidator" => "'{PropertyName}' must be uppercase.",
        "UrlValidator" => "'{PropertyName}' must be a legal Url address.",
        "EmailRegularValidator" => "'{PropertyName}' must be a legal Email.",
        "PasswordValidator" => "'{PropertyName}' password validation rule failed.",
        "IdentityValidator" => "'{PropertyName}' must be numbers, letters or . and - .",
        _ => string.Empty,
    };
}
