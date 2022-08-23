// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

public class TestBase
{
    protected ConfigurationOptions GetConfigurationOptions()
    {
        var configurationOptions = new ConfigurationOptions();
        configurationOptions.EndPoints.Add("localhost", 6379);
        return configurationOptions;
    }

    protected JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        return jsonSerializerOptions;
    }
}
