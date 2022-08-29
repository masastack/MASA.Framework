// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccOptions : DccSectionOptions
{
    public RedisConfigurationOptions RedisOptions { get; set; } = new();

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }

    /// <summary>
    /// public config id
    /// </summary>
    public string? PublicId { get; set; } = default!;

    public string? PublicSecret { get; set; }

    /// <summary>
    /// Expansion section information
    /// </summary>
    public List<DccSectionOptions> ExpandSections { get; set; } = new();

    public static implicit operator DccConfigurationOptions(DccOptions options)
    {
        var dccConfigurationOptions = new DccConfigurationOptions()
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
        return dccConfigurationOptions;
    }
}
