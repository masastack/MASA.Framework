// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace FluentValidation.Resources;

internal class EnglishLanguage : ILanguageProvider
{
    public string GetTranslation(string key) => key switch
    {
        "ChineseValidator" => "'{PropertyName}' must be Chinese.",
        "NumberValidator" => "'{PropertyName}' must be Number.",
        _ => null,
    };
}
