// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.Localization;

public class DefaultLanguageProvider : ILanguageProvider
{
    private readonly IOptionsMonitor<MasaLocalizationOptions> _options;

    public DefaultLanguageProvider(IOptionsMonitor<MasaLocalizationOptions> options)
    {
        _options = options;
    }

    public IReadOnlyList<LanguageInfo> GetLanguages()
        => _options.CurrentValue.Languages;
}
