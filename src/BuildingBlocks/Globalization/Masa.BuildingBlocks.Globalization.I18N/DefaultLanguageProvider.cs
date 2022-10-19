// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class DefaultLanguageProvider : ILanguageProvider
{
    private readonly IOptionsMonitor<MasaI18NOptions> _options;

    public DefaultLanguageProvider(IOptionsMonitor<MasaI18NOptions> options)
    {
        _options = options;
    }

    public IReadOnlyList<LanguageInfo> GetLanguages()
        => _options.CurrentValue.Languages;
}
