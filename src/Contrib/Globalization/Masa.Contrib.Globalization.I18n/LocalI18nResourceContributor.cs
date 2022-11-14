// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Localization;

public class LocalI18nResourceContributor : II18nResourceContributor
{
    /// <summary>
    /// Random number for handling special keys.
    /// </summary>
    private static readonly string _randomNumber = Guid.NewGuid().ToString();

    private readonly IConfiguration _configuration;

    public Type ResourceType { get; }

    public string CultureName { get; }

    public LocalI18nResourceContributor(
        Type resourceType,
        string cultureName,
        IConfiguration configuration)
    {
        ResourceType = resourceType;
        CultureName = cultureName;
        _configuration = configuration;
    }

    public string? GetOrNull(string name)
    {
        var section = _configuration.GetSection(Masa.BuildingBlocks.Globalization.I18n.Constant.DEFAULT_LOCAL_SECTION)?.GetSection(ResourceType.Name)?.GetSection(CultureName);
        if (section != null && section.Exists())
        {
            string newName = name.Replace("\\.", _randomNumber).Replace(".", ConfigurationPath.KeyDelimiter).Replace(_randomNumber, ".");
            return section.GetValue<string>(newName);
        }
        return null;
    }
}
