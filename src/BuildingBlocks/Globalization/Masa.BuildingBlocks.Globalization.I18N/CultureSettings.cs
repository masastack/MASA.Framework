// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18N;

public class CultureSettings
{
    public string? ResourcesDirectory { get; set; }

    public string? SupportCultureName { get; set; }

    public List<CultureModel> SupportedCultures { get; set; } = new();

    public void AddCulture(string culture, string displayName, string? icon = null)
        => AddCulture(new CultureModel(culture, displayName, icon));

    public void AddCulture(CultureModel cultureModel)
    {
        SupportedCultures.Add(cultureModel);
    }
}
