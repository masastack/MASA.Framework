// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class DefaultIdGeneratorFactory : IIdGeneratorFactory
{
    public IGuidGenerator? GuidGenerator { get; }

    public ISequentialGuidGenerator? SequentialGuidGenerator { get; }

    public ISnowflakeGenerator? SnowflakeGenerator { get; }

    public DefaultIdGeneratorFactory(
        IGuidGenerator? guidGenerator = null,
        ISequentialGuidGenerator? sequentialGuidGenerator = null,
        ISnowflakeGenerator? snowflakeGenerator = null)
    {
        GuidGenerator = guidGenerator;
        SequentialGuidGenerator = sequentialGuidGenerator;
        SnowflakeGenerator = snowflakeGenerator;
    }
}
