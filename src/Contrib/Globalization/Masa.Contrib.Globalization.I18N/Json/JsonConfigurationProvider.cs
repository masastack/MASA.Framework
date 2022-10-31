// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Json;

public class JsonConfigurationProvider : FileConfigurationProvider
{
    public JsonConfigurationProvider(JsonConfigurationSource configurationSource)
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
