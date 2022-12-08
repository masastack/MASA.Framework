// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class ChineseSimplifiedLanguage : ILanguageProvider
{
    public string GetTranslation(string key) => key switch
    {
        "ChineseLetterNumberUnderlineValidator" => "'{PropertyName}'必须是中文、数字、字母或下划线.",
        "ChineseLetterNumberValidator" => "'{PropertyName}'必须是中文、数字、字母.",
        "ChineseLetterUnderlineValidator" => "'{PropertyName}'必须是中文、字母或下划线.",
        "ChineseLetterValidator" => "'{PropertyName}'必须是中文、字母.",
        "ChineseValidator" => "'{PropertyName}' 必须是中文.",
        "IdCardValidator" => "'{PropertyName}' 不是一个有效的身份证.",
        "LetterNumberValidator" => "'{PropertyName}' 必须是字母或下划线.",
        "LetterValidator" => "'{PropertyName}' 必须是字母.",
        "LowerLetterValidator" => "'{PropertyName}' 必须是小写字母.",
        "NumberValidator" => "'{PropertyName}' 必须是数字.",
        "PhoneValidator" => "'{PropertyName}' 必须是一个合法的手机号码",
        "PortValidator" => "'{PropertyName}' 必须是一个合法的端口, 需要在[0 - 65535]之间",
        "RequiredValidator" => "'{PropertyName}' 是必需的",
        "UpperLetterValidator" => "'{PropertyName}' 必须是大写字母.",
        "UrlValidator" => "'{PropertyName}' 必须是一个合法的Url地址",
        _ => string.Empty,
    };
}
