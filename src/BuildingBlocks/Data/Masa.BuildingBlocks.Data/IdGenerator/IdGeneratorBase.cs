// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Data;

[Obsolete("BaseIdGenerator has expired, please use IdGeneratorBase")]
public abstract class BaseIdGenerator<TOut> : IdGeneratorBase<TOut>
    where TOut : notnull
{
}

public abstract class IdGeneratorBase<TOut> : IIdGenerator<TOut>
    where TOut : notnull
{
    public abstract TOut NewId();

    public virtual string NewStringId() => NewId().ToString()!;
}
