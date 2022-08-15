// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IIdGeneratorFactory
{
    IGuidGenerator GuidGenerator { get; }

    ISequentialGuidGenerator SequentialGuidGenerator { get; }

    ISnowflakeGenerator SnowflakeGenerator { get; }

    IIdGenerator<TOut> Create<TOut>() where TOut : notnull;

    IIdGenerator<TOut> Create<TOut>(string name) where TOut : notnull;

    IIdGenerator Create();

    IIdGenerator Create(string name);
}
