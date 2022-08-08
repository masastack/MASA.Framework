﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class IdGeneratorFactory
{
    private static IGuidGenerator? _guidGenerator;

    public static IGuidGenerator GuidGenerator => _guidGenerator ??= MasaApp.GetRequiredService<IGuidGenerator>() ??
        throw new Exception($"Unsupported {nameof(GuidGenerator)}");

    private static ISequentialGuidGenerator? _sequentialGuidGenerator;

    public static ISequentialGuidGenerator SequentialGuidGenerator
        => _sequentialGuidGenerator ??= MasaApp.GetRequiredService<ISequentialGuidGenerator>() ??
            throw new Exception($"Unsupported {nameof(SequentialGuidGenerator)}");

    private static ISnowflakeGenerator? _snowflakeGenerator;

    public static ISnowflakeGenerator SnowflakeGenerator
        => _snowflakeGenerator ??= MasaApp.GetRequiredService<ISnowflakeGenerator>() ??
            throw new Exception($"Unsupported {nameof(SnowflakeGenerator)}");
}
