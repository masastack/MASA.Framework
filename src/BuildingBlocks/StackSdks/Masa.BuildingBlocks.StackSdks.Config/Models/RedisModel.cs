// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Config.Models;

public class RedisModel
{
    public string RedisHost { get; set; }

    public int RedisPort { get; set; }

    public int RedisDb { get; set; }

    public string RedisPassword { get; set; }
}
