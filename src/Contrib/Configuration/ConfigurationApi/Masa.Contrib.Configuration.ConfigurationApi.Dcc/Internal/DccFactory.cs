// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

[ExcludeFromCodeCoverage]
internal static class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccConfigurationOptions dccConfigurationOptions)
    {
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);
        var logger = serviceProvider.GetService<ILogger<ConfigurationApiClient>>();
        return new ConfigurationApiClient(multilevelCacheClient,
            jsonSerializerOptions,
            dccConfigurationOptions,
            logger);
    }

    public static IConfigurationApiManage CreateManage(
        ICaller caller,
        JsonSerializerOptions jsonSerializerOptions,
        DccConfigurationOptions dccConfigurationOptions)
        => new ConfigurationApiManage(caller, jsonSerializerOptions, dccConfigurationOptions);
}
