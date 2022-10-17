// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.Localization;

public class LocalLocalizationResourceContributor : ILocalizationResourceContributor
{
    private readonly IConfiguration _configuration;

    public Type ResourceType { get; }

    public string CultureName { get; }

    public LocalLocalizationResourceContributor(
        Type resourceType,
        string cultureName,
        IConfiguration configuration,
        IMasaConfiguration? masaConfiguration = null)
    {
        ResourceType = resourceType;
        CultureName = cultureName;
        _configuration = masaConfiguration?.Local ?? configuration;
    }

    public string? GetOrNull(string name)
    {
        var section = _configuration.GetSection(Const.DEFAULT_LOCAL_SECTION)?.GetSection(ResourceType.Name)?.GetSection(CultureName);
        if (section != null && section.Exists())
        {
            return section.GetValue<string>(name);
        }
        return null;
    }
}
