// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;

public class DccConfigurationOptions
{
    public RedisConfigurationOptions RedisOptions { get; set; }

    public string ManageServiceAddress { get; set; } = default!;

    /// <summary>
    /// public config id
    /// </summary>
    public string? PublicId { get; set; }

    /// <summary>
    /// The prefix of Dcc PubSub, it is not recommended to modify
    /// </summary>
    public string? SubscribeKeyPrefix { get; set; }
}
