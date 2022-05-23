// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public class DefaultIdGenerator : IIdGenerator
{
    private readonly SnowflakeId _snowflakeId;

    public DefaultIdGenerator(IdGeneratorOptions idGeneratorOptions)
        => _snowflakeId = new SnowflakeId(idGeneratorOptions);

    public long Generate() => _snowflakeId.NextId();
}
