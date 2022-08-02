// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IIdGenerator<in T,out TOut>
    where T : notnull
    where TOut : notnull
{
    public TOut NewId();
}
