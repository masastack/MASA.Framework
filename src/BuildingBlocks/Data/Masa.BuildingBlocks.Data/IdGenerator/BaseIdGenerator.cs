// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public abstract class BaseIdGenerator<TOut> : IIdGenerator<TOut>
    where TOut : notnull
{
    public abstract TOut NewId();

    public virtual string NewStringId() => NewId().ToString()!;
}
