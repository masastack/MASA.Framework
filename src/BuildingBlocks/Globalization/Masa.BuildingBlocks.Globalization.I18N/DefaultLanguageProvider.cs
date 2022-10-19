// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class DefaultLanguageProvider : ILanguageProvider
{
    public IReadOnlyList<LanguageInfo> GetLanguages() => I18NResourceResourceConfiguration.Languages.ToArray();
}
