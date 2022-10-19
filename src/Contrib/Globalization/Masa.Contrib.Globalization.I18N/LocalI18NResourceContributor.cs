// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Localization;

public class LocalI18NResourceContributor : II18NResourceContributor
{
    private readonly IConfiguration _configuration;

    public Type ResourceType { get; }

    public string CultureName { get; }

    public LocalI18NResourceContributor(
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
        var section = _configuration.GetSection(Const.DEFAULT_LOCAL_SECTION)?.GetSection(ResourceType.Name)?.GetSection(CultureName);
        if (section != null && section.Exists())
        {
            return section.GetValue<string>(name.Replace(".", ConfigurationPath.KeyDelimiter));
        }
        return null;
    }
}
