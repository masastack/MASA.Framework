// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal static class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        string? configObjectSecret,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);
        var logger = serviceProvider.GetService<ILogger<ConfigurationApiClient>>();
        return new ConfigurationApiClient(multilevelCacheClient,
            jsonSerializerOptions,
            configObjectSecret,
            defaultSectionOption,
            expandSectionOptions,
            logger);
    }

    public static IConfigurationApiManage CreateManage(
        ICaller caller,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        => new ConfigurationApiManage(caller, jsonSerializerOptions,defaultSectionOption,  expandSectionOptions);
}
