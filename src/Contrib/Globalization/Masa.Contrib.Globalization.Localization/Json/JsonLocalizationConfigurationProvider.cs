// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public class JsonLocalizationConfigurationProvider : FileLocalizationConfigurationProvider
{
    public JsonLocalizationConfigurationProvider(JsonLocalizationConfigurationSource configurationSource) : base(configurationSource)
    {

    }

    protected override void AddFile(string basePath, string fileName)
    {
        ConfigurationBuilder = ConfigurationBuilder
            .SetBasePath(basePath)
            .AddJsonFile(fileName, false, true);
    }
}
