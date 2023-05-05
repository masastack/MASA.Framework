// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public static class IdGeneratorOptionsExtensions
{
    public static void UseSequentialGuidGenerator(this IdGeneratorOptions options)
        => options.UseSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);

    public static void UseSequentialGuidGenerator(this IdGeneratorOptions options, SequentialGuidType guidType)
    {
        if (options.Services.Any(service => service.ImplementationType == typeof(SequentialGuidGeneratorProvider))) return;

        options.Services.AddSingleton<SequentialGuidGeneratorProvider>();

        options.Services.AddSingleton<ISequentialGuidGenerator>(_ => new SequentialGuidGenerator(guidType));
        options.Services.TryAddSingleton<IIdGenerator<Guid>>(serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());
        options.Services.TryAddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());
    }

#pragma warning disable S2094
    private sealed class SequentialGuidGeneratorProvider
    {

    }
#pragma warning restore S2094
}
