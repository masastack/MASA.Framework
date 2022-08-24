// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class SubscribeConfigurationOptions
{
    public SubscribeKeyTypes SubscribeKeyTypes { get; set; }

    public string SubscribeKeyPrefix { get; set; }

    public static SubscribeConfigurationOptions Default = new()
    {
        SubscribeKeyTypes = SubscribeKeyTypes.ValueTypeFullName
    };
}
