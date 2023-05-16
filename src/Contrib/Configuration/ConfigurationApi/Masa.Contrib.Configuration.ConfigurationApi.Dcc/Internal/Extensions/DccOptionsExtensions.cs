// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

internal static class DccOptionsExtensions
{
    public static DccConfigurationOptions ConvertToDccConfigurationOptions(this DccOptions options)
    {
        return new DccConfigurationOptions()
        {
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = options.RedisOptions.Servers.Select(server => new RedisServerOptions(server.Host, server.Port)).ToList(),
                AbortOnConnectFail = options.RedisOptions.AbortOnConnectFail,
                AllowAdmin = options.RedisOptions.AllowAdmin,
                ClientName = options.RedisOptions.ClientName,
                ChannelPrefix = options.RedisOptions.ChannelPrefix,
                ConnectRetry = options.RedisOptions.ConnectRetry,
                ConnectTimeout = options.RedisOptions.ConnectTimeout,
                DefaultDatabase = options.RedisOptions.DefaultDatabase,
                Password = options.RedisOptions.Password,
                Proxy = options.RedisOptions.Proxy,
                Ssl = options.RedisOptions.Ssl,
                SyncTimeout = options.RedisOptions.SyncTimeout,
                AbsoluteExpiration = options.RedisOptions.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.RedisOptions.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.RedisOptions.SlidingExpiration
            },
            ManageServiceAddress = options.ManageServiceAddress,
            SubscribeKeyPrefix = options.SubscribeKeyPrefix,
            PublicId = options.PublicId,
            PublicSecret = options.PublicSecret,
            ConfigObjectSecret = options.ConfigObjectSecret,
            DefaultSection = new DccSectionOptions(
                options.AppId,
                options.Environment,
                options.Cluster,
                options.ConfigObjects,
                options.Secret),
            ExpandSections = options.ExpandSections.Select(section => new DccSectionOptions(
                section.AppId,
                section.Environment,
                section.Cluster,
                section.ConfigObjects,
                section.Secret)).ToList()
        };
    }
}
