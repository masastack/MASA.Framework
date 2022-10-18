// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public class JsonLocalizationConfigurationProvider : FileLocalizationConfigurationProvider
{
    public JsonLocalizationConfigurationProvider(JsonLocalizationConfigurationSource configurationSource)
        : base(configurationSource)
    {

    }

    protected override IConfigurationBuilder AddFile(IConfigurationBuilder configurationBuilder, string basePath, string cultureName)
    {
        return configurationBuilder
            .SetBasePath(basePath)
            .AddJsonFile(cultureName + ".json", false, true);
    }
}
