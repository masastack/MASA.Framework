// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class IdGeneratorFactory
{
    private static IGuidGenerator? _guidGenerator;
    public static IGuidGenerator GuidGenerator => _guidGenerator ?? throw new Exception($"Unsupported {nameof(GuidGenerator)}");

    private static ISequentialGuidGenerator? _sequentialGuidGenerator;
    public static ISequentialGuidGenerator SequentialGuidGenerator
        => _sequentialGuidGenerator ?? throw new Exception($"Unsupported {nameof(SequentialGuidGenerator)}");

    private static ISnowflakeGenerator? _snowflakeGenerator;
    public static ISnowflakeGenerator SnowflakeGenerator
        => _snowflakeGenerator ?? throw new Exception($"Unsupported {nameof(SnowflakeGenerator)}");

    public static void SetGuidGenerator(IGuidGenerator guidGenerator)
    {
        ArgumentNullException.ThrowIfNull(guidGenerator, nameof(guidGenerator));
        _guidGenerator = guidGenerator;
    }

    public static void SetSequentialGuidGenerator(ISequentialGuidGenerator sequentialGuidGenerator)
    {
        ArgumentNullException.ThrowIfNull(sequentialGuidGenerator, nameof(sequentialGuidGenerator));
        _sequentialGuidGenerator = sequentialGuidGenerator;
    }

    public static void SetSnowflakeGenerator(ISnowflakeGenerator snowflakeGenerator)
    {
        ArgumentNullException.ThrowIfNull(snowflakeGenerator, nameof(snowflakeGenerator));
        _snowflakeGenerator = snowflakeGenerator;
    }
}
