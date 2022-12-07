// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

internal class ChineseSimplifiedLanguage : ILanguageProvider
{
    public string GetTranslation(string key) => key switch
    {
        "ChineseValidator" => "'{PropertyName}' 必须是中文.",
        "NumberValidator" => "'{PropertyName}' 必须是数字.",
        _ => null,
    };
}
