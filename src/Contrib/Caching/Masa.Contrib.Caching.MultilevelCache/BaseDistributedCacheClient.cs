// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public abstract class BaseDistributedCacheClient
{
    protected static Guid UniquelyIdentifies;

    static BaseDistributedCacheClient() => UniquelyIdentifies = Guid.NewGuid();
}
