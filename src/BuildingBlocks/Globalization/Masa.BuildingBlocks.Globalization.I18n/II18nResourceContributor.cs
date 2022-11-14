// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public interface II18nResourceContributor
{
    string CultureName { get; }

    string? GetOrNull(string name);
}
