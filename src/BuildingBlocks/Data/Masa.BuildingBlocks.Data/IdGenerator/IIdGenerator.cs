// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IIdGenerator
{
    string NewStringId();
}

public interface IIdGenerator<out TOut>: IIdGenerator
    where TOut : notnull
{
    TOut NewId();
}
