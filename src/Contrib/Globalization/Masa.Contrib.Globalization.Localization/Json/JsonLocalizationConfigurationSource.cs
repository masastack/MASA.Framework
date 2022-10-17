// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public class JsonLocalizationConfigurationSource : IConfigurationSource
{
    public Type ResourceType { get; }

    public readonly List<string> LocalizationFilePaths;

    public bool UseMasaConfiguration { get; }

    public JsonLocalizationConfigurationSource(Type resourceType, List<string> localizationFilePaths, bool useMasaConfiguration)
    {
        ResourceType = resourceType;
        LocalizationFilePaths = localizationFilePaths;
        UseMasaConfiguration = useMasaConfiguration;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new JsonLocalizationConfigurationProvider(this);
    }
}
